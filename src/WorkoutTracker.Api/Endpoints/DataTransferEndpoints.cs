using System.Security.Claims;
using System.Text;
using System.Text.Json;
using WorkoutTracker.Application.Services;

namespace WorkoutTracker.Api.Endpoints;

public static class DataTransferEndpoints
{
    public static void MapDataTransferEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/export").WithTags("Data").RequireAuthorization();
        group.MapGet("/json", async (ClaimsPrincipal principal, DataTransferService data, CancellationToken ct) =>
        {
            var result = await data.ExportJsonAsync(principal.UserId(), ct);
            if (!result.Succeeded) return result.ToHttp();
            var bytes = JsonSerializer.SerializeToUtf8Bytes(result.Value, new JsonSerializerOptions { WriteIndented = true });
            return Results.File(bytes, "application/json", $"workouttracker-export-{DateTimeOffset.UtcNow:yyyyMMdd}.json");
        });
        group.MapGet("/csv", async (ClaimsPrincipal principal, DataTransferService data, string? dataset, CancellationToken ct) =>
        {
            var result = await data.ExportCsvAsync(principal.UserId(), dataset ?? "sets", ct);
            if (!result.Succeeded) return result.ToHttp();
            var (fileName, csv) = result.Value;
            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv)).ToArray();
            return Results.File(bytes, "text/csv", fileName);
        });

        var import = app.MapGroup("/api/import").WithTags("Data").RequireAuthorization();
        import.MapPost("/preview", async (ClaimsPrincipal principal, DataTransferService data, IFormFile file, CancellationToken ct) => (await data.PreviewImportAsync(principal.UserId(), await ReadTextAsync(file, ct), ct)).ToHttp()).DisableAntiforgery();
        import.MapPost("/commit", async (ClaimsPrincipal principal, DataTransferService data, IFormFile file, CancellationToken ct) => (await data.CommitImportAsync(principal.UserId(), await ReadTextAsync(file, ct), ct)).ToHttp()).DisableAntiforgery();
    }

    private static async Task<string> ReadTextAsync(IFormFile file, CancellationToken ct)
    {
        using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        return await reader.ReadToEndAsync(ct);
    }
}
