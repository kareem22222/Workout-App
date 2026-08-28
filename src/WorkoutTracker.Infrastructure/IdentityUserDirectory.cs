using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Application;

namespace WorkoutTracker.Infrastructure;

/// <summary>
/// Reads and updates Identity accounts on behalf of Application services. Admin status is
/// resolved from the <see cref="AppDbContext.AdminRole"/> role rather than a column, so
/// role membership stays the single source of truth.
/// </summary>
public sealed class IdentityUserDirectory(AppDbContext db, UserManager<ApplicationUser> users) : IUserDirectory
{
    public async Task<UserAccount?> FindAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) return null;

        var adminIds = await AdminUserIdsAsync(cancellationToken);
        return Map(user, adminIds.Contains(user.Id));
    }

    public async Task<IReadOnlyList<UserAccount>> ListAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await db.Users
            .AsNoTracking()
            .OrderBy(x => x.DisplayName)
            .ToListAsync(cancellationToken);

        var adminIds = await AdminUserIdsAsync(cancellationToken);
        return accounts.Select(x => Map(x, adminIds.Contains(x.Id))).ToList();
    }

    public async Task<bool> UpdateDisplayNameAsync(Guid userId, string displayName, CancellationToken cancellationToken = default)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) return false;

        user.DisplayName = displayName;
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SetDisabledAsync(Guid userId, bool isDisabled, CancellationToken cancellationToken = default)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) return false;

        user.IsDisabled = isDisabled;

        // Disabling must take effect immediately, so every refresh token is revoked and
        // the security stamp is rolled to invalidate any cached identity.
        if (isDisabled)
        {
            var tokens = await db.RefreshTokens
                .Where(x => x.UserId == userId && x.RevokedAt == null)
                .ToListAsync(cancellationToken);

            foreach (var token in tokens) token.RevokedAt = DateTimeOffset.UtcNow;
        }

        await db.SaveChangesAsync(cancellationToken);
        await users.UpdateSecurityStampAsync(user);
        return true;
    }

    /// <summary>Ids of all users in the admin role, fetched in one query.</summary>
    private async Task<HashSet<Guid>> AdminUserIdsAsync(CancellationToken cancellationToken)
    {
        var roleId = await db.Roles
            .Where(x => x.Name == AppDbContext.AdminRole)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (roleId == Guid.Empty) return [];

        var ids = await db.UserRoles
            .Where(x => x.RoleId == roleId)
            .Select(x => x.UserId)
            .ToListAsync(cancellationToken);

        return ids.ToHashSet();
    }

    private static UserAccount Map(ApplicationUser user, bool isAdmin) =>
        new(user.Id, user.DisplayName, user.Email ?? "", isAdmin, user.IsDisabled, user.CreatedAt);
}
