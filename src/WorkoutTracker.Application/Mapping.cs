using WorkoutTracker.Application.Contracts;
using WorkoutTracker.Domain;

namespace WorkoutTracker.Application;

/// <summary>
/// Entity to DTO projections. Centralized so every endpoint returns the same shape and
/// EF entities never leak past the Application boundary (spec 2.2).
/// </summary>
public static class Mapping
{
    public static MuscleDto ToDto(this Muscle muscle) =>
        new(muscle.Id, muscle.Slug, muscle.Name, muscle.BodyRegion);

    public static EquipmentDto ToDto(this Equipment equipment) =>
        new(equipment.Id, equipment.Slug, equipment.Name, equipment.DefaultBarWeightKg);

    public static ExerciseDto ToDto(this Exercise exercise, string? persistentNote = null) => new(
        exercise.Id,
        exercise.Name,
        exercise.Instructions,
        exercise.Type,
        exercise.Category,
        exercise.EquipmentId,
        exercise.Equipment?.Name,
        exercise.MediaUrl,
        exercise.DefaultRestSeconds,
        exercise.DefaultIncrementKg,
        exercise.OwnerId is not null,
        exercise.IsArchived,
        exercise.Muscles
            .OrderBy(x => x.Role)
            .ThenBy(x => x.Muscle?.Name)
            .Select(x => new ExerciseMuscleDto(x.MuscleId, x.Muscle?.Name ?? "", x.Role, x.ContributionWeight))
            .ToList(),
        persistentNote);

    public static RoutineSetTemplateDto ToDto(this RoutineSetTemplate template) =>
        new(template.Id, template.Order, template.TargetReps, template.TargetRepsMax, template.TargetWeight, template.Type);

    public static RoutineExerciseDto ToDto(this RoutineExercise routineExercise) => new(
        routineExercise.Id,
        routineExercise.ExerciseId,
        routineExercise.Exercise?.Name ?? "Removed exercise",
        routineExercise.Exercise?.Type ?? ExerciseType.WeightAndReps,
        routineExercise.Order,
        routineExercise.RestSeconds,
        routineExercise.Notes,
        routineExercise.SupersetGroup,
        routineExercise.SupersetKind,
        routineExercise.Sets.OrderBy(x => x.Order).Select(ToDto).ToList());

    public static RoutineDto ToDto(this Routine routine)
    {
        var exercises = routine.Exercises.OrderBy(x => x.Order).Select(ToDto).ToList();
        return new RoutineDto(
            routine.Id,
            routine.Name,
            routine.Description,
            routine.FolderId,
            routine.Folder?.Name,
            routine.Order,
            routine.CreatedAt,
            routine.UpdatedAt,
            exercises.Count,
            exercises.Sum(x => x.Sets.Count),
            exercises);
    }

    public static WorkoutSetDto ToDto(this WorkoutSet set, PreviousSetDto? previous = null) => new(
        set.Id,
        set.Order,
        set.Weight,
        set.Reps,
        set.Rpe,
        set.Type,
        set.DurationSeconds,
        set.DistanceMeters,
        set.Notes,
        set.IsCompleted,
        set.CompletedAt,
        previous);

    public static WorkoutExerciseDto ToDto(
        this WorkoutExercise exercise,
        string? persistentNote = null,
        IReadOnlyDictionary<int, PreviousSetDto>? previousByOrder = null) => new(
        exercise.Id,
        exercise.ExerciseId,
        exercise.ExerciseName,
        exercise.ExerciseType,
        exercise.Order,
        exercise.RestSeconds,
        exercise.Notes,
        exercise.SupersetGroup,
        exercise.SupersetKind,
        persistentNote,
        TrainingVolume.ForExercise(exercise),
        exercise.Sets
            .OrderBy(x => x.Order)
            .Select(set => set.ToDto(LookupPrevious(previousByOrder, set.Order)))
            .ToList());

    private static PreviousSetDto? LookupPrevious(IReadOnlyDictionary<int, PreviousSetDto>? source, int order)
        => source is not null && source.TryGetValue(order, out var previous) ? previous : null;

    public static WorkoutSessionDto ToDto(
        this WorkoutSession session,
        IReadOnlyDictionary<Guid, string>? persistentNotes = null,
        IReadOnlyDictionary<Guid, IReadOnlyDictionary<int, PreviousSetDto>>? previous = null)
    {
        var exercises = session.Exercises
            .OrderBy(x => x.Order)
            .Select(exercise => exercise.ToDto(
                LookupNote(persistentNotes, exercise.ExerciseId),
                LookupPreviousMap(previous, exercise.ExerciseId)))
            .ToList();

        var allSets = session.Exercises.SelectMany(x => x.Sets).ToList();

        return new WorkoutSessionDto(
            session.Id,
            session.RoutineId,
            session.Title,
            session.Status,
            session.StartedAt,
            session.CompletedAt,
            session.Notes,
            session.Version,
            (int)(session.Duration ?? DateTimeOffset.UtcNow - session.StartedAt).TotalSeconds,
            TrainingVolume.ForSession(session),
            TrainingVolume.CompletedWorkSets(session),
            allSets.Count,
            TrainingVolume.CompletedReps(session),
            exercises);
    }

    private static string? LookupNote(IReadOnlyDictionary<Guid, string>? source, Guid exerciseId)
        => source is not null && source.TryGetValue(exerciseId, out var note) ? note : null;

    private static IReadOnlyDictionary<int, PreviousSetDto>? LookupPreviousMap(
        IReadOnlyDictionary<Guid, IReadOnlyDictionary<int, PreviousSetDto>>? source,
        Guid exerciseId)
        => source is not null && source.TryGetValue(exerciseId, out var map) ? map : null;

    public static WorkoutSummaryRowDto ToSummaryRow(this WorkoutSession session) => new(
        session.Id,
        session.Title,
        session.Status,
        session.StartedAt,
        session.CompletedAt,
        (int)(session.Duration ?? TimeSpan.Zero).TotalSeconds,
        TrainingVolume.ForSession(session),
        TrainingVolume.CompletedWorkSets(session),
        session.Exercises.Count,
        session.Exercises.OrderBy(x => x.Order).Select(x => x.ExerciseName).ToList());

    public static PersonalRecordDto ToDto(this PersonalRecord record, string exerciseName) =>
        new(record.Id, record.ExerciseId, exerciseName, record.Type, record.Value, record.AtWeight,
            record.WorkoutSessionId, record.AchievedAt);

    public static BodyMeasurementDto ToDto(this BodyMeasurement measurement) => new(
        measurement.Id,
        measurement.MeasuredOn,
        measurement.WeightKg,
        measurement.BodyFatPercent,
        measurement.ChestCm,
        measurement.WaistCm,
        measurement.HipsCm,
        measurement.LeftArmCm,
        measurement.RightArmCm,
        measurement.LeftThighCm,
        measurement.RightThighCm,
        measurement.LeftCalfCm,
        measurement.RightCalfCm,
        measurement.ShouldersCm,
        measurement.NeckCm,
        measurement.Notes);

    public static ProgressPhotoDto ToDto(this ProgressPhoto photo) =>
        new(photo.Id, photo.TakenOn, photo.Pose, photo.WeightKg, photo.Notes, photo.SizeBytes, photo.ContentType);

    public static UserSettingsDto ToDto(this UserSetting settings) => new(
        settings.WeightUnit,
        settings.LengthUnit,
        settings.TimeZone,
        settings.Theme,
        settings.OneRepMaxFormula,
        settings.DefaultRestSeconds,
        settings.AutoStartRestTimer,
        settings.RestTimerSound,
        settings.RestTimerVibrate,
        settings.RestTimerNotifications,
        settings.BarWeightKg,
        settings.PlateInventoryKg.OrderByDescending(x => x).ToList(),
        settings.RoundingIncrementKg,
        settings.OverloadIncrementKg,
        settings.WarmupPercentages.OrderBy(x => x).ToList(),
        settings.WeeklyWorkoutGoal);

    public static PlateSolutionDto ToDto(this PlateSolution solution) => new(
        solution.RequestedKg,
        solution.AchievableKg,
        solution.BarKg,
        solution.PerSide.Select(x => new PlateStackDto(x.PlateKg, x.CountPerSide)).ToList(),
        solution.IsExact,
        solution.Message);

    public static WarmupSetDto ToDto(this WarmupSet set) =>
        new(set.Order, set.Percentage, set.WeightKg, set.Reps);

    public static WorkoutScheduleDto ToDto(this WorkoutSchedule schedule) =>
        new(schedule.Id, schedule.RoutineId, schedule.Routine?.Name ?? "Removed routine", schedule.DayOfWeek, schedule.IsEnabled);

    public static RoutineFolderDto ToDto(this RoutineFolder folder) =>
        new(folder.Id, folder.Name, folder.Order, folder.Routines.Count);
}
