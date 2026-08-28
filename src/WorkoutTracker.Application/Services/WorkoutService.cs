using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Application.Contracts;
using WorkoutTracker.Domain;

namespace WorkoutTracker.Application.Services;

/// <summary>
/// Workout lifecycle use cases: start, log, finish, cancel and correct history.
/// This is the core of the product, so it owns the rules that must not be duplicated in
/// the client: exactly one active session, previous-value resolution, optimistic
/// concurrency and personal record recomputation (spec Epics 4, 5, 9, 10).
/// </summary>
public sealed class WorkoutService(
    IAppDbContext db,
    SettingsService settings,
    PersonalRecordService records,
    IClock clock)
{
    private const int MaxExercisesPerWorkout = 60;
    private const int MaxSetsPerExercise = 50;

    // ---------------------------------------------------------------------------------
    // Reads
    // ---------------------------------------------------------------------------------

    /// <summary>The single in-progress session, if one exists.</summary>
    public async Task<Result<WorkoutSessionDto?>> GetActiveAsync(Guid ownerId, CancellationToken ct = default)
    {
        var session = await Query(ownerId).FirstOrDefaultAsync(x => x.Status == WorkoutStatus.Active, ct);

        return session is null
            ? Result<WorkoutSessionDto?>.Ok(null)
            : Result<WorkoutSessionDto?>.Ok(await DecorateAsync(ownerId, session, ct));
    }

    public async Task<Result<WorkoutSessionDto>> GetAsync(Guid ownerId, Guid workoutId, CancellationToken ct = default)
    {
        var session = await Query(ownerId).FirstOrDefaultAsync(x => x.Id == workoutId, ct);

        return session is null
            ? Result<WorkoutSessionDto>.NotFound("Workout not found.")
            : Result<WorkoutSessionDto>.Ok(await DecorateAsync(ownerId, session, ct));
    }

    /// <summary>
    /// Paginated history, newest first, with optional date/routine/exercise filters
    /// (spec US-080).
    /// </summary>
    public async Task<Result<PagedResult<WorkoutSummaryRowDto>>> ListAsync(
        Guid ownerId,
        int page = 1,
        int pageSize = 20,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        Guid? routineId = null,
        Guid? exerciseId = null,
        WorkoutStatus? status = WorkoutStatus.Completed,
        CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = db.WorkoutSessions.Where(x => x.OwnerId == ownerId);

        if (status is { } wanted) query = query.Where(x => x.Status == wanted);
        if (from is { } start) query = query.Where(x => x.StartedAt >= start);
        if (to is { } end) query = query.Where(x => x.StartedAt <= end);
        if (routineId is { } routine) query = query.Where(x => x.RoutineId == routine);
        if (exerciseId is { } exercise) query = query.Where(x => x.Exercises.Any(e => e.ExerciseId == exercise));

        var total = await query.CountAsync(ct);

        var sessions = await query
            .OrderByDescending(x => x.StartedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(x => x.Exercises.OrderBy(e => e.Order)).ThenInclude(x => x.Sets)
            .AsNoTracking()
            .ToListAsync(ct);

        return Result<PagedResult<WorkoutSummaryRowDto>>.Ok(new PagedResult<WorkoutSummaryRowDto>(
            sessions.Select(x => x.ToSummaryRow()).ToList(), page, pageSize, total));
    }

    /// <summary>
    /// Completed workouts grouped by the user's local calendar date, for the month
    /// calendar. Grouping uses the configured timezone so late-evening sessions land on
    /// the correct day (spec US-130).
    /// </summary>
    public async Task<Result<IReadOnlyDictionary<DateOnly, List<WorkoutSummaryRowDto>>>> GetCalendarAsync(
        Guid ownerId,
        int year,
        int month,
        CancellationToken ct = default)
    {
        if (month is < 1 or > 12)
            return Result<IReadOnlyDictionary<DateOnly, List<WorkoutSummaryRowDto>>>.Invalid(nameof(month), "Month must be 1 to 12.");

        var userSettings = await settings.GetOrCreateSettingsAsync(ownerId, ct);
        var zone = ResolveTimeZone(userSettings.TimeZone);

        // Query a padded UTC window so sessions near month boundaries are not missed
        // before local-time conversion decides which day they belong to.
        var monthStart = new DateTimeOffset(new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc));
        var windowStart = monthStart.AddDays(-2);
        var windowEnd = monthStart.AddMonths(1).AddDays(2);

        var sessions = await db.WorkoutSessions
            .Where(x => x.OwnerId == ownerId
                && x.Status == WorkoutStatus.Completed
                && x.StartedAt >= windowStart
                && x.StartedAt < windowEnd)
            .Include(x => x.Exercises).ThenInclude(x => x.Sets)
            .AsNoTracking()
            .ToListAsync(ct);

        var grouped = new Dictionary<DateOnly, List<WorkoutSummaryRowDto>>();

        foreach (var session in sessions)
        {
            var localDate = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(session.StartedAt, zone).DateTime);
            if (localDate.Year != year || localDate.Month != month) continue;

            if (!grouped.TryGetValue(localDate, out var list))
            {
                list = [];
                grouped[localDate] = list;
            }

            list.Add(session.ToSummaryRow());
        }

        return Result<IReadOnlyDictionary<DateOnly, List<WorkoutSummaryRowDto>>>.Ok(grouped);
    }

    // ---------------------------------------------------------------------------------
    // Start
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// Starts a session from a routine, from a previous workout, or empty. Exactly one
    /// active session is permitted; an existing one is returned as a conflict so the
    /// client can offer resume rather than silently creating a second (spec US-030).
    /// </summary>
    public async Task<Result<WorkoutSessionDto>> StartAsync(
        Guid ownerId,
        StartWorkoutRequest request,
        CancellationToken ct = default)
    {
        var existing = await Query(ownerId).FirstOrDefaultAsync(x => x.Status == WorkoutStatus.Active, ct);
        if (existing is not null)
        {
            return Result<WorkoutSessionDto>.Conflict(
                "You already have an active workout.",
                await DecorateAsync(ownerId, existing, ct));
        }

        var userSettings = await settings.GetOrCreateSettingsAsync(ownerId, ct);

        var session = new WorkoutSession
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Title = "Quick workout",
            Status = WorkoutStatus.Active,
            StartedAt = clock.UtcNow,
            Version = 1
        };

        if (request.RoutineId is { } routineId)
        {
            var routine = await db.Routines
                .Where(x => x.Id == routineId && x.OwnerId == ownerId)
                .Include(x => x.Exercises.OrderBy(e => e.Order)).ThenInclude(x => x.Exercise)
                .Include(x => x.Exercises).ThenInclude(x => x.Sets.OrderBy(s => s.Order))
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (routine is null) return Result<WorkoutSessionDto>.NotFound("Routine not found.");

            session.RoutineId = routine.Id;
            session.Title = routine.Name;
            session.Exercises = routine.Exercises.Select(BuildFromTemplate).ToList();
        }
        else if (request.CopyFromWorkoutId is { } sourceId)
        {
            var source = await db.WorkoutSessions
                .Where(x => x.Id == sourceId && x.OwnerId == ownerId)
                .Include(x => x.Exercises.OrderBy(e => e.Order)).ThenInclude(x => x.Sets.OrderBy(s => s.Order))
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (source is null) return Result<WorkoutSessionDto>.NotFound("Workout not found.");

            session.RoutineId = source.RoutineId;
            session.Title = source.Title;
            session.Exercises = source.Exercises.Select(BuildFromPrevious).ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.Title)) session.Title = request.Title.Trim();

        // An empty workout still needs a sensible default rest value from settings.
        foreach (var exercise in session.Exercises.Where(x => x.RestSeconds <= 0))
        {
            exercise.RestSeconds = userSettings.DefaultRestSeconds;
        }

        db.WorkoutSessions.Add(session);
        await db.SaveChangesAsync(ct);

        return await GetAsync(ownerId, session.Id, ct);
    }

    /// <summary>Creates workout exercises and planned sets from a routine template.</summary>
    private static WorkoutExercise BuildFromTemplate(RoutineExercise template) => new()
    {
        Id = Guid.NewGuid(),
        ExerciseId = template.ExerciseId,
        // Snapshot the name and type so later edits to the exercise cannot rewrite history.
        ExerciseName = template.Exercise?.Name ?? "Exercise",
        ExerciseType = template.Exercise?.Type ?? ExerciseType.WeightAndReps,
        Order = template.Order,
        RestSeconds = template.RestSeconds,
        Notes = template.Notes,
        SupersetGroup = template.SupersetGroup,
        SupersetKind = template.SupersetKind,
        Sets = template.Sets.OrderBy(x => x.Order).Select(set => new WorkoutSet
        {
            Id = Guid.NewGuid(),
            Order = set.Order,
            // Targets pre-fill the inputs but the set is not complete until the user says so.
            Weight = set.TargetWeight ?? 0m,
            Reps = set.TargetReps,
            Type = set.Type
        }).ToList()
    };

    /// <summary>Creates workout exercises by repeating a previous session's structure.</summary>
    private static WorkoutExercise BuildFromPrevious(WorkoutExercise source) => new()
    {
        Id = Guid.NewGuid(),
        ExerciseId = source.ExerciseId,
        ExerciseName = source.ExerciseName,
        ExerciseType = source.ExerciseType,
        Order = source.Order,
        RestSeconds = source.RestSeconds,
        Notes = source.Notes,
        SupersetGroup = source.SupersetGroup,
        SupersetKind = source.SupersetKind,
        Sets = source.Sets.OrderBy(x => x.Order).Select(set => new WorkoutSet
        {
            Id = Guid.NewGuid(),
            Order = set.Order,
            Weight = set.Weight,
            Reps = set.Reps,
            Type = set.Type,
            // Values carry over as a starting point; completion state deliberately does not.
            CompletedAt = null
        }).ToList()
    };

    // ---------------------------------------------------------------------------------
    // Update
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// Replaces the mutable contents of a session.
    /// <para>
    /// The client sends the whole exercise/set tree along with the version it last saw.
    /// A version mismatch is reported as a conflict with the current server state
    /// attached, so an offline replay can never silently overwrite newer data
    /// (spec Epic 30, 13.2).
    /// </para>
    /// </summary>
    public async Task<Result<WorkoutSessionDto>> UpdateAsync(
        Guid ownerId,
        Guid workoutId,
        UpdateWorkoutRequest request,
        CancellationToken ct = default)
    {
        var validation = ValidateSets(request);
        if (validation is not null) return validation;

        var session = await db.WorkoutSessions
            .Where(x => x.Id == workoutId && x.OwnerId == ownerId)
            .Include(x => x.Exercises).ThenInclude(x => x.Sets)
            .FirstOrDefaultAsync(ct);

        if (session is null) return Result<WorkoutSessionDto>.NotFound("Workout not found.");

        if (session.Status == WorkoutStatus.Cancelled)
            return Result<WorkoutSessionDto>.Conflict("A cancelled workout cannot be edited.");

        // Version 0 means "no opinion", which lets a first-time client adopt server state.
        if (request.Version != 0 && request.Version != session.Version)
        {
            return Result<WorkoutSessionDto>.Conflict(
                "This workout was changed elsewhere. Review the latest version before saving.",
                await DecorateAsync(ownerId, session, ct));
        }

        var exerciseIds = request.Exercises.Select(x => x.ExerciseId).Distinct().ToList();
        var known = await db.Exercises
            .Where(x => exerciseIds.Contains(x.Id) && (x.OwnerId == null || x.OwnerId == ownerId))
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Id, ct);

        if (known.Count != exerciseIds.Count)
            return Result<WorkoutSessionDto>.Invalid(nameof(request.Exercises), "One or more exercises are unavailable.");

        if (!string.IsNullOrWhiteSpace(request.Title)) session.Title = request.Title.Trim();
        if (request.Notes is not null) session.Notes = request.Notes.Trim();

        ApplyExercises(session, request, known);

        session.Version++;
        await db.SaveChangesAsync(ct);

        // Editing a completed workout invalidates derived records, so rebuild them.
        if (session.Status == WorkoutStatus.Completed) await records.RecomputeAsync(ownerId, ct);

        return await GetAsync(ownerId, session.Id, ct);
    }

    /// <summary>
    /// Reconciles the incoming tree with the tracked entities, reusing rows whose ids
    /// match so client-generated ids survive an offline replay unchanged.
    /// </summary>
    private void ApplyExercises(
        WorkoutSession session,
        UpdateWorkoutRequest request,
        Dictionary<Guid, Exercise> catalog)
    {
        var keptExercises = new List<WorkoutExercise>();

        for (var index = 0; index < request.Exercises.Count; index++)
        {
            var incoming = request.Exercises[index];
            var exercise = session.Exercises.FirstOrDefault(x => x.Id == incoming.Id);
            var catalogEntry = catalog[incoming.ExerciseId];

            if (exercise is null)
            {
                exercise = new WorkoutExercise
                {
                    // Honour the client-supplied id when present so replays are idempotent.
                    Id = incoming.Id == Guid.Empty ? Guid.NewGuid() : incoming.Id,
                    WorkoutSessionId = session.Id,
                    ExerciseId = incoming.ExerciseId,
                    ExerciseName = catalogEntry.Name,
                    ExerciseType = catalogEntry.Type
                };
                db.WorkoutExercises.Add(exercise);
            }

            exercise.ExerciseId = incoming.ExerciseId;
            exercise.ExerciseName = catalogEntry.Name;
            exercise.ExerciseType = catalogEntry.Type;
            exercise.Order = index;
            exercise.RestSeconds = incoming.RestSeconds;
            exercise.Notes = incoming.Notes?.Trim() ?? "";
            exercise.SupersetGroup = incoming.SupersetGroup;
            exercise.SupersetKind = incoming.SupersetGroup is null ? SupersetKind.None : incoming.SupersetKind;

            ApplySets(exercise, incoming);
            keptExercises.Add(exercise);
        }

        // Anything the client no longer lists has been deleted by the user.
        var removedExercises = session.Exercises.Where(x => keptExercises.All(kept => kept.Id != x.Id)).ToList();
        db.WorkoutSets.RemoveRange(removedExercises.SelectMany(x => x.Sets));
        db.WorkoutExercises.RemoveRange(removedExercises);
    }

    private void ApplySets(WorkoutExercise exercise, UpdateWorkoutExerciseRequest incoming)
    {
        var keptSets = new List<WorkoutSet>();

        for (var index = 0; index < incoming.Sets.Count; index++)
        {
            var incomingSet = incoming.Sets[index];
            var set = exercise.Sets.FirstOrDefault(x => x.Id == incomingSet.Id);

            if (set is null)
            {
                set = new WorkoutSet
                {
                    Id = incomingSet.Id == Guid.Empty ? Guid.NewGuid() : incomingSet.Id,
                    WorkoutExerciseId = exercise.Id
                };
                exercise.Sets.Add(set);
                db.WorkoutSets.Add(set);
            }

            set.Order = index;
            set.Weight = incomingSet.Weight;
            set.Reps = incomingSet.Reps;
            set.Rpe = incomingSet.Rpe;
            set.Type = incomingSet.Type;
            set.DurationSeconds = incomingSet.DurationSeconds;
            set.DistanceMeters = incomingSet.DistanceMeters;
            set.Notes = incomingSet.Notes?.Trim() ?? "";

            // Preserve the original completion timestamp so re-saving does not shift history.
            set.CompletedAt = incomingSet.Completed ? set.CompletedAt ?? clock.UtcNow : null;

            keptSets.Add(set);
        }

        var removed = exercise.Sets.Where(x => keptSets.All(kept => kept.Id != x.Id)).ToList();
        db.WorkoutSets.RemoveRange(removed);
    }

    // ---------------------------------------------------------------------------------
    // Finish, cancel, delete
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// Completes the session, recomputes records and returns the summary shown to the
    /// user (spec US-070).
    /// </summary>
    public async Task<Result<WorkoutCompletionDto>> FinishAsync(
        Guid ownerId,
        Guid workoutId,
        FinishWorkoutRequest? request = null,
        CancellationToken ct = default)
    {
        var session = await db.WorkoutSessions
            .Where(x => x.Id == workoutId && x.OwnerId == ownerId)
            .Include(x => x.Exercises).ThenInclude(x => x.Sets)
            .FirstOrDefaultAsync(ct);

        if (session is null) return Result<WorkoutCompletionDto>.NotFound("Workout not found.");

        if (session.Status != WorkoutStatus.Active)
            return Result<WorkoutCompletionDto>.Conflict("This workout is no longer active.");

        var completedSets = TrainingVolume.CompletedWorkSets(session);
        if (completedSets == 0)
            return Result<WorkoutCompletionDto>.Invalid("Complete at least one set before finishing, or cancel the workout.");

        // Discard planned-but-unperformed sets so history reflects what actually happened.
        var unfinished = session.Exercises.SelectMany(x => x.Sets).Where(x => !x.IsCompleted).ToList();
        db.WorkoutSets.RemoveRange(unfinished);
        foreach (var exercise in session.Exercises)
        {
            exercise.Sets = exercise.Sets.Where(x => x.IsCompleted).OrderBy(x => x.Order).ToList();
            for (var index = 0; index < exercise.Sets.Count; index++) exercise.Sets[index].Order = index;
        }

        var emptyExercises = session.Exercises.Where(x => x.Sets.Count == 0).ToList();
        db.WorkoutExercises.RemoveRange(emptyExercises);
        session.Exercises = session.Exercises.Except(emptyExercises).ToList();

        session.Status = WorkoutStatus.Completed;
        session.CompletedAt = clock.UtcNow;
        if (request?.Notes is not null) session.Notes = request.Notes.Trim();
        session.Version++;

        await db.SaveChangesAsync(ct);

        var allRecords = await records.RecomputeAsync(ownerId, ct);
        var newRecords = await records.RecordsForSessionAsync(ownerId, session.Id, allRecords, ct);
        var breakdown = await MuscleBreakdownAsync(session, ct);

        return Result<WorkoutCompletionDto>.Ok(new WorkoutCompletionDto(
            session.Id,
            session.Title,
            session.StartedAt,
            session.CompletedAt.Value,
            (int)(session.Duration ?? TimeSpan.Zero).TotalSeconds,
            TrainingVolume.CompletedWorkSets(session),
            TrainingVolume.CompletedReps(session),
            TrainingVolume.ForSession(session),
            newRecords,
            breakdown));
    }

    /// <summary>Abandons an active session. Cancelled sessions are excluded from all stats.</summary>
    public async Task<Result> CancelAsync(Guid ownerId, Guid workoutId, CancellationToken ct = default)
    {
        var session = await db.WorkoutSessions
            .FirstOrDefaultAsync(x => x.Id == workoutId && x.OwnerId == ownerId, ct);

        if (session is null) return Result.NotFound("Workout not found.");
        if (session.Status != WorkoutStatus.Active) return Result.Conflict("This workout is no longer active.");

        session.Status = WorkoutStatus.Cancelled;
        session.CompletedAt = clock.UtcNow;
        session.Version++;

        await db.SaveChangesAsync(ct);
        return Result.Ok();
    }

    /// <summary>Permanently deletes a session and recomputes records.</summary>
    public async Task<Result> DeleteAsync(Guid ownerId, Guid workoutId, CancellationToken ct = default)
    {
        var session = await db.WorkoutSessions
            .Where(x => x.Id == workoutId && x.OwnerId == ownerId)
            .Include(x => x.Exercises).ThenInclude(x => x.Sets)
            .FirstOrDefaultAsync(ct);

        if (session is null) return Result.NotFound("Workout not found.");

        db.WorkoutSets.RemoveRange(session.Exercises.SelectMany(x => x.Sets));
        db.WorkoutExercises.RemoveRange(session.Exercises);
        db.WorkoutSessions.Remove(session);

        await db.SaveChangesAsync(ct);
        await records.RecomputeAsync(ownerId, ct);

        return Result.Ok();
    }

    // ---------------------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------------------

    private IQueryable<WorkoutSession> Query(Guid ownerId)
        => db.WorkoutSessions
            .Where(x => x.OwnerId == ownerId)
            .Include(x => x.Exercises.OrderBy(e => e.Order)).ThenInclude(x => x.Sets.OrderBy(s => s.Order));

    /// <summary>
    /// Attaches persistent exercise notes and previous-performance values to a session DTO.
    /// </summary>
    private async Task<WorkoutSessionDto> DecorateAsync(Guid ownerId, WorkoutSession session, CancellationToken ct)
    {
        var exerciseIds = session.Exercises.Select(x => x.ExerciseId).Distinct().ToList();

        var notes = exerciseIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await db.ExerciseNotes
                .Where(x => x.OwnerId == ownerId && exerciseIds.Contains(x.ExerciseId))
                .AsNoTracking()
                .ToDictionaryAsync(x => x.ExerciseId, x => x.Text, ct);

        var previous = await ResolvePreviousAsync(ownerId, session, ct);
        return session.ToDto(notes, previous);
    }

    /// <summary>
    /// Resolves the most recent completed performance of each exercise, keyed by set
    /// position, so the active workout can show "previous" beside each row (spec US-032).
    /// </summary>
    private async Task<Dictionary<Guid, IReadOnlyDictionary<int, PreviousSetDto>>> ResolvePreviousAsync(
        Guid ownerId,
        WorkoutSession session,
        CancellationToken ct)
    {
        var result = new Dictionary<Guid, IReadOnlyDictionary<int, PreviousSetDto>>();
        var exerciseIds = session.Exercises.Select(x => x.ExerciseId).Distinct().ToList();
        if (exerciseIds.Count == 0) return result;

        // Pull the candidate history for all exercises in one query, then pick the most
        // recent session per exercise in memory.
        var history = await db.WorkoutExercises
            .Where(x => exerciseIds.Contains(x.ExerciseId)
                && x.WorkoutSession!.OwnerId == ownerId
                && x.WorkoutSession.Status == WorkoutStatus.Completed
                && x.WorkoutSessionId != session.Id)
            .OrderByDescending(x => x.WorkoutSession!.CompletedAt)
            .Select(x => new
            {
                x.ExerciseId,
                PerformedAt = x.WorkoutSession!.CompletedAt ?? x.WorkoutSession.StartedAt,
                Sets = x.Sets
                    .Where(s => s.CompletedAt != null)
                    .OrderBy(s => s.Order)
                    .Select(s => new { s.Order, s.Weight, s.Reps })
                    .ToList()
            })
            .AsNoTracking()
            .ToListAsync(ct);

        foreach (var group in history.GroupBy(x => x.ExerciseId))
        {
            var latest = group.Where(x => x.Sets.Count > 0).OrderByDescending(x => x.PerformedAt).FirstOrDefault();
            if (latest is null) continue;

            result[group.Key] = latest.Sets.ToDictionary(
                set => set.Order,
                set => new PreviousSetDto(set.Weight, set.Reps, latest.PerformedAt));
        }

        return result;
    }

    /// <summary>
    /// Ranked muscle contribution for one session, weighted by each exercise's configured
    /// primary/secondary contribution (spec US-110).
    /// </summary>
    public async Task<List<MuscleContributionDto>> MuscleBreakdownAsync(WorkoutSession session, CancellationToken ct = default)
    {
        var exerciseIds = session.Exercises.Select(x => x.ExerciseId).Distinct().ToList();
        if (exerciseIds.Count == 0) return [];

        var mappings = await db.ExerciseMuscles
            .Where(x => exerciseIds.Contains(x.ExerciseId))
            .Include(x => x.Muscle)
            .AsNoTracking()
            .ToListAsync(ct);

        if (mappings.Count == 0) return [];

        var scores = new Dictionary<Guid, (string Name, string Region, decimal Score, int Sets)>();

        foreach (var exercise in session.Exercises)
        {
            var completed = exercise.Sets.Count(x => x.IsCompleted && x.IsWorkSet);
            if (completed == 0) continue;

            // Volume scales the score where load exists; otherwise set count carries it.
            var volume = TrainingVolume.ForExercise(exercise);
            var weightPerSet = volume > 0 ? volume : completed * 100m;

            foreach (var mapping in mappings.Where(x => x.ExerciseId == exercise.ExerciseId))
            {
                if (mapping.Muscle is null) continue;

                var contribution = weightPerSet * mapping.ContributionWeight;
                var setShare = (int)Math.Round(completed * (double)mapping.ContributionWeight, MidpointRounding.AwayFromZero);

                if (scores.TryGetValue(mapping.MuscleId, out var current))
                {
                    scores[mapping.MuscleId] = (current.Name, current.Region, current.Score + contribution, current.Sets + setShare);
                }
                else
                {
                    scores[mapping.MuscleId] = (mapping.Muscle.Name, mapping.Muscle.BodyRegion, contribution, setShare);
                }
            }
        }

        return scores.Values
            .OrderByDescending(x => x.Score)
            .Select(x => new MuscleContributionDto(x.Name, x.Region, decimal.Round(x.Score, 2, MidpointRounding.AwayFromZero), x.Sets))
            .ToList();
    }

    private static Result<WorkoutSessionDto>? ValidateSets(UpdateWorkoutRequest request)
    {
        var exercises = request.Exercises ?? [];

        if (exercises.Count > MaxExercisesPerWorkout)
            return Result<WorkoutSessionDto>.Invalid(nameof(request.Exercises), $"A workout cannot exceed {MaxExercisesPerWorkout} exercises.");

        foreach (var exercise in exercises)
        {
            if (exercise.RestSeconds is < 0 or > 3600)
                return Result<WorkoutSessionDto>.Invalid(nameof(exercise.RestSeconds), "Rest must be between 0 and 3600 seconds.");

            var sets = exercise.Sets ?? [];
            if (sets.Count > MaxSetsPerExercise)
                return Result<WorkoutSessionDto>.Invalid(nameof(exercise.Sets), $"An exercise cannot exceed {MaxSetsPerExercise} sets.");

            foreach (var set in sets)
            {
                if (set.Weight is < 0 or > 2000)
                    return Result<WorkoutSessionDto>.Invalid(nameof(set.Weight), "Weight must be between 0 and 2000 kg.");

                if (set.Reps is < 0 or > 1000)
                    return Result<WorkoutSessionDto>.Invalid(nameof(set.Reps), "Reps must be between 0 and 1000.");

                if (set.Rpe is { } rpe && (rpe < 1m || rpe > 10m))
                    return Result<WorkoutSessionDto>.Invalid(nameof(set.Rpe), "RPE must be between 1 and 10.");

                if (set.DurationSeconds is < 0 or > 86400)
                    return Result<WorkoutSessionDto>.Invalid(nameof(set.DurationSeconds), "Duration must be between 0 and 86400 seconds.");

                if (set.DistanceMeters is < 0 or > 1000000)
                    return Result<WorkoutSessionDto>.Invalid(nameof(set.DistanceMeters), "Distance must be between 0 and 1000000 meters.");

                if (set.Notes is { Length: > 500 })
                    return Result<WorkoutSessionDto>.Invalid(nameof(set.Notes), "Set notes cannot exceed 500 characters.");
            }
        }

        return null;
    }

    /// <summary>Falls back to UTC rather than throwing if a stored timezone id is unavailable.</summary>
    internal static TimeZoneInfo ResolveTimeZone(string id)
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
        catch (TimeZoneNotFoundException) { return TimeZoneInfo.Utc; }
        catch (InvalidTimeZoneException) { return TimeZoneInfo.Utc; }
    }
}
