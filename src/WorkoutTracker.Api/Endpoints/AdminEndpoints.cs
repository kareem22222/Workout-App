using System.Security.Claims;
using WorkoutTracker.Application.Contracts;
using WorkoutTracker.Application.Services;
using WorkoutTracker.Infrastructure;

namespace WorkoutTracker.Api.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin").WithTags("Admin").RequireAuthorization(policy => policy.RequireRole(AppDbContext.AdminRole));
        group.MapGet("/users", async (AdminService admin, CancellationToken ct) => (await admin.ListUsersAsync(ct)).ToHttp());
        group.MapPut("/users/{id:guid}/disabled", async (Guid id, SetUserDisabledRequest request, ClaimsPrincipal principal, AdminService admin, CancellationToken ct) => (await admin.SetDisabledAsync(principal.UserId(), id, request.IsDisabled, ct)).ToHttp());
        group.MapGet("/info", async (IWebHostEnvironment environment, AppDbContext db, CancellationToken ct) => TypedResults.Ok(new
        {
            version = typeof(AdminEndpoints).Assembly.GetName().Version?.ToString() ?? "unknown",
            environment = environment.EnvironmentName,
            serverTimeUtc = DateTimeOffset.UtcNow,
            databaseHealthy = await db.Database.CanConnectAsync(ct)
        }));
    }
}
