namespace WorkoutTracker.Domain;

/// <summary>
/// Estimated one-rep-max formulas (spec 7.3). All methods are pure and guard against
/// rep counts where the underlying formulas stop being meaningful.
/// </summary>
public static class OneRepMax
{
    /// <summary>
    /// Above this rep count the linear formulas diverge badly, so estimates are refused
    /// rather than reported as fact.
    /// </summary>
    public const int MaxReliableReps = 12;

    /// <summary>Brzycki divides by (37 - reps), so reps must stay below 37.</summary>
    private const int BrzyckiLimit = 36;

    /// <summary>
    /// Estimates 1RM, or returns null when the inputs cannot produce a sensible value.
    /// A single rep is definitionally the lifted weight, so it is returned unchanged.
    /// </summary>
    public static decimal? Estimate(decimal weight, int reps, OneRepMaxFormula formula)
    {
        if (weight <= 0 || reps <= 0) return null;
        if (reps == 1) return weight;
        if (reps > MaxReliableReps) return null;

        decimal? estimate = formula switch
        {
            OneRepMaxFormula.Epley => weight * (1m + reps / 30m),
            OneRepMaxFormula.Brzycki when reps < BrzyckiLimit => weight * 36m / (37m - reps),
            OneRepMaxFormula.Brzycki => null,
            OneRepMaxFormula.Lombardi => LombardiEstimate(weight, reps),
            _ => null
        };

        return estimate is null ? null : decimal.Round(estimate.Value, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>Lombardi uses a fractional exponent, so it is evaluated in double precision then rounded.</summary>
    private static decimal LombardiEstimate(decimal weight, int reps)
        => (decimal)((double)weight * Math.Pow(reps, 0.10));

    /// <summary>
    /// Best estimated 1RM across a set collection, ignoring sets that do not qualify
    /// for records.
    /// </summary>
    public static decimal? BestEstimate(IEnumerable<WorkoutSet> sets, OneRepMaxFormula formula)
    {
        decimal? best = null;
        foreach (var set in sets.Where(x => x.CountsTowardRecords))
        {
            var estimate = Estimate(set.Weight, set.Reps, formula);
            if (estimate is not null && (best is null || estimate > best)) best = estimate;
        }
        return best;
    }
}

/// <summary>
/// Training volume rules (spec 7.1). Bodyweight movements deliberately contribute no
/// synthetic load; only genuine external load counts toward weight volume.
/// </summary>
public static class TrainingVolume
{
    /// <summary>Volume for one set: weight x reps for completed, load-bearing work sets.</summary>
    public static decimal ForSet(WorkoutSet set, ExerciseType exerciseType)
    {
        if (!set.IsCompleted || !set.IsWorkSet) return 0m;
        if (exerciseType is not (ExerciseType.WeightAndReps or ExerciseType.WeightedBodyweight)) return 0m;
        if (set.Weight <= 0 || set.Reps <= 0) return 0m;
        return set.Weight * set.Reps;
    }

    /// <summary>Sum of eligible set volumes for one exercise.</summary>
    public static decimal ForExercise(WorkoutExercise exercise)
        => exercise.Sets.Sum(set => ForSet(set, exercise.ExerciseType));

    /// <summary>Sum of eligible set volumes across a whole session.</summary>
    public static decimal ForSession(WorkoutSession session)
        => session.Exercises.Sum(ForExercise);

    /// <summary>Completed work sets, the count shown in summaries and stats.</summary>
    public static int CompletedWorkSets(WorkoutSession session)
        => session.Exercises.SelectMany(x => x.Sets).Count(x => x.IsCompleted && x.IsWorkSet);

    /// <summary>Total completed reps, which stays meaningful for bodyweight-only training.</summary>
    public static int CompletedReps(WorkoutSession session)
        => session.Exercises.SelectMany(x => x.Sets).Where(x => x.IsCompleted && x.IsWorkSet).Sum(x => x.Reps);
}

/// <summary>
/// Conversions between canonical storage units (kg, cm) and the user's display units
/// (spec 3.1). Weights are persisted in kilograms and converted only for presentation.
/// </summary>
public static class Units
{
    private const decimal KilogramsPerPound = 0.45359237m;
    private const decimal CentimetersPerInch = 2.54m;

    public static decimal KgToDisplay(decimal kg, WeightUnit unit)
        => unit == WeightUnit.Pounds ? decimal.Round(kg / KilogramsPerPound, 2, MidpointRounding.AwayFromZero) : kg;

    public static decimal DisplayToKg(decimal value, WeightUnit unit)
        => unit == WeightUnit.Pounds ? decimal.Round(value * KilogramsPerPound, 4, MidpointRounding.AwayFromZero) : value;

    public static decimal CmToDisplay(decimal cm, LengthUnit unit)
        => unit == LengthUnit.Inches ? decimal.Round(cm / CentimetersPerInch, 2, MidpointRounding.AwayFromZero) : cm;

    public static decimal DisplayToCm(decimal value, LengthUnit unit)
        => unit == LengthUnit.Inches ? decimal.Round(value * CentimetersPerInch, 4, MidpointRounding.AwayFromZero) : value;

    /// <summary>Rounds a load to the nearest achievable step for the user's equipment.</summary>
    public static decimal RoundToIncrement(decimal weight, decimal increment)
    {
        if (increment <= 0) return decimal.Round(weight, 2, MidpointRounding.AwayFromZero);
        var steps = decimal.Round(weight / increment, 0, MidpointRounding.AwayFromZero);
        return decimal.Round(steps * increment, 2, MidpointRounding.AwayFromZero);
    }
}
