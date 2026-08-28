using System.Security.Claims;
using WorkoutTracker.Application;

namespace WorkoutTracker.Api;

/// <summary>
/// Translates Application results into HTTP responses so every endpoint returns the same
/// documented error shape (spec 5).
/// </summary>
public static class ResultExtensions
{
    /// <summary>Maps a value-returning result, using 200 OK on success.</summary>
    public static IResult ToHttp<T>(this Result<T> result)
        => result.Succeeded ? TypedResults.Ok(result.Value) : Failure(result);

    /// <summary>Maps a result to 201 Created with a location header.</summary>
    public static IResult ToCreated<T>(this Result<T> result, Func<T, string> location)
        => result.Succeeded ? TypedResults.Created(location(result.Value!), result.Value) : Failure(result);

    /// <summary>Maps a void result, using 204 No Content on success.</summary>
    public static IResult ToHttp(this Result result)
        => result.Succeeded
            ? TypedResults.NoContent()
            : result.Error switch
            {
                ErrorKind.NotFound => Problem(404, "Not found", result.Message),
                ErrorKind.Conflict => Problem(409, "Conflict", result.Message),
                ErrorKind.Forbidden => Problem(403, "Forbidden", result.Message),
                _ => Problem(400, "Validation failed", result.Message)
            };

    private static IResult Failure<T>(Result<T> result) => result.Error switch
    {
        ErrorKind.NotFound => Problem(404, "Not found", result.Message),

        // A conflict may carry the current server state so the client can reconcile.
        ErrorKind.Conflict => TypedResults.Json(new
        {
            title = "Conflict",
            status = 409,
            detail = result.Message,
            current = result.ConflictPayload
        }, statusCode: 409),

        ErrorKind.Forbidden => Problem(403, "Forbidden", result.Message),

        _ => result.ValidationErrors is { Count: > 0 }
            ? TypedResults.ValidationProblem(result.ValidationErrors.ToDictionary(x => x.Key, x => x.Value), detail: result.Message)
            : Problem(400, "Validation failed", result.Message)
    };

    private static IResult Problem(int status, string title, string? detail)
        => TypedResults.Problem(detail: detail, statusCode: status, title: title);

    /// <summary>
    /// Resolves the authenticated user id from the token. Ownership is always taken from
    /// the principal, never from the request body (spec 2.2).
    /// </summary>
    public static Guid UserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Authenticated principal is missing a subject claim.");

        return Guid.Parse(value);
    }
}
