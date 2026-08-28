using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using WorkoutTracker.Domain;
using WorkoutTracker.Infrastructure;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>().AddSignInManager();

var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is required");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"], ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), ClockSkew = TimeSpan.FromSeconds(30)
    };
});
builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(options => options.AddPolicy("auth", context =>
    RateLimitPartition.GetFixedWindowLimiter(context.Connection.RemoteIpAddress?.ToString() ?? "unknown", _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 10, Window = TimeSpan.FromMinutes(1), QueueLimit = 0
    })));
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
    .WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseExceptionHandler(error => error.Run(async context =>
{
    context.Response.StatusCode = 500;
    await context.Response.WriteAsJsonAsync(new { error = "Something went wrong." });
}));
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.UseCors();
if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");

var auth = app.MapGroup("/api/auth").RequireRateLimiting("auth");
auth.MapPost("/register", async (RegisterRequest request, HttpResponse response, UserManager<ApplicationUser> users, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.DisplayName) || string.IsNullOrWhiteSpace(request.Email))
        return Results.BadRequest(new { error = "Display name and email are required." });
    var user = new ApplicationUser { Id = Guid.NewGuid(), DisplayName = request.DisplayName.Trim(), Email = request.Email.Trim(), UserName = request.Email.Trim() };
    var result = await users.CreateAsync(user, request.Password);
    return result.Succeeded ? await IssueTokens(user, db, response) : Results.ValidationProblem(result.Errors.ToDictionary(x => x.Code, x => new[] { x.Description }));
});
auth.MapPost("/login", async (LoginRequest request, HttpResponse response, UserManager<ApplicationUser> users, AppDbContext db) =>
{
    var user = await users.FindByEmailAsync(request.Email);
    return user is not null && await users.CheckPasswordAsync(user, request.Password)
        ? await IssueTokens(user, db, response)
        : Results.Unauthorized();
});
auth.MapPost("/refresh", async (HttpRequest request, HttpResponse response, AppDbContext db) =>
{
    if (!request.Cookies.TryGetValue("refreshToken", out var raw)) return Results.Unauthorized();
    var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));
    var token = await db.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == hash && x.RevokedAt == null && x.ExpiresAt > DateTimeOffset.UtcNow);
    if (token is null) return Results.Unauthorized();
    token.RevokedAt = DateTimeOffset.UtcNow;
    var user = await db.Users.FindAsync(token.UserId);
    return user is null ? Results.Unauthorized() : await IssueTokens(user, db, response);
});
auth.MapPost("/logout", async (HttpRequest request, HttpResponse response, AppDbContext db) =>
{
    if (request.Cookies.TryGetValue("refreshToken", out var raw))
    {
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));
        var token = await db.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == hash && x.RevokedAt == null);
        if (token is not null) token.RevokedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
    }
    response.Cookies.Delete("refreshToken");
    return Results.NoContent();
});
auth.MapGet("/me", async (ClaimsPrincipal principal, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(UserId(principal));
    return user is null ? Results.NotFound() : Results.Ok(new { user.Id, user.DisplayName, user.Email, user.PreferredUnits, user.TimeZone });
}).RequireAuthorization();

app.MapGet("/api/exercises", async (ClaimsPrincipal principal, string? search, AppDbContext db) =>
{
    var userId = UserId(principal);
    var query = db.Exercises.Where(x => x.OwnerId == null || x.OwnerId == userId);
    if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => EF.Functions.ILike(x.Name, $"%{search}%"));
    return await query.OrderBy(x => x.Name).Select(x => new { x.Id, x.Name, x.Muscle, x.Equipment, x.Instructions, IsCustom = x.OwnerId != null }).ToListAsync();
}).RequireAuthorization();

var routines = app.MapGroup("/api/routines").RequireAuthorization();
routines.MapGet("/", async (ClaimsPrincipal principal, AppDbContext db) =>
{
    var userId = UserId(principal);
    var items = await db.Routines.Where(x => x.OwnerId == userId).Include(x => x.Exercises.OrderBy(e => e.Order)).ThenInclude(x => x.Exercise).Include(x => x.Exercises).ThenInclude(x => x.Sets.OrderBy(s => s.Order)).ToListAsync();
    return items.Select(RoutineDto);
});
routines.MapPost("/", async (ClaimsPrincipal principal, RoutineRequest request, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.Name) || request.Exercises.Count == 0 || request.Exercises.Any(x => x.SetCount is < 1 or > 20 || x.TargetReps is < 0 or > 100 || x.RestSeconds is < 0 or > 1800 || x.TargetWeight < 0))
        return Results.BadRequest(new { error = "Routine name, exercises and set targets are invalid." });
    var userId = UserId(principal);
    var exerciseIds = request.Exercises.Select(x => x.ExerciseId).ToHashSet();
    var allowed = await db.Exercises.Where(x => exerciseIds.Contains(x.Id) && (x.OwnerId == null || x.OwnerId == userId)).Select(x => x.Id).ToListAsync();
    if (allowed.Count != exerciseIds.Count) return Results.BadRequest(new { error = "One or more exercises are unavailable." });
    var routine = new Routine { Id = Guid.NewGuid(), OwnerId = userId, Name = request.Name.Trim(), Description = request.Description ?? "" };
    routine.Exercises = request.Exercises.Select((x, i) => new RoutineExercise { Id = Guid.NewGuid(), ExerciseId = x.ExerciseId, Order = i, RestSeconds = x.RestSeconds, Sets = Enumerable.Range(0, x.SetCount).Select(s => new RoutineSetTemplate { Id = Guid.NewGuid(), Order = s, TargetReps = x.TargetReps, TargetWeight = x.TargetWeight }).ToList() }).ToList();
    db.Routines.Add(routine);
    await db.SaveChangesAsync();
    return Results.Created($"/api/routines/{routine.Id}", RoutineDto(routine));
});

var workouts = app.MapGroup("/api/workouts").RequireAuthorization();
workouts.MapGet("/", async (ClaimsPrincipal principal, AppDbContext db) =>
{
    var userId = UserId(principal);
    var items = await db.WorkoutSessions.Where(x => x.OwnerId == userId).OrderByDescending(x => x.StartedAt).Include(x => x.Exercises.OrderBy(e => e.Order)).ThenInclude(x => x.Sets.OrderBy(s => s.Order)).ToListAsync();
    return items.Select(WorkoutDto);
});
workouts.MapPost("/start", async (ClaimsPrincipal principal, StartWorkoutRequest request, AppDbContext db) =>
{
    var userId = UserId(principal);
    var active = await db.WorkoutSessions.Include(x => x.Exercises).ThenInclude(x => x.Sets).SingleOrDefaultAsync(x => x.OwnerId == userId && x.Status == WorkoutStatus.Active);
    if (active is not null) return Results.Conflict(new { error = "An active workout already exists.", workout = WorkoutDto(active) });
    var session = new WorkoutSession { Id = Guid.NewGuid(), OwnerId = userId, RoutineId = request.RoutineId, Title = request.Title?.Trim() ?? "Quick workout" };
    if (request.RoutineId is Guid routineId)
    {
        var routine = await db.Routines.Where(x => x.Id == routineId && x.OwnerId == userId).Include(x => x.Exercises.OrderBy(e => e.Order)).ThenInclude(x => x.Exercise).Include(x => x.Exercises).ThenInclude(x => x.Sets.OrderBy(s => s.Order)).SingleOrDefaultAsync();
        if (routine is null) return Results.NotFound();
        session.Title = routine.Name;
        session.Exercises = routine.Exercises.Select(re => new WorkoutExercise { Id = Guid.NewGuid(), ExerciseId = re.ExerciseId, ExerciseName = re.Exercise!.Name, Order = re.Order, RestSeconds = re.RestSeconds, Notes = re.Notes, Sets = re.Sets.Select(s => new WorkoutSet { Id = Guid.NewGuid(), Order = s.Order, Weight = s.TargetWeight ?? 0, Reps = s.TargetReps, Type = s.Type }).ToList() }).ToList();
    }
    db.WorkoutSessions.Add(session);
    await db.SaveChangesAsync();
    return Results.Created($"/api/workouts/{session.Id}", WorkoutDto(session));
});
workouts.MapPut("/{id:guid}", async (Guid id, ClaimsPrincipal principal, UpdateWorkoutRequest request, AppDbContext db) =>
{
    if (request.Sets.Any(x => x.Weight < 0 || x.Reps is < 0 or > 1000 || x.Rpe is < 1 or > 10))
        return Results.BadRequest(new { error = "Set values are invalid." });
    var userId = UserId(principal);
    var workout = await db.WorkoutSessions.Where(x => x.Id == id && x.OwnerId == userId && x.Status == WorkoutStatus.Active).Include(x => x.Exercises).ThenInclude(x => x.Sets).SingleOrDefaultAsync();
    if (workout is null) return Results.NotFound();
    foreach (var input in request.Sets)
    {
        var set = workout.Exercises.SelectMany(x => x.Sets).SingleOrDefault(x => x.Id == input.Id);
        if (set is null) return Results.BadRequest(new { error = "Unknown set." });
        set.Weight = input.Weight; set.Reps = input.Reps; set.Rpe = input.Rpe; set.CompletedAt = input.Completed ? DateTimeOffset.UtcNow : null;
    }
    await db.SaveChangesAsync();
    return Results.Ok(WorkoutDto(workout));
});
workouts.MapPost("/{id:guid}/finish", async (Guid id, ClaimsPrincipal principal, AppDbContext db) =>
{
    var userId = UserId(principal);
    var workout = await db.WorkoutSessions.Where(x => x.Id == id && x.OwnerId == userId && x.Status == WorkoutStatus.Active).Include(x => x.Exercises).ThenInclude(x => x.Sets).SingleOrDefaultAsync();
    if (workout is null) return Results.NotFound();
    workout.Status = WorkoutStatus.Completed; workout.CompletedAt = DateTimeOffset.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(new { workout.Id, workout.Title, workout.StartedAt, workout.CompletedAt, Sets = workout.Exercises.SelectMany(x => x.Sets).Count(x => x.CompletedAt != null), Volume = workout.Exercises.SelectMany(x => x.Sets).Where(x => x.CompletedAt != null).Sum(x => x.Weight * x.Reps) });
});

app.Run();

Guid UserId(ClaimsPrincipal principal) => Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

object RoutineDto(Routine routine) => new
{
    routine.Id, routine.Name, routine.Description, routine.CreatedAt,
    Exercises = routine.Exercises.OrderBy(x => x.Order).Select(x => new
    {
        x.Id, x.ExerciseId, ExerciseName = x.Exercise?.Name, x.Order, x.RestSeconds, x.Notes,
        Sets = x.Sets.OrderBy(s => s.Order).Select(s => new { s.Id, s.Order, s.TargetReps, s.TargetWeight, Type = s.Type.ToString() })
    })
};

object WorkoutDto(WorkoutSession workout) => new
{
    workout.Id, workout.RoutineId, workout.Title, Status = workout.Status.ToString(), workout.StartedAt, workout.CompletedAt, workout.Notes,
    Exercises = workout.Exercises.OrderBy(x => x.Order).Select(x => new
    {
        x.Id, x.ExerciseId, x.ExerciseName, x.Order, x.RestSeconds, x.Notes,
        Sets = x.Sets.OrderBy(s => s.Order).Select(s => new { s.Id, s.Order, s.Weight, s.Reps, s.Rpe, Type = s.Type.ToString(), Completed = s.CompletedAt != null })
    })
};

async Task<IResult> IssueTokens(ApplicationUser user, AppDbContext db, HttpResponse response)
{
    var now = DateTimeOffset.UtcNow;
    var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Name, user.DisplayName), new Claim(ClaimTypes.Email, user.Email ?? "") };
    var token = new JwtSecurityToken(builder.Configuration["Jwt:Issuer"], builder.Configuration["Jwt:Audience"], claims, now.UtcDateTime, now.AddMinutes(15).UtcDateTime, new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), SecurityAlgorithms.HmacSha256));
    var rawRefresh = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
    db.RefreshTokens.Add(new RefreshToken { Id = Guid.NewGuid(), UserId = user.Id, TokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawRefresh))), ExpiresAt = now.AddDays(30) });
    await db.SaveChangesAsync();
    response.Cookies.Append("refreshToken", rawRefresh, new CookieOptions { HttpOnly = true, Secure = !app.Environment.IsDevelopment(), SameSite = SameSiteMode.Strict, Expires = now.AddDays(30), Path = "/api/auth" });
    return Results.Ok(new { accessToken = new JwtSecurityTokenHandler().WriteToken(token), expiresAt = now.AddMinutes(15), user = new { user.Id, user.DisplayName, user.Email, user.PreferredUnits, user.TimeZone } });
}

record RegisterRequest(string DisplayName, string Email, string Password);
record LoginRequest(string Email, string Password);
record RoutineExerciseRequest(Guid ExerciseId, int SetCount = 3, int TargetReps = 8, decimal? TargetWeight = null, int RestSeconds = 90);
record RoutineRequest(string Name, string? Description, List<RoutineExerciseRequest> Exercises);
record StartWorkoutRequest(Guid? RoutineId, string? Title);
record WorkoutSetRequest(Guid Id, decimal Weight, int Reps, decimal? Rpe, bool Completed);
record UpdateWorkoutRequest(List<WorkoutSetRequest> Sets);
