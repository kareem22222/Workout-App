using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Application.Contracts;
using WorkoutTracker.Domain;

namespace WorkoutTracker.Application.Services;

/// <summary>
/// Analytics and training-aid use cases: progress charts, statistics, and the plate,
/// warmup and progressive-overload helpers (spec Epics 13, 14, 17, 21, 22).
/// </summary>
public sealed class ProgressService(
    IAppDbContext db,
    SettingsService settings,
    PersonalRecordService records,
    IClock clock)
{
    /// <summary>
    /// Chart ranges offered by the UI, mapped to a lookback window. "all" has no bound.
    /// </summary>
    private static readonly Dictionary<string, int?> RangeDays = new(StringComparer.OrdinalIgnoreCase)
    {
        ["1m"] = 30,
        ["3m"] = 90,
        ["6m"] = 180,
        ["1y"] = 365,
        ["all"] = null
    };

    /// <summary>
    /// Series for one exercise: best weight, estimated 1RM, volume and max reps per
    /// session within the requested range (spec US-098).
    /// </summary>
    public async Task<Result<ExerciseProgressDto>> GetExerciseProgressAsync(
        Guid ownerId,
        Guid exerciseId,
        string range = "3m",
        CancellationToken ct = default)
    {
        if (!RangeDays.TryGetValue(range, out var days))
            return Result<ExerciseProgressDto>.Invalid(nameof(range), "Range must be one of 1m, 3m, 6m, 1y, all.");

        var exercise = await db.Exercises
            .Where(x => x.Id == exerciseId && (x.OwnerId == null || x.OwnerId == ownerId))
            .Select(x => new { x.Id, x.Name })
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (exercise is null) return Result<ExerciseProgressDto>.NotFound("Exercise not found.");

        var query = db.WorkoutSessions
            .Where(x => x.OwnerId == ownerId
                && x.Status == WorkoutStatus.Completed
                && x.Exercises.Any(e => e.ExerciseId == exerciseId));

        if (days is { } window)
        {
            var from = clock.UtcNow.AddDays(-window);
            query = query.Where(x => x.StartedAt >= from);
        }

        var sessions = await query
            .OrderBy(x => x.CompletedAt ?? x.StartedAt)
            .Include(x => x.Exercises.Where(e => e.ExerciseId == exerciseId)).ThenInclude(x => x.Sets)
            .AsNoTracking()
            .ToListAsync(ct);

        var formula = (await settings.GetOrCreateSettingsAsync(ownerId, ct)).OneRepMaxFormula;

        var bestWeight = new List<ChartPointDto>();
        var oneRepMax = new List<ChartPointDto>();
        var volume = new List<ChartPointDto>();
        var maxReps = new List<ChartPointDto>();

        foreach (var session in sessions)
        {
            var performedAt = session.CompletedAt ?? session.StartedAt;

            foreach (var workoutExercise in session.Exercises)
            {
                var eligible = workoutExercise.Sets.Where(x => x.CountsTowardRecords).ToList();
                if (eligible.Count == 0) continue;

                bestWeight.Add(new ChartPointDto(performedAt, eligible.Max(x => x.Weight)));
                maxReps.Add(new ChartPointDto(performedAt, eligible.Max(x => x.Reps)));

                var sessionVolume = TrainingVolume.ForExercise(workoutExercise);
                if (sessionVolume > 0) volume.Add(new ChartPointDto(performedAt, sessionVolume));

                var estimate = OneRepMax.BestEstimate(eligible, formula);
                if (estimate is { } value) oneRepMax.Add(new ChartPointDto(performedAt, value));
            }
        }

        var exerciseRecords = await records.ListAsync(ownerId, exerciseId, ct);

        return Result<ExerciseProgressDto>.Ok(new ExerciseProgressDto(
            exercise.Id,
            exercise.Name,
            range.ToLowerInvariant(),
            bestWeight,
            oneRepMax,
            volume,
            maxReps,
            exerciseRecords.Value ?? []));
    }

    /// <summary>
    /// Weekly or monthly training statistics. Cancelled workouts are excluded
    /// (spec US-120).
    /// </summary>
    public async Task<Result<TrainingStatsDto>> GetStatsAsync(
        Guid ownerId,
        string range = "3m",
        string groupBy = "week",
        CancellationToken ct = default)
    {
        if (!RangeDays.TryGetValue(range, out var days))
            return Result<TrainingStatsDto>.Invalid(nameof(range), "Range must be one of 1m, 3m, 6m, 1y, all.");

        var byMonth = string.Equals(groupBy, "month", StringComparison.OrdinalIgnoreCase);
        var userSettings = await settings.GetOrCreateSettingsAsync(ownerId, ct);
        var zone = WorkoutService.ResolveTimeZone(userSettings.TimeZone);

        var from = days is { } window ? clock.UtcNow.AddDays(-window) : DateTimeOffset.UnixEpoch;

        var sessions = await db.WorkoutSessions
            .Where(x => x.OwnerId == ownerId && x.Status == WorkoutStatus.Completed && x.StartedAt >= from)
            .Include(x => x.Exercises).ThenInclude(x => x.Sets)
            .AsNoTracking()
            .ToListAsync(ct);

        var buckets = new Dictionary<DateTimeOffset, (decimal Volume, int Workouts, int Sets)>();

        foreach (var session in sessions)
        {
            var local = TimeZoneInfo.ConvertTime(session.CompletedAt ?? session.StartedAt, zone);
            var key = byMonth ? StartOfMonth(local) : StartOfWeek(local);

            var volume = TrainingVolume.ForSession(session);
            var sets = TrainingVolume.CompletedWorkSets(session);

            buckets[key] = buckets.TryGetValue(key, out var current)
                ? (current.Volume + volume, current.Workouts + 1, current.Sets + sets)
                : (volume, 1, sets);
        }

        var series = buckets
            .OrderBy(x => x.Key)
            .Select(x => new VolumePointDto(x.Key, decimal.Round(x.Value.Volume, 2, MidpointRounding.AwayFromZero), x.Value.Workouts, x.Value.Sets))
            .ToList();

        var recordCount = await db.PersonalRecords
            .CountAsync(x => x.OwnerId == ownerId && x.AchievedAt >= from && x.Type != PersonalRecordType.MostRepsAtWeight, ct);

        return Result<TrainingStatsDto>.Ok(new TrainingStatsDto(
            from,
            clock.UtcNow,
            sessions.Count,
            (int)sessions.Sum(x => (x.Duration ?? TimeSpan.Zero).TotalMinutes),
            decimal.Round(sessions.Sum(TrainingVolume.ForSession), 2, MidpointRounding.AwayFromZero),
            sessions.Sum(TrainingVolume.CompletedWorkSets),
            sessions.Sum(TrainingVolume.CompletedReps),
            sessions.SelectMany(x => x.Exercises).Select(x => x.ExerciseId).Distinct().Count(),
            recordCount,
            await GetStreakWeeksAsync(ownerId, ct),
            series));
    }

    /// <summary>
    /// Aggregated training volume over time, independent of any single exercise
    /// (spec /api/progress/volume).
    /// </summary>
    public async Task<Result<IReadOnlyList<VolumePointDto>>> GetVolumeAsync(
        Guid ownerId,
        string range = "3m",
        string groupBy = "week",
        CancellationToken ct = default)
    {
        var stats = await GetStatsAsync(ownerId, range, groupBy, ct);
        return stats.Succeeded
            ? Result<IReadOnlyList<VolumePointDto>>.Ok(stats.Value!.Series)
            : Result<IReadOnlyList<VolumePointDto>>.Invalid(stats.Message ?? "Invalid request.");
    }

    /// <summary>
    /// Estimated 1RM trend for every exercise the user has trained, newest value per
    /// exercise (spec /api/progress/estimated-one-rep-max).
    /// </summary>
    public async Task<Result<IReadOnlyList<OverloadSuggestionDto>>> GetOneRepMaxSummaryAsync(
        Guid ownerId,
        CancellationToken ct = default)
    {
        var formula = (await settings.GetOrCreateSettingsAsync(ownerId, ct)).OneRepMaxFormula;

        var sessions = await db.WorkoutSessions
            .Where(x => x.OwnerId == ownerId && x.Status == WorkoutStatus.Completed)
            .Include(x => x.Exercises).ThenInclude(x => x.Sets)
            .AsNoTracking()
            .ToListAsync(ct);

        var results = sessions
            .SelectMany(x => x.Exercises)
            .GroupBy(x => new { x.ExerciseId, x.ExerciseName })
            .Select(group =>
            {
                var best = OneRepMax.BestEstimate(group.SelectMany(x => x.Sets), formula);
                return new OverloadSuggestionDto(
                    group.Key.ExerciseId,
                    group.Key.ExerciseName,
                    best is null ? OverloadAction.NotEnoughData : OverloadAction.Maintain,
                    best,
                    null,
                    best is null
                        ? "No set in a reliable rep range yet."
                        : $"Best estimated 1RM is {best:0.##} kg using the {formula} formula.");
            })
            .OrderByDescending(x => x.SuggestedWeightKg ?? 0)
            .ToList();

        return Result<IReadOnlyList<OverloadSuggestionDto>>.Ok(results);
    }

    /// <summary>
    /// Consecutive weeks, counting back from the current week, containing at least one
    /// completed workout.
    /// <para>
    /// The current week does not break the streak while it is still in progress: a streak
    /// only ends at a fully elapsed week with no training.
    /// </para>
    /// </summary>
    public async Task<int> GetStreakWeeksAsync(Guid ownerId, CancellationToken ct = default)
    {
        var userSettings = await settings.GetOrCreateSettingsAsync(ownerId, ct);
        var zone = WorkoutService.ResolveTimeZone(userSettings.TimeZone);

        var dates = await db.WorkoutSessions
            .Where(x => x.OwnerId == ownerId && x.Status == WorkoutStatus.Completed)
            .Select(x => x.CompletedAt ?? x.StartedAt)
            .ToListAsync(ct);

        if (dates.Count == 0) return 0;

        var trainedWeeks = dates
            .Select(x => StartOfWeek(TimeZoneInfo.ConvertTime(x, zone)))
            .ToHashSet();

        var cursor = StartOfWeek(TimeZoneInfo.ConvertTime(clock.UtcNow, zone));
        var streak = 0;

        // Skip an untrained current week so an in-progress week cannot zero the streak.
        if (!trainedWeeks.Contains(cursor)) cursor = cursor.AddDays(-7);

        while (trainedWeeks.Contains(cursor))
        {
            streak++;
            cursor = cursor.AddDays(-7);
        }

        return streak;
    }

    // ---------------------------------------------------------------------------------
    // Training aids
    // ---------------------------------------------------------------------------------

    /// <summary>Plates to load for a target weight using the user's own inventory.</summary>
    public async Task<Result<PlateSolutionDto>> GetPlatesAsync(
        Guid ownerId,
        decimal targetKg,
        decimal? barKg = null,
        CancellationToken ct = default)
    {
        if (targetKg is <= 0 or > 2000)
            return Result<PlateSolutionDto>.Invalid(nameof(targetKg), "Target must be between 0 and 2000 kg.");

        var userSettings = await settings.GetOrCreateSettingsAsync(ownerId, ct);
        var bar = barKg ?? userSettings.BarWeightKg;

        var solution = PlateCalculator.Solve(targetKg, bar, userSettings.PlateInventoryKg);
        return Result<PlateSolutionDto>.Ok(solution.ToDto());
    }

    /// <summary>Percentage-based warmup ramp for a working weight.</summary>
    public async Task<Result<IReadOnlyList<WarmupSetDto>>> GetWarmupAsync(
        Guid ownerId,
        decimal workingWeightKg,
        int workingReps = 8,
        CancellationToken ct = default)
    {
        if (workingWeightKg is <= 0 or > 2000)
            return Result<IReadOnlyList<WarmupSetDto>>.Invalid(nameof(workingWeightKg), "Working weight must be between 0 and 2000 kg.");

        var userSettings = await settings.GetOrCreateSettingsAsync(ownerId, ct);

        var sets = WarmupCalculator.Build(
            workingWeightKg,
            userSettings.WarmupPercentages,
            userSettings.RoundingIncrementKg,
            userSettings.BarWeightKg,
            workingReps);

        return Result<IReadOnlyList<WarmupSetDto>>.Ok(sets.Select(x => x.ToDto()).ToList());
    }

    /// <summary>
    /// Load suggestion for the next session of an exercise, based on the most recent
    /// completed work sets against the routine's target rep range (spec US-170).
    /// </summary>
    public async Task<Result<OverloadSuggestionDto>> GetOverloadSuggestionAsync(
        Guid ownerId,
        Guid exerciseId,
        CancellationToken ct = default)
    {
        var exercise = await db.Exercises
            .Where(x => x.Id == exerciseId && (x.OwnerId == null || x.OwnerId == ownerId))
            .Select(x => new { x.Id, x.Name, x.DefaultIncrementKg })
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (exercise is null) return Result<OverloadSuggestionDto>.NotFound("Exercise not found.");

        var userSettings = await settings.GetOrCreateSettingsAsync(ownerId, ct);

        var latest = await db.WorkoutExercises
            .Where(x => x.ExerciseId == exerciseId
                && x.WorkoutSession!.OwnerId == ownerId
                && x.WorkoutSession.Status == WorkoutStatus.Completed)
            .OrderByDescending(x => x.WorkoutSession!.CompletedAt)
            .Include(x => x.Sets)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        // Prefer the rep target the user actually programmed for this exercise.
        var topOfRange = await db.RoutineSetTemplates
            .Where(x => x.RoutineExercise!.ExerciseId == exerciseId && x.RoutineExercise.Routine!.OwnerId == ownerId)
            .Select(x => x.TargetRepsMax ?? x.TargetReps)
            .OrderByDescending(x => x)
            .FirstOrDefaultAsync(ct);

        if (topOfRange == 0) topOfRange = 8;

        var increment = userSettings.OverloadIncrementKg > 0 ? userSettings.OverloadIncrementKg : exercise.DefaultIncrementKg;

        var suggestion = OverloadAdvisor.Suggest(
            latest?.Sets.ToList() ?? [],
            topOfRange,
            increment,
            userSettings.RoundingIncrementKg);

        return Result<OverloadSuggestionDto>.Ok(new OverloadSuggestionDto(
            exercise.Id,
            exercise.Name,
            suggestion.Action,
            suggestion.SuggestedWeightKg,
            suggestion.PreviousWeightKg,
            suggestion.Rationale));
    }

    /// <summary>Ranked muscle contribution across a date range (spec US-110).</summary>
    public async Task<Result<IReadOnlyList<MuscleContributionDto>>> GetMuscleBreakdownAsync(
        Guid ownerId,
        string range = "1m",
        CancellationToken ct = default)
    {
        if (!RangeDays.TryGetValue(range, out var days))
            return Result<IReadOnlyList<MuscleContributionDto>>.Invalid(nameof(range), "Range must be one of 1m, 3m, 6m, 1y, all.");

        var from = days is { } window ? clock.UtcNow.AddDays(-window) : DateTimeOffset.UnixEpoch;

        var sessions = await db.WorkoutSessions
            .Where(x => x.OwnerId == ownerId && x.Status == WorkoutStatus.Completed && x.StartedAt >= from)
            .Include(x => x.Exercises).ThenInclude(x => x.Sets)
            .AsNoTracking()
            .ToListAsync(ct);

        var exerciseIds = sessions.SelectMany(x => x.Exercises).Select(x => x.ExerciseId).Distinct().ToList();
        if (exerciseIds.Count == 0) return Result<IReadOnlyList<MuscleContributionDto>>.Ok([]);

        var mappings = await db.ExerciseMuscles
            .Where(x => exerciseIds.Contains(x.ExerciseId))
            .Include(x => x.Muscle)
            .AsNoTracking()
            .ToListAsync(ct);

        var totals = new Dictionary<Guid, (string Name, string Region, decimal Score, int Sets)>();

        foreach (var exercise in sessions.SelectMany(x => x.Exercises))
        {
            var completed = exercise.Sets.Count(x => x.IsCompleted && x.IsWorkSet);
            if (completed == 0) continue;

            var volume = TrainingVolume.ForExercise(exercise);
            var weight = volume > 0 ? volume : completed * 100m;

            foreach (var mapping in mappings.Where(x => x.ExerciseId == exercise.ExerciseId && x.Muscle is not null))
            {
                var score = weight * mapping.ContributionWeight;
                var setShare = (int)Math.Round(completed * (double)mapping.ContributionWeight, MidpointRounding.AwayFromZero);

                totals[mapping.MuscleId] = totals.TryGetValue(mapping.MuscleId, out var current)
                    ? (current.Name, current.Region, current.Score + score, current.Sets + setShare)
                    : (mapping.Muscle!.Name, mapping.Muscle.BodyRegion, score, setShare);
            }
        }

        var ranked = totals.Values
            .OrderByDescending(x => x.Score)
            .Select(x => new MuscleContributionDto(x.Name, x.Region, decimal.Round(x.Score, 2, MidpointRounding.AwayFromZero), x.Sets))
            .ToList();

        return Result<IReadOnlyList<MuscleContributionDto>>.Ok(ranked);
    }

    /// <summary>Monday-based start of the local week, used consistently for weekly buckets.</summary>
    internal static DateTimeOffset StartOfWeek(DateTimeOffset value)
    {
        var offsetFromMonday = ((int)value.DayOfWeek + 6) % 7;
        var date = value.Date.AddDays(-offsetFromMonday);
        return new DateTimeOffset(date, value.Offset);
    }

    internal static DateTimeOffset StartOfMonth(DateTimeOffset value)
        => new(new DateTime(value.Year, value.Month, 1), value.Offset);
}
