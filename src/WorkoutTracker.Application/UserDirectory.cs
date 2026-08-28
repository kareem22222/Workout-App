namespace WorkoutTracker.Application;

/// <summary>Identity account facts needed by use cases, free of any Identity types.</summary>
public sealed record UserAccount(
    Guid Id,
    string DisplayName,
    string Email,
    bool IsAdmin,
    bool IsDisabled,
    DateTimeOffset CreatedAt);

/// <summary>
/// Read/update access to Identity accounts, implemented in Infrastructure. This keeps
/// ASP.NET Identity out of the Application layer while still allowing profile and admin
/// use cases to report account facts (spec 2).
/// </summary>
public interface IUserDirectory
{
    Task<UserAccount?> FindAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserAccount>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>Updates the account display name. Returns false when the user does not exist.</summary>
    Task<bool> UpdateDisplayNameAsync(Guid userId, string displayName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enables or disables sign-in for an account. Disabled users are blocked at
    /// authentication time (spec US-290).
    /// </summary>
    Task<bool> SetDisabledAsync(Guid userId, bool isDisabled, CancellationToken cancellationToken = default);
}

/// <summary>
/// Abstraction over private binary storage for progress photos and avatars. Keys are
/// never public URLs; retrieval always goes through an authorized endpoint (spec 3).
/// </summary>
public interface IMediaStorage
{
    Task<string> SaveAsync(Guid ownerId, string fileName, string contentType, Stream content, CancellationToken cancellationToken = default);

    Task<Stream?> OpenAsync(string storageKey, CancellationToken cancellationToken = default);

    Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);
}

/// <summary>Abstraction over the system clock so time-dependent rules stay testable.</summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

/// <summary>Default clock backed by the system time.</summary>
public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
