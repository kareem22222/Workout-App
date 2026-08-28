using WorkoutTracker.Domain;

namespace WorkoutTracker.Application.Contracts;

// =====================================================================================
// Reference data
// =====================================================================================

public sealed record MuscleDto(Guid Id, string Slug, string Name, string BodyRegion);

public sealed record EquipmentDto(Guid Id, string Slug, string Name, decimal? DefaultBarWeightKg);

public sealed record ExerciseMuscleDto(Guid MuscleId, string MuscleName, MuscleRole Role, decimal ContributionWeight);

// =====================================================================================
// Exercises
// =====================================================================================

public sealed record ExerciseDto(
    Guid Id,
    string Name,
    string Instructions,
    ExerciseType Type,
    string Category,
    Guid? EquipmentId,
    string? EquipmentName,
    string? MediaUrl,
    int DefaultRestSeconds,
    decimal DefaultIncrementKg,
    bool IsCustom,
    bool IsArchived,
    IReadOnlyList<ExerciseMuscleDto> Muscles,
    string? PersistentNote);

public sealed record ExerciseMuscleRequest(Guid MuscleId, MuscleRole Role = MuscleRole.Primary, decimal? ContributionWeight = null);

public sealed record SaveExerciseRequest(
    string Name,
    string? Instructions,
    ExerciseType Type,
    string? Category,
    Guid? EquipmentId,
    string? MediaUrl,
    int DefaultRestSeconds = 90,
    decimal DefaultIncrementKg = 2.5m,
    List<ExerciseMuscleRequest>? Muscles = null);

public sealed record SaveExerciseNoteRequest(string Text);

/// <summary>One past performance of an exercise, used by the exercise detail screen.</summary>
public sealed record ExerciseHistoryEntryDto(
    Guid WorkoutSessionId,
    string WorkoutTitle,
    DateTimeOffset PerformedAt,
    IReadOnlyList<LoggedSetDto> Sets,
    decimal Volume,
    decimal? BestEstimatedOneRepMax);

public sealed record LoggedSetDto(
    int Order,
    decimal Weight,
    int Reps,
    decimal? Rpe,
    WorkoutSetType Type,
    int? DurationSeconds,
    decimal? DistanceMeters,
    string Notes);

// =====================================================================================
// Routines
// =====================================================================================

public sealed record RoutineFolderDto(Guid Id, string Name, int Order, int RoutineCount);

public sealed record SaveRoutineFolderRequest(string Name, int Order = 0);

public sealed record RoutineSetTemplateDto(
    Guid Id,
    int Order,
    int TargetReps,
    int? TargetRepsMax,
    decimal? TargetWeight,
    WorkoutSetType Type);

public sealed record RoutineExerciseDto(
    Guid Id,
    Guid ExerciseId,
    string ExerciseName,
    ExerciseType ExerciseType,
    int Order,
    int RestSeconds,
    string Notes,
    int? SupersetGroup,
    SupersetKind SupersetKind,
    IReadOnlyList<RoutineSetTemplateDto> Sets);

public sealed record RoutineDto(
    Guid Id,
    string Name,
    string Description,
    Guid? FolderId,
    string? FolderName,
    int Order,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    int ExerciseCount,
    int SetCount,
    IReadOnlyList<RoutineExerciseDto> Exercises);

public sealed record SaveRoutineSetTemplateRequest(
    int TargetReps = 8,
    int? TargetRepsMax = null,
    decimal? TargetWeight = null,
    WorkoutSetType Type = WorkoutSetType.Normal);

public sealed record SaveRoutineExerciseRequest(
    Guid ExerciseId,
    int RestSeconds = 90,
    string? Notes = null,
    int? SupersetGroup = null,
    SupersetKind SupersetKind = SupersetKind.None,
    List<SaveRoutineSetTemplateRequest>? Sets = null);

public sealed record SaveRoutineRequest(
    string Name,
    string? Description,
    Guid? FolderId,
    List<SaveRoutineExerciseRequest> Exercises);

public sealed record ReorderRoutinesRequest(List<Guid> RoutineIdsInOrder, Guid? FolderId);

// =====================================================================================
// Workouts
// =====================================================================================

/// <summary>
/// A logged set. <c>Previous</c> carries the last comparable performance for this set
/// position so it can be shown beside the current values while training (spec US-032).
/// </summary>
public sealed record WorkoutSetDto(
    Guid Id,
    int Order,
    decimal Weight,
    int Reps,
    decimal? Rpe,
    WorkoutSetType Type,
    int? DurationSeconds,
    decimal? DistanceMeters,
    string Notes,
    bool Completed,
    DateTimeOffset? CompletedAt,
    PreviousSetDto? Previous);

/// <summary>Last comparable performance shown beside the current set while training.</summary>
public sealed record PreviousSetDto(decimal Weight, int Reps, DateTimeOffset PerformedAt);

public sealed record WorkoutExerciseDto(
    Guid Id,
    Guid ExerciseId,
    string ExerciseName,
    ExerciseType ExerciseType,
    int Order,
    int RestSeconds,
    string Notes,
    int? SupersetGroup,
    SupersetKind SupersetKind,
    string? PersistentNote,
    decimal Volume,
    IReadOnlyList<WorkoutSetDto> Sets);

public sealed record WorkoutSessionDto(
    Guid Id,
    Guid? RoutineId,
    string Title,
    WorkoutStatus Status,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt,
    string Notes,
    int Version,
    int DurationSeconds,
    decimal TotalVolume,
    int CompletedSets,
    int TotalSets,
    int TotalReps,
    IReadOnlyList<WorkoutExerciseDto> Exercises);

/// <summary>Lightweight history row that avoids sending every set for list views.</summary>
public sealed record WorkoutSummaryRowDto(
    Guid Id,
    string Title,
    WorkoutStatus Status,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt,
    int DurationSeconds,
    decimal TotalVolume,
    int CompletedSets,
    int ExerciseCount,
    IReadOnlyList<string> ExerciseNames);

public sealed record StartWorkoutRequest(Guid? RoutineId, string? Title, Guid? CopyFromWorkoutId);

public sealed record UpdateWorkoutSetRequest(
    Guid Id,
    int Order,
    decimal Weight,
    int Reps,
    decimal? Rpe,
    WorkoutSetType Type,
    int? DurationSeconds,
    decimal? DistanceMeters,
    string? Notes,
    bool Completed);

public sealed record UpdateWorkoutExerciseRequest(
    Guid Id,
    Guid ExerciseId,
    int Order,
    int RestSeconds,
    string? Notes,
    int? SupersetGroup,
    SupersetKind SupersetKind,
    List<UpdateWorkoutSetRequest> Sets);

/// <summary>
/// Full replace of the mutable parts of a session. The client echoes the
/// <see cref="Version"/> it last saw so stale offline writes are rejected (spec Epic 30).
/// </summary>
public sealed record UpdateWorkoutRequest(
    string? Title,
    string? Notes,
    int Version,
    List<UpdateWorkoutExerciseRequest> Exercises);

public sealed record FinishWorkoutRequest(string? Notes);

/// <summary>Post-workout summary including newly detected records (spec US-070).</summary>
public sealed record WorkoutCompletionDto(
    Guid Id,
    string Title,
    DateTimeOffset StartedAt,
    DateTimeOffset CompletedAt,
    int DurationSeconds,
    int CompletedSets,
    int TotalReps,
    decimal TotalVolume,
    IReadOnlyList<PersonalRecordDto> NewRecords,
    IReadOnlyList<MuscleContributionDto> MuscleBreakdown);

// =====================================================================================
// Progress, records and statistics
// =====================================================================================

public sealed record PersonalRecordDto(
    Guid Id,
    Guid ExerciseId,
    string ExerciseName,
    PersonalRecordType Type,
    decimal Value,
    decimal? AtWeight,
    Guid WorkoutSessionId,
    DateTimeOffset AchievedAt);

/// <summary>A single point on a progress chart (spec US-098).</summary>
public sealed record ChartPointDto(DateTimeOffset Date, decimal Value);

/// <summary>
/// Chart series for one exercise across the requested range, including the accessible
/// textual values required by the acceptance criteria.
/// </summary>
public sealed record ExerciseProgressDto(
    Guid ExerciseId,
    string ExerciseName,
    string Range,
    IReadOnlyList<ChartPointDto> BestWeight,
    IReadOnlyList<ChartPointDto> EstimatedOneRepMax,
    IReadOnlyList<ChartPointDto> Volume,
    IReadOnlyList<ChartPointDto> MaxReps,
    IReadOnlyList<PersonalRecordDto> Records);

public sealed record VolumePointDto(DateTimeOffset PeriodStart, decimal Volume, int Workouts, int Sets);

public sealed record TrainingStatsDto(
    DateTimeOffset From,
    DateTimeOffset To,
    int Workouts,
    int TrainingMinutes,
    decimal TotalVolume,
    int TotalSets,
    int TotalReps,
    int DistinctExercises,
    int PersonalRecords,
    int CurrentStreakWeeks,
    IReadOnlyList<VolumePointDto> Series);

/// <summary>Ranked muscle contribution used by the heatmap summary (spec US-110).</summary>
public sealed record MuscleContributionDto(string MuscleName, string BodyRegion, decimal Score, int Sets);

// =====================================================================================
// Body measurements and photos
// =====================================================================================

public sealed record BodyMeasurementDto(
    Guid Id,
    DateOnly MeasuredOn,
    decimal? WeightKg,
    decimal? BodyFatPercent,
    decimal? ChestCm,
    decimal? WaistCm,
    decimal? HipsCm,
    decimal? LeftArmCm,
    decimal? RightArmCm,
    decimal? LeftThighCm,
    decimal? RightThighCm,
    decimal? LeftCalfCm,
    decimal? RightCalfCm,
    decimal? ShouldersCm,
    decimal? NeckCm,
    string Notes);

public sealed record SaveBodyMeasurementRequest(
    DateOnly MeasuredOn,
    decimal? WeightKg,
    decimal? BodyFatPercent,
    decimal? ChestCm,
    decimal? WaistCm,
    decimal? HipsCm,
    decimal? LeftArmCm,
    decimal? RightArmCm,
    decimal? LeftThighCm,
    decimal? RightThighCm,
    decimal? LeftCalfCm,
    decimal? RightCalfCm,
    decimal? ShouldersCm,
    decimal? NeckCm,
    string? Notes);

public sealed record ProgressPhotoDto(
    Guid Id,
    DateOnly TakenOn,
    PhotoPose Pose,
    decimal? WeightKg,
    string Notes,
    long SizeBytes,
    string ContentType);

// =====================================================================================
// Profile and settings
// =====================================================================================

public sealed record UserProfileDto(
    Guid UserId,
    string DisplayName,
    string Email,
    bool IsAdmin,
    DateOnly? DateOfBirth,
    string? Gender,
    decimal? HeightCm,
    TrainingGoal Goal,
    bool HasAvatar,
    decimal? LatestWeightKg);

public sealed record UpdateProfileRequest(
    string DisplayName,
    DateOnly? DateOfBirth,
    string? Gender,
    decimal? HeightCm,
    TrainingGoal Goal);

public sealed record UserSettingsDto(
    WeightUnit WeightUnit,
    LengthUnit LengthUnit,
    string TimeZone,
    ThemePreference Theme,
    OneRepMaxFormula OneRepMaxFormula,
    int DefaultRestSeconds,
    bool AutoStartRestTimer,
    bool RestTimerSound,
    bool RestTimerVibrate,
    bool RestTimerNotifications,
    decimal BarWeightKg,
    IReadOnlyList<decimal> PlateInventoryKg,
    decimal RoundingIncrementKg,
    decimal OverloadIncrementKg,
    IReadOnlyList<int> WarmupPercentages,
    int WeeklyWorkoutGoal);

public sealed record UpdateSettingsRequest(
    WeightUnit WeightUnit,
    LengthUnit LengthUnit,
    string TimeZone,
    ThemePreference Theme,
    OneRepMaxFormula OneRepMaxFormula,
    int DefaultRestSeconds,
    bool AutoStartRestTimer,
    bool RestTimerSound,
    bool RestTimerVibrate,
    bool RestTimerNotifications,
    decimal BarWeightKg,
    List<decimal>? PlateInventoryKg,
    decimal RoundingIncrementKg,
    decimal OverloadIncrementKg,
    List<int>? WarmupPercentages,
    int WeeklyWorkoutGoal);

// =====================================================================================
// Scheduling and dashboard
// =====================================================================================

public sealed record WorkoutScheduleDto(Guid Id, Guid RoutineId, string RoutineName, DayOfWeek DayOfWeek, bool IsEnabled);

public sealed record SaveScheduleRequest(Guid RoutineId, DayOfWeek DayOfWeek, bool IsEnabled = true);

public sealed record DashboardSummaryDto(
    string DisplayName,
    WorkoutSessionDto? ActiveWorkout,
    RoutineDto? NextScheduledRoutine,
    DayOfWeek? NextScheduledDay,
    int WorkoutsThisWeek,
    int WeeklyWorkoutGoal,
    decimal VolumeThisWeek,
    int TrainingMinutesThisWeek,
    int CurrentStreakWeeks,
    IReadOnlyList<PersonalRecordDto> RecentRecords,
    IReadOnlyList<WorkoutSummaryRowDto> RecentWorkouts,
    decimal? LatestWeightKg,
    DateOnly? LatestWeightOn,
    decimal? WeightChange30DaysKg);

// =====================================================================================
// Training aids
// =====================================================================================

public sealed record PlateStackDto(decimal PlateKg, int CountPerSide);

public sealed record PlateSolutionDto(
    decimal RequestedKg,
    decimal AchievableKg,
    decimal BarKg,
    IReadOnlyList<PlateStackDto> PerSide,
    bool IsExact,
    string? Message);

public sealed record WarmupSetDto(int Order, int Percentage, decimal WeightKg, int Reps);

public sealed record OverloadSuggestionDto(
    Guid ExerciseId,
    string ExerciseName,
    OverloadAction Action,
    decimal? SuggestedWeightKg,
    decimal? PreviousWeightKg,
    string Rationale);

// =====================================================================================
// Export and import
// =====================================================================================

/// <summary>Full user-owned data export with schema metadata (spec US-200).</summary>
public sealed record ExportBundleDto(
    string Schema,
    int Version,
    DateTimeOffset ExportedAt,
    UserProfileDto Profile,
    UserSettingsDto Settings,
    IReadOnlyList<ExerciseDto> CustomExercises,
    IReadOnlyList<RoutineDto> Routines,
    IReadOnlyList<WorkoutSessionDto> Workouts,
    IReadOnlyList<BodyMeasurementDto> Measurements,
    IReadOnlyList<PersonalRecordDto> PersonalRecords);

/// <summary>A single parsed import row with any validation problem attached (spec US-210).</summary>
public sealed record ImportRowPreviewDto(int RowNumber, string Date, string Exercise, string Weight, string Reps, string? Error);

public sealed record ImportPreviewDto(
    int TotalRows,
    int ValidRows,
    int InvalidRows,
    bool CanCommit,
    IReadOnlyList<ImportRowPreviewDto> Rows);

public sealed record ImportResultDto(int WorkoutsCreated, int SetsCreated, int RowsSkipped);

// =====================================================================================
// Administration
// =====================================================================================

public sealed record AdminUserDto(
    Guid Id,
    string DisplayName,
    string Email,
    bool IsAdmin,
    bool IsDisabled,
    DateTimeOffset CreatedAt,
    int WorkoutCount,
    DateTimeOffset? LastWorkoutAt);

public sealed record SetUserDisabledRequest(bool IsDisabled);
