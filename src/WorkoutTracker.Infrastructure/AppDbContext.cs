using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Application;
using WorkoutTracker.Domain;

namespace WorkoutTracker.Infrastructure;

/// <summary>Identity user extended with the display name and account status.</summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    public required string DisplayName { get; set; }

    /// <summary>
    /// Disabled accounts are refused at login and on token refresh, which is how an
    /// operator revokes access without deleting training history (spec US-290).
    /// </summary>
    public bool IsDisabled { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// A hashed, rotating refresh token. Tokens are chained through
/// <see cref="ReplacedByTokenId"/> so reuse of an already-rotated token can be detected
/// and the whole family revoked (spec 3).
/// </summary>
public sealed class RefreshToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    /// <summary>SHA-256 of the token. The raw value is never stored.</summary>
    public required string TokenHash { get; set; }

    /// <summary>
    /// Groups all tokens descended from one login so a detected replay can revoke the
    /// entire device session.
    /// </summary>
    public Guid FamilyId { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset? RevokedAt { get; set; }

    /// <summary>Set when this token has been rotated, pointing at its successor.</summary>
    public Guid? ReplacedByTokenId { get; set; }

    /// <summary>Coarse device label for the session list. Never contains credentials.</summary>
    public string? UserAgent { get; set; }

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTimeOffset.UtcNow;
}

/// <summary>
/// EF Core context combining ASP.NET Identity with the workout domain. Implements
/// <see cref="IAppDbContext"/> so Application services can query without referencing
/// Infrastructure (spec 2).
/// </summary>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options), IAppDbContext
{
    /// <summary>Role name granting access to the admin surface.</summary>
    public const string AdminRole = "Admin";

    public DbSet<Muscle> Muscles => Set<Muscle>();
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<ExerciseMuscle> ExerciseMuscles => Set<ExerciseMuscle>();
    public DbSet<ExerciseNote> ExerciseNotes => Set<ExerciseNote>();
    public DbSet<RoutineFolder> RoutineFolders => Set<RoutineFolder>();
    public DbSet<Routine> Routines => Set<Routine>();
    public DbSet<RoutineExercise> RoutineExercises => Set<RoutineExercise>();
    public DbSet<RoutineSetTemplate> RoutineSetTemplates => Set<RoutineSetTemplate>();
    public DbSet<WorkoutSchedule> WorkoutSchedules => Set<WorkoutSchedule>();
    public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();
    public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();
    public DbSet<WorkoutSet> WorkoutSets => Set<WorkoutSet>();
    public DbSet<PersonalRecord> PersonalRecords => Set<PersonalRecord>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<UserSetting> UserSettings => Set<UserSetting>();
    public DbSet<BodyMeasurement> BodyMeasurements => Set<BodyMeasurement>();
    public DbSet<ProgressPhoto> ProgressPhotos => Set<ProgressPhoto>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    Task<int> IAppDbContext.SaveChangesAsync(CancellationToken cancellationToken)
        => base.SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureIdentity(builder);
        ConfigureTaxonomy(builder);
        ConfigureRoutines(builder);
        ConfigureWorkouts(builder);
        ConfigureUserData(builder);

        SeedData.Apply(builder);
    }

    private static void ConfigureIdentity(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(x => x.DisplayName).HasMaxLength(60).IsRequired();
            // Identity already normalizes email; a unique index enforces it at the database level.
            entity.HasIndex(x => x.NormalizedEmail).IsUnique();
        });

        builder.Entity<RefreshToken>(entity =>
        {
            entity.Property(x => x.TokenHash).HasMaxLength(128).IsRequired();
            entity.Property(x => x.UserAgent).HasMaxLength(256);
            entity.HasIndex(x => x.TokenHash).IsUnique();
            entity.HasIndex(x => new { x.UserId, x.FamilyId });

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureTaxonomy(ModelBuilder builder)
    {
        builder.Entity<Muscle>(entity =>
        {
            entity.Property(x => x.Slug).HasMaxLength(60).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(80).IsRequired();
            entity.Property(x => x.BodyRegion).HasMaxLength(40).IsRequired();
            entity.HasIndex(x => x.Slug).IsUnique();
        });

        builder.Entity<Equipment>(entity =>
        {
            entity.Property(x => x.Slug).HasMaxLength(60).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(80).IsRequired();
            entity.Property(x => x.DefaultBarWeightKg).HasPrecision(6, 2);
            entity.HasIndex(x => x.Slug).IsUnique();
        });

        builder.Entity<Exercise>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Instructions).HasMaxLength(4000);
            entity.Property(x => x.Category).HasMaxLength(60);
            entity.Property(x => x.MediaUrl).HasMaxLength(500);
            entity.Property(x => x.DefaultIncrementKg).HasPrecision(6, 3);

            // Primary lookup path: the built-in catalog plus one user's own exercises.
            entity.HasIndex(x => new { x.OwnerId, x.Name });
            entity.HasIndex(x => x.Name);

            entity.HasOne(x => x.Equipment)
                .WithMany(x => x.Exercises)
                .HasForeignKey(x => x.EquipmentId)
                // Equipment is reference data; removing it must not delete exercises.
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ExerciseMuscle>(entity =>
        {
            entity.Property(x => x.ContributionWeight).HasPrecision(4, 2);
            entity.HasIndex(x => new { x.ExerciseId, x.MuscleId }).IsUnique();

            entity.HasOne(x => x.Exercise)
                .WithMany(x => x.Muscles)
                .HasForeignKey(x => x.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Muscle)
                .WithMany(x => x.Exercises)
                .HasForeignKey(x => x.MuscleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ExerciseNote>(entity =>
        {
            entity.Property(x => x.Text).HasMaxLength(2000);
            // One persistent note per user per exercise.
            entity.HasIndex(x => new { x.OwnerId, x.ExerciseId }).IsUnique();

            entity.HasOne(x => x.Exercise)
                .WithMany()
                .HasForeignKey(x => x.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureRoutines(ModelBuilder builder)
    {
        builder.Entity<RoutineFolder>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(80).IsRequired();
            entity.HasIndex(x => new { x.OwnerId, x.Name });

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Routine>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(2000);
            entity.HasIndex(x => new { x.OwnerId, x.Order });

            entity.HasOne(x => x.Folder)
                .WithMany(x => x.Routines)
                .HasForeignKey(x => x.FolderId)
                // Deleting a folder must not delete routines (spec US-021).
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<RoutineExercise>(entity =>
        {
            entity.Property(x => x.Notes).HasMaxLength(2000);
            entity.HasIndex(x => new { x.RoutineId, x.Order });

            entity.HasOne(x => x.Routine)
                .WithMany(x => x.Exercises)
                .HasForeignKey(x => x.RoutineId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Exercise)
                .WithMany()
                .HasForeignKey(x => x.ExerciseId)
                // Exercises are archived rather than deleted when referenced, so a
                // restrictive rule here surfaces genuine bugs instead of losing templates.
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<RoutineSetTemplate>(entity =>
        {
            entity.Property(x => x.TargetWeight).HasPrecision(8, 3);
            entity.HasIndex(x => new { x.RoutineExerciseId, x.Order });

            entity.HasOne(x => x.RoutineExercise)
                .WithMany(x => x.Sets)
                .HasForeignKey(x => x.RoutineExerciseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<WorkoutSchedule>(entity =>
        {
            // One routine per day of week keeps "next workout" unambiguous.
            entity.HasIndex(x => new { x.OwnerId, x.DayOfWeek }).IsUnique();

            entity.HasOne(x => x.Routine)
                .WithMany()
                .HasForeignKey(x => x.RoutineId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureWorkouts(ModelBuilder builder)
    {
        builder.Entity<WorkoutSession>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Notes).HasMaxLength(4000);

            // History is always read newest-first for one owner.
            entity.HasIndex(x => new { x.OwnerId, x.StartedAt });
            entity.HasIndex(x => new { x.OwnerId, x.Status });

            entity.Ignore(x => x.Duration);

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Routine>()
                .WithMany()
                .HasForeignKey(x => x.RoutineId)
                // History must survive routine deletion (spec 4.1).
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<WorkoutExercise>(entity =>
        {
            entity.Property(x => x.ExerciseName).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Notes).HasMaxLength(2000);
            entity.HasIndex(x => new { x.WorkoutSessionId, x.Order });
            entity.HasIndex(x => x.ExerciseId);

            entity.HasOne(x => x.WorkoutSession)
                .WithMany(x => x.Exercises)
                .HasForeignKey(x => x.WorkoutSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Exercise)
                .WithMany()
                .HasForeignKey(x => x.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<WorkoutSet>(entity =>
        {
            // Decimal, not floating point, so 82.5 kg is exact (spec 4.1).
            entity.Property(x => x.Weight).HasPrecision(8, 3);
            entity.Property(x => x.Rpe).HasPrecision(3, 1);
            entity.Property(x => x.DistanceMeters).HasPrecision(10, 2);
            entity.Property(x => x.Notes).HasMaxLength(500);

            entity.HasIndex(x => new { x.WorkoutExerciseId, x.Order });

            entity.Ignore(x => x.IsCompleted);
            entity.Ignore(x => x.IsWorkSet);
            entity.Ignore(x => x.CountsTowardRecords);

            entity.HasOne(x => x.WorkoutExercise)
                .WithMany(x => x.Sets)
                .HasForeignKey(x => x.WorkoutExerciseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<PersonalRecord>(entity =>
        {
            entity.Property(x => x.Value).HasPrecision(12, 3);
            entity.Property(x => x.AtWeight).HasPrecision(8, 3);

            entity.HasIndex(x => new { x.OwnerId, x.ExerciseId, x.Type });
            entity.HasIndex(x => new { x.OwnerId, x.AchievedAt });

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Records are a recomputable projection keyed by id only, so no foreign keys
            // are declared to exercises or sessions. Workout-level records deliberately
            // carry Guid.Empty as their exercise id.
            entity.Ignore(x => x.Exercise);
        });
    }

    private static void ConfigureUserData(ModelBuilder builder)
    {
        builder.Entity<UserProfile>(entity =>
        {
            entity.Property(x => x.Gender).HasMaxLength(40);
            entity.Property(x => x.HeightCm).HasPrecision(5, 1);
            entity.Property(x => x.AvatarStorageKey).HasMaxLength(300);
            entity.HasIndex(x => x.OwnerId).IsUnique();

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<UserSetting>(entity =>
        {
            entity.Property(x => x.TimeZone).HasMaxLength(80).IsRequired();
            entity.Property(x => x.BarWeightKg).HasPrecision(6, 2);
            entity.Property(x => x.RoundingIncrementKg).HasPrecision(6, 3);
            entity.Property(x => x.OverloadIncrementKg).HasPrecision(6, 3);
            entity.HasIndex(x => x.OwnerId).IsUnique();

            // Small fixed-size collections of scalars; a JSON column keeps them together
            // without needing extra tables.
            entity.PrimitiveCollection(x => x.PlateInventoryKg);
            entity.PrimitiveCollection(x => x.WarmupPercentages);

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<BodyMeasurement>(entity =>
        {
            entity.Property(x => x.WeightKg).HasPrecision(6, 2);
            entity.Property(x => x.BodyFatPercent).HasPrecision(4, 1);

            foreach (var property in new[]
                     {
                         nameof(BodyMeasurement.ChestCm), nameof(BodyMeasurement.WaistCm),
                         nameof(BodyMeasurement.HipsCm), nameof(BodyMeasurement.LeftArmCm),
                         nameof(BodyMeasurement.RightArmCm), nameof(BodyMeasurement.LeftThighCm),
                         nameof(BodyMeasurement.RightThighCm), nameof(BodyMeasurement.LeftCalfCm),
                         nameof(BodyMeasurement.RightCalfCm), nameof(BodyMeasurement.ShouldersCm),
                         nameof(BodyMeasurement.NeckCm)
                     })
            {
                entity.Property(property).HasPrecision(5, 1);
            }

            entity.Property(x => x.Notes).HasMaxLength(1000);
            // One entry per day per user, which keeps the weight trend unambiguous.
            entity.HasIndex(x => new { x.OwnerId, x.MeasuredOn }).IsUnique();

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ProgressPhoto>(entity =>
        {
            entity.Property(x => x.StorageKey).HasMaxLength(300).IsRequired();
            entity.Property(x => x.ContentType).HasMaxLength(80).IsRequired();
            entity.Property(x => x.WeightKg).HasPrecision(6, 2);
            entity.Property(x => x.Notes).HasMaxLength(1000);
            entity.HasIndex(x => new { x.OwnerId, x.TakenOn });

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
