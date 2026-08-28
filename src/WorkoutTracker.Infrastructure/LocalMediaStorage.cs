using Microsoft.Extensions.Logging;
using WorkoutTracker.Application;

namespace WorkoutTracker.Infrastructure;

/// <summary>
/// Stores private media on the local filesystem.
/// <para>
/// This is the default provider for a small self-hosted deployment. It deliberately sits
/// behind <see cref="IMediaStorage"/> so it can be swapped for object storage such as R2 or
/// Supabase without touching use cases (spec 9). Files are written outside the web root and
/// are only ever served through an authorized endpoint.
/// </para>
/// </summary>
public sealed class LocalMediaStorage : IMediaStorage
{
    private readonly string _rootPath;
    private readonly ILogger<LocalMediaStorage> _logger;

    public LocalMediaStorage(string rootPath, ILogger<LocalMediaStorage> logger)
    {
        _rootPath = Path.GetFullPath(rootPath);
        _logger = logger;
        Directory.CreateDirectory(_rootPath);
    }

    public async Task<string> SaveAsync(
        Guid ownerId,
        string fileName,
        string contentType,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        // The key is generated server-side. The client file name only contributes an
        // extension, which prevents path traversal and name collisions.
        var extension = SafeExtension(fileName, contentType);
        var key = $"{ownerId:N}/{Guid.NewGuid():N}{extension}";

        var absolute = ResolveKey(key);
        Directory.CreateDirectory(Path.GetDirectoryName(absolute)!);

        await using var target = File.Create(absolute);
        await content.CopyToAsync(target, cancellationToken);

        return key;
    }

    public Task<Stream?> OpenAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var absolute = ResolveKey(storageKey);
            return Task.FromResult<Stream?>(File.Exists(absolute)
                ? File.OpenRead(absolute)
                : null);
        }
        catch (ArgumentException)
        {
            // Rejected keys are treated as missing rather than surfacing an error.
            return Task.FromResult<Stream?>(null);
        }
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var absolute = ResolveKey(storageKey);
            if (File.Exists(absolute)) File.Delete(absolute);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException)
        {
            // Metadata has already been removed; a failed byte cleanup must not fail the request.
            _logger.LogWarning(exception, "Failed to delete stored media for key {Key}.", storageKey);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Resolves a storage key to an absolute path, refusing anything that escapes the
    /// configured root.
    /// </summary>
    private string ResolveKey(string storageKey)
    {
        if (string.IsNullOrWhiteSpace(storageKey)) throw new ArgumentException("Storage key is required.", nameof(storageKey));

        var combined = Path.GetFullPath(Path.Combine(_rootPath, storageKey));

        if (!combined.StartsWith(_rootPath, StringComparison.Ordinal))
            throw new ArgumentException("Storage key resolves outside the media root.", nameof(storageKey));

        return combined;
    }

    /// <summary>Derives a safe extension from the content type, ignoring the client name.</summary>
    private static string SafeExtension(string fileName, string contentType) => contentType.ToLowerInvariant() switch
    {
        "image/jpeg" => ".jpg",
        "image/png" => ".png",
        "image/webp" => ".webp",
        _ => Path.GetExtension(fileName) is { Length: > 0 and <= 5 } extension && extension.All(IsSafeExtensionChar)
            ? extension.ToLowerInvariant()
            : ".bin"
    };

    private static bool IsSafeExtensionChar(char value) => value == '.' || char.IsAsciiLetterOrDigit(value);
}
