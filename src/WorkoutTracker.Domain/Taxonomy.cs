namespace WorkoutTracker.Domain;

/// <summary>Canonical muscle taxonomy. Global reference data, not user owned.</summary>
public sealed class Muscle
{
    public Guid Id { get; set; }

    /// <summary>Stable machine-readable key used by seed upserts.</summary>
    public required string Slug { get; set; }

    public required string Name { get; set; }

    /// <summary>Coarse body region, e.g. Chest, Back, Legs, Arms, Core.</summary>
    public required string BodyRegion { get; set; }

    public ICollection<ExerciseMuscle> Exercises { get; set; } = [];
}

/// <summary>Canonical equipment taxonomy. Global reference data, not user owned.</summary>
public sealed class Equipment
{
    public Guid Id { get; set; }

    /// <summary>Stable machine-readable key used by seed upserts.</summary>
    public required string Slug { get; set; }

    public required string Name { get; set; }

    /// <summary>Default bar/implement weight in kg, when the equipment implies one (e.g. 20 kg barbell).</summary>
    public decimal? DefaultBarWeightKg { get; set; }

    public ICollection<Exercise> Exercises { get; set; } = [];
}

/// <summary>
/// A built-in or user-created movement. Built-in exercises have a null <see cref="OwnerId"/>
/// and are visible to everyone; user-created exercises are private to their owner.
/// </summary>
public sealed class Exercise
{
    public Guid Id { get; set; }

    /// <summary>Null for the built-in catalog, otherwise the owning user.</summary>
    public Guid? OwnerId { get; set; }

    public required string Name { get; set; }

    public string Instructions { get; set; } = "";

    public ExerciseType Type { get; set; } = ExerciseType.WeightAndReps;

    /// <summary>Free-form grouping such as Push, Pull, Legs, Olympic, Cardio.</summary>
    public string Category { get; set; } = "";

    public Guid? EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }

    /// <summary>Optional non-copyrighted media URL supplied by the user.</summary>
    public string? MediaUrl { get; set; }

    /// <summary>Default rest in seconds when this exercise is added to a workout.</summary>
    public int DefaultRestSeconds { get; set; } = 90;

    /// <summary>Smallest load step achievable for this movement, used for rounding suggestions.</summary>
    public decimal DefaultIncrementKg { get; set; } = 2.5m;

    /// <summary>
    /// Soft delete. Retained so historical workouts referencing the exercise stay meaningful (spec 4.1).
    /// </summary>
    public bool IsArchived { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<ExerciseMuscle> Muscles { get; set; } = [];

    /// <summary>True when the exercise tracks an external load and can contribute to weight volume.</summary>
    public bool TracksLoad => Type is ExerciseType.WeightAndReps or ExerciseType.WeightedBodyweight;
}

/// <summary>
/// Exercise-to-muscle relation carrying role and a configurable contribution weight so the
/// muscle summary can be tuned without code changes (spec US-110).
/// </summary>
public sealed class ExerciseMuscle
{
    public Guid Id { get; set; }

    public Guid ExerciseId { get; set; }
    public Exercise? Exercise { get; set; }

    public Guid MuscleId { get; set; }
    public Muscle? Muscle { get; set; }

    public MuscleRole Role { get; set; } = MuscleRole.Primary;

    /// <summary>Fraction of a set's work attributed to this muscle. Primary defaults to 1.0, secondary to 0.4.</summary>
    public decimal ContributionWeight { get; set; } = 1.0m;
}

/// <summary>
/// A persistent, user-specific note that reappears the next time the exercise is trained
/// (spec US-150). Distinct from the historical per-workout note.
/// </summary>
public sealed class ExerciseNote
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }

    public Guid ExerciseId { get; set; }
    public Exercise? Exercise { get; set; }

    public string Text { get; set; } = "";

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
