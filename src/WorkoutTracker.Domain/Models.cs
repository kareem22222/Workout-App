namespace WorkoutTracker.Domain;

public enum WorkoutStatus { Active, Completed, Cancelled }
public enum WorkoutSetType { Normal, Warmup, DropSet, Failure, Amrap, Backoff }

public sealed class Exercise
{
    public Guid Id { get; set; }
    public Guid? OwnerId { get; set; }
    public required string Name { get; set; }
    public required string Muscle { get; set; }
    public required string Equipment { get; set; }
    public string Instructions { get; set; } = "";
}

public sealed class Routine
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public List<RoutineExercise> Exercises { get; set; } = [];
}

public sealed class RoutineExercise
{
    public Guid Id { get; set; }
    public Guid RoutineId { get; set; }
    public Routine? Routine { get; set; }
    public Guid ExerciseId { get; set; }
    public Exercise? Exercise { get; set; }
    public int Order { get; set; }
    public int RestSeconds { get; set; } = 90;
    public string Notes { get; set; } = "";
    public List<RoutineSetTemplate> Sets { get; set; } = [];
}

public sealed class RoutineSetTemplate
{
    public Guid Id { get; set; }
    public Guid RoutineExerciseId { get; set; }
    public RoutineExercise? RoutineExercise { get; set; }
    public int Order { get; set; }
    public int TargetReps { get; set; } = 8;
    public decimal? TargetWeight { get; set; }
    public WorkoutSetType Type { get; set; }
}

public sealed class WorkoutSession
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public Guid? RoutineId { get; set; }
    public required string Title { get; set; }
    public WorkoutStatus Status { get; set; } = WorkoutStatus.Active;
    public DateTimeOffset StartedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAt { get; set; }
    public string Notes { get; set; } = "";
    public List<WorkoutExercise> Exercises { get; set; } = [];
}

public sealed class WorkoutExercise
{
    public Guid Id { get; set; }
    public Guid WorkoutSessionId { get; set; }
    public WorkoutSession? WorkoutSession { get; set; }
    public Guid ExerciseId { get; set; }
    public required string ExerciseName { get; set; }
    public int Order { get; set; }
    public int RestSeconds { get; set; } = 90;
    public string Notes { get; set; } = "";
    public List<WorkoutSet> Sets { get; set; } = [];
}

public sealed class WorkoutSet
{
    public Guid Id { get; set; }
    public Guid WorkoutExerciseId { get; set; }
    public WorkoutExercise? WorkoutExercise { get; set; }
    public int Order { get; set; }
    public decimal Weight { get; set; }
    public int Reps { get; set; }
    public decimal? Rpe { get; set; }
    public WorkoutSetType Type { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}
