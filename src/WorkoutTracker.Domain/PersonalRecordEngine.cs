namespace WorkoutTracker.Domain;

/// <summary>
/// Deterministic personal record detection (spec 7.2).
/// <para>
/// Records are always derived from the full set of completed sessions rather than being
/// appended as one-off events. That makes historical corrections safe: recomputing from
/// scratch is the normal path, so an edited or deleted workout cannot leave a stale PR
/// behind (spec US-082, 13.2).
/// </para>
/// </summary>
public static class PersonalRecordEngine
{
    /// <summary>
    /// Rebuilds the complete current-best record set for one user from their completed sessions.
    /// </summary>
    /// <param name="ownerId">Owner the records belong to.</param>
    /// <param name="sessions">All sessions for the user; non-completed ones are ignored.</param>
    /// <param name="formula">The user's configured estimated-1RM formula.</param>
    public static List<PersonalRecord> Recompute(
        Guid ownerId,
        IEnumerable<WorkoutSession> sessions,
        OneRepMaxFormula formula)
    {
        var completed = sessions
            .Where(x => x.Status == WorkoutStatus.Completed)
            // Chronological order makes ties resolve to the earliest achievement.
            .OrderBy(x => x.CompletedAt ?? x.StartedAt)
            .ToList();

        var records = new List<PersonalRecord>();

        foreach (var group in EnumerateWorkSets(completed).GroupBy(x => x.Exercise.ExerciseId))
        {
            var candidates = group.ToList();
            if (candidates.Count == 0) continue;

            records.AddRange(BuildExerciseRecords(ownerId, group.Key, candidates, formula));
        }

        var workoutVolumeRecord = BuildWorkoutVolumeRecord(ownerId, completed);
        if (workoutVolumeRecord is not null) records.Add(workoutVolumeRecord);

        return records;
    }

    /// <summary>
    /// Determines which of the recomputed records were achieved during a specific session,
    /// which is what the post-workout summary celebrates (spec US-070).
    /// </summary>
    public static List<PersonalRecord> RecordsAchievedIn(IEnumerable<PersonalRecord> records, Guid sessionId)
        => records.Where(x => x.WorkoutSessionId == sessionId).ToList();

    /// <summary>Flattens sessions into eligible sets while retaining their parent context.</summary>
    private static IEnumerable<SetContext> EnumerateWorkSets(IEnumerable<WorkoutSession> sessions)
    {
        foreach (var session in sessions)
        {
            foreach (var exercise in session.Exercises)
            {
                foreach (var set in exercise.Sets.Where(x => x.CountsTowardRecords))
                {
                    yield return new SetContext(session, exercise, set);
                }
            }
        }
    }

    private static IEnumerable<PersonalRecord> BuildExerciseRecords(
        Guid ownerId,
        Guid exerciseId,
        List<SetContext> candidates,
        OneRepMaxFormula formula)
    {
        // Heaviest weight lifted for a single completed work set.
        var heaviest = candidates.MaxBySelector(x => x.Set.Weight);
        if (heaviest is not null)
        {
            yield return Record(ownerId, exerciseId, PersonalRecordType.HeaviestWeight, heaviest.Set.Weight, null, heaviest);
        }

        // Best single-set volume (weight x reps).
        var bestSetVolume = candidates
            .Where(x => x.Exercise.ExerciseType is ExerciseType.WeightAndReps or ExerciseType.WeightedBodyweight)
            .MaxBySelector(x => x.Set.Weight * x.Set.Reps);
        if (bestSetVolume is not null)
        {
            var volume = bestSetVolume.Set.Weight * bestSetVolume.Set.Reps;
            yield return Record(ownerId, exerciseId, PersonalRecordType.BestSetVolume, volume, null, bestSetVolume);
        }

        // Highest estimated 1RM across all eligible sets.
        SetContext? bestOneRm = null;
        decimal bestOneRmValue = 0m;
        foreach (var candidate in candidates)
        {
            var estimate = OneRepMax.Estimate(candidate.Set.Weight, candidate.Set.Reps, formula);
            if (estimate is null) continue;
            if (bestOneRm is null || estimate > bestOneRmValue)
            {
                bestOneRm = candidate;
                bestOneRmValue = estimate.Value;
            }
        }
        if (bestOneRm is not null)
        {
            yield return Record(ownerId, exerciseId, PersonalRecordType.BestEstimatedOneRepMax, bestOneRmValue, null, bestOneRm);
        }

        // Most reps achieved at each distinct load. Reps are only comparable at equal weight.
        foreach (var byWeight in candidates.GroupBy(x => x.Set.Weight))
        {
            var mostReps = byWeight.MaxBySelector(x => x.Set.Reps);
            if (mostReps is null) continue;
            yield return Record(ownerId, exerciseId, PersonalRecordType.MostRepsAtWeight,
                mostReps.Set.Reps, byWeight.Key, mostReps);
        }
    }

    /// <summary>Single best whole-session volume across the user's history.</summary>
    private static PersonalRecord? BuildWorkoutVolumeRecord(Guid ownerId, List<WorkoutSession> sessions)
    {
        WorkoutSession? best = null;
        var bestVolume = 0m;

        foreach (var session in sessions)
        {
            var volume = TrainingVolume.ForSession(session);
            if (volume <= 0) continue;
            if (best is null || volume > bestVolume)
            {
                best = session;
                bestVolume = volume;
            }
        }

        if (best is null) return null;

        return new PersonalRecord
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            // Workout-level records are not tied to a single exercise; Guid.Empty marks that.
            ExerciseId = Guid.Empty,
            Type = PersonalRecordType.BestWorkoutVolume,
            Value = decimal.Round(bestVolume, 2, MidpointRounding.AwayFromZero),
            AtWeight = null,
            WorkoutSetId = null,
            WorkoutSessionId = best.Id,
            AchievedAt = best.CompletedAt ?? best.StartedAt
        };
    }

    private static PersonalRecord Record(
        Guid ownerId,
        Guid exerciseId,
        PersonalRecordType type,
        decimal value,
        decimal? atWeight,
        SetContext context) => new()
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            ExerciseId = exerciseId,
            Type = type,
            Value = decimal.Round(value, 2, MidpointRounding.AwayFromZero),
            AtWeight = atWeight,
            WorkoutSetId = context.Set.Id,
            WorkoutSessionId = context.Session.Id,
            AchievedAt = context.Set.CompletedAt ?? context.Session.CompletedAt ?? context.Session.StartedAt
        };

    /// <summary>A set together with the exercise and session it belongs to.</summary>
    private sealed record SetContext(WorkoutSession Session, WorkoutExercise Exercise, WorkoutSet Set);

    /// <summary>
    /// Picks the maximum by a selector, keeping the first element on ties. Candidates are
    /// supplied in chronological order, so ties credit the earliest achievement.
    /// </summary>
    private static SetContext? MaxBySelector(this IEnumerable<SetContext> source, Func<SetContext, decimal> selector)
    {
        SetContext? best = null;
        decimal bestValue = 0m;

        foreach (var item in source)
        {
            var value = selector(item);
            if (value <= 0) continue;
            if (best is null || value > bestValue)
            {
                best = item;
                bestValue = value;
            }
        }

        return best;
    }
}
