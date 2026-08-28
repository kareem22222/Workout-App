using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Application.Contracts;
using WorkoutTracker.Domain;

namespace WorkoutTracker.Application.Services;

/// <summary>
/// Persists personal records derived by <see cref="PersonalRecordEngine"/>.
/// <para>
/// Records are stored as a fully replaceable projection of workout history. Any change to
/// a completed workout triggers a full recompute for that user, which is what makes
/// historical corrections safe (spec 7.2, US-082).
/// </para>
/// </summary>
public sealed class PersonalRecordService(IAppDbContext db, SettingsService settings)
{
    /// <summary>
    /// Rebuilds and persists every record for the user, returning the fresh set.
    /// Caller is responsible for the surrounding unit of work.
    /// </summary>
    public async Task<List<PersonalRecord>> RecomputeAsync(Guid ownerId, CancellationToken ct = default)
    {
        var sessions = await db.WorkoutSessions
            .Where(x => x.OwnerId == ownerId && x.Status == WorkoutStatus.Completed)
            .Include(x => x.Exercises).ThenInclude(x => x.Sets)
            .AsNoTracking()
            .ToListAsync(ct);

        var formula = (await settings.GetOrCreateSettingsAsync(ownerId, ct)).OneRepMaxFormula;
        var recomputed = PersonalRecordEngine.Recompute(ownerId, sessions, formula);

        var existing = await db.PersonalRecords.Where(x => x.OwnerId == ownerId).ToListAsync(ct);
        db.PersonalRecords.RemoveRange(existing);
        db.PersonalRecords.AddRange(recomputed);

        await db.SaveChangesAsync(ct);
        return recomputed;
    }

    /// <summary>
    /// Records achieved in a specific session, used for the post-workout celebration.
    /// Only the headline record types are surfaced; "most reps at weight" fires so often
    /// that including it would make the summary noisy.
    /// </summary>
    public async Task<List<PersonalRecordDto>> RecordsForSessionAsync(
        Guid ownerId,
        Guid sessionId,
        IEnumerable<PersonalRecord> records,
        CancellationToken ct = default)
    {
        var achieved = records
            .Where(x => x.WorkoutSessionId == sessionId)
            .Where(x => x.Type is PersonalRecordType.HeaviestWeight
                or PersonalRecordType.BestEstimatedOneRepMax
                or PersonalRecordType.BestSetVolume
                or PersonalRecordType.BestWorkoutVolume)
            .ToList();

        return await AttachNamesAsync(achieved, ct);
    }

    public async Task<Result<IReadOnlyList<PersonalRecordDto>>> ListAsync(
        Guid ownerId,
        Guid? exerciseId = null,
        CancellationToken ct = default)
    {
        var query = db.PersonalRecords.Where(x => x.OwnerId == ownerId);
        if (exerciseId is { } id) query = query.Where(x => x.ExerciseId == id);

        var records = await query
            .OrderByDescending(x => x.AchievedAt)
            .AsNoTracking()
            .ToListAsync(ct);

        return Result<IReadOnlyList<PersonalRecordDto>>.Ok(await AttachNamesAsync(records, ct));
    }

    /// <summary>Most recently achieved records for the dashboard.</summary>
    public async Task<List<PersonalRecordDto>> RecentAsync(Guid ownerId, int take, CancellationToken ct = default)
    {
        var records = await db.PersonalRecords
            .Where(x => x.OwnerId == ownerId && x.Type != PersonalRecordType.MostRepsAtWeight)
            .OrderByDescending(x => x.AchievedAt)
            .Take(take)
            .AsNoTracking()
            .ToListAsync(ct);

        return await AttachNamesAsync(records, ct);
    }

    /// <summary>
    /// Resolves exercise names in one query. Workout-volume records are not tied to an
    /// exercise, so they get a descriptive label instead.
    /// </summary>
    private async Task<List<PersonalRecordDto>> AttachNamesAsync(List<PersonalRecord> records, CancellationToken ct)
    {
        if (records.Count == 0) return [];

        var exerciseIds = records.Select(x => x.ExerciseId).Where(x => x != Guid.Empty).Distinct().ToList();

        var names = exerciseIds.Count == 0
            ? []
            : await db.Exercises
                .Where(x => exerciseIds.Contains(x.Id))
                .AsNoTracking()
                .ToDictionaryAsync(x => x.Id, x => x.Name, ct);

        return records
            .Select(record => record.ToDto(
                record.ExerciseId == Guid.Empty
                    ? "Whole workout"
                    : names.GetValueOrDefault(record.ExerciseId, "Removed exercise")))
            .ToList();
    }
}
