using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Application.Contracts;
using WorkoutTracker.Domain;

namespace WorkoutTracker.Application.Services;

/// <summary>
/// Exercise library use cases. Visibility is always the built-in catalog plus the
/// caller's own custom exercises, so private movements never leak between users
/// (spec US-012).
/// </summary>
public sealed class ExerciseService(IAppDbContext db, SettingsService settings, IClock clock)
{
    /// <summary>Secondary muscles contribute less work than primaries by default (spec US-110).</summary>
    private const decimal DefaultSecondaryContribution = 0.4m;

    /// <summary>
    /// Searchable, filterable list of exercises available to the caller.
    /// </summary>
    /// <param name="search">Case-insensitive name fragment.</param>
    /// <param name="muscleId">Restrict to exercises training this muscle in any role.</param>
    /// <param name="equipmentId">Restrict to exercises using this equipment.</param>
    /// <param name="category">Restrict to a category such as Push or Pull.</param>
    /// <param name="includeArchived">Include soft-deleted custom exercises.</param>
    public async Task<Result<IReadOnlyList<ExerciseDto>>> ListAsync(
        Guid ownerId,
        string? search = null,
        Guid? muscleId = null,
        Guid? equipmentId = null,
        string? category = null,
        bool includeArchived = false,
        CancellationToken ct = default)
    {
        var query = VisibleExercises(ownerId);

        if (!includeArchived) query = query.Where(x => !x.IsArchived);

        if (!string.IsNullOrWhiteSpace(search))
        {
            // Lowered Contains keeps this provider-agnostic; PostgreSQL turns it into
            // lower(name) LIKE '%term%'.
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(x => x.Name.ToLower().Contains(term));
        }

        if (muscleId is { } muscle) query = query.Where(x => x.Muscles.Any(m => m.MuscleId == muscle));
        if (equipmentId is { } equipment) query = query.Where(x => x.EquipmentId == equipment);
        if (!string.IsNullOrWhiteSpace(category)) query = query.Where(x => x.Category == category);

        var exercises = await query
            .Include(x => x.Equipment)
            .Include(x => x.Muscles).ThenInclude(x => x.Muscle)
            .OrderBy(x => x.Name)
            .AsNoTracking()
            .ToListAsync(ct);

        var notes = await NotesByExerciseAsync(ownerId, exercises.Select(x => x.Id).ToList(), ct);

        return Result<IReadOnlyList<ExerciseDto>>.Ok(
            exercises.Select(x => x.ToDto(notes.GetValueOrDefault(x.Id))).ToList());
    }

    public async Task<Result<ExerciseDto>> GetAsync(Guid ownerId, Guid exerciseId, CancellationToken ct = default)
    {
        var exercise = await VisibleExercises(ownerId)
            .Include(x => x.Equipment)
            .Include(x => x.Muscles).ThenInclude(x => x.Muscle)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == exerciseId, ct);

        if (exercise is null) return Result<ExerciseDto>.NotFound("Exercise not found.");

        var note = await db.ExerciseNotes
            .Where(x => x.OwnerId == ownerId && x.ExerciseId == exerciseId)
            .Select(x => x.Text)
            .FirstOrDefaultAsync(ct);

        return Result<ExerciseDto>.Ok(exercise.ToDto(note));
    }

    public async Task<Result<ExerciseDto>> CreateAsync(
        Guid ownerId,
        SaveExerciseRequest request,
        CancellationToken ct = default)
    {
        var validation = await ValidateAsync(ownerId, request, null, ct);
        if (validation is not null) return validation;

        var exercise = new Exercise
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Name = request.Name.Trim(),
            Instructions = request.Instructions?.Trim() ?? "",
            Type = request.Type,
            Category = request.Category?.Trim() ?? "",
            EquipmentId = request.EquipmentId,
            MediaUrl = NormalizeMediaUrl(request.MediaUrl),
            DefaultRestSeconds = request.DefaultRestSeconds,
            DefaultIncrementKg = request.DefaultIncrementKg,
            CreatedAt = clock.UtcNow
        };

        exercise.Muscles = BuildMuscles(exercise.Id, request.Muscles);

        db.Exercises.Add(exercise);
        await db.SaveChangesAsync(ct);

        return await GetAsync(ownerId, exercise.Id, ct);
    }

    public async Task<Result<ExerciseDto>> UpdateAsync(
        Guid ownerId,
        Guid exerciseId,
        SaveExerciseRequest request,
        CancellationToken ct = default)
    {
        var exercise = await db.Exercises
            .Include(x => x.Muscles)
            .FirstOrDefaultAsync(x => x.Id == exerciseId, ct);

        if (exercise is null) return Result<ExerciseDto>.NotFound("Exercise not found.");

        // Built-in catalog entries are global reference data and are not user editable.
        if (exercise.OwnerId is null)
            return Result<ExerciseDto>.Forbidden("Built-in exercises cannot be modified.");

        if (exercise.OwnerId != ownerId)
            return Result<ExerciseDto>.NotFound("Exercise not found.");

        var validation = await ValidateAsync(ownerId, request, exerciseId, ct);
        if (validation is not null) return validation;

        exercise.Name = request.Name.Trim();
        exercise.Instructions = request.Instructions?.Trim() ?? "";
        exercise.Type = request.Type;
        exercise.Category = request.Category?.Trim() ?? "";
        exercise.EquipmentId = request.EquipmentId;
        exercise.MediaUrl = NormalizeMediaUrl(request.MediaUrl);
        exercise.DefaultRestSeconds = request.DefaultRestSeconds;
        exercise.DefaultIncrementKg = request.DefaultIncrementKg;

        db.ExerciseMuscles.RemoveRange(exercise.Muscles);
        exercise.Muscles = BuildMuscles(exercise.Id, request.Muscles);

        await db.SaveChangesAsync(ct);
        return await GetAsync(ownerId, exercise.Id, ct);
    }

    /// <summary>
    /// Deletes a custom exercise. If it has been used in a workout or routine it is
    /// archived instead, so historical data never breaks (spec 4.1).
    /// </summary>
    public async Task<Result> DeleteAsync(Guid ownerId, Guid exerciseId, CancellationToken ct = default)
    {
        var exercise = await db.Exercises.FirstOrDefaultAsync(x => x.Id == exerciseId, ct);
        if (exercise is null) return Result.NotFound("Exercise not found.");
        if (exercise.OwnerId is null) return Result.Forbidden("Built-in exercises cannot be deleted.");
        if (exercise.OwnerId != ownerId) return Result.NotFound("Exercise not found.");

        var isReferenced =
            await db.WorkoutExercises.AnyAsync(x => x.ExerciseId == exerciseId, ct) ||
            await db.RoutineExercises.AnyAsync(x => x.ExerciseId == exerciseId, ct);

        if (isReferenced)
        {
            exercise.IsArchived = true;
        }
        else
        {
            db.Exercises.Remove(exercise);
        }

        await db.SaveChangesAsync(ct);
        return Result.Ok();
    }

    /// <summary>
    /// Saves the persistent note that reappears next time the exercise is trained.
    /// An empty note removes the record (spec US-150).
    /// </summary>
    public async Task<Result<string>> SaveNoteAsync(
        Guid ownerId,
        Guid exerciseId,
        string? text,
        CancellationToken ct = default)
    {
        var exists = await VisibleExercises(ownerId).AnyAsync(x => x.Id == exerciseId, ct);
        if (!exists) return Result<string>.NotFound("Exercise not found.");

        var trimmed = text?.Trim() ?? "";
        if (trimmed.Length > 2000)
            return Result<string>.Invalid(nameof(text), "Note cannot exceed 2000 characters.");

        var note = await db.ExerciseNotes.FirstOrDefaultAsync(x => x.OwnerId == ownerId && x.ExerciseId == exerciseId, ct);

        if (trimmed.Length == 0)
        {
            if (note is not null) db.ExerciseNotes.Remove(note);
            await db.SaveChangesAsync(ct);
            return Result<string>.Ok("");
        }

        if (note is null)
        {
            note = new ExerciseNote { Id = Guid.NewGuid(), OwnerId = ownerId, ExerciseId = exerciseId };
            db.ExerciseNotes.Add(note);
        }

        note.Text = trimmed;
        note.UpdatedAt = clock.UtcNow;

        await db.SaveChangesAsync(ct);
        return Result<string>.Ok(trimmed);
    }

    /// <summary>
    /// Chronological history of every session containing this exercise (spec US-085).
    /// </summary>
    public async Task<Result<PagedResult<ExerciseHistoryEntryDto>>> GetHistoryAsync(
        Guid ownerId,
        Guid exerciseId,
        int page = 1,
        int pageSize = 20,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken ct = default)
    {
        if (!await VisibleExercises(ownerId).AnyAsync(x => x.Id == exerciseId, ct))
            return Result<PagedResult<ExerciseHistoryEntryDto>>.NotFound("Exercise not found.");

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = db.WorkoutSessions
            .Where(x => x.OwnerId == ownerId
                && x.Status == WorkoutStatus.Completed
                && x.Exercises.Any(e => e.ExerciseId == exerciseId));

        if (from is { } start) query = query.Where(x => x.StartedAt >= start);
        if (to is { } end) query = query.Where(x => x.StartedAt <= end);

        var total = await query.CountAsync(ct);

        var sessions = await query
            .OrderByDescending(x => x.CompletedAt ?? x.StartedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(x => x.Exercises.Where(e => e.ExerciseId == exerciseId))
                .ThenInclude(x => x.Sets)
            .AsNoTracking()
            .ToListAsync(ct);

        var formula = (await settings.GetOrCreateSettingsAsync(ownerId, ct)).OneRepMaxFormula;

        var entries = sessions.Select(session =>
        {
            var exercise = session.Exercises.First();
            var sets = exercise.Sets
                .Where(x => x.IsCompleted)
                .OrderBy(x => x.Order)
                .Select(x => new LoggedSetDto(x.Order, x.Weight, x.Reps, x.Rpe, x.Type, x.DurationSeconds, x.DistanceMeters, x.Notes))
                .ToList();

            return new ExerciseHistoryEntryDto(
                session.Id,
                session.Title,
                session.CompletedAt ?? session.StartedAt,
                sets,
                TrainingVolume.ForExercise(exercise),
                OneRepMax.BestEstimate(exercise.Sets, formula));
        }).ToList();

        return Result<PagedResult<ExerciseHistoryEntryDto>>.Ok(
            new PagedResult<ExerciseHistoryEntryDto>(entries, page, pageSize, total));
    }

    public async Task<IReadOnlyList<MuscleDto>> ListMusclesAsync(CancellationToken ct = default)
        => await db.Muscles.OrderBy(x => x.BodyRegion).ThenBy(x => x.Name)
            .Select(x => new MuscleDto(x.Id, x.Slug, x.Name, x.BodyRegion))
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<IReadOnlyList<EquipmentDto>> ListEquipmentAsync(CancellationToken ct = default)
        => await db.Equipment.OrderBy(x => x.Name)
            .Select(x => new EquipmentDto(x.Id, x.Slug, x.Name, x.DefaultBarWeightKg))
            .AsNoTracking()
            .ToListAsync(ct);

    /// <summary>Built-in catalog plus the caller's own exercises.</summary>
    private IQueryable<Exercise> VisibleExercises(Guid ownerId)
        => db.Exercises.Where(x => x.OwnerId == null || x.OwnerId == ownerId);

    private async Task<Dictionary<Guid, string>> NotesByExerciseAsync(
        Guid ownerId,
        List<Guid> exerciseIds,
        CancellationToken ct)
    {
        if (exerciseIds.Count == 0) return [];

        return await db.ExerciseNotes
            .Where(x => x.OwnerId == ownerId && exerciseIds.Contains(x.ExerciseId))
            .AsNoTracking()
            .ToDictionaryAsync(x => x.ExerciseId, x => x.Text, ct);
    }

    private async Task<Result<ExerciseDto>?> ValidateAsync(
        Guid ownerId,
        SaveExerciseRequest request,
        Guid? excludeId,
        CancellationToken ct)
    {
        var name = request.Name?.Trim() ?? "";
        if (name.Length is < 2 or > 120)
            return Result<ExerciseDto>.Invalid(nameof(request.Name), "Name must be 2 to 120 characters.");

        if (request.DefaultRestSeconds is < 0 or > 3600)
            return Result<ExerciseDto>.Invalid(nameof(request.DefaultRestSeconds), "Rest must be between 0 and 3600 seconds.");

        if (request.DefaultIncrementKg is <= 0 or > 50)
            return Result<ExerciseDto>.Invalid(nameof(request.DefaultIncrementKg), "Increment must be greater than 0 and at most 50 kg.");

        if (request.EquipmentId is { } equipmentId && !await db.Equipment.AnyAsync(x => x.Id == equipmentId, ct))
            return Result<ExerciseDto>.Invalid(nameof(request.EquipmentId), "Unknown equipment.");

        var muscleIds = (request.Muscles ?? []).Select(x => x.MuscleId).Distinct().ToList();
        if (muscleIds.Count > 0)
        {
            var known = await db.Muscles.CountAsync(x => muscleIds.Contains(x.Id), ct);
            if (known != muscleIds.Count)
                return Result<ExerciseDto>.Invalid(nameof(request.Muscles), "One or more muscles are unknown.");
        }

        if (request.MediaUrl is { Length: > 0 } && NormalizeMediaUrl(request.MediaUrl) is null)
            return Result<ExerciseDto>.Invalid(nameof(request.MediaUrl), "Media URL must be an absolute http or https URL.");

        // Duplicate names within a user's own library are confusing during search.
        var duplicate = await db.Exercises.AnyAsync(
            x => x.OwnerId == ownerId && x.Name.ToLower() == name.ToLower() && (excludeId == null || x.Id != excludeId),
            ct);

        return duplicate
            ? Result<ExerciseDto>.Invalid(nameof(request.Name), "You already have an exercise with this name.")
            : null;
    }

    private static List<ExerciseMuscle> BuildMuscles(Guid exerciseId, List<ExerciseMuscleRequest>? requested)
        => (requested ?? [])
            .GroupBy(x => x.MuscleId)
            .Select(group =>
            {
                var item = group.First();
                var contribution = item.ContributionWeight
                    ?? (item.Role == MuscleRole.Primary ? 1.0m : DefaultSecondaryContribution);

                return new ExerciseMuscle
                {
                    Id = Guid.NewGuid(),
                    ExerciseId = exerciseId,
                    MuscleId = item.MuscleId,
                    Role = item.Role,
                    ContributionWeight = Math.Clamp(contribution, 0.05m, 1.0m)
                };
            })
            .ToList();

    /// <summary>
    /// Accepts only absolute http/https URLs. User-supplied media must not be able to
    /// inject javascript: or data: URIs into the client.
    /// </summary>
    private static string? NormalizeMediaUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        if (!Uri.TryCreate(value.Trim(), UriKind.Absolute, out var uri)) return null;
        return uri.Scheme is "http" or "https" ? uri.ToString() : null;
    }
}
