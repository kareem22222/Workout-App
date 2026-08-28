using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Domain;

namespace WorkoutTracker.Infrastructure;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public required string DisplayName { get; set; }
    public string PreferredUnits { get; set; } = "kg";
    public string TimeZone { get; set; } = "UTC";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string TokenHash { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
}

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<Routine> Routines => Set<Routine>();
    public DbSet<RoutineExercise> RoutineExercises => Set<RoutineExercise>();
    public DbSet<RoutineSetTemplate> RoutineSetTemplates => Set<RoutineSetTemplate>();
    public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();
    public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();
    public DbSet<WorkoutSet> WorkoutSets => Set<WorkoutSet>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ApplicationUser>().HasIndex(x => x.NormalizedEmail).IsUnique();
        builder.Entity<Exercise>().HasIndex(x => new { x.OwnerId, x.Name });
        builder.Entity<Routine>().HasIndex(x => new { x.OwnerId, x.Name });
        builder.Entity<WorkoutSession>().HasIndex(x => new { x.OwnerId, x.StartedAt });
        builder.Entity<RefreshToken>().HasIndex(x => x.TokenHash).IsUnique();
        builder.Entity<WorkoutSet>().Property(x => x.Weight).HasPrecision(8, 2);
        builder.Entity<WorkoutSet>().Property(x => x.Rpe).HasPrecision(3, 1);
        builder.Entity<RoutineSetTemplate>().Property(x => x.TargetWeight).HasPrecision(8, 2);

        builder.Entity<Exercise>().HasData(SeedExercises.All);
    }
}

internal static class SeedExercises
{
    public static readonly Exercise[] All =
    [
        New("0a271274-162d-49b0-a776-a93ad60c1371", "Bench Press", "Chest", "Barbell"),
        New("46812be0-5b29-4528-87fc-c58df5038344", "Squat", "Quadriceps", "Barbell"),
        New("ca1d7b51-d920-41e9-ae5e-a8ec94608512", "Deadlift", "Back", "Barbell"),
        New("4eb5b149-099c-49cf-9d7d-5c293580afdc", "Overhead Press", "Shoulders", "Barbell"),
        New("7f85ab41-0653-45bc-935e-a1c7b663ee8f", "Pull Up", "Back", "Bodyweight"),
        New("d27f33a5-78f1-4ce4-a243-09df1bb78c35", "Dumbbell Row", "Back", "Dumbbell"),
        New("c2c36f7d-e769-4eab-aee0-52df83985a67", "Romanian Deadlift", "Hamstrings", "Barbell"),
        New("751bb2fc-928f-4362-bd42-dd7208d042c8", "Lat Pulldown", "Back", "Cable")
    ];

    private static Exercise New(string id, string name, string muscle, string equipment) => new()
    {
        Id = Guid.Parse(id), Name = name, Muscle = muscle, Equipment = equipment
    };
}
