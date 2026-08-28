using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Application.Contracts;
using WorkoutTracker.Domain;

namespace WorkoutTracker.Application.Services;

/// <summary>
/// Data ownership use cases: complete export and compatible CSV import
/// (spec Epics 25, 26). Exports contain only the requesting user's data.
/// </summary>
public sealed class DataTransferService(
    IAppDbContext db,
    SettingsService settings,
    ExerciseService exercises,
    RoutineService routines,
    PersonalRecordService records,
    IClock clock)
{
    /// <summary>Bumped whenever the export shape changes, so importers can adapt.</summary>
    public const int ExportSchemaVersion = 1;

    private const string ExportSchemaName = "workouttracker.export";

    /// <summary>Columns a workout-sets CSV import must provide.</summary>
    private static readonly string[] RequiredImportColumns = ["date", "exercise", "weight", "reps"];

    // ---------------------------------------------------------------------------------
    // Export
    // ---------------------------------------------------------------------------------

    /// <summary>Complete JSON export of everything the user owns (spec US-200).</summary>
    public async Task<Result<ExportBundleDto>> ExportJsonAsync(Guid ownerId, CancellationToken ct = default)
    {
        var profile = await settings.GetProfileAsync(ownerId, ct);
        if (!profile.Succeeded) return Result<ExportBundleDto>.NotFound("Account not found.");

        var userSettings = await settings.GetOrCreateSettingsAsync(ownerId, ct);

        var custom = await exercises.ListAsync(ownerId, includeArchived: true, ct: ct);
        var routineList = await routines.ListAsync(ownerId, includeArchived: true, ct: ct);

        var sessions = await db.WorkoutSessions
            .Where(x => x.OwnerId == ownerId)
            .OrderBy(x => x.StartedAt)
            .Include(x => x.Exercises.OrderBy(e => e.Order)).ThenInclude(x => x.Sets.OrderBy(s => s.Order))
            .AsNoTracking()
            .ToListAsync(ct);

        var measurements = await db.BodyMeasurements
            .Where(x => x.OwnerId == ownerId)
            .OrderBy(x => x.MeasuredOn)
            .AsNoTracking()
            .ToListAsync(ct);

        var recordList = await records.ListAsync(ownerId, ct: ct);

        return Result<ExportBundleDto>.Ok(new ExportBundleDto(
            ExportSchemaName,
            ExportSchemaVersion,
            clock.UtcNow,
            profile.Value!,
            userSettings.ToDto(),
            (custom.Value ?? []).Where(x => x.IsCustom).ToList(),
            routineList.Value ?? [],
            sessions.Select(x => x.ToDto()).ToList(),
            measurements.Select(x => x.ToDto()).ToList(),
            recordList.Value ?? []));
    }

    /// <summary>
    /// CSV export for one dataset. Kept as flat, spreadsheet-friendly tables rather than
    /// nested structures (spec US-200).
    /// </summary>
    /// <param name="dataset">One of workouts, sets, exercises, measurements.</param>
    public async Task<Result<(string FileName, string Csv)>> ExportCsvAsync(
        Guid ownerId,
        string dataset,
        CancellationToken ct = default)
    {
        var stamp = clock.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

        switch (dataset?.ToLowerInvariant())
        {
            case "workouts":
                return Result<(string, string)>.Ok(($"workouts-{stamp}.csv", await WorkoutsCsvAsync(ownerId, ct)));

            case "sets":
                return Result<(string, string)>.Ok(($"workout-sets-{stamp}.csv", await SetsCsvAsync(ownerId, ct)));

            case "exercises":
                return Result<(string, string)>.Ok(($"exercises-{stamp}.csv", await ExercisesCsvAsync(ownerId, ct)));

            case "measurements":
                return Result<(string, string)>.Ok(($"measurements-{stamp}.csv", await MeasurementsCsvAsync(ownerId, ct)));

            default:
                return Result<(string, string)>.Invalid(nameof(dataset),
                    "Dataset must be one of workouts, sets, exercises, measurements.");
        }
    }

    private async Task<string> WorkoutsCsvAsync(Guid ownerId, CancellationToken ct)
    {
        var sessions = await db.WorkoutSessions
            .Where(x => x.OwnerId == ownerId)
            .OrderBy(x => x.StartedAt)
            .Include(x => x.Exercises).ThenInclude(x => x.Sets)
            .AsNoTracking()
            .ToListAsync(ct);

        var builder = new StringBuilder();
        builder.AppendLine("workout_id,title,status,started_at,completed_at,duration_seconds,exercises,completed_sets,total_reps,volume_kg,notes");

        foreach (var session in sessions)
        {
            builder.AppendLine(string.Join(',',
                Csv(session.Id),
                Csv(session.Title),
                Csv(session.Status),
                Csv(Iso(session.StartedAt)),
                Csv(session.CompletedAt is { } done ? Iso(done) : ""),
                Csv((int)(session.Duration ?? TimeSpan.Zero).TotalSeconds),
                Csv(session.Exercises.Count),
                Csv(TrainingVolume.CompletedWorkSets(session)),
                Csv(TrainingVolume.CompletedReps(session)),
                Csv(TrainingVolume.ForSession(session)),
                Csv(session.Notes)));
        }

        return builder.ToString();
    }

    private async Task<string> SetsCsvAsync(Guid ownerId, CancellationToken ct)
    {
        var sessions = await db.WorkoutSessions
            .Where(x => x.OwnerId == ownerId)
            .OrderBy(x => x.StartedAt)
            .Include(x => x.Exercises.OrderBy(e => e.Order)).ThenInclude(x => x.Sets.OrderBy(s => s.Order))
            .AsNoTracking()
            .ToListAsync(ct);

        var builder = new StringBuilder();
        builder.AppendLine("date,workout_id,workout_title,exercise,exercise_order,set_order,set_type,weight_kg,reps,rpe,duration_seconds,distance_meters,completed,notes");

        foreach (var session in sessions)
        {
            foreach (var exercise in session.Exercises)
            {
                foreach (var set in exercise.Sets)
                {
                    builder.AppendLine(string.Join(',',
                        Csv(Iso(session.StartedAt)),
                        Csv(session.Id),
                        Csv(session.Title),
                        Csv(exercise.ExerciseName),
                        Csv(exercise.Order),
                        Csv(set.Order),
                        Csv(set.Type),
                        Csv(set.Weight),
                        Csv(set.Reps),
                        Csv(set.Rpe),
                        Csv(set.DurationSeconds),
                        Csv(set.DistanceMeters),
                        Csv(set.IsCompleted),
                        Csv(set.Notes)));
                }
            }
        }

        return builder.ToString();
    }

    private async Task<string> ExercisesCsvAsync(Guid ownerId, CancellationToken ct)
    {
        var items = await db.Exercises
            .Where(x => x.OwnerId == null || x.OwnerId == ownerId)
            .Include(x => x.Equipment)
            .Include(x => x.Muscles).ThenInclude(x => x.Muscle)
            .OrderBy(x => x.Name)
            .AsNoTracking()
            .ToListAsync(ct);

        var builder = new StringBuilder();
        builder.AppendLine("exercise_id,name,type,category,equipment,primary_muscles,secondary_muscles,is_custom,is_archived,instructions");

        foreach (var exercise in items)
        {
            var primary = exercise.Muscles.Where(x => x.Role == MuscleRole.Primary).Select(x => x.Muscle?.Name ?? "");
            var secondary = exercise.Muscles.Where(x => x.Role == MuscleRole.Secondary).Select(x => x.Muscle?.Name ?? "");

            builder.AppendLine(string.Join(',',
                Csv(exercise.Id),
                Csv(exercise.Name),
                Csv(exercise.Type),
                Csv(exercise.Category),
                Csv(exercise.Equipment?.Name ?? ""),
                Csv(string.Join("; ", primary)),
                Csv(string.Join("; ", secondary)),
                Csv(exercise.OwnerId is not null),
                Csv(exercise.IsArchived),
                Csv(exercise.Instructions)));
        }

        return builder.ToString();
    }

    private async Task<string> MeasurementsCsvAsync(Guid ownerId, CancellationToken ct)
    {
        var items = await db.BodyMeasurements
            .Where(x => x.OwnerId == ownerId)
            .OrderBy(x => x.MeasuredOn)
            .AsNoTracking()
            .ToListAsync(ct);

        var builder = new StringBuilder();
        builder.AppendLine("date,weight_kg,body_fat_percent,chest_cm,waist_cm,hips_cm,left_arm_cm,right_arm_cm,left_thigh_cm,right_thigh_cm,left_calf_cm,right_calf_cm,shoulders_cm,neck_cm,notes");

        foreach (var item in items)
        {
            builder.AppendLine(string.Join(',',
                Csv(item.MeasuredOn.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                Csv(item.WeightKg),
                Csv(item.BodyFatPercent),
                Csv(item.ChestCm),
                Csv(item.WaistCm),
                Csv(item.HipsCm),
                Csv(item.LeftArmCm),
                Csv(item.RightArmCm),
                Csv(item.LeftThighCm),
                Csv(item.RightThighCm),
                Csv(item.LeftCalfCm),
                Csv(item.RightCalfCm),
                Csv(item.ShouldersCm),
                Csv(item.NeckCm),
                Csv(item.Notes)));
        }

        return builder.ToString();
    }

    // ---------------------------------------------------------------------------------
    // Import
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// Parses a workout-sets CSV and reports per-row problems without writing anything,
    /// so the user can review before committing (spec US-210).
    /// </summary>
    public async Task<Result<ImportPreviewDto>> PreviewImportAsync(
        Guid ownerId,
        string csv,
        CancellationToken ct = default)
    {
        var parsed = await ParseAsync(ownerId, csv, ct);
        if (!parsed.Succeeded) return Result<ImportPreviewDto>.Invalid(parsed.Message ?? "Invalid CSV.");

        var rows = parsed.Value!;
        var previews = rows
            .Select(x => new ImportRowPreviewDto(x.RowNumber, x.RawDate, x.RawExercise, x.RawWeight, x.RawReps, x.Error))
            .ToList();

        var valid = previews.Count(x => x.Error is null);

        return Result<ImportPreviewDto>.Ok(new ImportPreviewDto(
            previews.Count,
            valid,
            previews.Count - valid,
            valid > 0,
            previews));
    }

    /// <summary>
    /// Commits a previously previewed CSV. Rows are grouped into one completed workout per
    /// date, and the whole import shares a single SaveChanges so it either lands or does not
    /// (spec US-210).
    /// </summary>
    public async Task<Result<ImportResultDto>> CommitImportAsync(
        Guid ownerId,
        string csv,
        CancellationToken ct = default)
    {
        var parsed = await ParseAsync(ownerId, csv, ct);
        if (!parsed.Succeeded) return Result<ImportResultDto>.Invalid(parsed.Message ?? "Invalid CSV.");

        var rows = parsed.Value!.Where(x => x.Error is null).ToList();
        if (rows.Count == 0) return Result<ImportResultDto>.Invalid("No valid rows to import.");

        var skipped = parsed.Value!.Count - rows.Count;
        var workoutsCreated = 0;
        var setsCreated = 0;

        foreach (var byDate in rows.GroupBy(x => x.Date))
        {
            var startedAt = new DateTimeOffset(byDate.Key.ToDateTime(new TimeOnly(12, 0)), TimeSpan.Zero);

            // Skip a date that already has an imported workout, which makes re-running safe.
            var alreadyImported = await db.WorkoutSessions.AnyAsync(
                x => x.OwnerId == ownerId && x.StartedAt == startedAt && x.Title == ImportedTitle(byDate.Key), ct);

            if (alreadyImported)
            {
                skipped += byDate.Count();
                continue;
            }

            var session = new WorkoutSession
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                Title = ImportedTitle(byDate.Key),
                Status = WorkoutStatus.Completed,
                StartedAt = startedAt,
                CompletedAt = startedAt.AddHours(1),
                Notes = "Imported from CSV.",
                Version = 1
            };

            var order = 0;
            foreach (var byExercise in byDate.GroupBy(x => x.ExerciseId))
            {
                var first = byExercise.First();

                var exercise = new WorkoutExercise
                {
                    Id = Guid.NewGuid(),
                    ExerciseId = byExercise.Key,
                    ExerciseName = first.ExerciseName,
                    ExerciseType = first.ExerciseType,
                    Order = order++,
                    RestSeconds = 90
                };

                var setOrder = 0;
                foreach (var row in byExercise)
                {
                    exercise.Sets.Add(new WorkoutSet
                    {
                        Id = Guid.NewGuid(),
                        Order = setOrder++,
                        Weight = row.Weight,
                        Reps = row.Reps,
                        Type = WorkoutSetType.Normal,
                        CompletedAt = startedAt
                    });
                    setsCreated++;
                }

                session.Exercises.Add(exercise);
            }

            db.WorkoutSessions.Add(session);
            workoutsCreated++;
        }

        await db.SaveChangesAsync(ct);

        // Imported history changes the record picture, so rebuild it.
        if (workoutsCreated > 0) await records.RecomputeAsync(ownerId, ct);

        return Result<ImportResultDto>.Ok(new ImportResultDto(workoutsCreated, setsCreated, skipped));
    }

    private static string ImportedTitle(DateOnly date)
        => $"Imported workout {date:yyyy-MM-dd}";

    /// <summary>
    /// Parses and validates every row, resolving exercise names against the user's
    /// visible library. Unknown names are reported rather than silently created.
    /// </summary>
    private async Task<Result<List<ImportRow>>> ParseAsync(Guid ownerId, string csv, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(csv)) return Result<List<ImportRow>>.Invalid("The CSV file is empty.");

        var lines = csv.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (lines.Length < 2) return Result<List<ImportRow>>.Invalid("The CSV must contain a header row and at least one data row.");

        var header = SplitCsvLine(lines[0]).Select(x => x.Trim().ToLowerInvariant()).ToList();

        var missing = RequiredImportColumns.Where(x => !header.Contains(x)).ToList();
        if (missing.Count > 0)
            return Result<List<ImportRow>>.Invalid($"Missing required columns: {string.Join(", ", missing)}.");

        var dateIndex = header.IndexOf("date");
        var exerciseIndex = header.IndexOf("exercise");
        var weightIndex = header.IndexOf("weight");
        var repsIndex = header.IndexOf("reps");

        var library = await db.Exercises
            .Where(x => x.OwnerId == null || x.OwnerId == ownerId)
            .Select(x => new { x.Id, x.Name, x.Type })
            .AsNoTracking()
            .ToListAsync(ct);

        var byName = library
            .GroupBy(x => x.Name.ToLowerInvariant())
            .ToDictionary(x => x.Key, x => x.First());

        var rows = new List<ImportRow>();

        for (var index = 1; index < lines.Length; index++)
        {
            var fields = SplitCsvLine(lines[index]);
            var rowNumber = index + 1;

            var rawDate = Field(fields, dateIndex);
            var rawExercise = Field(fields, exerciseIndex);
            var rawWeight = Field(fields, weightIndex);
            var rawReps = Field(fields, repsIndex);

            var row = new ImportRow
            {
                RowNumber = rowNumber,
                RawDate = rawDate,
                RawExercise = rawExercise,
                RawWeight = rawWeight,
                RawReps = rawReps
            };

            if (!DateOnly.TryParse(rawDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
                && !DateOnly.TryParse(rawDate, out date))
            {
                row.Error = $"Unrecognized date '{rawDate}'.";
                rows.Add(row);
                continue;
            }

            if (!byName.TryGetValue(rawExercise.Trim().ToLowerInvariant(), out var match))
            {
                row.Error = $"Unknown exercise '{rawExercise}'. Create it before importing.";
                rows.Add(row);
                continue;
            }

            if (!decimal.TryParse(rawWeight, NumberStyles.Float, CultureInfo.InvariantCulture, out var weight)
                || weight is < 0 or > 2000)
            {
                row.Error = $"Invalid weight '{rawWeight}'.";
                rows.Add(row);
                continue;
            }

            if (!int.TryParse(rawReps, NumberStyles.Integer, CultureInfo.InvariantCulture, out var reps)
                || reps is < 0 or > 1000)
            {
                row.Error = $"Invalid reps '{rawReps}'.";
                rows.Add(row);
                continue;
            }

            row.Date = date;
            row.ExerciseId = match.Id;
            row.ExerciseName = match.Name;
            row.ExerciseType = match.Type;
            row.Weight = weight;
            row.Reps = reps;

            rows.Add(row);
        }

        return Result<List<ImportRow>>.Ok(rows);
    }

    private static string Field(List<string> fields, int index)
        => index >= 0 && index < fields.Count ? fields[index].Trim() : "";

    /// <summary>
    /// Minimal RFC 4180 style splitter handling quoted fields and escaped quotes, which
    /// avoids taking a CSV dependency for one import path.
    /// </summary>
    private static List<string> SplitCsvLine(string line)
    {
        var fields = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        for (var index = 0; index < line.Length; index++)
        {
            var character = line[index];

            if (inQuotes)
            {
                if (character == '"')
                {
                    // A doubled quote inside a quoted field is a literal quote.
                    if (index + 1 < line.Length && line[index + 1] == '"')
                    {
                        current.Append('"');
                        index++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    current.Append(character);
                }
                continue;
            }

            switch (character)
            {
                case '"':
                    inQuotes = true;
                    break;
                case ',':
                    fields.Add(current.ToString());
                    current.Clear();
                    break;
                default:
                    current.Append(character);
                    break;
            }
        }

        fields.Add(current.ToString());
        return fields;
    }

    private static string Iso(DateTimeOffset value)
        => value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

    /// <summary>
    /// Renders a CSV field, quoting and escaping when required so text containing commas,
    /// quotes or newlines cannot corrupt the output.
    /// </summary>
    private static string Csv(object? value)
    {
        var text = value switch
        {
            null => "",
            bool flag => flag ? "true" : "false",
            decimal number => number.ToString("0.####", CultureInfo.InvariantCulture),
            double number => number.ToString("0.####", CultureInfo.InvariantCulture),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString() ?? ""
        };

        if (text.Length == 0) return "";

        var needsQuotes = text.Contains(',') || text.Contains('"') || text.Contains('\n') || text.Contains('\r');
        return needsQuotes ? $"\"{text.Replace("\"", "\"\"")}\"" : text;
    }

    /// <summary>One parsed CSV row with either resolved values or an error.</summary>
    private sealed class ImportRow
    {
        public int RowNumber { get; set; }
        public string RawDate { get; set; } = "";
        public string RawExercise { get; set; } = "";
        public string RawWeight { get; set; } = "";
        public string RawReps { get; set; } = "";
        public string? Error { get; set; }

        public DateOnly Date { get; set; }
        public Guid ExerciseId { get; set; }
        public string ExerciseName { get; set; } = "";
        public ExerciseType ExerciseType { get; set; }
        public decimal Weight { get; set; }
        public int Reps { get; set; }
    }
}

/// <summary>
/// Operational use cases for the owner/admin role (spec US-290). Admin actions never
/// expose passwords or private photo content.
/// </summary>
public sealed class AdminService(IAppDbContext db, IUserDirectory users)
{
    public async Task<Result<IReadOnlyList<AdminUserDto>>> ListUsersAsync(CancellationToken ct = default)
    {
        var accounts = await users.ListAsync(ct);

        var stats = await db.WorkoutSessions
            .Where(x => x.Status == WorkoutStatus.Completed)
            .GroupBy(x => x.OwnerId)
            .Select(group => new
            {
                OwnerId = group.Key,
                Count = group.Count(),
                Last = group.Max(x => x.CompletedAt)
            })
            .AsNoTracking()
            .ToListAsync(ct);

        var byOwner = stats.ToDictionary(x => x.OwnerId);

        var result = accounts.Select(account =>
        {
            byOwner.TryGetValue(account.Id, out var stat);
            return new AdminUserDto(
                account.Id,
                account.DisplayName,
                account.Email,
                account.IsAdmin,
                account.IsDisabled,
                account.CreatedAt,
                stat?.Count ?? 0,
                stat?.Last);
        }).ToList();

        return Result<IReadOnlyList<AdminUserDto>>.Ok(result);
    }

    /// <summary>
    /// Enables or disables an account. Admins cannot disable themselves, which prevents
    /// locking the last operator out.
    /// </summary>
    public async Task<Result> SetDisabledAsync(
        Guid actingAdminId,
        Guid targetUserId,
        bool isDisabled,
        CancellationToken ct = default)
    {
        if (actingAdminId == targetUserId && isDisabled)
            return Result.Invalid("You cannot disable your own account.");

        return await users.SetDisabledAsync(targetUserId, isDisabled, ct)
            ? Result.Ok()
            : Result.NotFound("User not found.");
    }
}
