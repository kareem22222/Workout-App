namespace WorkoutTracker.Domain;

/// <summary>
/// Demographic and physical profile data kept separate from the Identity user so
/// authentication concerns stay in Infrastructure (spec 4).
/// </summary>
public sealed class UserProfile
{
    public Guid Id { get; set; }

    /// <summary>One profile per user.</summary>
    public Guid OwnerId { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    /// <summary>Free text rather than an enum so users are not forced into fixed categories.</summary>
    public string? Gender { get; set; }

    /// <summary>Height in canonical centimeters.</summary>
    public decimal? HeightCm { get; set; }

    /// <summary>Storage key for the profile image. Never a public URL.</summary>
    public string? AvatarStorageKey { get; set; }

    public TrainingGoal Goal { get; set; } = TrainingGoal.GeneralFitness;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Per-user behaviour settings: units, timers, 1RM formula, plate inventory and
/// overload preferences (spec 4).
/// </summary>
public sealed class UserSetting
{
    public Guid Id { get; set; }

    /// <summary>One settings row per user.</summary>
    public Guid OwnerId { get; set; }

    public WeightUnit WeightUnit { get; set; } = WeightUnit.Kilograms;

    public LengthUnit LengthUnit { get; set; } = LengthUnit.Centimeters;

    /// <summary>IANA timezone id used to group history by local date (spec 3.1).</summary>
    public string TimeZone { get; set; } = "UTC";

    public ThemePreference Theme { get; set; } = ThemePreference.Dark;

    public OneRepMaxFormula OneRepMaxFormula { get; set; } = OneRepMaxFormula.Epley;

    public int DefaultRestSeconds { get; set; } = 90;

    /// <summary>Start the rest timer automatically when a set is marked complete (spec US-040).</summary>
    public bool AutoStartRestTimer { get; set; } = true;

    public bool RestTimerSound { get; set; } = true;

    public bool RestTimerVibrate { get; set; } = true;

    /// <summary>Browser notifications are strictly opt-in (spec US-240).</summary>
    public bool RestTimerNotifications { get; set; }

    /// <summary>Bar weight in kg used by the plate calculator (spec US-160).</summary>
    public decimal BarWeightKg { get; set; } = 20m;

    /// <summary>
    /// Available plate sizes per side in kg, largest first. Stored as a list so the
    /// calculator reflects the user's actual gym inventory.
    /// </summary>
    public List<decimal> PlateInventoryKg { get; set; } = [25m, 20m, 15m, 10m, 5m, 2.5m, 1.25m];

    /// <summary>Smallest load step used when rounding warmup and overload suggestions.</summary>
    public decimal RoundingIncrementKg { get; set; } = 2.5m;

    /// <summary>Load increment suggested when the overload rule fires (spec 7.4).</summary>
    public decimal OverloadIncrementKg { get; set; } = 2.5m;

    /// <summary>Warmup ramp as percentages of the working weight (spec US-060).</summary>
    public List<int> WarmupPercentages { get; set; } = [40, 60, 80];

    /// <summary>Target number of workouts per week, used for dashboard progress.</summary>
    public int WeeklyWorkoutGoal { get; set; } = 4;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// A dated body measurement. Every metric except the date is optional so partial
/// entries are valid (spec US-101).
/// </summary>
public sealed class BodyMeasurement
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }

    /// <summary>Local calendar date of the measurement.</summary>
    public DateOnly MeasuredOn { get; set; }

    /// <summary>Body weight in canonical kilograms.</summary>
    public decimal? WeightKg { get; set; }

    public decimal? BodyFatPercent { get; set; }

    // Circumferences in canonical centimeters.
    public decimal? ChestCm { get; set; }
    public decimal? WaistCm { get; set; }
    public decimal? HipsCm { get; set; }
    public decimal? LeftArmCm { get; set; }
    public decimal? RightArmCm { get; set; }
    public decimal? LeftThighCm { get; set; }
    public decimal? RightThighCm { get; set; }
    public decimal? LeftCalfCm { get; set; }
    public decimal? RightCalfCm { get; set; }
    public decimal? ShouldersCm { get; set; }
    public decimal? NeckCm { get; set; }

    public string Notes { get; set; } = "";

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Metadata for a private progress photo. Only the storage key is persisted; bytes
/// live in object storage and are served through an authorized endpoint (spec US-102).
/// </summary>
public sealed class ProgressPhoto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }

    public DateOnly TakenOn { get; set; }

    public PhotoPose Pose { get; set; } = PhotoPose.Front;

    /// <summary>Non-public object key. Never exposed directly to clients.</summary>
    public required string StorageKey { get; set; }

    public string ContentType { get; set; } = "image/jpeg";

    public long SizeBytes { get; set; }

    /// <summary>Optional body weight at the time of the photo, in canonical kilograms.</summary>
    public decimal? WeightKg { get; set; }

    public string Notes { get; set; } = "";

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
