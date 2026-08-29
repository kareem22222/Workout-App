using System.Security.Claims;
using WorkoutTracker.Application.Contracts;
using WorkoutTracker.Application.Services;

namespace WorkoutTracker.Api.Endpoints;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dashboard/summary", async (ClaimsPrincipal principal, DashboardService dashboard, CancellationToken ct) => (await dashboard.GetSummaryAsync(principal.UserId(), ct)).ToHttp()).WithTags("Dashboard").RequireAuthorization();

        var settings = app.MapGroup("/api/settings").WithTags("Settings").RequireAuthorization();
        settings.MapGet("/", async (ClaimsPrincipal principal, SettingsService service, CancellationToken ct) => (await service.GetSettingsAsync(principal.UserId(), ct)).ToHttp());
        settings.MapPut("/", async (UpdateSettingsRequest request, ClaimsPrincipal principal, SettingsService service, CancellationToken ct) => (await service.UpdateSettingsAsync(principal.UserId(), request, ct)).ToHttp());
    }
}
