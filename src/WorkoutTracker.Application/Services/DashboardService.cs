using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Application.Contracts;
using WorkoutTracker.Domain;

namespace WorkoutTracker.Application.Services;

/// <summary>
/// Builds the home screen in a single call. The dashboard is the most frequently loaded
/// screen, so it deliberately aggregates server-side rather than making the client fan
/// out into many requests (spec US-190).
/// </summary>
public sealed class DashboardService(
    IAppDbContext db,
    SettingsService settings,
    WorkoutService workouts,
    RoutineService routines,
    PersonalRecordService records,
    ProgressService progress,
    IUserDirectory users,
    IClock clock)
{
    private const int RecentWorkoutCount = 5;
    private const int RecentRecordCount = 5;

    public async Task<Result<DashboardSummaryDto>> GetSummaryAsync(Guid ownerId, CancellationToken ct = default)
    {
        var account = await users.FindAsync(ownerId, ct);
        if (account is null) return Result<DashboardSummaryDto>.NotFound("Account not found.");

        var userSettings = await settings.GetOrCreateSettingsAsync(ownerId, ct);
        var zone = WorkoutService.ResolveTimeZone(userSettings.TimeZone);

        var localNow = TimeZoneInfo.ConvertTime(clock.UtcNow, zone);
        var weekStartLocal = ProgressService.StartOfWeek(localNow);
        var weekStartUtc = weekStartLocal.ToUniversalTime();

        var thisWeek = await db.WorkoutSessions
            .Where(x => x.OwnerId == ownerId && x.Status == WorkoutStatus.Completed && x.StartedAt >= weekStartUtc)
            .Include(x => x.Exercises).ThenInclude(x => x.Sets)
            .AsNoTracking()
            .ToListAsync(ct);

        var recent = await db.WorkoutSessions
            .Where(x => x.OwnerId == ownerId && x.Status == WorkoutStatus.Completed)
            .OrderByDescending(x => x.CompletedAt ?? x.StartedAt)
            .Take(RecentWorkoutCount)
            .Include(x => x.Exercises.OrderBy(e => e.Order)).ThenInclude(x => x.Sets)
            .AsNoTracking()
            .ToListAsync(ct);

        var active = await workouts.GetActiveAsync(ownerId, ct);
        var (nextRoutine, nextDay) = await ResolveNextScheduledAsync(ownerId, localNow, ct);
        var (latestWeight, latestOn, change) = await WeightTrendAsync(ownerId, ct);

        return Result<DashboardSummaryDto>.Ok(new DashboardSummaryDto(
            account.DisplayName,
            active.Value,
            nextRoutine,
            nextDay,
            thisWeek.Count,
            userSettings.WeeklyWorkoutGoal,
            decimal.Round(thisWeek.Sum(TrainingVolume.ForSession), 2, MidpointRounding.AwayFromZero),
            (int)thisWeek.Sum(x => (x.Duration ?? TimeSpan.Zero).TotalMinutes),
            await progress.GetStreakWeeksAsync(ownerId, ct),
            await records.RecentAsync(ownerId, RecentRecordCount, ct),
            recent.Select(x => x.ToSummaryRow()).ToList(),
            latestWeight,
            latestOn,
            change));
    }

    /// <summary>
    /// Finds the next enabled scheduled routine, searching today first then forward
    /// through the week. A schedule never auto-creates a workout (spec US-140).
    /// </summary>
    private async Task<(RoutineDto? Routine, DayOfWeek? Day)> ResolveNextScheduledAsync(
        Guid ownerId,
        DateTimeOffset localNow,
        CancellationToken ct)
    {
        var schedules = await db.WorkoutSchedules
            .Where(x => x.OwnerId == ownerId && x.IsEnabled)
            .AsNoTracking()
            .ToListAsync(ct);

        if (schedules.Count == 0) return (null, null);

        for (var offset = 0; offset < 7; offset++)
        {
            var day = localNow.AddDays(offset).DayOfWeek;
            var match = schedules.FirstOrDefault(x => x.DayOfWeek == day);
            if (match is null) continue;

            var routine = await routines.GetAsync(ownerId, match.RoutineId, ct);
            if (routine.Succeeded) return (routine.Value, day);
        }

        return (null, null);
    }

    /// <summary>Latest body weight plus the change over the trailing 30 days.</summary>
    private async Task<(decimal? Latest, DateOnly? On, decimal? Change)> WeightTrendAsync(Guid ownerId, CancellationToken ct)
    {
        var entries = await db.BodyMeasurements
            .Where(x => x.OwnerId == ownerId && x.WeightKg != null)
            .OrderByDescending(x => x.MeasuredOn)
            .Select(x => new { x.MeasuredOn, x.WeightKg })
            .Take(120)
            .AsNoTracking()
            .ToListAsync(ct);

        if (entries.Count == 0) return (null, null, null);

        var latest = entries[0];
        var cutoff = latest.MeasuredOn.AddDays(-30);

        // Compare against the oldest reading still inside the 30-day window.
        var baseline = entries.LastOrDefault(x => x.MeasuredOn >= cutoff && x.MeasuredOn < latest.MeasuredOn);

        var change = baseline is null ? null : latest.WeightKg - baseline.WeightKg;
        return (latest.WeightKg, latest.MeasuredOn, change);
    }
}
