using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Application.Contracts;
using WorkoutTracker.Application.Services;
using WorkoutTracker.Domain;
using WorkoutTracker.Infrastructure;

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

/// <summary>Analytics and training-aid endpoints (spec 5 /api/progress).</summary>
public static class ProgressEndpoints
{
    public static void MapProgressEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/progress").WithTags("Progress").RequireAuthorization();

        group.MapGet("/exercise/{exerciseId:guid}", async (
            Guid exerciseId,
            ClaimsPrincipal principal,
            ProgressService progress,
            string? range,
            CancellationToken ct) =>
            (await progress.GetExerciseProgressAsync(principal.UserId(), exerciseId, range ?? "3m", ct)).ToHttp());

        group.MapGet("/personal-records", async (
            ClaimsPrincipal principal,
            PersonalRecordService records,
            Guid? exerciseId,
            CancellationToken ct) =>
            (await records.ListAsync(principal.UserId(), exerciseId, ct)).ToHttp());

        group.MapGet("/volume", async (
            ClaimsPrincipal principal,
            ProgressService progress,
            string? range,
            string? groupBy,
            CancellationToken ct) =>
            (await progress.GetVolumeAsync(principal.UserId(), range ?? "3m", groupBy ?? "week", ct)).ToHttp());

        group.MapGet("/estimated-one-rep-max", async (
            ClaimsPrincipal principal,
            ProgressService progress,
            CancellationToken ct) =>
            (await progress.GetOneRepMaxSummaryAsync(principal.UserId(), ct)).ToHttp());

        group.MapGet("/stats", async (
            ClaimsPrincipal principal,
            ProgressService progress,
            string? range,
            string? groupBy,
            CancellationToken ct) =>
            (await progress.GetStatsAsync(principal.UserId(), range ?? "3m", groupBy ?? "week", ct)).ToHttp());

        group.MapGet("/muscles", async (
            ClaimsPrincipal principal,
            ProgressService progress,
            string? range,
            CancellationToken ct) =>
            (await progress.GetMuscleBreakdownAsync(principal.UserId(), range ?? "1m", ct)).ToHttp());

        // ----- Training aids -----
        var tools = app.MapGroup("/api/tools").WithTags("Tools").RequireAuthorization();

        tools.MapGet("/plates", async (
            ClaimsPrincipal principal,
            ProgressService progress,
            decimal targetKg,
            decimal? barKg,
            CancellationToken ct) =>
            (await progress.GetPlatesAsync(principal.UserId(), targetKg, barKg, ct)).ToHttp());

        tools.MapGet("/warmup", async (
            ClaimsPrincipal principal,
            ProgressService progress,
            decimal workingWeightKg,
            int? reps,
            CancellationToken ct) =>
            (await progress.GetWarmupAsync(principal.UserId(), workingWeightKg, reps ?? 8, ct)).ToHttp());

        tools.MapGet("/overload/{exerciseId:guid}", async (
            Guid exerciseId,
            ClaimsPrincipal principal,
            ProgressService progress,
            CancellationToken ct) =>
            (await progress.GetOverloadSuggestionAsync(principal.UserId(), exerciseId, ct)).ToHttp());
    }
}

/// <summary>Body measurement and progress photo endpoints (spec 5 /api/measurements).</summary>
public static class MeasurementEndpoints
{
    public static void MapMeasurementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/measurements").WithTags("Measurements").RequireAuthorization();

        group.MapGet("/", async (
            ClaimsPrincipal principal,
            MeasurementService measurements,
            DateOnly? from,
            DateOnly? to,
            CancellationToken ct) =>
            (await measurements.ListAsync(principal.UserId(), from, to, ct)).ToHttp());

        group.MapPost("/", async (
            SaveBodyMeasurementRequest request,
            ClaimsPrincipal principal,
            MeasurementService measurements,
            CancellationToken ct) =>
            (await measurements.SaveAsync(principal.UserId(), request, ct))
                .ToCreated(dto => $"/api/measurements/{dto.Id}"));

        group.MapPut("/{id:guid}", async (
            Guid id,
            SaveBodyMeasurementRequest request,
            ClaimsPrincipal principal,
            MeasurementService measurements,
            CancellationToken ct) =>
            (await measurements.UpdateAsync(principal.UserId(), id, request, ct)).ToHttp());

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal principal,
            MeasurementService measurements,
            CancellationToken ct) =>
            (await measurements.DeleteAsync(principal.UserId(), id, ct)).ToHttp());

        // ----- Private progress photos -----
        var photos = app.MapGroup("/api/photos").WithTags("Measurements").RequireAuthorization();

        photos.MapGet("/", async (ClaimsPrincipal principal, MeasurementService measurements, CancellationToken ct) =>
            (await measurements.ListPhotosAsync(principal.UserId(), ct)).ToHttp());

        photos.MapPost("/", async (
            ClaimsPrincipal principal,
            MeasurementService measurements,
            IFormFile file,
            [FromForm] DateOnly takenOn,
            [FromForm] PhotoPose pose,
            [FromForm] decimal? weightKg,
            [FromForm] string? notes,
            CancellationToken ct) =>
        {
            await using var stream = file.OpenReadStream();

            var result = await measurements.AddPhotoAsync(
                principal.UserId(), takenOn, pose, weightKg, notes,
                file.FileName, file.ContentType, file.Length, stream, ct);

            return result.ToCreated(dto => $"/api/photos/{dto.Id}");
        }).DisableAntiforgery();

        // Bytes are streamed through this authorized route; storage keys stay private.
        photos.MapGet("/{id:guid}/content", async (
            Guid id,
            ClaimsPrincipal principal,
            MeasurementService measurements,
            CancellationToken ct) =>
        {
            var result = await measurements.OpenPhotoAsync(principal.UserId(), id, ct);
            if (!result.Succeeded) return result.ToHttp();

            var (content, contentType) = result.Value;
            return Results.Stream(content, contentType);
        });

        photos.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal principal,
            MeasurementService measurements,
            CancellationToken ct) =>
            (await measurements.DeletePhotoAsync(principal.UserId(), id, ct)).ToHttp());
    }
}

/// <summary>Dashboard and settings endpoints (spec 5 /api/dashboard).</summary>
public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dashboard/summary", async (
                ClaimsPrincipal principal,
                DashboardService dashboard,
                CancellationToken ct) =>
                (await dashboard.GetSummaryAsync(principal.UserId(), ct)).ToHttp())
            .WithTags("Dashboard")
            .RequireAuthorization();

        var settings = app.MapGroup("/api/settings").WithTags("Settings").RequireAuthorization();

        settings.MapGet("/", async (ClaimsPrincipal principal, SettingsService service, CancellationToken ct) =>
            (await service.GetSettingsAsync(principal.UserId(), ct)).ToHttp());

        settings.MapPut("/", async (
            UpdateSettingsRequest request,
            ClaimsPrincipal principal,
            SettingsService service,
            CancellationToken ct) =>
            (await service.UpdateSettingsAsync(principal.UserId(), request, ct)).ToHttp());
    }
}

/// <summary>Export and import endpoints (spec 5 /api/export, Epic 26).</summary>
public static class DataEndpoints
{
    public static void MapDataEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/export").WithTags("Data").RequireAuthorization();

        group.MapGet("/json", async (
            ClaimsPrincipal principal,
            DataTransferService data,
            CancellationToken ct) =>
        {
            var result = await data.ExportJsonAsync(principal.UserId(), ct);
            if (!result.Succeeded) return result.ToHttp();

            var json = JsonSerializer.SerializeToUtf8Bytes(result.Value, new JsonSerializerOptions { WriteIndented = true });
            var fileName = $"workouttracker-export-{DateTimeOffset.UtcNow:yyyyMMdd}.json";

            return Results.File(json, "application/json", fileName);
        });

        group.MapGet("/csv", async (
            ClaimsPrincipal principal,
            DataTransferService data,
            string? dataset,
            CancellationToken ct) =>
        {
            var result = await data.ExportCsvAsync(principal.UserId(), dataset ?? "sets", ct);
            if (!result.Succeeded) return result.ToHttp();

            var (fileName, csv) = result.Value;
            // A BOM keeps non-ASCII exercise names readable when opened in Excel.
            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv)).ToArray();

            return Results.File(bytes, "text/csv", fileName);
        });

        var import = app.MapGroup("/api/import").WithTags("Data").RequireAuthorization();

        import.MapPost("/preview", async (
            ClaimsPrincipal principal,
            DataTransferService data,
            IFormFile file,
            CancellationToken ct) =>
        {
            var csv = await ReadTextAsync(file, ct);
            return (await data.PreviewImportAsync(principal.UserId(), csv, ct)).ToHttp();
        }).DisableAntiforgery();

        import.MapPost("/commit", async (
            ClaimsPrincipal principal,
            DataTransferService data,
            IFormFile file,
            CancellationToken ct) =>
        {
            var csv = await ReadTextAsync(file, ct);
            return (await data.CommitImportAsync(principal.UserId(), csv, ct)).ToHttp();
        }).DisableAntiforgery();
    }

    private static async Task<string> ReadTextAsync(IFormFile file, CancellationToken ct)
    {
        using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        return await reader.ReadToEndAsync(ct);
    }
}

/// <summary>Operational endpoints for the admin role (spec Epic 34).</summary>
public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin")
            .WithTags("Admin")
            .RequireAuthorization(policy => policy.RequireRole(AppDbContext.AdminRole));

        group.MapGet("/users", async (AdminService admin, CancellationToken ct) =>
            (await admin.ListUsersAsync(ct)).ToHttp());

        group.MapPut("/users/{id:guid}/disabled", async (
            Guid id,
            SetUserDisabledRequest request,
            ClaimsPrincipal principal,
            AdminService admin,
            CancellationToken ct) =>
            (await admin.SetDisabledAsync(principal.UserId(), id, request.IsDisabled, ct)).ToHttp());

        // Version and environment only. No secrets or connection details (spec 3).
        group.MapGet("/info", (IWebHostEnvironment environment) => TypedResults.Ok(new
        {
            version = typeof(AdminEndpoints).Assembly.GetName().Version?.ToString() ?? "unknown",
            environment = environment.EnvironmentName,
            serverTimeUtc = DateTimeOffset.UtcNow
        }));
    }
}
