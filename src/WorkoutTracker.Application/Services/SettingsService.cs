using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Application.Contracts;
using WorkoutTracker.Domain;

namespace WorkoutTracker.Application.Services;

/// <summary>
/// Profile and preference use cases. Settings and profile rows are created on first
/// access so a freshly registered account always has usable defaults (spec US-003).
/// </summary>
public sealed class SettingsService(IAppDbContext db, IUserDirectory users, IClock clock)
{
    /// <summary>Returns the user's settings, creating the default row if absent.</summary>
    public async Task<UserSetting> GetOrCreateSettingsAsync(Guid ownerId, CancellationToken ct = default)
    {
        var settings = await db.UserSettings.FirstOrDefaultAsync(x => x.OwnerId == ownerId, ct);
        if (settings is not null) return settings;

        settings = new UserSetting { Id = Guid.NewGuid(), OwnerId = ownerId, UpdatedAt = clock.UtcNow };
        db.UserSettings.Add(settings);
        await db.SaveChangesAsync(ct);
        return settings;
    }

    /// <summary>Returns the user's profile, creating the default row if absent.</summary>
    public async Task<UserProfile> GetOrCreateProfileAsync(Guid ownerId, CancellationToken ct = default)
    {
        var profile = await db.UserProfiles.FirstOrDefaultAsync(x => x.OwnerId == ownerId, ct);
        if (profile is not null) return profile;

        profile = new UserProfile { Id = Guid.NewGuid(), OwnerId = ownerId, UpdatedAt = clock.UtcNow };
        db.UserProfiles.Add(profile);
        await db.SaveChangesAsync(ct);
        return profile;
    }

    public async Task<Result<UserSettingsDto>> GetSettingsAsync(Guid ownerId, CancellationToken ct = default)
    {
        var settings = await GetOrCreateSettingsAsync(ownerId, ct);
        return Result<UserSettingsDto>.Ok(settings.ToDto());
    }

    public async Task<Result<UserSettingsDto>> UpdateSettingsAsync(
        Guid ownerId,
        UpdateSettingsRequest request,
        CancellationToken ct = default)
    {
        if (!IsKnownTimeZone(request.TimeZone))
            return Result<UserSettingsDto>.Invalid(nameof(request.TimeZone), "Unknown timezone identifier.");

        if (request.DefaultRestSeconds is < 0 or > 3600)
            return Result<UserSettingsDto>.Invalid(nameof(request.DefaultRestSeconds), "Rest must be between 0 and 3600 seconds.");

        if (request.BarWeightKg is < 0 or > 100)
            return Result<UserSettingsDto>.Invalid(nameof(request.BarWeightKg), "Bar weight must be between 0 and 100 kg.");

        if (request.RoundingIncrementKg is <= 0 or > 50)
            return Result<UserSettingsDto>.Invalid(nameof(request.RoundingIncrementKg), "Rounding increment must be greater than 0 and at most 50 kg.");

        if (request.OverloadIncrementKg is < 0 or > 50)
            return Result<UserSettingsDto>.Invalid(nameof(request.OverloadIncrementKg), "Overload increment must be between 0 and 50 kg.");

        if (request.WeeklyWorkoutGoal is < 0 or > 14)
            return Result<UserSettingsDto>.Invalid(nameof(request.WeeklyWorkoutGoal), "Weekly goal must be between 0 and 14 workouts.");

        var plates = (request.PlateInventoryKg ?? [])
            .Where(x => x > 0 && x <= 100)
            .Distinct()
            .OrderByDescending(x => x)
            .ToList();

        if (plates.Count == 0)
            return Result<UserSettingsDto>.Invalid(nameof(request.PlateInventoryKg), "At least one plate size is required.");

        var warmups = (request.WarmupPercentages ?? [])
            .Where(x => x is > 0 and < 100)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var settings = await GetOrCreateSettingsAsync(ownerId, ct);

        settings.WeightUnit = request.WeightUnit;
        settings.LengthUnit = request.LengthUnit;
        settings.TimeZone = request.TimeZone;
        settings.Theme = request.Theme;
        settings.OneRepMaxFormula = request.OneRepMaxFormula;
        settings.DefaultRestSeconds = request.DefaultRestSeconds;
        settings.AutoStartRestTimer = request.AutoStartRestTimer;
        settings.RestTimerSound = request.RestTimerSound;
        settings.RestTimerVibrate = request.RestTimerVibrate;
        settings.RestTimerNotifications = request.RestTimerNotifications;
        settings.BarWeightKg = request.BarWeightKg;
        settings.PlateInventoryKg = plates;
        settings.RoundingIncrementKg = request.RoundingIncrementKg;
        settings.OverloadIncrementKg = request.OverloadIncrementKg;
        settings.WarmupPercentages = warmups;
        settings.WeeklyWorkoutGoal = request.WeeklyWorkoutGoal;
        settings.UpdatedAt = clock.UtcNow;

        await db.SaveChangesAsync(ct);
        return Result<UserSettingsDto>.Ok(settings.ToDto());
    }

    public async Task<Result<UserProfileDto>> GetProfileAsync(Guid ownerId, CancellationToken ct = default)
    {
        var account = await users.FindAsync(ownerId, ct);
        if (account is null) return Result<UserProfileDto>.NotFound("Account not found.");

        var profile = await GetOrCreateProfileAsync(ownerId, ct);
        var latestWeight = await LatestWeightAsync(ownerId, ct);

        return Result<UserProfileDto>.Ok(new UserProfileDto(
            account.Id,
            account.DisplayName,
            account.Email,
            account.IsAdmin,
            profile.DateOfBirth,
            profile.Gender,
            profile.HeightCm,
            profile.Goal,
            profile.AvatarStorageKey is not null,
            latestWeight));
    }

    public async Task<Result<UserProfileDto>> UpdateProfileAsync(
        Guid ownerId,
        UpdateProfileRequest request,
        CancellationToken ct = default)
    {
        var displayName = request.DisplayName?.Trim() ?? "";
        if (displayName.Length is < 1 or > 60)
            return Result<UserProfileDto>.Invalid(nameof(request.DisplayName), "Display name must be 1 to 60 characters.");

        if (request.HeightCm is < 50 or > 260)
            return Result<UserProfileDto>.Invalid(nameof(request.HeightCm), "Height must be between 50 and 260 cm.");

        if (request.DateOfBirth is { } dob && dob > DateOnly.FromDateTime(clock.UtcNow.UtcDateTime))
            return Result<UserProfileDto>.Invalid(nameof(request.DateOfBirth), "Date of birth cannot be in the future.");

        if (!await users.UpdateDisplayNameAsync(ownerId, displayName, ct))
            return Result<UserProfileDto>.NotFound("Account not found.");

        var profile = await GetOrCreateProfileAsync(ownerId, ct);
        profile.DateOfBirth = request.DateOfBirth;
        profile.Gender = string.IsNullOrWhiteSpace(request.Gender) ? null : request.Gender.Trim();
        profile.HeightCm = request.HeightCm;
        profile.Goal = request.Goal;
        profile.UpdatedAt = clock.UtcNow;

        await db.SaveChangesAsync(ct);
        return await GetProfileAsync(ownerId, ct);
    }

    /// <summary>Most recent recorded body weight, used on the profile and dashboard.</summary>
    private async Task<decimal?> LatestWeightAsync(Guid ownerId, CancellationToken ct)
        => await db.BodyMeasurements
            .Where(x => x.OwnerId == ownerId && x.WeightKg != null)
            .OrderByDescending(x => x.MeasuredOn)
            .Select(x => x.WeightKg)
            .FirstOrDefaultAsync(ct);

    /// <summary>
    /// Validates an IANA/Windows timezone id. Unknown ids are rejected so history
    /// grouping cannot silently fall back to UTC.
    /// </summary>
    private static bool IsKnownTimeZone(string? id)
    {
        if (string.IsNullOrWhiteSpace(id)) return false;
        if (string.Equals(id, "UTC", StringComparison.OrdinalIgnoreCase)) return true;
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(id);
            return true;
        }
        catch (TimeZoneNotFoundException) { return false; }
        catch (InvalidTimeZoneException) { return false; }
    }
}
