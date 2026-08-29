using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Application.Contracts;
using WorkoutTracker.Application.Services;
using WorkoutTracker.Domain;

namespace WorkoutTracker.Api.Endpoints;

public static class MeasurementEndpoints
{
    public static void MapMeasurementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/measurements").WithTags("Measurements").RequireAuthorization();
        group.MapGet("/", async (ClaimsPrincipal principal, MeasurementService measurements, DateOnly? from, DateOnly? to, CancellationToken ct) => (await measurements.ListAsync(principal.UserId(), from, to, ct)).ToHttp());
        group.MapPost("/", async (SaveBodyMeasurementRequest request, ClaimsPrincipal principal, MeasurementService measurements, CancellationToken ct) => (await measurements.SaveAsync(principal.UserId(), request, ct)).ToCreated(dto => $"/api/measurements/{dto.Id}"));
        group.MapPut("/{id:guid}", async (Guid id, SaveBodyMeasurementRequest request, ClaimsPrincipal principal, MeasurementService measurements, CancellationToken ct) => (await measurements.UpdateAsync(principal.UserId(), id, request, ct)).ToHttp());
        group.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal principal, MeasurementService measurements, CancellationToken ct) => (await measurements.DeleteAsync(principal.UserId(), id, ct)).ToHttp());

        var photos = app.MapGroup("/api/photos").WithTags("Measurements").RequireAuthorization();
        photos.MapGet("/", async (ClaimsPrincipal principal, MeasurementService measurements, CancellationToken ct) => (await measurements.ListPhotosAsync(principal.UserId(), ct)).ToHttp());
        photos.MapPost("/", async (ClaimsPrincipal principal, MeasurementService measurements, IFormFile file, [FromForm] DateOnly takenOn, [FromForm] PhotoPose pose, [FromForm] decimal? weightKg, [FromForm] string? notes, CancellationToken ct) =>
        {
            await using var stream = file.OpenReadStream();
            return (await measurements.AddPhotoAsync(principal.UserId(), takenOn, pose, weightKg, notes, file.FileName, file.ContentType, file.Length, stream, ct)).ToCreated(dto => $"/api/photos/{dto.Id}");
        }).DisableAntiforgery();
        photos.MapGet("/{id:guid}/content", async (Guid id, ClaimsPrincipal principal, MeasurementService measurements, CancellationToken ct) =>
        {
            var result = await measurements.OpenPhotoAsync(principal.UserId(), id, ct);
            return !result.Succeeded ? result.ToHttp() : Results.Stream(result.Value.Content, result.Value.ContentType);
        });
        photos.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal principal, MeasurementService measurements, CancellationToken ct) => (await measurements.DeletePhotoAsync(principal.UserId(), id, ct)).ToHttp());
    }
}
