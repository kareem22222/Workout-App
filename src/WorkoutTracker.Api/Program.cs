using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using WorkoutTracker.Api;
using WorkoutTracker.Api.Endpoints;
using WorkoutTracker.Infrastructure;

// Bootstrap logger so failures during startup are still captured.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console());

    // ---------------------------------------------------------------------------------
    // Configuration
    // ---------------------------------------------------------------------------------

    var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();

    if (string.IsNullOrWhiteSpace(jwtOptions.Key) || jwtOptions.Key.Length < 32)
    {
        throw new InvalidOperationException(
            "Jwt:Key must be configured with at least 32 characters. Set it via environment variables or a secret store.");
    }

    builder.Services.AddSingleton(jwtOptions);

    // Persistence, use-case services and provider abstractions.
    builder.Services.AddWorkoutTracker(builder.Configuration);
    builder.Services.AddScoped<TokenService>();

    // ---------------------------------------------------------------------------------
    // Identity and authentication
    // ---------------------------------------------------------------------------------

    builder.Services
        .AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
            options.Lockout.MaxFailedAccessAttempts = 10;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        })
        .AddRoles<Microsoft.AspNetCore.Identity.IdentityRole<Guid>>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddSignInManager();

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                ClockSkew = TimeSpan.FromSeconds(30)
            };

            options.Events = new JwtBearerEvents
            {
                // Re-check the account on every request so disabling a user or changing a
                // password takes effect before the short-lived token would expire.
                OnTokenValidated = async context =>
                {
                    var principal = context.Principal;
                    var subject = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(subject, out var userId))
                    {
                        context.Fail("Token is missing a valid subject.");
                        return;
                    }

                    var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

                    var user = await db.Users
                        .AsNoTracking()
                        .Where(x => x.Id == userId)
                        .Select(x => new { x.IsDisabled, x.SecurityStamp })
                        .FirstOrDefaultAsync();

                    if (user is null || user.IsDisabled)
                    {
                        context.Fail("Account is unavailable.");
                        return;
                    }

                    var stamp = principal?.FindFirstValue(JwtRegisteredClaimNames.SecurityStamp);
                    if (!string.IsNullOrEmpty(user.SecurityStamp) && stamp != user.SecurityStamp)
                    {
                        context.Fail("Session is no longer valid.");
                    }
                }
            };
        });

    builder.Services.AddAuthorization();

    // ---------------------------------------------------------------------------------
    // Cross-cutting concerns
    // ---------------------------------------------------------------------------------

    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // Credential endpoints are the brute-force surface, so they get a tight window.
        options.AddPolicy("auth", context => RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
    });

    builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    {
        // Strict allow-list. Credentials are required for the refresh cookie, which
        // forbids a wildcard origin.
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? ["http://localhost:5173"];

        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    }));

    builder.Services.AddOpenApi();
    builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>("database");
    builder.Services.AddProblemDetails();

    // Enums travel as strings so the contract stays readable and the client can use
    // string unions instead of brittle numeric values.
    builder.Services.ConfigureHttpJsonOptions(options =>
        options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

    var app = builder.Build();

    // ---------------------------------------------------------------------------------
    // Pipeline
    // ---------------------------------------------------------------------------------

    app.UseSerilogRequestLogging(options =>
    {
        options.GetLevel = (httpContext, _, exception) =>
            exception is not null || httpContext.Response.StatusCode >= 500
                ? LogEventLevel.Error
                : LogEventLevel.Information;
    });

    // Correlation id so a user-reported error can be traced without exposing internals.
    app.Use(async (context, next) =>
    {
        const string header = "X-Correlation-Id";
        var correlationId = context.Request.Headers[header].FirstOrDefault() ?? context.TraceIdentifier;

        context.Response.Headers[header] = correlationId;

        using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next();
        }
    });

    app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        Log.Error(feature?.Error, "Unhandled exception for {Path}.", context.Request.Path);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        // Never leak exception details or stack traces to clients (spec 3).
        await context.Response.WriteAsJsonAsync(new
        {
            type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            title = "Server error",
            status = 500,
            detail = "Something went wrong. Please try again.",
            correlationId = context.Response.Headers["X-Correlation-Id"].FirstOrDefault()
        });
    }));

    app.Use(async (context, next) =>
    {
        var headers = context.Response.Headers;
        headers["X-Content-Type-Options"] = "nosniff";
        headers["X-Frame-Options"] = "DENY";
        headers["Referrer-Policy"] = "no-referrer";
        await next();
    });

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }
    else
    {
        app.UseHttpsRedirection();
        app.UseHsts();
    }

    app.UseCors();
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();

    // Liveness only; no personal data or configuration is exposed.
    app.MapHealthChecks("/health");

    app.MapAuthEndpoints();
    app.MapExerciseEndpoints();
    app.MapRoutineEndpoints();
    app.MapWorkoutEndpoints();
    app.MapProgressEndpoints();
    app.MapMeasurementEndpoints();
    app.MapDashboardEndpoints();
    app.MapDataEndpoints();
    app.MapAdminEndpoints();

    // Ensure the admin role exists so the first registration can be promoted. A database
    // that is briefly unavailable at boot must not stop the API from starting; the role is
    // re-checked on the next start.
    try
    {
        using var scope = app.Services.CreateScope();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<Microsoft.AspNetCore.Identity.IdentityRole<Guid>>>();

        if (!await roles.RoleExistsAsync(AppDbContext.AdminRole))
        {
            await roles.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole<Guid>(AppDbContext.AdminRole)
            {
                Id = Guid.NewGuid()
            });
        }
    }
    catch (Exception exception)
    {
        Log.Warning(exception, "Could not verify the {Role} role at startup. It will be retried on the next start.", AppDbContext.AdminRole);
    }

    app.Run();
}
catch (Exception exception) when (exception is not HostAbortedException)
{
    Log.Fatal(exception, "The API terminated unexpectedly.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
