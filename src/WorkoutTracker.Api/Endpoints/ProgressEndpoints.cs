using System.Security.Claims;
using WorkoutTracker.Application.Services;

namespace WorkoutTracker.Api.Endpoints;

public static class ProgressEndpoints
{
    public static void MapProgressEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/progress").WithTags("Progress").RequireAuthorization();
        group.MapGet("/exercise/{exerciseId:guid}", async (Guid exerciseId, ClaimsPrincipal principal, ProgressService progress, string? range, CancellationToken ct) => (await progress.GetExerciseProgressAsync(principal.UserId(), exerciseId, range ?? "3m", ct)).ToHttp());
        group.MapGet("/personal-records", async (ClaimsPrincipal principal, PersonalRecordService records, Guid? exerciseId, CancellationToken ct) => (await records.ListAsync(principal.UserId(), exerciseId, ct)).ToHttp());
        group.MapGet("/volume", async (ClaimsPrincipal principal, ProgressService progress, string? range, string? groupBy, CancellationToken ct) => (await progress.GetVolumeAsync(principal.UserId(), range ?? "3m", groupBy ?? "week", ct)).ToHttp());
        group.MapGet("/estimated-one-rep-max", async (ClaimsPrincipal principal, ProgressService progress, CancellationToken ct) => (await progress.GetOneRepMaxSummaryAsync(principal.UserId(), ct)).ToHttp());
        group.MapGet("/stats", async (ClaimsPrincipal principal, ProgressService progress, string? range, string? groupBy, CancellationToken ct) => (await progress.GetStatsAsync(principal.UserId(), range ?? "3m", groupBy ?? "week", ct)).ToHttp());
        group.MapGet("/muscles", async (ClaimsPrincipal principal, ProgressService progress, string? range, CancellationToken ct) => (await progress.GetMuscleBreakdownAsync(principal.UserId(), range ?? "1m", ct)).ToHttp());

        var tools = app.MapGroup("/api/tools").WithTags("Tools").RequireAuthorization();
        tools.MapGet("/plates", async (ClaimsPrincipal principal, ProgressService progress, decimal targetKg, decimal? barKg, CancellationToken ct) => (await progress.GetPlatesAsync(principal.UserId(), targetKg, barKg, ct)).ToHttp());
        tools.MapGet("/warmup", async (ClaimsPrincipal principal, ProgressService progress, decimal workingWeightKg, int? reps, CancellationToken ct) => (await progress.GetWarmupAsync(principal.UserId(), workingWeightKg, reps ?? 8, ct)).ToHttp());
        tools.MapGet("/overload/{exerciseId:guid}", async (Guid exerciseId, ClaimsPrincipal principal, ProgressService progress, CancellationToken ct) => (await progress.GetOverloadSuggestionAsync(principal.UserId(), exerciseId, ct)).ToHttp());
    }
}
