using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WorkoutTracker.Infrastructure;

namespace WorkoutTracker.Api;

/// <summary>Strongly typed JWT settings bound from configuration.</summary>
public sealed class JwtOptions
{
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
    public string Key { get; set; } = "";

    /// <summary>Access tokens are deliberately short-lived (spec 3).</summary>
    public int AccessTokenMinutes { get; set; } = 15;

    public int RefreshTokenDays { get; set; } = 30;
}

/// <summary>The access token returned to the client. The refresh token travels by cookie only.</summary>
public sealed record AuthTokens(string AccessToken, DateTimeOffset ExpiresAt);

/// <summary>
/// Issues access tokens and manages the rotating refresh-token chain.
/// <para>
/// Refresh tokens are stored only as SHA-256 hashes, rotate on every use, and are grouped
/// into a family. Presenting an already-rotated token indicates theft or replay, so the
/// entire family is revoked rather than just the presented token (spec 3).
/// </para>
/// </summary>
public sealed class TokenService(AppDbContext db, UserManager<ApplicationUser> users, JwtOptions options)
{
    /// <summary>Name of the HttpOnly cookie carrying the refresh token.</summary>
    public const string RefreshCookieName = "refreshToken";

    /// <summary>
    /// Scoping the cookie to the auth routes means it is not attached to ordinary API
    /// calls, which limits its exposure.
    /// </summary>
    private const string RefreshCookiePath = "/api/auth";

    /// <summary>Issues a new access token and a fresh refresh-token family for a login.</summary>
    public async Task<AuthTokens> IssueAsync(
        ApplicationUser user,
        HttpResponse response,
        string? userAgent,
        CancellationToken ct = default)
    {
        var familyId = Guid.NewGuid();
        return await IssueInternalAsync(user, response, familyId, null, userAgent, ct);
    }

    /// <summary>
    /// Validates and rotates a refresh token, returning null when the token is unusable.
    /// </summary>
    public async Task<AuthTokens?> RotateAsync(
        HttpRequest request,
        HttpResponse response,
        CancellationToken ct = default)
    {
        if (!request.Cookies.TryGetValue(RefreshCookieName, out var raw) || string.IsNullOrWhiteSpace(raw))
            return null;

        var hash = Hash(raw);
        var token = await db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == hash, ct);

        if (token is null)
        {
            ClearCookie(response);
            return null;
        }

        // An already-rotated or revoked token means the chain has been replayed.
        if (token.RevokedAt is not null || token.ReplacedByTokenId is not null)
        {
            await RevokeFamilyAsync(token.UserId, token.FamilyId, ct);
            ClearCookie(response);
            return null;
        }

        if (token.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            token.RevokedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(ct);
            ClearCookie(response);
            return null;
        }

        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == token.UserId, ct);

        // A disabled account must not be able to refresh its way back in.
        if (user is null || user.IsDisabled)
        {
            await RevokeFamilyAsync(token.UserId, token.FamilyId, ct);
            ClearCookie(response);
            return null;
        }

        return await IssueInternalAsync(user, response, token.FamilyId, token, request.Headers.UserAgent.ToString(), ct);
    }

    /// <summary>Revokes the presented refresh token and clears the cookie.</summary>
    public async Task LogoutAsync(HttpRequest request, HttpResponse response, CancellationToken ct = default)
    {
        if (request.Cookies.TryGetValue(RefreshCookieName, out var raw) && !string.IsNullOrWhiteSpace(raw))
        {
            var hash = Hash(raw);
            var token = await db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == hash, ct);

            if (token is not null)
            {
                // Revoke the whole family so every rotated descendant dies with it.
                await RevokeFamilyAsync(token.UserId, token.FamilyId, ct);
            }
        }

        ClearCookie(response);
    }

    private async Task<AuthTokens> IssueInternalAsync(
        ApplicationUser user,
        HttpResponse response,
        Guid familyId,
        RefreshToken? rotatedFrom,
        string? userAgent,
        CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var roles = await users.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Email, user.Email ?? ""),
            new(JwtRegisteredClaimNames.SecurityStamp, user.SecurityStamp ?? "")
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var expiresAt = now.AddMinutes(options.AccessTokenMinutes);

        var jwt = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key)),
                SecurityAlgorithms.HmacSha256));

        // 256 bits of entropy; only the hash is persisted.
        var rawRefresh = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

        var refresh = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = Hash(rawRefresh),
            FamilyId = familyId,
            CreatedAt = now,
            ExpiresAt = now.AddDays(options.RefreshTokenDays),
            UserAgent = Truncate(userAgent, 256)
        };

        db.RefreshTokens.Add(refresh);

        if (rotatedFrom is not null)
        {
            rotatedFrom.RevokedAt = now;
            rotatedFrom.ReplacedByTokenId = refresh.Id;
        }

        await db.SaveChangesAsync(ct);

        response.Cookies.Append(RefreshCookieName, rawRefresh, BuildCookieOptions(refresh.ExpiresAt));

        return new AuthTokens(new JwtSecurityTokenHandler().WriteToken(jwt), expiresAt);
    }

    private async Task RevokeFamilyAsync(Guid userId, Guid familyId, CancellationToken ct)
    {
        var family = await db.RefreshTokens
            .Where(x => x.UserId == userId && x.FamilyId == familyId && x.RevokedAt == null)
            .ToListAsync(ct);

        var now = DateTimeOffset.UtcNow;
        foreach (var token in family) token.RevokedAt = now;

        await db.SaveChangesAsync(ct);
    }

    private void ClearCookie(HttpResponse response)
        => response.Cookies.Delete(RefreshCookieName, BuildCookieOptions(DateTimeOffset.UnixEpoch));

    private CookieOptions BuildCookieOptions(DateTimeOffset expires) => new()
    {
        HttpOnly = true,
        // Secure is required for SameSite=None, which the cookie needs when the SPA is
        // hosted on a different origin from the API.
        Secure = true,
        SameSite = SameSiteMode.None,
        Expires = expires,
        Path = RefreshCookiePath,
        IsEssential = true
    };

    private static string Hash(string value)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

    private static string? Truncate(string? value, int max)
        => string.IsNullOrWhiteSpace(value) ? null : value.Length <= max ? value : value[..max];
}

/// <summary>Custom claim names kept in one place.</summary>
public static class JwtRegisteredClaimNames
{
    /// <summary>
    /// Identity security stamp. Comparing it on each request lets password changes and
    /// account disabling invalidate tokens that have not yet expired.
    /// </summary>
    public const string SecurityStamp = "sstamp";
}
