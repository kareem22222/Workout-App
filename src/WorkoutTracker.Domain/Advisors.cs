namespace WorkoutTracker.Domain;

/// <summary>One plate size and how many of them go on each side of the bar.</summary>
public sealed record PlateStack(decimal PlateKg, int CountPerSide);

/// <summary>
/// Result of a plate calculation. When the exact target cannot be loaded, the result
/// reports the closest achievable total instead of failing (spec US-160).
/// </summary>
public sealed record PlateSolution(
    decimal RequestedKg,
    decimal AchievableKg,
    decimal BarKg,
    IReadOnlyList<PlateStack> PerSide,
    bool IsExact,
    string? Message);

/// <summary>
/// Greedy plate loading against the user's actual inventory (spec US-160). A greedy
/// pass is optimal for the standard halving plate progressions found in real gyms, and
/// any residual is reported rather than hidden.
/// </summary>
public static class PlateCalculator
{
    /// <summary>
    /// Computes the plates to load per side for a target total weight.
    /// </summary>
    /// <param name="targetKg">Desired total weight including the bar.</param>
    /// <param name="barKg">Weight of the empty bar.</param>
    /// <param name="inventoryKg">Available plate sizes. Assumed unlimited in count per size.</param>
    public static PlateSolution Solve(decimal targetKg, decimal barKg, IEnumerable<decimal> inventoryKg)
    {
        var plates = inventoryKg.Where(x => x > 0).Distinct().OrderByDescending(x => x).ToList();

        if (targetKg < barKg)
        {
            return new PlateSolution(targetKg, barKg, barKg, [], targetKg == barKg,
                $"Target is below the {barKg:0.##} kg bar weight.");
        }

        // Only half the load goes on each side, so all arithmetic is done per side.
        var perSideTarget = (targetKg - barKg) / 2m;
        var remaining = perSideTarget;
        var stacks = new List<PlateStack>();

        foreach (var plate in plates)
        {
            if (remaining < plate) continue;
            var count = (int)decimal.Floor(remaining / plate);
            if (count == 0) continue;
            stacks.Add(new PlateStack(plate, count));
            remaining -= count * plate;
        }

        var achievable = barKg + (perSideTarget - remaining) * 2m;
        var isExact = remaining == 0m;

        string? message = null;
        if (!isExact)
        {
            var nextUp = NextAchievableAbove(achievable, barKg, plates);
            message = nextUp is null
                ? $"Closest loadable weight is {achievable:0.##} kg with your plates."
                : $"Exact target not loadable. Nearest options are {achievable:0.##} kg or {nextUp:0.##} kg.";
        }

        return new PlateSolution(targetKg, achievable, barKg, stacks, isExact, message);
    }

    /// <summary>Smallest loadable total strictly above <paramref name="weight"/>, if one exists.</summary>
    private static decimal? NextAchievableAbove(decimal weight, decimal barKg, IReadOnlyList<decimal> plates)
    {
        if (plates.Count == 0) return null;
        var smallest = plates[^1];
        return weight + smallest * 2m;
    }
}

/// <summary>A single suggested warmup set.</summary>
public sealed record WarmupSet(int Order, int Percentage, decimal WeightKg, int Reps);

/// <summary>
/// Percentage-based warmup ramp rounded to achievable increments (spec US-060).
/// Suggestions are advisory: the caller decides whether to insert them.
/// </summary>
public static class WarmupCalculator
{
    /// <summary>
    /// Builds a warmup ramp toward a working weight. Sets that round to at or above the
    /// working weight, or below the bar, are dropped so the ramp stays useful.
    /// </summary>
    public static IReadOnlyList<WarmupSet> Build(
        decimal workingWeightKg,
        IEnumerable<int> percentages,
        decimal roundingIncrementKg,
        decimal barKg,
        int workingReps = 8)
    {
        if (workingWeightKg <= 0) return [];

        var results = new List<WarmupSet>();
        var order = 0;
        var seen = new HashSet<decimal>();

        foreach (var percentage in percentages.Where(x => x is > 0 and < 100).OrderBy(x => x))
        {
            var raw = workingWeightKg * percentage / 100m;
            var rounded = Units.RoundToIncrement(raw, roundingIncrementKg);

            // Skip loads that are not loadable, duplicate an earlier rung, or defeat the purpose.
            if (rounded < barKg || rounded >= workingWeightKg) continue;
            if (!seen.Add(rounded)) continue;

            // Higher percentages get fewer reps, which is how warmups actually progress.
            var reps = percentage switch
            {
                <= 50 => Math.Max(5, workingReps),
                <= 70 => Math.Max(3, workingReps - 3),
                _ => Math.Max(1, workingReps - 5)
            };

            results.Add(new WarmupSet(order++, percentage, rounded, reps));
        }

        return results;
    }
}

/// <summary>A transparent overload recommendation including the reasoning shown to the user.</summary>
public sealed record OverloadSuggestion(
    OverloadAction Action,
    decimal? SuggestedWeightKg,
    decimal? PreviousWeightKg,
    string Rationale);

/// <summary>
/// Rule-based progressive overload (spec 7.4). Deliberately simple and transparent:
/// it only ever suggests, never mutates a workout.
/// </summary>
public static class OverloadAdvisor
{
    /// <summary>RPE at or above this suggests the user is already at their limit.</summary>
    private const decimal HardRpeThreshold = 9.5m;

    /// <summary>
    /// Recommends the next load for an exercise based on the most recent completed
    /// work sets against the prescribed rep range.
    /// </summary>
    /// <param name="lastSets">Completed work sets from the most recent session for this exercise.</param>
    /// <param name="topOfRepRange">Top of the configured target rep range.</param>
    /// <param name="incrementKg">Load step to add when progressing.</param>
    /// <param name="roundingIncrementKg">Smallest achievable load step.</param>
    public static OverloadSuggestion Suggest(
        IReadOnlyList<WorkoutSet> lastSets,
        int topOfRepRange,
        decimal incrementKg,
        decimal roundingIncrementKg)
    {
        var working = lastSets.Where(x => x.CountsTowardRecords).ToList();

        if (working.Count == 0 || topOfRepRange <= 0)
        {
            return new OverloadSuggestion(OverloadAction.NotEnoughData, null, null,
                "No completed work sets yet for this exercise, so no suggestion is offered.");
        }

        var referenceWeight = working.Max(x => x.Weight);
        var setsAtReference = working.Where(x => x.Weight == referenceWeight).ToList();
        var allReachedTop = setsAtReference.All(x => x.Reps >= topOfRepRange);
        var hardestRpe = working.Where(x => x.Rpe is not null).Select(x => x.Rpe!.Value).DefaultIfEmpty(0m).Max();

        if (allReachedTop && hardestRpe < HardRpeThreshold)
        {
            var next = Units.RoundToIncrement(referenceWeight + incrementKg, roundingIncrementKg);

            // Rounding must not stall progress when the increment is smaller than the step.
            if (next <= referenceWeight) next = referenceWeight + roundingIncrementKg;

            var rpeNote = hardestRpe > 0 ? $" at RPE {hardestRpe:0.#}" : "";
            return new OverloadSuggestion(OverloadAction.IncreaseLoad, next, referenceWeight,
                $"All {setsAtReference.Count} work sets reached {topOfRepRange} reps{rpeNote}. Try {next:0.##} kg next time.");
        }

        if (allReachedTop)
        {
            return new OverloadSuggestion(OverloadAction.Maintain, referenceWeight, referenceWeight,
                $"Rep target met, but RPE {hardestRpe:0.#} indicates near-maximal effort. Repeat {referenceWeight:0.##} kg to consolidate.");
        }

        var shortfall = setsAtReference.Count(x => x.Reps < topOfRepRange);
        return new OverloadSuggestion(OverloadAction.Maintain, referenceWeight, referenceWeight,
            $"{shortfall} of {setsAtReference.Count} sets fell short of {topOfRepRange} reps. Stay at {referenceWeight:0.##} kg.");
    }
}
