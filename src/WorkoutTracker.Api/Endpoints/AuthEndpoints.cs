using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Application.Contracts;
using WorkoutTracker.Application.Services;
using WorkoutTracker.Infrastructure;

namespace WorkoutTracker.Api.Endpoints;

public sealed record RegisterRequest(
    [Required, StringLength(60, MinimumLength = 1)] string DisplayName,
    [Required, EmailAddress] string Email,
    [Required, StringLength(128, MinimumLength = 8)] string Password);

public sealed record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public sealed record ChangePasswordRequest(
    [Required] string CurrentPassword,
    [Required, StringLength(128, MinimumLength = 8)] string NewPassword);

/// <summary>
/// Authentication and account endpoints (spec 5 /api/auth). Credential errors are
/// deliberately generic so the API cannot be used to enumerate registered emails.
/// </summary>
public static class AuthEndpoints
{
    /// <summary>Single message used for every failed credential check.</summary>
    private const string InvalidCredentials = "Invalid email or password.";

    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        // Rate limiting protects only the unauthenticated credential surface.
        var credentials = app.MapGroup("/api/auth").WithTags("Auth").RequireRateLimiting("auth");

        credentials.MapPost("/register", Register);
        credentials.MapPost("/login", Login);
        credentials.MapPost("/refresh", Refresh);

        group.MapPost("/logout", Logout);
        group.MapGet("/me", Me).RequireAuthorization();
        group.MapPut("/me", UpdateMe).RequireAuthorization();
        group.MapPost("/change-password", ChangePassword).RequireAuthorization();
    }

    private static async Task<IResult> Register(
        RegisterRequest request,
        HttpRequest httpRequest,
        HttpResponse response,
        UserManager<ApplicationUser> users,
        TokenService tokens,
        SettingsService settings,
        IConfiguration configuration,
        CancellationToken ct)
    {
        // Allows the deployment to be locked down to the intended small group (spec 14).
        if (!configuration.GetValue("Auth:AllowRegistration", true))
            return TypedResults.Problem(detail: "Registration is disabled.", statusCode: 403, title: "Forbidden");

        var displayName = request.DisplayName?.Trim() ?? "";
        var email = request.Email?.Trim() ?? "";

        if (displayName.Length == 0 || email.Length == 0)
            return TypedResults.Problem(detail: "Display name and email are required.", statusCode: 400, title: "Validation failed");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            DisplayName = displayName,
            Email = email,
            UserName = email,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var result = await users.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .GroupBy(x => x.Code.Contains("Password") ? nameof(request.Password) : nameof(request.Email))
                .ToDictionary(x => x.Key, x => x.Select(error => error.Description).ToArray());

            return TypedResults.ValidationProblem(errors);
        }

        // The first account to register becomes the operator, so the admin surface is
        // reachable without seeding credentials (spec 12.1).
        if (users.Users.Count() == 1) await users.AddToRoleAsync(user, AppDbContext.AdminRole);

        // Create defaults immediately so the client never sees a half-configured account.
        await settings.GetOrCreateSettingsAsync(user.Id, ct);
        await settings.GetOrCreateProfileAsync(user.Id, ct);

        var issued = await tokens.IssueAsync(user, response, httpRequest.Headers.UserAgent.ToString(), ct);
        var profile = await settings.GetProfileAsync(user.Id, ct);

        return TypedResults.Ok(new { accessToken = issued.AccessToken, expiresAt = issued.ExpiresAt, user = profile.Value });
    }

    private static async Task<IResult> Login(
        LoginRequest request,
        HttpRequest httpRequest,
        HttpResponse response,
        UserManager<ApplicationUser> users,
        TokenService tokens,
        SettingsService settings,
        CancellationToken ct)
    {
        var user = await users.FindByEmailAsync(request.Email?.Trim() ?? "");

        // Always run a password check shape that does not leak whether the user exists.
        if (user is null || !await users.CheckPasswordAsync(user, request.Password))
            return TypedResults.Problem(detail: InvalidCredentials, statusCode: 401, title: "Unauthorized");

        if (user.IsDisabled)
            return TypedResults.Problem(detail: "This account has been disabled.", statusCode: 403, title: "Forbidden");

        await settings.GetOrCreateSettingsAsync(user.Id, ct);
        await settings.GetOrCreateProfileAsync(user.Id, ct);

        var issued = await tokens.IssueAsync(user, response, httpRequest.Headers.UserAgent.ToString(), ct);
        var profile = await settings.GetProfileAsync(user.Id, ct);

        return TypedResults.Ok(new { accessToken = issued.AccessToken, expiresAt = issued.ExpiresAt, user = profile.Value });
    }

    private static async Task<IResult> Refresh(
        HttpRequest request,
        HttpResponse response,
        TokenService tokens,
        SettingsService settings,
        CancellationToken ct)
    {
        var issued = await tokens.RotateAsync(request, response, ct);

        if (issued is null)
            return TypedResults.Problem(detail: "Your session has expired. Please sign in again.", statusCode: 401, title: "Unauthorized");

        return TypedResults.Ok(new { accessToken = issued.AccessToken, expiresAt = issued.ExpiresAt });
    }

    private static async Task<IResult> Logout(
        HttpRequest request,
        HttpResponse response,
        TokenService tokens,
        CancellationToken ct)
    {
        await tokens.LogoutAsync(request, response, ct);
        return TypedResults.NoContent();
    }

    private static async Task<IResult> Me(
        ClaimsPrincipal principal,
        SettingsService settings,
        CancellationToken ct)
        => (await settings.GetProfileAsync(principal.UserId(), ct)).ToHttp();

    private static async Task<IResult> UpdateMe(
        ClaimsPrincipal principal,
        UpdateProfileRequest request,
        SettingsService settings,
        CancellationToken ct)
        => (await settings.UpdateProfileAsync(principal.UserId(), request, ct)).ToHttp();

    private static async Task<IResult> ChangePassword(
        ClaimsPrincipal principal,
        ChangePasswordRequest request,
        UserManager<ApplicationUser> users,
        TokenService tokens,
        HttpRequest httpRequest,
        HttpResponse response,
        CancellationToken ct)
    {
        var user = await users.FindByIdAsync(principal.UserId().ToString());
        if (user is null) return TypedResults.Problem(detail: "Account not found.", statusCode: 404, title: "Not found");

        var result = await users.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(request.CurrentPassword)] = result.Errors.Select(x => x.Description).ToArray()
            });
        }

        // Changing the password ends other sessions, then re-establishes this one.
        await tokens.LogoutAsync(httpRequest, response, ct);
        var issued = await tokens.IssueAsync(user, response, httpRequest.Headers.UserAgent.ToString(), ct);

        return TypedResults.Ok(new { accessToken = issued.AccessToken, expiresAt = issued.ExpiresAt });
    }
}
