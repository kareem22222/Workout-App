using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Domain;

namespace WorkoutTracker.Application;

/// <summary>
/// Data access contract owned by the Application layer and implemented by
/// Infrastructure. This keeps use-case services free of any database provider
/// dependency while still allowing efficient EF Core queries (spec 2).
/// </summary>
public interface IAppDbContext
{
    DbSet<Muscle> Muscles { get; }
    DbSet<Equipment> Equipment { get; }
    DbSet<Exercise> Exercises { get; }
    DbSet<ExerciseMuscle> ExerciseMuscles { get; }
    DbSet<ExerciseNote> ExerciseNotes { get; }
    DbSet<RoutineFolder> RoutineFolders { get; }
    DbSet<Routine> Routines { get; }
    DbSet<RoutineExercise> RoutineExercises { get; }
    DbSet<RoutineSetTemplate> RoutineSetTemplates { get; }
    DbSet<WorkoutSchedule> WorkoutSchedules { get; }
    DbSet<WorkoutSession> WorkoutSessions { get; }
    DbSet<WorkoutExercise> WorkoutExercises { get; }
    DbSet<WorkoutSet> WorkoutSets { get; }
    DbSet<PersonalRecord> PersonalRecords { get; }
    DbSet<UserProfile> UserProfiles { get; }
    DbSet<UserSetting> UserSettings { get; }
    DbSet<BodyMeasurement> BodyMeasurements { get; }
    DbSet<ProgressPhoto> ProgressPhotos { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Why a use case failed, so the API can map to the correct HTTP status without
/// services knowing about HTTP (spec 5 documented error shapes).
/// </summary>
public enum ErrorKind
{
    None = 0,

    /// <summary>Input failed validation. Maps to 400.</summary>
    Validation = 1,

    /// <summary>
    /// Resource is absent, or belongs to another user. Both collapse to 404 so the API
    /// never reveals the existence of another user's data (spec US-260).
    /// </summary>
    NotFound = 2,

    /// <summary>Request conflicts with current state, e.g. a second active workout. Maps to 409.</summary>
    Conflict = 3,

    /// <summary>Caller is authenticated but not permitted. Maps to 403.</summary>
    Forbidden = 4
}

/// <summary>Outcome of a use case that returns a value.</summary>
/// <typeparam name="T">The success payload type.</typeparam>
public sealed record Result<T>
{
    private Result() { }

    public bool Succeeded { get; private init; }
    public T? Value { get; private init; }
    public ErrorKind Error { get; private init; }
    public string? Message { get; private init; }

    /// <summary>Field-level validation messages, keyed by property name.</summary>
    public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; private init; }

    /// <summary>Extra payload returned alongside a conflict, e.g. the existing active workout.</summary>
    public object? ConflictPayload { get; private init; }

    public static Result<T> Ok(T value) => new() { Succeeded = true, Value = value };

    public static Result<T> Invalid(string message) =>
        new() { Error = ErrorKind.Validation, Message = message };

    public static Result<T> Invalid(string field, string message) => new()
    {
        Error = ErrorKind.Validation,
        Message = message,
        ValidationErrors = new Dictionary<string, string[]> { [field] = [message] }
    };

    public static Result<T> NotFound(string message = "The requested resource was not found.") =>
        new() { Error = ErrorKind.NotFound, Message = message };

    public static Result<T> Conflict(string message, object? payload = null) =>
        new() { Error = ErrorKind.Conflict, Message = message, ConflictPayload = payload };

    public static Result<T> Forbidden(string message = "You do not have access to this resource.") =>
        new() { Error = ErrorKind.Forbidden, Message = message };
}

/// <summary>Outcome of a use case with no return value.</summary>
public sealed record Result
{
    private Result() { }

    public bool Succeeded { get; private init; }
    public ErrorKind Error { get; private init; }
    public string? Message { get; private init; }

    public static Result Ok() => new() { Succeeded = true };

    public static Result Invalid(string message) => new() { Error = ErrorKind.Validation, Message = message };

    public static Result NotFound(string message = "The requested resource was not found.") =>
        new() { Error = ErrorKind.NotFound, Message = message };

    public static Result Conflict(string message) => new() { Error = ErrorKind.Conflict, Message = message };

    public static Result Forbidden(string message = "You do not have access to this resource.") =>
        new() { Error = ErrorKind.Forbidden, Message = message };
}

/// <summary>A page of results for the paginated history and exercise-history lists.</summary>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasMore => Page * PageSize < TotalCount;
}
