using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Application.Contracts;
using WorkoutTracker.Domain;

namespace WorkoutTracker.Application.Services;

/// <summary>
/// Routine, folder and schedule use cases. Every query is scoped by the authenticated
/// owner id, never by a client-supplied value (spec 2.2).
/// </summary>
public sealed class RoutineService(IAppDbContext db, IClock clock)
{
    private const int MaxExercisesPerRoutine = 60;
    private const int MaxSetsPerExercise = 30;

    public async Task<Result<IReadOnlyList<RoutineDto>>> ListAsync(
        Guid ownerId,
        bool includeArchived = false,
        CancellationToken ct = default)
    {
        var query = db.Routines.Where(x => x.OwnerId == ownerId);
        if (!includeArchived) query = query.Where(x => !x.IsArchived);

        var routines = await query
            .Include(x => x.Folder)
            .Include(x => x.Exercises.OrderBy(e => e.Order)).ThenInclude(x => x.Exercise)
            .Include(x => x.Exercises).ThenInclude(x => x.Sets.OrderBy(s => s.Order))
            .OrderBy(x => x.Order).ThenBy(x => x.Name)
            .AsNoTracking()
            .ToListAsync(ct);

        return Result<IReadOnlyList<RoutineDto>>.Ok(routines.Select(x => x.ToDto()).ToList());
    }

    public async Task<Result<RoutineDto>> GetAsync(Guid ownerId, Guid routineId, CancellationToken ct = default)
    {
        var routine = await LoadAsync(ownerId, routineId, asNoTracking: true, ct);
        return routine is null
            ? Result<RoutineDto>.NotFound("Routine not found.")
            : Result<RoutineDto>.Ok(routine.ToDto());
    }

    public async Task<Result<RoutineDto>> CreateAsync(
        Guid ownerId,
        SaveRoutineRequest request,
        CancellationToken ct = default)
    {
        var validation = await ValidateAsync(ownerId, request, ct);
        if (validation is not null) return validation;

        var nextOrder = await db.Routines
            .Where(x => x.OwnerId == ownerId && x.FolderId == request.FolderId)
            .Select(x => (int?)x.Order)
            .MaxAsync(ct) ?? -1;

        var routine = new Routine
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim() ?? "",
            FolderId = request.FolderId,
            Order = nextOrder + 1,
            CreatedAt = clock.UtcNow,
            UpdatedAt = clock.UtcNow
        };

        routine.Exercises = BuildExercises(request.Exercises);

        db.Routines.Add(routine);
        await db.SaveChangesAsync(ct);

        return await GetAsync(ownerId, routine.Id, ct);
    }

    /// <summary>
    /// Replaces the routine's exercises and set templates. Children are rebuilt rather
    /// than diffed, which keeps ordering unambiguous; workout history is unaffected
    /// because sessions snapshot their own copies.
    /// </summary>
    public async Task<Result<RoutineDto>> UpdateAsync(
        Guid ownerId,
        Guid routineId,
        SaveRoutineRequest request,
        CancellationToken ct = default)
    {
        var routine = await LoadAsync(ownerId, routineId, asNoTracking: false, ct);
        if (routine is null) return Result<RoutineDto>.NotFound("Routine not found.");

        var validation = await ValidateAsync(ownerId, request, ct);
        if (validation is not null) return validation;

        routine.Name = request.Name.Trim();
        routine.Description = request.Description?.Trim() ?? "";
        routine.FolderId = request.FolderId;
        routine.UpdatedAt = clock.UtcNow;

        db.RoutineExercises.RemoveRange(routine.Exercises);
        routine.Exercises = BuildExercises(request.Exercises);

        await db.SaveChangesAsync(ct);
        return await GetAsync(ownerId, routine.Id, ct);
    }

    /// <summary>Creates an independent copy owned by the same user (spec US-023).</summary>
    public async Task<Result<RoutineDto>> DuplicateAsync(Guid ownerId, Guid routineId, CancellationToken ct = default)
    {
        var source = await LoadAsync(ownerId, routineId, asNoTracking: true, ct);
        if (source is null) return Result<RoutineDto>.NotFound("Routine not found.");

        var nextOrder = await db.Routines
            .Where(x => x.OwnerId == ownerId && x.FolderId == source.FolderId)
            .Select(x => (int?)x.Order)
            .MaxAsync(ct) ?? -1;

        var copy = new Routine
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Name = await UniqueCopyNameAsync(ownerId, source.Name, ct),
            Description = source.Description,
            FolderId = source.FolderId,
            Order = nextOrder + 1,
            CreatedAt = clock.UtcNow,
            UpdatedAt = clock.UtcNow,
            Exercises = source.Exercises.OrderBy(x => x.Order).Select(exercise => new RoutineExercise
            {
                Id = Guid.NewGuid(),
                ExerciseId = exercise.ExerciseId,
                Order = exercise.Order,
                RestSeconds = exercise.RestSeconds,
                Notes = exercise.Notes,
                SupersetGroup = exercise.SupersetGroup,
                SupersetKind = exercise.SupersetKind,
                Sets = exercise.Sets.OrderBy(x => x.Order).Select(set => new RoutineSetTemplate
                {
                    Id = Guid.NewGuid(),
                    Order = set.Order,
                    TargetReps = set.TargetReps,
                    TargetRepsMax = set.TargetRepsMax,
                    TargetWeight = set.TargetWeight,
                    Type = set.Type
                }).ToList()
            }).ToList()
        };

        db.Routines.Add(copy);
        await db.SaveChangesAsync(ct);

        return await GetAsync(ownerId, copy.Id, ct);
    }

    /// <summary>
    /// Deletes a routine, or archives it when workout history references it so past
    /// sessions keep their link (spec 4.1).
    /// </summary>
    public async Task<Result> DeleteAsync(Guid ownerId, Guid routineId, CancellationToken ct = default)
    {
        var routine = await LoadAsync(ownerId, routineId, asNoTracking: false, ct);
        if (routine is null) return Result.NotFound("Routine not found.");

        var referenced = await db.WorkoutSessions.AnyAsync(x => x.RoutineId == routineId, ct);

        if (referenced)
        {
            routine.IsArchived = true;
        }
        else
        {
            db.RoutineExercises.RemoveRange(routine.Exercises);
            db.Routines.Remove(routine);
        }

        // Schedules pointing at a removed routine would surface a dangling dashboard entry.
        var schedules = await db.WorkoutSchedules.Where(x => x.OwnerId == ownerId && x.RoutineId == routineId).ToListAsync(ct);
        db.WorkoutSchedules.RemoveRange(schedules);

        await db.SaveChangesAsync(ct);
        return Result.Ok();
    }

    /// <summary>Applies an explicit routine ordering within a folder (spec US-021).</summary>
    public async Task<Result> ReorderAsync(Guid ownerId, ReorderRoutinesRequest request, CancellationToken ct = default)
    {
        var ids = request.RoutineIdsInOrder ?? [];
        if (ids.Count == 0) return Result.Invalid("No routines supplied.");

        var routines = await db.Routines.Where(x => x.OwnerId == ownerId && ids.Contains(x.Id)).ToListAsync(ct);
        if (routines.Count != ids.Count) return Result.NotFound("One or more routines were not found.");

        for (var index = 0; index < ids.Count; index++)
        {
            var routine = routines.First(x => x.Id == ids[index]);
            routine.Order = index;
            routine.FolderId = request.FolderId;
            routine.UpdatedAt = clock.UtcNow;
        }

        await db.SaveChangesAsync(ct);
        return Result.Ok();
    }

    // ---------------------------------------------------------------------------------
    // Folders
    // ---------------------------------------------------------------------------------

    public async Task<Result<IReadOnlyList<RoutineFolderDto>>> ListFoldersAsync(Guid ownerId, CancellationToken ct = default)
    {
        var folders = await db.RoutineFolders
            .Where(x => x.OwnerId == ownerId)
            .Include(x => x.Routines)
            .OrderBy(x => x.Order).ThenBy(x => x.Name)
            .AsNoTracking()
            .ToListAsync(ct);

        return Result<IReadOnlyList<RoutineFolderDto>>.Ok(folders.Select(x => x.ToDto()).ToList());
    }

    public async Task<Result<RoutineFolderDto>> CreateFolderAsync(
        Guid ownerId,
        SaveRoutineFolderRequest request,
        CancellationToken ct = default)
    {
        var name = request.Name?.Trim() ?? "";
        if (name.Length is < 1 or > 80)
            return Result<RoutineFolderDto>.Invalid(nameof(request.Name), "Folder name must be 1 to 80 characters.");

        var folder = new RoutineFolder
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Name = name,
            Order = request.Order,
            CreatedAt = clock.UtcNow
        };

        db.RoutineFolders.Add(folder);
        await db.SaveChangesAsync(ct);

        return Result<RoutineFolderDto>.Ok(folder.ToDto());
    }

    public async Task<Result<RoutineFolderDto>> RenameFolderAsync(
        Guid ownerId,
        Guid folderId,
        SaveRoutineFolderRequest request,
        CancellationToken ct = default)
    {
        var folder = await db.RoutineFolders
            .Include(x => x.Routines)
            .FirstOrDefaultAsync(x => x.Id == folderId && x.OwnerId == ownerId, ct);

        if (folder is null) return Result<RoutineFolderDto>.NotFound("Folder not found.");

        var name = request.Name?.Trim() ?? "";
        if (name.Length is < 1 or > 80)
            return Result<RoutineFolderDto>.Invalid(nameof(request.Name), "Folder name must be 1 to 80 characters.");

        folder.Name = name;
        folder.Order = request.Order;

        await db.SaveChangesAsync(ct);
        return Result<RoutineFolderDto>.Ok(folder.ToDto());
    }

    /// <summary>
    /// Deletes a folder without deleting its routines; they become ungrouped
    /// (spec US-021).
    /// </summary>
    public async Task<Result> DeleteFolderAsync(Guid ownerId, Guid folderId, CancellationToken ct = default)
    {
        var folder = await db.RoutineFolders
            .Include(x => x.Routines)
            .FirstOrDefaultAsync(x => x.Id == folderId && x.OwnerId == ownerId, ct);

        if (folder is null) return Result.NotFound("Folder not found.");

        foreach (var routine in folder.Routines)
        {
            routine.FolderId = null;
            routine.UpdatedAt = clock.UtcNow;
        }

        db.RoutineFolders.Remove(folder);
        await db.SaveChangesAsync(ct);
        return Result.Ok();
    }

    // ---------------------------------------------------------------------------------
    // Schedule
    // ---------------------------------------------------------------------------------

    public async Task<Result<IReadOnlyList<WorkoutScheduleDto>>> ListSchedulesAsync(Guid ownerId, CancellationToken ct = default)
    {
        var schedules = await db.WorkoutSchedules
            .Where(x => x.OwnerId == ownerId)
            .Include(x => x.Routine)
            .OrderBy(x => x.DayOfWeek)
            .AsNoTracking()
            .ToListAsync(ct);

        return Result<IReadOnlyList<WorkoutScheduleDto>>.Ok(schedules.Select(x => x.ToDto()).ToList());
    }

    /// <summary>
    /// Assigns a routine to a day of week. One routine per day keeps the dashboard's
    /// "next workout" unambiguous, so an existing assignment is replaced.
    /// </summary>
    public async Task<Result<WorkoutScheduleDto>> SaveScheduleAsync(
        Guid ownerId,
        SaveScheduleRequest request,
        CancellationToken ct = default)
    {
        var routine = await db.Routines
            .FirstOrDefaultAsync(x => x.Id == request.RoutineId && x.OwnerId == ownerId && !x.IsArchived, ct);

        if (routine is null) return Result<WorkoutScheduleDto>.NotFound("Routine not found.");

        var schedule = await db.WorkoutSchedules
            .FirstOrDefaultAsync(x => x.OwnerId == ownerId && x.DayOfWeek == request.DayOfWeek, ct);

        if (schedule is null)
        {
            schedule = new WorkoutSchedule { Id = Guid.NewGuid(), OwnerId = ownerId, DayOfWeek = request.DayOfWeek };
            db.WorkoutSchedules.Add(schedule);
        }

        schedule.RoutineId = request.RoutineId;
        schedule.IsEnabled = request.IsEnabled;

        await db.SaveChangesAsync(ct);

        schedule.Routine = routine;
        return Result<WorkoutScheduleDto>.Ok(schedule.ToDto());
    }

    public async Task<Result> DeleteScheduleAsync(Guid ownerId, Guid scheduleId, CancellationToken ct = default)
    {
        var schedule = await db.WorkoutSchedules.FirstOrDefaultAsync(x => x.Id == scheduleId && x.OwnerId == ownerId, ct);
        if (schedule is null) return Result.NotFound("Schedule not found.");

        db.WorkoutSchedules.Remove(schedule);
        await db.SaveChangesAsync(ct);
        return Result.Ok();
    }

    // ---------------------------------------------------------------------------------
    // Internals
    // ---------------------------------------------------------------------------------

    private async Task<Routine?> LoadAsync(Guid ownerId, Guid routineId, bool asNoTracking, CancellationToken ct)
    {
        var query = db.Routines
            .Where(x => x.Id == routineId && x.OwnerId == ownerId)
            .Include(x => x.Folder)
            .Include(x => x.Exercises.OrderBy(e => e.Order)).ThenInclude(x => x.Exercise)
            .Include(x => x.Exercises).ThenInclude(x => x.Sets.OrderBy(s => s.Order))
            .AsQueryable();

        if (asNoTracking) query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync(ct);
    }

    private async Task<Result<RoutineDto>?> ValidateAsync(Guid ownerId, SaveRoutineRequest request, CancellationToken ct)
    {
        var name = request.Name?.Trim() ?? "";
        if (name.Length is < 1 or > 120)
            return Result<RoutineDto>.Invalid(nameof(request.Name), "Routine name must be 1 to 120 characters.");

        var exercises = request.Exercises ?? [];
        if (exercises.Count == 0)
            return Result<RoutineDto>.Invalid(nameof(request.Exercises), "A routine needs at least one exercise.");

        if (exercises.Count > MaxExercisesPerRoutine)
            return Result<RoutineDto>.Invalid(nameof(request.Exercises), $"A routine cannot exceed {MaxExercisesPerRoutine} exercises.");

        foreach (var exercise in exercises)
        {
            if (exercise.RestSeconds is < 0 or > 3600)
                return Result<RoutineDto>.Invalid(nameof(exercise.RestSeconds), "Rest must be between 0 and 3600 seconds.");

            var sets = exercise.Sets ?? [];
            if (sets.Count is 0 or > MaxSetsPerExercise)
                return Result<RoutineDto>.Invalid(nameof(exercise.Sets), $"Each exercise needs 1 to {MaxSetsPerExercise} sets.");

            foreach (var set in sets)
            {
                if (set.TargetReps is < 0 or > 1000)
                    return Result<RoutineDto>.Invalid(nameof(set.TargetReps), "Target reps must be between 0 and 1000.");

                if (set.TargetRepsMax is { } max && (max < set.TargetReps || max > 1000))
                    return Result<RoutineDto>.Invalid(nameof(set.TargetRepsMax), "Top of the rep range must be at least the bottom and at most 1000.");

                if (set.TargetWeight is < 0 or > 2000)
                    return Result<RoutineDto>.Invalid(nameof(set.TargetWeight), "Target weight must be between 0 and 2000 kg.");
            }
        }

        if (request.FolderId is { } folderId
            && !await db.RoutineFolders.AnyAsync(x => x.Id == folderId && x.OwnerId == ownerId, ct))
        {
            return Result<RoutineDto>.Invalid(nameof(request.FolderId), "Folder not found.");
        }

        // Confirm every referenced exercise is actually visible to this user.
        var exerciseIds = exercises.Select(x => x.ExerciseId).Distinct().ToList();
        var visible = await db.Exercises
            .CountAsync(x => exerciseIds.Contains(x.Id) && (x.OwnerId == null || x.OwnerId == ownerId), ct);

        return visible != exerciseIds.Count
            ? Result<RoutineDto>.Invalid(nameof(request.Exercises), "One or more exercises are unavailable.")
            : null;
    }

    private static List<RoutineExercise> BuildExercises(List<SaveRoutineExerciseRequest> requested)
        => requested.Select((exercise, index) => new RoutineExercise
        {
            Id = Guid.NewGuid(),
            ExerciseId = exercise.ExerciseId,
            Order = index,
            RestSeconds = exercise.RestSeconds,
            Notes = exercise.Notes?.Trim() ?? "",
            SupersetGroup = exercise.SupersetGroup,
            SupersetKind = exercise.SupersetGroup is null ? SupersetKind.None : exercise.SupersetKind,
            Sets = (exercise.Sets ?? []).Select((set, setIndex) => new RoutineSetTemplate
            {
                Id = Guid.NewGuid(),
                Order = setIndex,
                TargetReps = set.TargetReps,
                TargetRepsMax = set.TargetRepsMax,
                TargetWeight = set.TargetWeight,
                Type = set.Type
            }).ToList()
        }).ToList();

    /// <summary>Produces "Name Copy", "Name Copy 2", ... so duplicates stay distinguishable.</summary>
    private async Task<string> UniqueCopyNameAsync(Guid ownerId, string sourceName, CancellationToken ct)
    {
        var existing = await db.Routines
            .Where(x => x.OwnerId == ownerId)
            .Select(x => x.Name)
            .ToListAsync(ct);

        var candidate = $"{sourceName} Copy";
        if (!existing.Contains(candidate)) return Truncate(candidate);

        for (var suffix = 2; suffix < 100; suffix++)
        {
            candidate = $"{sourceName} Copy {suffix}";
            if (!existing.Contains(candidate)) return Truncate(candidate);
        }

        return Truncate($"{sourceName} Copy {Guid.NewGuid():N}"[..40]);
    }

    private static string Truncate(string value) => value.Length <= 120 ? value : value[..120];
}
