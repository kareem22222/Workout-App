namespace WorkoutTracker.Domain;

/// <summary>
/// A program/folder used to organize routines. Deleting a folder must not delete its
/// routines; they fall back to the ungrouped list (spec US-021).
/// </summary>
public sealed class RoutineFolder
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }

    public required string Name { get; set; }

    /// <summary>Explicit ordering of folders in the library.</summary>
    public int Order { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Routine> Routines { get; set; } = [];
}

/// <summary>A user-owned reusable workout template.</summary>
public sealed class Routine
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }

    public required string Name { get; set; }

    public string Description { get; set; } = "";

    public Guid? FolderId { get; set; }
    public RoutineFolder? Folder { get; set; }

    /// <summary>Explicit ordering within its folder (spec US-021).</summary>
    public int Order { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Soft delete so historical workouts that reference this routine remain intact.</summary>
    public bool IsArchived { get; set; }

    public List<RoutineExercise> Exercises { get; set; } = [];
}

/// <summary>An ordered exercise slot within a routine.</summary>
public sealed class RoutineExercise
{
    public Guid Id { get; set; }

    public Guid RoutineId { get; set; }
    public Routine? Routine { get; set; }

    public Guid ExerciseId { get; set; }
    public Exercise? Exercise { get; set; }

    public int Order { get; set; }

    /// <summary>Per-exercise rest override; the timer uses the most specific value available (spec US-041).</summary>
    public int RestSeconds { get; set; } = 90;

    public string Notes { get; set; } = "";

    /// <summary>
    /// Exercises sharing a non-null group key are performed as a superset. Rest is taken
    /// after the group completes rather than after each exercise (spec US-050).
    /// </summary>
    public int? SupersetGroup { get; set; }

    public SupersetKind SupersetKind { get; set; } = SupersetKind.None;

    public List<RoutineSetTemplate> Sets { get; set; } = [];
}

/// <summary>Target prescription for a single set within a routine exercise.</summary>
public sealed class RoutineSetTemplate
{
    public Guid Id { get; set; }

    public Guid RoutineExerciseId { get; set; }
    public RoutineExercise? RoutineExercise { get; set; }

    public int Order { get; set; }

    /// <summary>Bottom of the target rep range. Equals <see cref="TargetRepsMax"/> for a fixed target.</summary>
    public int TargetReps { get; set; } = 8;

    /// <summary>
    /// Top of the target rep range. Drives the progressive overload rule, which triggers
    /// when every work set reaches this value (spec 7.4).
    /// </summary>
    public int? TargetRepsMax { get; set; }

    /// <summary>Optional prescribed load in kg.</summary>
    public decimal? TargetWeight { get; set; }

    public WorkoutSetType Type { get; set; } = WorkoutSetType.Normal;

    /// <summary>Effective top of the target range, falling back to the fixed target.</summary>
    public int EffectiveTopReps => TargetRepsMax ?? TargetReps;
}

/// <summary>
/// Optional recurring day-of-week routine assignment. A schedule never creates a
/// workout on its own; the user must start it (spec US-140).
/// </summary>
public sealed class WorkoutSchedule
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }

    public Guid RoutineId { get; set; }
    public Routine? Routine { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public bool IsEnabled { get; set; } = true;
}
