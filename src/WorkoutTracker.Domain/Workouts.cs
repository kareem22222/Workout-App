namespace WorkoutTracker.Domain;

/// <summary>
/// A started, completed or cancelled workout. Sessions are historical records: display
/// names and notes are snapshotted so later edits to routines or exercises cannot
/// retroactively change what was logged (spec 4.1).
/// </summary>
public sealed class WorkoutSession
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }

    /// <summary>Routine the workout was started from, if any. Kept nullable so routines can be deleted.</summary>
    public Guid? RoutineId { get; set; }

    /// <summary>Snapshot of the routine name at start time.</summary>
    public required string Title { get; set; }

    public WorkoutStatus Status { get; set; } = WorkoutStatus.Active;

    public DateTimeOffset StartedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? CompletedAt { get; set; }

    public string Notes { get; set; } = "";

    /// <summary>
    /// Optimistic concurrency token. The client echoes the version it last saw and the
    /// server rejects stale offline writes rather than silently overwriting newer data
    /// (spec 3, Epic 30).
    /// </summary>
    public int Version { get; set; }

    public List<WorkoutExercise> Exercises { get; set; } = [];

    /// <summary>Wall-clock duration derived from the start/finish timestamps (spec 7.5).</summary>
    public TimeSpan? Duration => CompletedAt is { } finished ? finished - StartedAt : null;
}

/// <summary>A snapshot of an exercise as performed within one workout.</summary>
public sealed class WorkoutExercise
{
    public Guid Id { get; set; }

    public Guid WorkoutSessionId { get; set; }
    public WorkoutSession? WorkoutSession { get; set; }

    public Guid ExerciseId { get; set; }
    public Exercise? Exercise { get; set; }

    /// <summary>Snapshot of the exercise name so history survives renames and deletions.</summary>
    public required string ExerciseName { get; set; }

    /// <summary>Snapshot of the exercise type so volume rules stay stable over time.</summary>
    public ExerciseType ExerciseType { get; set; } = ExerciseType.WeightAndReps;

    public int Order { get; set; }

    public int RestSeconds { get; set; } = 90;

    /// <summary>Historical note for this exercise in this workout.</summary>
    public string Notes { get; set; } = "";

    public int? SupersetGroup { get; set; }

    public SupersetKind SupersetKind { get; set; } = SupersetKind.None;

    public List<WorkoutSet> Sets { get; set; } = [];
}

/// <summary>
/// A single logged set. Ordering is explicit and stable, and weights use decimal so
/// fractional plate loads such as 82.5 kg are exact (spec 4.1).
/// </summary>
public sealed class WorkoutSet
{
    public Guid Id { get; set; }

    public Guid WorkoutExerciseId { get; set; }
    public WorkoutExercise? WorkoutExercise { get; set; }

    /// <summary>Explicit, stable ordering within the exercise.</summary>
    public int Order { get; set; }

    /// <summary>Load in canonical kilograms.</summary>
    public decimal Weight { get; set; }

    public int Reps { get; set; }

    /// <summary>Rate of perceived exertion, 1-10 in half steps.</summary>
    public decimal? Rpe { get; set; }

    public WorkoutSetType Type { get; set; } = WorkoutSetType.Normal;

    /// <summary>Duration in seconds for timed movements.</summary>
    public int? DurationSeconds { get; set; }

    /// <summary>Distance in meters for cardio movements.</summary>
    public decimal? DistanceMeters { get; set; }

    /// <summary>Optional historical note for this specific set (spec US-150).</summary>
    public string Notes { get; set; } = "";

    /// <summary>Set when the user marks the set done. Null means planned but not performed.</summary>
    public DateTimeOffset? CompletedAt { get; set; }

    public bool IsCompleted => CompletedAt is not null;

    /// <summary>
    /// Warmup sets are excluded from PR detection and volume by default (spec 7.1/7.2).
    /// </summary>
    public bool IsWorkSet => Type != WorkoutSetType.Warmup;

    /// <summary>A completed work set, the unit of measurement for PRs and volume.</summary>
    public bool CountsTowardRecords => IsCompleted && IsWorkSet && Reps > 0 && Weight > 0;
}

/// <summary>
/// A detected record. Rows are fully reproducible from workout data so historical
/// edits can trigger a deterministic recomputation (spec 7.2).
/// </summary>
public sealed class PersonalRecord
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }

    public Guid ExerciseId { get; set; }
    public Exercise? Exercise { get; set; }

    public PersonalRecordType Type { get; set; }

    /// <summary>The record value: kg, reps, estimated 1RM or volume depending on <see cref="Type"/>.</summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Qualifier for <see cref="PersonalRecordType.MostRepsAtWeight"/>, which is only
    /// comparable at an identical load.
    /// </summary>
    public decimal? AtWeight { get; set; }

    /// <summary>The set that produced the record. Null for workout-level records.</summary>
    public Guid? WorkoutSetId { get; set; }

    public Guid WorkoutSessionId { get; set; }

    /// <summary>When the record was achieved, i.e. the set's completion time.</summary>
    public DateTimeOffset AchievedAt { get; set; }
}
