using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Domain;

namespace WorkoutTracker.Infrastructure;

/// <summary>
/// Reference data seeded through migrations: the muscle and equipment taxonomies plus a
/// curated built-in exercise library.
/// <para>
/// Identifiers are derived deterministically from a stable slug, so repeated deployments
/// produce identical keys and never create duplicates (spec 12.1). All content is written
/// for this project; no third-party exercise media or copyrighted descriptions are used.
/// </para>
/// </summary>
internal static class SeedData
{
    public static void Apply(ModelBuilder builder)
    {
        builder.Entity<Muscle>().HasData(Muscles);
        builder.Entity<Equipment>().HasData(Equipment);
        builder.Entity<Exercise>().HasData(Exercises);
        builder.Entity<ExerciseMuscle>().HasData(ExerciseMuscles);
    }

    /// <summary>
    /// Produces a stable GUID from a namespace and key. MD5 is used purely as a
    /// deterministic key-derivation step for seed identifiers, never for security.
    /// </summary>
    private static Guid StableId(string scope, string key)
    {
        var bytes = MD5.HashData(Encoding.UTF8.GetBytes($"workouttracker:{scope}:{key}"));
        return new Guid(bytes);
    }

    private static Guid MuscleId(string slug) => StableId("muscle", slug);
    private static Guid EquipmentId(string slug) => StableId("equipment", slug);
    private static Guid ExerciseId(string slug) => StableId("exercise", slug);

    // =================================================================================
    // Taxonomy
    // =================================================================================

    private static readonly Muscle[] Muscles =
    [
        Muscle("chest", "Chest", "Chest"),
        Muscle("upper-back", "Upper Back", "Back"),
        Muscle("lats", "Lats", "Back"),
        Muscle("lower-back", "Lower Back", "Back"),
        Muscle("traps", "Trapezius", "Back"),
        Muscle("front-delts", "Front Deltoids", "Shoulders"),
        Muscle("side-delts", "Side Deltoids", "Shoulders"),
        Muscle("rear-delts", "Rear Deltoids", "Shoulders"),
        Muscle("biceps", "Biceps", "Arms"),
        Muscle("triceps", "Triceps", "Arms"),
        Muscle("forearms", "Forearms", "Arms"),
        Muscle("abs", "Abdominals", "Core"),
        Muscle("obliques", "Obliques", "Core"),
        Muscle("quads", "Quadriceps", "Legs"),
        Muscle("hamstrings", "Hamstrings", "Legs"),
        Muscle("glutes", "Glutes", "Legs"),
        Muscle("calves", "Calves", "Legs"),
        Muscle("adductors", "Adductors", "Legs"),
        Muscle("abductors", "Abductors", "Legs"),
        Muscle("cardio", "Cardiovascular", "Full Body")
    ];

    private static Muscle Muscle(string slug, string name, string region) =>
        new() { Id = MuscleId(slug), Slug = slug, Name = name, BodyRegion = region };

    private static readonly Equipment[] Equipment =
    [
        Kit("barbell", "Barbell", 20m),
        Kit("ez-bar", "EZ Bar", 10m),
        Kit("trap-bar", "Trap Bar", 25m),
        Kit("smith-machine", "Smith Machine", 20m),
        Kit("dumbbell", "Dumbbell", null),
        Kit("kettlebell", "Kettlebell", null),
        Kit("machine", "Machine", null),
        Kit("cable", "Cable", null),
        Kit("bodyweight", "Bodyweight", null),
        Kit("resistance-band", "Resistance Band", null),
        Kit("plate", "Weight Plate", null),
        Kit("cardio-machine", "Cardio Machine", null)
    ];

    private static Equipment Kit(string slug, string name, decimal? barWeight) =>
        new() { Id = EquipmentId(slug), Slug = slug, Name = name, DefaultBarWeightKg = barWeight };

    // =================================================================================
    // Exercise library
    // =================================================================================

    /// <summary>
    /// Compact definition of one built-in exercise before it is expanded into entities.
    /// </summary>
    private sealed record Seed(
        string Slug,
        string Name,
        string Equipment,
        string Category,
        string[] Primary,
        string[] Secondary,
        string Instructions,
        ExerciseType Type = ExerciseType.WeightAndReps,
        int Rest = 90,
        decimal Increment = 2.5m);

    private static readonly Seed[] Library =
    [
        // ----- Chest / push -----
        new("bench-press", "Bench Press", "barbell", "Push",
            ["chest"], ["front-delts", "triceps"],
            "Set your shoulder blades back and down, keep your feet planted, lower the bar to the mid chest under control, then press back to full extension.",
            Rest: 150),

        new("incline-bench-press", "Incline Bench Press", "barbell", "Push",
            ["chest"], ["front-delts", "triceps"],
            "Set the bench to roughly 30 degrees. Lower the bar to the upper chest and press without letting the elbows flare excessively.",
            Rest: 120),

        new("dumbbell-bench-press", "Dumbbell Bench Press", "dumbbell", "Push",
            ["chest"], ["front-delts", "triceps"],
            "Press both dumbbells from chest level to arms extended, keeping the forearms vertical throughout.",
            Increment: 2m),

        new("incline-dumbbell-press", "Incline Dumbbell Press", "dumbbell", "Push",
            ["chest"], ["front-delts", "triceps"],
            "Use a low incline and press with the forearms vertical, stopping just short of the dumbbells touching.",
            Increment: 2m),

        new("dumbbell-fly", "Dumbbell Fly", "dumbbell", "Push",
            ["chest"], ["front-delts"],
            "Keep a soft elbow bend and open the arms in a wide arc until you feel a stretch across the chest, then squeeze back together.",
            Rest: 60, Increment: 2m),

        new("cable-crossover", "Cable Crossover", "cable", "Push",
            ["chest"], ["front-delts"],
            "Step forward slightly, keep a small elbow bend and bring the handles together in front of the chest.",
            Rest: 60),

        new("push-up", "Push Up", "bodyweight", "Push",
            ["chest"], ["front-delts", "triceps", "abs"],
            "Keep a straight line from head to heels, lower until the chest is just above the floor, then press away.",
            ExerciseType.BodyweightReps, Rest: 60),

        new("chest-press-machine", "Chest Press Machine", "machine", "Push",
            ["chest"], ["front-delts", "triceps"],
            "Align the handles with the mid chest and press without letting the shoulders roll forward.",
            Rest: 90),

        new("dip", "Dip", "bodyweight", "Push",
            ["chest", "triceps"], ["front-delts"],
            "Lean slightly forward for chest emphasis or stay upright for triceps. Lower until the upper arms are roughly parallel to the floor.",
            ExerciseType.WeightedBodyweight, Rest: 120),

        // ----- Shoulders -----
        new("overhead-press", "Overhead Press", "barbell", "Push",
            ["front-delts"], ["side-delts", "triceps", "abs"],
            "Brace your core and glutes, press the bar overhead in a straight line and finish with the bar over the mid foot.",
            Rest: 150),

        new("dumbbell-shoulder-press", "Dumbbell Shoulder Press", "dumbbell", "Push",
            ["front-delts"], ["side-delts", "triceps"],
            "Press from shoulder height to overhead without excessive lower back arch.",
            Increment: 2m),

        new("lateral-raise", "Lateral Raise", "dumbbell", "Push",
            ["side-delts"], ["traps"],
            "Lead with the elbows and raise to about shoulder height. Keep the motion controlled rather than swinging.",
            Rest: 60, Increment: 1m),

        new("rear-delt-fly", "Rear Delt Fly", "dumbbell", "Pull",
            ["rear-delts"], ["upper-back"],
            "Hinge forward, keep a slight elbow bend and open the arms wide while keeping the neck relaxed.",
            Rest: 60, Increment: 1m),

        new("face-pull", "Face Pull", "cable", "Pull",
            ["rear-delts"], ["upper-back", "traps"],
            "Pull the rope toward the forehead while separating the hands and rotating the shoulders back.",
            Rest: 60),

        new("shrug", "Shrug", "barbell", "Pull",
            ["traps"], ["forearms"],
            "Elevate the shoulders straight up without rolling them, pause briefly at the top and lower under control.",
            Rest: 60),

        // ----- Back / pull -----
        new("deadlift", "Deadlift", "barbell", "Pull",
            ["lower-back", "glutes", "hamstrings"], ["lats", "upper-back", "quads", "forearms"],
            "Set the bar over the mid foot, take the slack out of the bar, then stand up by driving the floor away while keeping the bar close.",
            Rest: 180, Increment: 5m),

        new("romanian-deadlift", "Romanian Deadlift", "barbell", "Pull",
            ["hamstrings", "glutes"], ["lower-back", "forearms"],
            "Push the hips back with a near-straight knee, keep the bar in contact with the legs and stop when the hamstrings limit the range.",
            Rest: 120),

        new("barbell-row", "Barbell Row", "barbell", "Pull",
            ["upper-back", "lats"], ["biceps", "rear-delts", "lower-back"],
            "Hinge to roughly 45 degrees, keep a neutral spine and pull the bar toward the lower ribs.",
            Rest: 120),

        new("dumbbell-row", "Dumbbell Row", "dumbbell", "Pull",
            ["lats", "upper-back"], ["biceps", "rear-delts"],
            "Support yourself with one hand, pull the dumbbell toward the hip and avoid twisting the torso.",
            Increment: 2m),

        new("pull-up", "Pull Up", "bodyweight", "Pull",
            ["lats"], ["upper-back", "biceps", "forearms"],
            "Start from a full hang, drive the elbows down toward the sides and clear the chin above the bar.",
            ExerciseType.WeightedBodyweight, Rest: 120),

        new("chin-up", "Chin Up", "bodyweight", "Pull",
            ["lats", "biceps"], ["upper-back", "forearms"],
            "Use a supinated grip and pull until the chin passes the bar, controlling the descent.",
            ExerciseType.WeightedBodyweight, Rest: 120),

        new("lat-pulldown", "Lat Pulldown", "cable", "Pull",
            ["lats"], ["upper-back", "biceps"],
            "Keep the chest tall and pull the bar to the upper chest, then let it rise under control.",
            Rest: 90),

        new("seated-cable-row", "Seated Cable Row", "cable", "Pull",
            ["upper-back", "lats"], ["biceps", "rear-delts"],
            "Sit tall, pull the handle to the abdomen and avoid rocking the torso for momentum.",
            Rest: 90),

        new("t-bar-row", "T-Bar Row", "barbell", "Pull",
            ["upper-back", "lats"], ["biceps", "lower-back"],
            "Hinge forward and row the handle toward the sternum, keeping the spine neutral.",
            Rest: 120),

        new("back-extension", "Back Extension", "bodyweight", "Pull",
            ["lower-back"], ["glutes", "hamstrings"],
            "Hinge at the hips and extend until the torso lines up with the legs. Avoid hyperextending.",
            ExerciseType.WeightedBodyweight, Rest: 60),

        // ----- Legs -----
        new("back-squat", "Back Squat", "barbell", "Legs",
            ["quads", "glutes"], ["hamstrings", "lower-back", "abs", "adductors"],
            "Brace before each rep, sit down between the hips, keep the whole foot planted and drive up evenly.",
            Rest: 180, Increment: 5m),

        new("front-squat", "Front Squat", "barbell", "Legs",
            ["quads"], ["glutes", "abs", "upper-back"],
            "Keep the elbows high and the torso upright, descending under control.",
            Rest: 150),

        new("leg-press", "Leg Press", "machine", "Legs",
            ["quads"], ["glutes", "hamstrings"],
            "Lower under control without letting the lower back lift from the pad, then press without locking harshly.",
            Rest: 120, Increment: 5m),

        new("bulgarian-split-squat", "Bulgarian Split Squat", "dumbbell", "Legs",
            ["quads", "glutes"], ["hamstrings", "adductors"],
            "Elevate the rear foot, descend straight down and drive through the front foot.",
            Rest: 90, Increment: 2m),

        new("lunge", "Lunge", "dumbbell", "Legs",
            ["quads", "glutes"], ["hamstrings", "calves"],
            "Step forward and lower until the front thigh is roughly parallel, then push back to standing.",
            Rest: 90, Increment: 2m),

        new("leg-extension", "Leg Extension", "machine", "Legs",
            ["quads"], [],
            "Extend the knees fully without slamming the weight, and lower under control.",
            Rest: 60),

        new("leg-curl", "Leg Curl", "machine", "Legs",
            ["hamstrings"], ["calves"],
            "Curl the heels toward the glutes and resist the return.",
            Rest: 60),

        new("hip-thrust", "Hip Thrust", "barbell", "Legs",
            ["glutes"], ["hamstrings", "abs"],
            "Tuck the ribs down, drive the hips up until the torso is level and squeeze at the top.",
            Rest: 120, Increment: 5m),

        new("calf-raise", "Calf Raise", "machine", "Legs",
            ["calves"], [],
            "Rise onto the toes through a full range and lower until you feel a stretch.",
            Rest: 45),

        new("hip-abduction", "Hip Abduction", "machine", "Legs",
            ["abductors"], ["glutes"],
            "Press the knees outward under control and resist on the way back in.",
            Rest: 45),

        // ----- Arms -----
        new("barbell-curl", "Barbell Curl", "barbell", "Arms",
            ["biceps"], ["forearms"],
            "Keep the elbows pinned at the sides and avoid swinging the torso.",
            Rest: 60, Increment: 1.25m),

        new("dumbbell-curl", "Dumbbell Curl", "dumbbell", "Arms",
            ["biceps"], ["forearms"],
            "Curl without moving the elbows forward, and control the lowering phase.",
            Rest: 60, Increment: 1m),

        new("hammer-curl", "Hammer Curl", "dumbbell", "Arms",
            ["biceps", "forearms"], [],
            "Hold a neutral grip throughout and keep the wrists straight.",
            Rest: 60, Increment: 1m),

        new("preacher-curl", "Preacher Curl", "ez-bar", "Arms",
            ["biceps"], ["forearms"],
            "Keep the upper arms flat on the pad and avoid letting the elbows drift.",
            Rest: 60, Increment: 1.25m),

        new("triceps-pushdown", "Triceps Pushdown", "cable", "Arms",
            ["triceps"], [],
            "Pin the elbows to the sides and extend fully without leaning over the cable.",
            Rest: 60),

        new("overhead-triceps-extension", "Overhead Triceps Extension", "dumbbell", "Arms",
            ["triceps"], [],
            "Keep the elbows close to the head and lower until you feel a stretch, then extend.",
            Rest: 60, Increment: 2m),

        new("skullcrusher", "Skullcrusher", "ez-bar", "Arms",
            ["triceps"], [],
            "Lower the bar toward the forehead with the upper arms nearly vertical, then extend.",
            Rest: 60, Increment: 1.25m),

        new("close-grip-bench-press", "Close Grip Bench Press", "barbell", "Arms",
            ["triceps"], ["chest", "front-delts"],
            "Use a shoulder-width grip, keep the elbows tucked and press to full extension.",
            Rest: 120),

        new("wrist-curl", "Wrist Curl", "dumbbell", "Arms",
            ["forearms"], [],
            "Let the wrists extend fully, then curl the weight using the forearms only.",
            Rest: 45, Increment: 1m),

        // ----- Core -----
        new("plank", "Plank", "bodyweight", "Core",
            ["abs"], ["obliques", "front-delts"],
            "Hold a straight line from head to heels, squeezing the glutes and bracing the abs.",
            ExerciseType.Duration, Rest: 60),

        new("hanging-leg-raise", "Hanging Leg Raise", "bodyweight", "Core",
            ["abs"], ["obliques", "forearms"],
            "Hang from a bar and raise the legs without swinging, curling the pelvis at the top.",
            ExerciseType.WeightedBodyweight, Rest: 60),

        new("cable-crunch", "Cable Crunch", "cable", "Core",
            ["abs"], ["obliques"],
            "Kneel below the cable and flex the spine to bring the elbows toward the knees.",
            Rest: 60),

        new("russian-twist", "Russian Twist", "plate", "Core",
            ["obliques"], ["abs"],
            "Rotate the torso side to side under control, keeping the chest tall.",
            Rest: 45, Increment: 1.25m),

        new("ab-wheel-rollout", "Ab Wheel Rollout", "bodyweight", "Core",
            ["abs"], ["obliques", "lats"],
            "Roll out only as far as you can keep the lower back from arching, then pull back.",
            ExerciseType.BodyweightReps, Rest: 60),

        // ----- Full body / cardio -----
        new("kettlebell-swing", "Kettlebell Swing", "kettlebell", "Full Body",
            ["glutes", "hamstrings"], ["lower-back", "abs", "front-delts"],
            "Hinge at the hips and snap them forward to float the bell to chest height.",
            Rest: 60),

        new("clean-and-press", "Clean and Press", "barbell", "Full Body",
            ["front-delts", "quads"], ["traps", "glutes", "triceps", "lower-back"],
            "Pull the bar from the floor to the shoulders, then press overhead in one controlled sequence.",
            Rest: 180, Increment: 5m),

        new("farmers-walk", "Farmer's Walk", "dumbbell", "Full Body",
            ["forearms", "traps"], ["abs", "glutes"],
            "Carry a heavy load at the sides with the chest tall and ribs down.",
            ExerciseType.Duration, Rest: 90, Increment: 2m),

        new("treadmill-run", "Treadmill Run", "cardio-machine", "Cardio",
            ["cardio"], ["quads", "hamstrings", "calves"],
            "Maintain a steady effort. Record duration and distance rather than load.",
            ExerciseType.Cardio, Rest: 0),

        new("stationary-bike", "Stationary Bike", "cardio-machine", "Cardio",
            ["cardio"], ["quads", "calves"],
            "Keep a consistent cadence and record duration and distance.",
            ExerciseType.Cardio, Rest: 0),

        new("rowing-machine", "Rowing Machine", "cardio-machine", "Cardio",
            ["cardio"], ["upper-back", "lats", "quads", "glutes"],
            "Drive with the legs, then the back, then the arms, and reverse the order on the recovery.",
            ExerciseType.Cardio, Rest: 0),

        new("jump-rope", "Jump Rope", "bodyweight", "Cardio",
            ["cardio"], ["calves"],
            "Stay light on the balls of the feet with relaxed shoulders. Record duration.",
            ExerciseType.Duration, Rest: 0)
    ];

    private static readonly Exercise[] Exercises = Library
        .Select(seed => new Exercise
        {
            Id = ExerciseId(seed.Slug),
            // Null owner marks a built-in catalog entry visible to every user.
            OwnerId = null,
            Name = seed.Name,
            Instructions = seed.Instructions,
            Type = seed.Type,
            Category = seed.Category,
            EquipmentId = EquipmentId(seed.Equipment),
            MediaUrl = null,
            DefaultRestSeconds = seed.Rest,
            DefaultIncrementKg = seed.Increment,
            IsArchived = false,
            // Fixed timestamp keeps the seed deterministic across migrations.
            CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
        })
        .ToArray();

    private static readonly ExerciseMuscle[] ExerciseMuscles = Library
        .SelectMany(seed => seed.Primary
            .Select(muscle => Link(seed.Slug, muscle, MuscleRole.Primary, 1.0m))
            .Concat(seed.Secondary
                .Select(muscle => Link(seed.Slug, muscle, MuscleRole.Secondary, 0.4m))))
        .ToArray();

    private static ExerciseMuscle Link(string exerciseSlug, string muscleSlug, MuscleRole role, decimal contribution) =>
        new()
        {
            Id = StableId("exercise-muscle", $"{exerciseSlug}:{muscleSlug}"),
            ExerciseId = ExerciseId(exerciseSlug),
            MuscleId = MuscleId(muscleSlug),
            Role = role,
            ContributionWeight = contribution
        };
}
