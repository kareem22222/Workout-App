using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WorkoutTracker.Application;
using WorkoutTracker.Application.Services;

namespace WorkoutTracker.Infrastructure;

/// <summary>
/// Registers the persistence layer and the Application use-case services, so the API host
/// has a single composition entry point.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddWorkoutTracker(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("ConnectionStrings:Database is required.");

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(
            connectionString,
            postgres => postgres
                .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                // Hosted Postgres drops idle connections and scale-to-zero tiers pause
                // entirely, so transient failures are normal rather than exceptional.
                // Safe here because no code path opens an explicit transaction, which is
                // the one thing an execution strategy cannot retry.
                .EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorCodesToAdd: null)));

        // Application services resolve the context through its abstraction.
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        services.AddScoped<IUserDirectory, IdentityUserDirectory>();
        services.AddSingleton<IClock, SystemClock>();

        var mediaRoot = configuration["Storage:MediaRoot"];
        if (string.IsNullOrWhiteSpace(mediaRoot)) mediaRoot = Path.Combine(AppContext.BaseDirectory, "media");

        services.AddSingleton<IMediaStorage>(provider =>
            new LocalMediaStorage(mediaRoot, provider.GetRequiredService<ILogger<LocalMediaStorage>>()));

        // Use-case services. Scoped so they share the request's DbContext and transaction.
        services.AddScoped<SettingsService>();
        services.AddScoped<ExerciseService>();
        services.AddScoped<RoutineService>();
        services.AddScoped<PersonalRecordService>();
        services.AddScoped<WorkoutService>();
        services.AddScoped<ProgressService>();
        services.AddScoped<MeasurementService>();
        services.AddScoped<DashboardService>();
        services.AddScoped<DataTransferService>();
        services.AddScoped<AdminService>();

        return services;
    }
}
