using System.Security.Claims;
using WorkoutTracker.Application.Contracts;
using WorkoutTracker.Application.Services;

namespace WorkoutTracker.Api.Endpoints;

/// <summary>Exercise library and reference data endpoints (spec 5 /api/exercises).</summary>
public static class ExerciseEndpoints
{
    public static void MapExerciseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/exercises").WithTags("Exercises").RequireAuthorization();

        group.MapGet("/", async (
            ClaimsPrincipal principal,
            ExerciseService exercises,
            string? search,
            Guid? muscleId,
            Guid? equipmentId,
            string? category,
            bool includeArchived,
            CancellationToken ct) =>
            (await exercises.ListAsync(principal.UserId(), search, muscleId, equipmentId, category, includeArchived, ct)).ToHttp());

        group.MapGet("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal principal,
            ExerciseService exercises,
            CancellationToken ct) =>
            (await exercises.GetAsync(principal.UserId(), id, ct)).ToHttp());

        group.MapPost("/", async (
            SaveExerciseRequest request,
            ClaimsPrincipal principal,
            ExerciseService exercises,
            CancellationToken ct) =>
            (await exercises.CreateAsync(principal.UserId(), request, ct))
                .ToCreated(dto => $"/api/exercises/{dto.Id}"));

        group.MapPut("/{id:guid}", async (
            Guid id,
            SaveExerciseRequest request,
            ClaimsPrincipal principal,
            ExerciseService exercises,
            CancellationToken ct) =>
            (await exercises.UpdateAsync(principal.UserId(), id, request, ct)).ToHttp());

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal principal,
            ExerciseService exercises,
            CancellationToken ct) =>
            (await exercises.DeleteAsync(principal.UserId(), id, ct)).ToHttp());

        // Persistent note that reappears next time the exercise is trained (spec US-150).
        group.MapPut("/{id:guid}/note", async (
            Guid id,
            SaveExerciseNoteRequest request,
            ClaimsPrincipal principal,
            ExerciseService exercises,
            CancellationToken ct) =>
            (await exercises.SaveNoteAsync(principal.UserId(), id, request.Text, ct)).ToHttp());

        group.MapGet("/{id:guid}/history", async (
            Guid id,
            ClaimsPrincipal principal,
            ExerciseService exercises,
            int page,
            int pageSize,
            DateTimeOffset? from,
            DateTimeOffset? to,
            CancellationToken ct) =>
            (await exercises.GetHistoryAsync(principal.UserId(), id, page is 0 ? 1 : page, pageSize is 0 ? 20 : pageSize, from, to, ct)).ToHttp());

        // Reference taxonomies used by the filter UI. Safe to cache client-side.
        var reference = app.MapGroup("/api/reference").WithTags("Reference").RequireAuthorization();

        reference.MapGet("/muscles", async (ExerciseService exercises, CancellationToken ct) =>
            TypedResults.Ok(await exercises.ListMusclesAsync(ct)));

        reference.MapGet("/equipment", async (ExerciseService exercises, CancellationToken ct) =>
            TypedResults.Ok(await exercises.ListEquipmentAsync(ct)));
    }
}

/// <summary>Routine, folder and schedule endpoints (spec 5 /api/routines).</summary>
public static class RoutineEndpoints
{
    public static void MapRoutineEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/routines").WithTags("Routines").RequireAuthorization();

        group.MapGet("/", async (
            ClaimsPrincipal principal,
            RoutineService routines,
            bool includeArchived,
            CancellationToken ct) =>
            (await routines.ListAsync(principal.UserId(), includeArchived, ct)).ToHttp());

        group.MapGet("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal principal,
            RoutineService routines,
            CancellationToken ct) =>
            (await routines.GetAsync(principal.UserId(), id, ct)).ToHttp());

        group.MapPost("/", async (
            SaveRoutineRequest request,
            ClaimsPrincipal principal,
            RoutineService routines,
            CancellationToken ct) =>
            (await routines.CreateAsync(principal.UserId(), request, ct))
                .ToCreated(dto => $"/api/routines/{dto.Id}"));

        group.MapPut("/{id:guid}", async (
            Guid id,
            SaveRoutineRequest request,
            ClaimsPrincipal principal,
            RoutineService routines,
            CancellationToken ct) =>
            (await routines.UpdateAsync(principal.UserId(), id, request, ct)).ToHttp());

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal principal,
            RoutineService routines,
            CancellationToken ct) =>
            (await routines.DeleteAsync(principal.UserId(), id, ct)).ToHttp());

        group.MapPost("/{id:guid}/duplicate", async (
            Guid id,
            ClaimsPrincipal principal,
            RoutineService routines,
            CancellationToken ct) =>
            (await routines.DuplicateAsync(principal.UserId(), id, ct))
                .ToCreated(dto => $"/api/routines/{dto.Id}"));

        group.MapPost("/reorder", async (
            ReorderRoutinesRequest request,
            ClaimsPrincipal principal,
            RoutineService routines,
            CancellationToken ct) =>
            (await routines.ReorderAsync(principal.UserId(), request, ct)).ToHttp());

        // ----- Folders -----
        var folders = app.MapGroup("/api/routine-folders").WithTags("Routines").RequireAuthorization();

        folders.MapGet("/", async (ClaimsPrincipal principal, RoutineService routines, CancellationToken ct) =>
            (await routines.ListFoldersAsync(principal.UserId(), ct)).ToHttp());

        folders.MapPost("/", async (
            SaveRoutineFolderRequest request,
            ClaimsPrincipal principal,
            RoutineService routines,
            CancellationToken ct) =>
            (await routines.CreateFolderAsync(principal.UserId(), request, ct))
                .ToCreated(dto => $"/api/routine-folders/{dto.Id}"));

        folders.MapPut("/{id:guid}", async (
            Guid id,
            SaveRoutineFolderRequest request,
            ClaimsPrincipal principal,
            RoutineService routines,
            CancellationToken ct) =>
            (await routines.RenameFolderAsync(principal.UserId(), id, request, ct)).ToHttp());

        folders.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal principal,
            RoutineService routines,
            CancellationToken ct) =>
            (await routines.DeleteFolderAsync(principal.UserId(), id, ct)).ToHttp());

        // ----- Schedule -----
        var schedule = app.MapGroup("/api/schedule").WithTags("Routines").RequireAuthorization();

        schedule.MapGet("/", async (ClaimsPrincipal principal, RoutineService routines, CancellationToken ct) =>
            (await routines.ListSchedulesAsync(principal.UserId(), ct)).ToHttp());

        schedule.MapPut("/", async (
            SaveScheduleRequest request,
            ClaimsPrincipal principal,
            RoutineService routines,
            CancellationToken ct) =>
            (await routines.SaveScheduleAsync(principal.UserId(), request, ct)).ToHttp());

        schedule.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal principal,
            RoutineService routines,
            CancellationToken ct) =>
            (await routines.DeleteScheduleAsync(principal.UserId(), id, ct)).ToHttp());
    }
}
