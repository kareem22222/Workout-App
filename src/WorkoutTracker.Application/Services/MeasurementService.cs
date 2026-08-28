using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Application.Contracts;
using WorkoutTracker.Domain;

namespace WorkoutTracker.Application.Services;

/// <summary>
/// Body measurement and progress photo use cases. Photos are private by default: only
/// metadata is returned and bytes are served through an authorized endpoint (spec US-102).
/// </summary>
public sealed class MeasurementService(IAppDbContext db, IMediaStorage storage, IClock clock)
{
    /// <summary>Upload cap for progress photos.</summary>
    public const long MaxPhotoBytes = 8 * 1024 * 1024;

    private static readonly string[] AllowedPhotoTypes = ["image/jpeg", "image/png", "image/webp"];

    public async Task<Result<IReadOnlyList<BodyMeasurementDto>>> ListAsync(
        Guid ownerId,
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken ct = default)
    {
        var query = db.BodyMeasurements.Where(x => x.OwnerId == ownerId);

        if (from is { } start) query = query.Where(x => x.MeasuredOn >= start);
        if (to is { } end) query = query.Where(x => x.MeasuredOn <= end);

        var items = await query
            .OrderByDescending(x => x.MeasuredOn)
            .AsNoTracking()
            .ToListAsync(ct);

        return Result<IReadOnlyList<BodyMeasurementDto>>.Ok(items.Select(x => x.ToDto()).ToList());
    }

    /// <summary>
    /// Creates or replaces the entry for a date. One entry per day keeps the weight trend
    /// unambiguous, so re-submitting a date updates it rather than duplicating.
    /// </summary>
    public async Task<Result<BodyMeasurementDto>> SaveAsync(
        Guid ownerId,
        SaveBodyMeasurementRequest request,
        CancellationToken ct = default)
    {
        var validation = Validate(request);
        if (validation is not null) return validation;

        var measurement = await db.BodyMeasurements
            .FirstOrDefaultAsync(x => x.OwnerId == ownerId && x.MeasuredOn == request.MeasuredOn, ct);

        if (measurement is null)
        {
            measurement = new BodyMeasurement
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                MeasuredOn = request.MeasuredOn,
                CreatedAt = clock.UtcNow
            };
            db.BodyMeasurements.Add(measurement);
        }

        Apply(measurement, request);
        await db.SaveChangesAsync(ct);

        return Result<BodyMeasurementDto>.Ok(measurement.ToDto());
    }

    public async Task<Result<BodyMeasurementDto>> UpdateAsync(
        Guid ownerId,
        Guid measurementId,
        SaveBodyMeasurementRequest request,
        CancellationToken ct = default)
    {
        var validation = Validate(request);
        if (validation is not null) return validation;

        var measurement = await db.BodyMeasurements
            .FirstOrDefaultAsync(x => x.Id == measurementId && x.OwnerId == ownerId, ct);

        if (measurement is null) return Result<BodyMeasurementDto>.NotFound("Measurement not found.");

        // Moving an entry onto a date that already has one would violate the per-day rule.
        var clash = await db.BodyMeasurements.AnyAsync(
            x => x.OwnerId == ownerId && x.MeasuredOn == request.MeasuredOn && x.Id != measurementId, ct);

        if (clash) return Result<BodyMeasurementDto>.Conflict("Another entry already exists for that date.");

        measurement.MeasuredOn = request.MeasuredOn;
        Apply(measurement, request);

        await db.SaveChangesAsync(ct);
        return Result<BodyMeasurementDto>.Ok(measurement.ToDto());
    }

    public async Task<Result> DeleteAsync(Guid ownerId, Guid measurementId, CancellationToken ct = default)
    {
        var measurement = await db.BodyMeasurements
            .FirstOrDefaultAsync(x => x.Id == measurementId && x.OwnerId == ownerId, ct);

        if (measurement is null) return Result.NotFound("Measurement not found.");

        db.BodyMeasurements.Remove(measurement);
        await db.SaveChangesAsync(ct);
        return Result.Ok();
    }

    // ---------------------------------------------------------------------------------
    // Progress photos
    // ---------------------------------------------------------------------------------

    public async Task<Result<IReadOnlyList<ProgressPhotoDto>>> ListPhotosAsync(Guid ownerId, CancellationToken ct = default)
    {
        var photos = await db.ProgressPhotos
            .Where(x => x.OwnerId == ownerId)
            .OrderByDescending(x => x.TakenOn)
            .AsNoTracking()
            .ToListAsync(ct);

        return Result<IReadOnlyList<ProgressPhotoDto>>.Ok(photos.Select(x => x.ToDto()).ToList());
    }

    /// <summary>
    /// Stores a progress photo after validating type and size. The storage key is
    /// generated server-side and never derived from the client file name.
    /// </summary>
    public async Task<Result<ProgressPhotoDto>> AddPhotoAsync(
        Guid ownerId,
        DateOnly takenOn,
        PhotoPose pose,
        decimal? weightKg,
        string? notes,
        string fileName,
        string contentType,
        long sizeBytes,
        Stream content,
        CancellationToken ct = default)
    {
        if (!AllowedPhotoTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
            return Result<ProgressPhotoDto>.Invalid(nameof(contentType), "Only JPEG, PNG and WebP images are accepted.");

        if (sizeBytes <= 0 || sizeBytes > MaxPhotoBytes)
            return Result<ProgressPhotoDto>.Invalid(nameof(sizeBytes), $"Image must be between 1 byte and {MaxPhotoBytes / 1024 / 1024} MB.");

        if (weightKg is < 0 or > 500)
            return Result<ProgressPhotoDto>.Invalid(nameof(weightKg), "Weight must be between 0 and 500 kg.");

        var storageKey = await storage.SaveAsync(ownerId, fileName, contentType, content, ct);

        var photo = new ProgressPhoto
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            TakenOn = takenOn,
            Pose = pose,
            StorageKey = storageKey,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            WeightKg = weightKg,
            Notes = notes?.Trim() ?? "",
            CreatedAt = clock.UtcNow
        };

        db.ProgressPhotos.Add(photo);
        await db.SaveChangesAsync(ct);

        return Result<ProgressPhotoDto>.Ok(photo.ToDto());
    }

    /// <summary>
    /// Opens a photo's bytes for the owner only. Ownership is re-checked here rather than
    /// trusting the storage key, which is what keeps photos private.
    /// </summary>
    public async Task<Result<(Stream Content, string ContentType)>> OpenPhotoAsync(
        Guid ownerId,
        Guid photoId,
        CancellationToken ct = default)
    {
        var photo = await db.ProgressPhotos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == photoId && x.OwnerId == ownerId, ct);

        if (photo is null) return Result<(Stream, string)>.NotFound("Photo not found.");

        var content = await storage.OpenAsync(photo.StorageKey, ct);
        return content is null
            ? Result<(Stream, string)>.NotFound("Photo content is unavailable.")
            : Result<(Stream, string)>.Ok((content, photo.ContentType));
    }

    public async Task<Result> DeletePhotoAsync(Guid ownerId, Guid photoId, CancellationToken ct = default)
    {
        var photo = await db.ProgressPhotos.FirstOrDefaultAsync(x => x.Id == photoId && x.OwnerId == ownerId, ct);
        if (photo is null) return Result.NotFound("Photo not found.");

        db.ProgressPhotos.Remove(photo);
        await db.SaveChangesAsync(ct);

        // Remove bytes after the row is gone so a storage failure cannot orphan metadata.
        await storage.DeleteAsync(photo.StorageKey, ct);
        return Result.Ok();
    }

    // ---------------------------------------------------------------------------------
    // Internals
    // ---------------------------------------------------------------------------------

    private Result<BodyMeasurementDto>? Validate(SaveBodyMeasurementRequest request)
    {
        if (request.MeasuredOn > DateOnly.FromDateTime(clock.UtcNow.UtcDateTime).AddDays(1))
            return Result<BodyMeasurementDto>.Invalid(nameof(request.MeasuredOn), "Measurement date cannot be in the future.");

        if (request.WeightKg is < 0 or > 500)
            return Result<BodyMeasurementDto>.Invalid(nameof(request.WeightKg), "Weight must be between 0 and 500 kg.");

        if (request.BodyFatPercent is < 0 or > 70)
            return Result<BodyMeasurementDto>.Invalid(nameof(request.BodyFatPercent), "Body fat must be between 0 and 70 percent.");

        // All circumferences share the same plausible range.
        var circumferences = new[]
        {
            (nameof(request.ChestCm), request.ChestCm),
            (nameof(request.WaistCm), request.WaistCm),
            (nameof(request.HipsCm), request.HipsCm),
            (nameof(request.LeftArmCm), request.LeftArmCm),
            (nameof(request.RightArmCm), request.RightArmCm),
            (nameof(request.LeftThighCm), request.LeftThighCm),
            (nameof(request.RightThighCm), request.RightThighCm),
            (nameof(request.LeftCalfCm), request.LeftCalfCm),
            (nameof(request.RightCalfCm), request.RightCalfCm),
            (nameof(request.ShouldersCm), request.ShouldersCm),
            (nameof(request.NeckCm), request.NeckCm)
        };

        foreach (var (field, value) in circumferences)
        {
            if (value is < 0 or > 300)
                return Result<BodyMeasurementDto>.Invalid(field, "Measurement must be between 0 and 300 cm.");
        }

        return request.Notes is { Length: > 1000 }
            ? Result<BodyMeasurementDto>.Invalid(nameof(request.Notes), "Notes cannot exceed 1000 characters.")
            : null;
    }

    private static void Apply(BodyMeasurement measurement, SaveBodyMeasurementRequest request)
    {
        measurement.WeightKg = request.WeightKg;
        measurement.BodyFatPercent = request.BodyFatPercent;
        measurement.ChestCm = request.ChestCm;
        measurement.WaistCm = request.WaistCm;
        measurement.HipsCm = request.HipsCm;
        measurement.LeftArmCm = request.LeftArmCm;
        measurement.RightArmCm = request.RightArmCm;
        measurement.LeftThighCm = request.LeftThighCm;
        measurement.RightThighCm = request.RightThighCm;
        measurement.LeftCalfCm = request.LeftCalfCm;
        measurement.RightCalfCm = request.RightCalfCm;
        measurement.ShouldersCm = request.ShouldersCm;
        measurement.NeckCm = request.NeckCm;
        measurement.Notes = request.Notes?.Trim() ?? "";
    }
}
