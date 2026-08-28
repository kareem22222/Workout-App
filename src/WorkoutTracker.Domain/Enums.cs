namespace WorkoutTracker.Domain;

/// <summary>Lifecycle of a workout session (spec 4.1).</summary>
public enum WorkoutStatus
{
    Active = 0,
    Completed = 1,
    Cancelled = 2
}

/// <summary>Set classification. Warmups are excluded from PR/volume by default (spec 7.1/7.2).</summary>
public enum WorkoutSetType
{
    Normal = 0,
    Warmup = 1,
    DropSet = 2,
    Failure = 3,
    Amrap = 4,
    Backoff = 5
}

/// <summary>How an exercise is measured, which drives volume rules (spec 7.1).</summary>
public enum ExerciseType
{
    /// <summary>Weight x reps. Standard resistance training.</summary>
    WeightAndReps = 0,

    /// <summary>Reps only, no external load implied.</summary>
    BodyweightReps = 1,

    /// <summary>Bodyweight movement that accepts added external load.</summary>
    WeightedBodyweight = 2,

    /// <summary>Duration based, e.g. plank.</summary>
    Duration = 3,

    /// <summary>Distance and duration based, e.g. running.</summary>
    Cardio = 4
}

/// <summary>Role an exercise plays for a muscle, used for contribution weighting (spec Epic 16).</summary>
public enum MuscleRole
{
    Primary = 0,
    Secondary = 1
}

/// <summary>Detected record categories (spec Epic 12).</summary>
public enum PersonalRecordType
{
    HeaviestWeight = 0,
    MostRepsAtWeight = 1,
    BestEstimatedOneRepMax = 2,
    BestSetVolume = 3,
    BestWorkoutVolume = 4
}

/// <summary>Estimated 1RM formulas (spec 7.3).</summary>
public enum OneRepMaxFormula
{
    Epley = 0,
    Brzycki = 1,
    Lombardi = 2
}

/// <summary>Descriptive training goal; must not rewrite programs (spec Epic 23).</summary>
public enum TrainingGoal
{
    GeneralFitness = 0,
    FatLoss = 1,
    Strength = 2,
    Hypertrophy = 3,
    Endurance = 4
}

/// <summary>Weight unit preference.</summary>
public enum WeightUnit
{
    Kilograms = 0,
    Pounds = 1
}

/// <summary>Body measurement length unit preference.</summary>
public enum LengthUnit
{
    Centimeters = 0,
    Inches = 1
}

/// <summary>Theme preference (spec Epic 27).</summary>
public enum ThemePreference
{
    System = 0,
    Dark = 1,
    Light = 2
}

/// <summary>Progress photo pose (spec US-102).</summary>
public enum PhotoPose
{
    Front = 0,
    Side = 1,
    Back = 2
}

/// <summary>Grouping style for supersets (spec Epic 7).</summary>
public enum SupersetKind
{
    None = 0,
    Superset = 1,
    TriSet = 2,
    GiantSet = 3,
    Circuit = 4
}

/// <summary>Direction of a progressive overload recommendation (spec 7.4).</summary>
public enum OverloadAction
{
    Maintain = 0,
    IncreaseLoad = 1,
    ReduceLoad = 2,
    NotEnoughData = 3
}
