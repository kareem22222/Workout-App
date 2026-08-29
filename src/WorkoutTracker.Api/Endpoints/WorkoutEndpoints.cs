using System.Security.Claims;
using WorkoutTracker.Application.Contracts;
using WorkoutTracker.Application.Services;
using WorkoutTracker.Domain;

namespace WorkoutTracker.Api.Endpoints;

/// <summary>Workout lifecycle and history endpoints (spec 5 /api/workouts).</summary>
public static class WorkoutEndpoints
{
    public static void MapWorkoutEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/workouts").WithTags("Workouts").RequireAuthorization();

        group.MapGet("/", async (
            ClaimsPrincipal principal,
            WorkoutService workouts,
            int page,
            int pageSize,
            DateTimeOffset? from,
            DateTimeOffset? to,
            Guid? routineId,
            Guid? exerciseId,
            WorkoutStatus? status,
            CancellationToken ct) =>
            (await workouts.ListAsync(
                principal.UserId(),
                page is 0 ? 1 : page,
                pageSize is 0 ? 20 : pageSize,
                from, to, routineId, exerciseId,
                status ?? WorkoutStatus.Completed,
                ct)).ToHttp());

        // Declared before the {id} route so "active" is not parsed as an id.
        group.MapGet("/active", async (ClaimsPrincipal principal, WorkoutService workouts, CancellationToken ct) =>
            (await workouts.GetActiveAsync(principal.UserId(), ct)).ToHttp());

        group.MapGet("/calendar", async (
            ClaimsPrincipal principal,
            WorkoutService workouts,
            int year,
            int month,
            CancellationToken ct) =>
        {
            var now = DateTimeOffset.UtcNow;
            var result = await workouts.GetCalendarAsync(
                principal.UserId(),
                year is 0 ? now.Year : year,
                month is 0 ? now.Month : month,
                ct);

            if (!result.Succeeded) return result.ToHttp();

            // Serialize dates as ISO strings so the client gets stable keys.
            var payload = result.Value!.ToDictionary(
                pair => pair.Key.ToString("yyyy-MM-dd"),
                pair => pair.Value);

            return TypedResults.Ok(payload);
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal principal,
            WorkoutService workouts,
            CancellationToken ct) =>
            (await workouts.GetAsync(principal.UserId(), id, ct)).ToHttp());

        group.MapPost("/start", async (
            StartWorkoutRequest request,
            ClaimsPrincipal principal,
            WorkoutService workouts,
            CancellationToken ct) =>
            (await workouts.StartAsync(principal.UserId(), request, ct))
                .ToCreated(dto => $"/api/workouts/{dto.Id}"));

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateWorkoutRequest request,
            ClaimsPrincipal principal,
            WorkoutService workouts,
            CancellationToken ct) =>
            (await workouts.UpdateAsync(principal.UserId(), id, request, ct)).ToHttp());

        group.MapPost("/{id:guid}/finish", async (
            Guid id,
            FinishWorkoutRequest? request,
            ClaimsPrincipal principal,
            WorkoutService workouts,
            CancellationToken ct) =>
            (await workouts.FinishAsync(principal.UserId(), id, request, ct)).ToHttp());

        group.MapPost("/{id:guid}/cancel", async (
            Guid id,
            ClaimsPrincipal principal,
            WorkoutService workouts,
            CancellationToken ct) =>
            (await workouts.CancelAsync(principal.UserId(), id, ct)).ToHttp());

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal principal,
            WorkoutService workouts,
            CancellationToken ct) =>
            (await workouts.DeleteAsync(principal.UserId(), id, ct)).ToHttp());
    }
}
