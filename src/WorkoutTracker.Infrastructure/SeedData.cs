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
        Kit("cardio-machine", "Cardio Machine", null),
        Kit("landmine", "Landmine", null),
        Kit("suspension-trainer", "Suspension Trainer", null),
        Kit("medicine-ball", "Medicine Ball", null),
        Kit("stability-ball", "Stability Ball", null),
        Kit("sled", "Sled", null),
        Kit("battle-rope", "Battle Rope", null),
        Kit("gymnastic-rings", "Gymnastic Rings", null),
        Kit("plyo-box", "Plyometric Box", null)
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
            ExerciseType.Duration, Rest: 0),

        // ----- Expanded chest -----
        new("decline-bench-press", "Decline Bench Press", "barbell", "Push", ["chest"], ["triceps", "front-delts"],
            "Secure the legs, lower the bar to the lower chest and press over the shoulders.", Rest: 120),
        new("decline-dumbbell-press", "Decline Dumbbell Press", "dumbbell", "Push", ["chest"], ["triceps", "front-delts"],
            "Keep the shoulder blades set and press the dumbbells from lower-chest level.", Rest: 90, Increment: 2m),
        new("smith-machine-bench-press", "Smith Machine Bench Press", "smith-machine", "Push", ["chest"], ["triceps", "front-delts"],
            "Position the bench so the bar reaches mid chest, then press without losing upper-back tension.", Rest: 120),
        new("smith-machine-incline-press", "Smith Machine Incline Press", "smith-machine", "Push", ["chest"], ["front-delts", "triceps"],
            "Use a low incline and lower the bar toward the upper chest with controlled elbows.", Rest: 120),
        new("pec-deck-fly", "Pec Deck Fly", "machine", "Push", ["chest"], ["front-delts"],
            "Keep the chest against the pad and bring the arms together without shrugging.", Rest: 60),
        new("machine-incline-press", "Machine Incline Press", "machine", "Push", ["chest"], ["front-delts", "triceps"],
            "Set the seat for an upper-chest press path and control both ends of every rep.", Rest: 90),
        new("cable-fly-low-to-high", "Low to High Cable Fly", "cable", "Push", ["chest"], ["front-delts"],
            "Sweep the handles upward and inward while keeping a soft bend in the elbows.", Rest: 60),
        new("cable-fly-high-to-low", "High to Low Cable Fly", "cable", "Push", ["chest"], ["front-delts"],
            "Sweep the handles down and together without rounding the shoulders.", Rest: 60),
        new("single-arm-cable-press", "Single Arm Cable Chest Press", "cable", "Push", ["chest"], ["triceps", "abs"],
            "Brace against rotation and press one handle forward from chest height.", Rest: 60),
        new("dumbbell-pullover", "Dumbbell Pullover", "dumbbell", "Push", ["chest", "lats"], ["triceps"],
            "Lower the dumbbell behind the head with bent elbows, then pull it back over the chest.", Rest: 60, Increment: 2m),
        new("incline-push-up", "Incline Push Up", "bodyweight", "Push", ["chest"], ["triceps", "front-delts"],
            "Place the hands on a raised surface and keep the body rigid through each rep.", ExerciseType.BodyweightReps, Rest: 45),
        new("decline-push-up", "Decline Push Up", "bodyweight", "Push", ["chest"], ["front-delts", "triceps"],
            "Elevate the feet and lower the chest between the hands while keeping the trunk braced.", ExerciseType.BodyweightReps, Rest: 60),
        new("diamond-push-up", "Diamond Push Up", "bodyweight", "Push", ["triceps", "chest"], ["front-delts"],
            "Keep the hands close, elbows controlled and body straight as you press.", ExerciseType.BodyweightReps, Rest: 60),

        // ----- Expanded shoulders -----
        new("arnold-press", "Arnold Press", "dumbbell", "Push", ["front-delts", "side-delts"], ["triceps"],
            "Rotate from palms-in at shoulder height to palms-forward as you press overhead.", Rest: 90, Increment: 2m),
        new("machine-shoulder-press", "Machine Shoulder Press", "machine", "Push", ["front-delts", "side-delts"], ["triceps"],
            "Adjust the seat so the handles begin near shoulder height and press smoothly overhead.", Rest: 90),
        new("smith-machine-shoulder-press", "Smith Machine Shoulder Press", "smith-machine", "Push", ["front-delts"], ["side-delts", "triceps"],
            "Set the bench upright and press the bar overhead without excessive back arch.", Rest: 120),
        new("landmine-press", "Half Kneeling Landmine Press", "landmine", "Push", ["front-delts"], ["chest", "triceps", "abs"],
            "Brace in a half-kneeling stance and press the bar up and forward.", Rest: 75),
        new("cable-lateral-raise", "Cable Lateral Raise", "cable", "Push", ["side-delts"], ["traps"],
            "Raise the arm out to shoulder height while keeping tension on the cable.", Rest: 45, Increment: 1m),
        new("machine-lateral-raise", "Machine Lateral Raise", "machine", "Push", ["side-delts"], ["traps"],
            "Drive the pads outward with the elbows and lower slowly.", Rest: 45),
        new("dumbbell-front-raise", "Dumbbell Front Raise", "dumbbell", "Push", ["front-delts"], ["chest"],
            "Raise the weights to shoulder height without leaning back or swinging.", Rest: 45, Increment: 1m),
        new("cable-front-raise", "Cable Front Raise", "cable", "Push", ["front-delts"], [],
            "Lift the handle forward to shoulder height while keeping the ribs down.", Rest: 45),
        new("upright-row", "Upright Row", "ez-bar", "Pull", ["side-delts", "traps"], ["biceps"],
            "Pull the bar toward the upper chest with elbows leading and stop at a comfortable height.", Rest: 60, Increment: 1.25m),
        new("reverse-pec-deck", "Reverse Pec Deck", "machine", "Pull", ["rear-delts"], ["upper-back", "traps"],
            "Keep the chest on the pad and sweep the arms back without shrugging.", Rest: 60),
        new("pike-push-up", "Pike Push Up", "bodyweight", "Push", ["front-delts"], ["triceps", "chest"],
            "Keep the hips high and lower the head between the hands before pressing away.", ExerciseType.BodyweightReps, Rest: 60),
        new("handstand-push-up", "Handstand Push Up", "bodyweight", "Push", ["front-delts"], ["triceps", "traps"],
            "Use a stable wall setup, lower under control and press without losing trunk tension.", ExerciseType.BodyweightReps, Rest: 120),

        // ----- Expanded back and posterior chain -----
        new("sumo-deadlift", "Sumo Deadlift", "barbell", "Pull", ["glutes", "adductors", "quads"], ["hamstrings", "lower-back", "traps"],
            "Take a wide stance, brace and push the floor apart while keeping the bar close.", Rest: 180, Increment: 5m),
        new("trap-bar-deadlift", "Trap Bar Deadlift", "trap-bar", "Pull", ["glutes", "quads"], ["hamstrings", "lower-back", "traps"],
            "Stand centered in the frame, brace and drive through the floor to stand tall.", Rest: 180, Increment: 5m),
        new("rack-pull", "Rack Pull", "barbell", "Pull", ["lower-back", "glutes"], ["traps", "hamstrings", "forearms"],
            "Start below the knees, brace hard and extend the hips while keeping the bar close.", Rest: 150, Increment: 5m),
        new("good-morning", "Good Morning", "barbell", "Pull", ["hamstrings", "lower-back"], ["glutes"],
            "Keep a soft knee bend and hinge the hips back while the spine stays neutral.", Rest: 90),
        new("chest-supported-dumbbell-row", "Chest Supported Dumbbell Row", "dumbbell", "Pull", ["upper-back", "lats"], ["biceps", "rear-delts"],
            "Lie chest-down on an incline bench and row toward the lower ribs.", Rest: 75, Increment: 2m),
        new("machine-row", "Seated Row Machine", "machine", "Pull", ["upper-back", "lats"], ["biceps", "rear-delts"],
            "Keep the chest supported and pull the handles toward the ribs without shrugging.", Rest: 75),
        new("iso-lateral-row", "Iso-Lateral Row Machine", "machine", "Pull", ["lats", "upper-back"], ["biceps", "rear-delts"],
            "Pull each handle toward the hip while keeping the torso against the pad.", Rest: 75),
        new("single-arm-cable-row", "Single Arm Cable Row", "cable", "Pull", ["lats", "upper-back"], ["biceps", "abs"],
            "Row one handle toward the hip and resist rotating through the torso.", Rest: 60),
        new("straight-arm-pulldown", "Straight Arm Pulldown", "cable", "Pull", ["lats"], ["triceps"],
            "Keep the arms nearly straight and sweep the bar from shoulder height to the thighs.", Rest: 60),
        new("close-grip-lat-pulldown", "Close Grip Lat Pulldown", "cable", "Pull", ["lats"], ["biceps", "upper-back"],
            "Pull the neutral handle toward the upper chest while keeping the torso tall.", Rest: 75),
        new("single-arm-lat-pulldown", "Single Arm Lat Pulldown", "cable", "Pull", ["lats"], ["biceps"],
            "Drive one elbow down toward the hip and control the full return.", Rest: 60),
        new("assisted-pull-up", "Assisted Pull Up", "machine", "Pull", ["lats"], ["biceps", "upper-back"],
            "Use enough assistance for a full range and control the platform on every rep.", Rest: 90),
        new("inverted-row", "Inverted Row", "bodyweight", "Pull", ["upper-back", "lats"], ["biceps", "rear-delts"],
            "Keep the body rigid and pull the chest to the bar or handles.", ExerciseType.BodyweightReps, Rest: 75),
        new("ring-row", "Ring Row", "gymnastic-rings", "Pull", ["upper-back", "lats"], ["biceps", "rear-delts", "abs"],
            "Keep the rings steady and pull the chest between them with a rigid body line.", ExerciseType.BodyweightReps, Rest: 75),
        new("meadows-row", "Meadows Row", "landmine", "Pull", ["lats", "upper-back"], ["biceps", "rear-delts"],
            "Stand across the bar and row the sleeve toward the hip with minimal torso rotation.", Rest: 75),
        new("seal-row", "Seal Row", "barbell", "Pull", ["upper-back", "lats"], ["biceps", "rear-delts"],
            "Lie prone on a raised bench and row the bar to the underside of the bench.", Rest: 90),
        new("machine-pullover", "Pullover Machine", "machine", "Pull", ["lats"], ["chest", "triceps"],
            "Drive the pads down in an arc while keeping the ribs controlled.", Rest: 60),
        new("band-pull-apart", "Band Pull Apart", "resistance-band", "Pull", ["rear-delts", "upper-back"], ["traps"],
            "Pull the band across the chest until the arms form a straight line.", ExerciseType.BodyweightReps, Rest: 30),

        // ----- Expanded legs and glutes -----
        new("goblet-squat", "Goblet Squat", "dumbbell", "Legs", ["quads", "glutes"], ["abs", "adductors"],
            "Hold the weight at the chest, sit between the hips and keep the whole foot planted.", Rest: 90, Increment: 2m),
        new("hack-squat", "Hack Squat Machine", "machine", "Legs", ["quads"], ["glutes", "hamstrings"],
            "Keep the back on the pad, descend comfortably and drive through the whole foot.", Rest: 120, Increment: 5m),
        new("pendulum-squat", "Pendulum Squat", "machine", "Legs", ["quads"], ["glutes", "adductors"],
            "Stay against the pad and follow the machine arc through a controlled depth.", Rest: 120, Increment: 5m),
        new("smith-machine-squat", "Smith Machine Squat", "smith-machine", "Legs", ["quads", "glutes"], ["hamstrings", "abs"],
            "Set the feet for balance, brace and squat through a comfortable range.", Rest: 120),
        new("belt-squat", "Belt Squat", "machine", "Legs", ["quads", "glutes"], ["adductors"],
            "Keep the torso tall and squat while the belt loads the hips.", Rest: 120, Increment: 5m),
        new("sissy-squat", "Sissy Squat", "bodyweight", "Legs", ["quads"], ["abs"],
            "Keep the hips extended and bend the knees forward only through a controlled range.", ExerciseType.BodyweightReps, Rest: 60),
        new("step-up", "Dumbbell Step Up", "dumbbell", "Legs", ["quads", "glutes"], ["hamstrings", "calves"],
            "Plant the whole lead foot on the box and stand without pushing off the trailing leg.", Rest: 75, Increment: 2m),
        new("reverse-lunge", "Reverse Lunge", "dumbbell", "Legs", ["quads", "glutes"], ["hamstrings"],
            "Step back, lower under control and drive through the lead foot to return.", Rest: 75, Increment: 2m),
        new("walking-lunge", "Walking Lunge", "dumbbell", "Legs", ["quads", "glutes"], ["hamstrings", "calves"],
            "Take controlled steps and keep the lead knee tracking over the foot.", Rest: 90, Increment: 2m),
        new("lateral-lunge", "Lateral Lunge", "dumbbell", "Legs", ["adductors", "glutes"], ["quads", "hamstrings"],
            "Step wide, sit into one hip and keep the opposite leg long.", Rest: 75, Increment: 2m),
        new("single-leg-squat", "Pistol Squat", "bodyweight", "Legs", ["quads", "glutes"], ["abs", "calves"],
            "Balance on one leg, descend with control and use assistance when needed.", ExerciseType.BodyweightReps, Rest: 90),
        new("dumbbell-romanian-deadlift", "Dumbbell Romanian Deadlift", "dumbbell", "Legs", ["hamstrings", "glutes"], ["lower-back", "forearms"],
            "Push the hips back and keep the dumbbells close to the legs.", Rest: 90, Increment: 2m),
        new("single-leg-romanian-deadlift", "Single Leg Romanian Deadlift", "dumbbell", "Legs", ["hamstrings", "glutes"], ["abs", "lower-back"],
            "Hinge over one leg with square hips and return by driving the stance hip forward.", Rest: 75, Increment: 2m),
        new("lying-leg-curl", "Lying Leg Curl", "machine", "Legs", ["hamstrings"], ["calves"],
            "Keep the hips on the pad, curl the heels up and lower slowly.", Rest: 60),
        new("seated-leg-curl", "Seated Leg Curl", "machine", "Legs", ["hamstrings"], ["calves"],
            "Secure the thigh pad, curl through the full range and control the return.", Rest: 60),
        new("nordic-hamstring-curl", "Nordic Hamstring Curl", "bodyweight", "Legs", ["hamstrings"], ["glutes", "calves"],
            "Keep the hips extended and lower the torso slowly, using the hands only as needed.", ExerciseType.BodyweightReps, Rest: 90),
        new("glute-bridge", "Glute Bridge", "bodyweight", "Legs", ["glutes"], ["hamstrings", "abs"],
            "Drive through the heels and extend the hips without arching the lower back.", ExerciseType.WeightedBodyweight, Rest: 60),
        new("cable-pull-through", "Cable Pull Through", "cable", "Legs", ["glutes", "hamstrings"], ["lower-back"],
            "Hinge away from the stack, then drive the hips through to stand tall.", Rest: 60),
        new("cable-glute-kickback", "Cable Glute Kickback", "cable", "Legs", ["glutes"], ["hamstrings"],
            "Extend one hip behind you without rotating the pelvis or arching the back.", Rest: 45),
        new("hip-adduction", "Hip Adduction Machine", "machine", "Legs", ["adductors"], [],
            "Bring the knees together smoothly and control the return.", Rest: 45),
        new("standing-calf-raise", "Standing Calf Raise", "machine", "Legs", ["calves"], [],
            "Rise through the toes, pause at the top and lower into a full stretch.", Rest: 45),
        new("seated-calf-raise", "Seated Calf Raise", "machine", "Legs", ["calves"], [],
            "Keep the knees under the pads and move through the largest controlled ankle range.", Rest: 45),
        new("donkey-calf-raise", "Donkey Calf Raise", "machine", "Legs", ["calves"], [],
            "Keep the hips hinged while raising and lowering the heels through a full range.", Rest: 45),
        new("tibialis-raise", "Tibialis Raise", "bodyweight", "Legs", ["calves"], [],
            "Keep the heels planted and lift the toes toward the shins under control.", ExerciseType.BodyweightReps, Rest: 30),

        // ----- Expanded arms -----
        new("incline-dumbbell-curl", "Incline Dumbbell Curl", "dumbbell", "Arms", ["biceps"], ["forearms"],
            "Keep the upper arms behind the torso and curl without moving the shoulders.", Rest: 60, Increment: 1m),
        new("concentration-curl", "Concentration Curl", "dumbbell", "Arms", ["biceps"], ["forearms"],
            "Brace the upper arm against the thigh and curl through a controlled range.", Rest: 45, Increment: 1m),
        new("spider-curl", "Spider Curl", "ez-bar", "Arms", ["biceps"], ["forearms"],
            "Lie chest-down on an incline and curl while the upper arms hang vertical.", Rest: 60, Increment: 1.25m),
        new("cable-curl", "Cable Curl", "cable", "Arms", ["biceps"], ["forearms"],
            "Keep the elbows still and curl against continuous cable tension.", Rest: 60),
        new("bayesian-cable-curl", "Bayesian Cable Curl", "cable", "Arms", ["biceps"], ["forearms"],
            "Face away from the stack and curl with the working arm slightly behind the torso.", Rest: 45),
        new("reverse-curl", "Reverse Curl", "ez-bar", "Arms", ["forearms"], ["biceps"],
            "Use an overhand grip, keep the wrists straight and curl without swinging.", Rest: 60, Increment: 1.25m),
        new("machine-biceps-curl", "Machine Biceps Curl", "machine", "Arms", ["biceps"], ["forearms"],
            "Align the elbows with the machine pivot and curl without lifting from the pad.", Rest: 60),
        new("rope-hammer-curl", "Rope Hammer Curl", "cable", "Arms", ["biceps", "forearms"], [],
            "Curl the rope with neutral wrists and separate the ends slightly at the top.", Rest: 60),
        new("bench-dip", "Bench Dip", "bodyweight", "Arms", ["triceps"], ["chest", "front-delts"],
            "Keep the shoulders down and lower only through a comfortable range.", ExerciseType.WeightedBodyweight, Rest: 60),
        new("cable-overhead-triceps-extension", "Cable Overhead Triceps Extension", "cable", "Arms", ["triceps"], [],
            "Keep the elbows forward and extend the arms without moving the upper arms.", Rest: 60),
        new("single-arm-triceps-pushdown", "Single Arm Triceps Pushdown", "cable", "Arms", ["triceps"], [],
            "Keep the elbow at the side and extend the handle to full lockout.", Rest: 45),
        new("rope-triceps-pushdown", "Rope Triceps Pushdown", "cable", "Arms", ["triceps"], [],
            "Extend the elbows and separate the rope ends without letting the shoulders roll forward.", Rest: 60),
        new("dumbbell-triceps-kickback", "Dumbbell Triceps Kickback", "dumbbell", "Arms", ["triceps"], [],
            "Hold the upper arm still and extend the elbow until the arm is straight.", Rest: 45, Increment: 1m),
        new("machine-triceps-extension", "Machine Triceps Extension", "machine", "Arms", ["triceps"], [],
            "Align the elbows with the pivot and extend without lifting from the pad.", Rest: 60),
        new("jm-press", "JM Press", "barbell", "Arms", ["triceps"], ["chest", "front-delts"],
            "Lower the bar toward the upper chest by bending the elbows, then extend smoothly.", Rest: 90),
        new("reverse-wrist-curl", "Reverse Wrist Curl", "dumbbell", "Arms", ["forearms"], [],
            "Support the forearms and lift the backs of the hands through a controlled range.", Rest: 45, Increment: 1m),
        new("plate-pinch", "Plate Pinch Hold", "plate", "Arms", ["forearms"], ["traps"],
            "Pinch smooth plates at the sides and hold them with tall posture.", ExerciseType.Duration, Rest: 60),

        // ----- Expanded core -----
        new("crunch", "Crunch", "bodyweight", "Core", ["abs"], [],
            "Curl the ribs toward the pelvis without pulling on the neck.", ExerciseType.BodyweightReps, Rest: 45),
        new("sit-up", "Sit Up", "bodyweight", "Core", ["abs"], ["obliques"],
            "Brace the feet as needed and roll the torso up under control.", ExerciseType.BodyweightReps, Rest: 45),
        new("reverse-crunch", "Reverse Crunch", "bodyweight", "Core", ["abs"], ["obliques"],
            "Curl the pelvis toward the ribs without swinging the legs.", ExerciseType.BodyweightReps, Rest: 45),
        new("bicycle-crunch", "Bicycle Crunch", "bodyweight", "Core", ["abs", "obliques"], [],
            "Rotate shoulder toward opposite knee while extending the other leg slowly.", ExerciseType.BodyweightReps, Rest: 45),
        new("dead-bug", "Dead Bug", "bodyweight", "Core", ["abs"], ["obliques"],
            "Keep the lower back gently pressed down while extending opposite arm and leg.", ExerciseType.BodyweightReps, Rest: 45),
        new("bird-dog", "Bird Dog", "bodyweight", "Core", ["abs", "lower-back"], ["glutes"],
            "Reach opposite arm and leg long without rotating the hips.", ExerciseType.BodyweightReps, Rest: 45),
        new("side-plank", "Side Plank", "bodyweight", "Core", ["obliques"], ["abs", "side-delts"],
            "Stack the body in a straight line and keep the hips lifted.", ExerciseType.Duration, Rest: 45),
        new("pallof-press", "Pallof Press", "cable", "Core", ["obliques", "abs"], [],
            "Press the handle away from the chest while resisting rotation.", Rest: 45),
        new("cable-wood-chop", "Cable Wood Chop", "cable", "Core", ["obliques"], ["abs"],
            "Rotate through the torso and hips while moving the handle diagonally.", Rest: 45),
        new("mountain-climber", "Mountain Climber", "bodyweight", "Core", ["abs"], ["front-delts", "cardio"],
            "Hold a strong plank and alternate driving the knees forward.", ExerciseType.Duration, Rest: 30),
        new("v-up", "V Up", "bodyweight", "Core", ["abs"], ["obliques"],
            "Lift the legs and torso together, reaching toward the feet without swinging.", ExerciseType.BodyweightReps, Rest: 45),
        new("stability-ball-crunch", "Stability Ball Crunch", "stability-ball", "Core", ["abs"], ["obliques"],
            "Support the lower back on the ball and curl the ribs toward the pelvis.", ExerciseType.BodyweightReps, Rest: 45),
        new("stability-ball-rollout", "Stability Ball Rollout", "stability-ball", "Core", ["abs"], ["lats", "front-delts"],
            "Roll the forearms forward while maintaining a braced trunk, then pull back.", ExerciseType.BodyweightReps, Rest: 60),
        new("suitcase-carry", "Suitcase Carry", "dumbbell", "Core", ["obliques", "forearms"], ["traps", "glutes"],
            "Carry one heavy weight without leaning and switch sides evenly.", ExerciseType.Duration, Rest: 60, Increment: 2m),

        // ----- Power, conditioning and free movement -----
        new("power-clean", "Power Clean", "barbell", "Full Body", ["glutes", "quads", "traps"], ["hamstrings", "front-delts", "abs"],
            "Drive from the floor, extend rapidly and receive the bar on the shoulders in a partial squat.", Rest: 180, Increment: 5m),
        new("hang-clean", "Hang Clean", "barbell", "Full Body", ["glutes", "traps", "quads"], ["hamstrings", "front-delts"],
            "Start above the knees, extend powerfully and receive the bar with stable feet.", Rest: 150, Increment: 5m),
        new("barbell-snatch", "Barbell Snatch", "barbell", "Full Body", ["glutes", "quads", "traps"], ["hamstrings", "front-delts", "abs"],
            "Accelerate the bar from the floor and receive it overhead with locked arms.", Rest: 180, Increment: 2.5m),
        new("push-press", "Push Press", "barbell", "Full Body", ["front-delts", "quads"], ["triceps", "glutes", "abs"],
            "Dip straight down, drive with the legs and finish the bar overhead.", Rest: 120),
        new("barbell-thruster", "Barbell Thruster", "barbell", "Full Body", ["quads", "front-delts"], ["glutes", "triceps", "abs"],
            "Rise from a front squat and carry the momentum into an overhead press.", Rest: 120),
        new("dumbbell-thruster", "Dumbbell Thruster", "dumbbell", "Full Body", ["quads", "front-delts"], ["glutes", "triceps", "abs"],
            "Stand from the squat and press both dumbbells overhead in one fluid motion.", Rest: 90, Increment: 2m),
        new("turkish-get-up", "Turkish Get Up", "kettlebell", "Full Body", ["abs", "front-delts"], ["glutes", "quads", "triceps"],
            "Keep the weight stacked overhead while moving deliberately from the floor to standing and back.", Rest: 90),
        new("kettlebell-clean", "Kettlebell Clean", "kettlebell", "Full Body", ["glutes", "hamstrings"], ["traps", "biceps", "abs"],
            "Drive the hips and guide the bell softly into the rack position.", Rest: 75),
        new("kettlebell-snatch", "Kettlebell Snatch", "kettlebell", "Full Body", ["glutes", "front-delts"], ["hamstrings", "traps", "abs"],
            "Drive the bell from the hinge and punch the hand through to a stable overhead finish.", Rest: 90),
        new("burpee", "Burpee", "bodyweight", "Conditioning", ["cardio"], ["chest", "quads", "glutes", "abs"],
            "Move from standing to a plank and back, adding a jump while maintaining control.", ExerciseType.BodyweightReps, Rest: 60),
        new("bear-crawl", "Bear Crawl", "bodyweight", "Conditioning", ["abs", "front-delts"], ["quads", "glutes", "cardio"],
            "Keep the knees low and crawl with opposite hand and foot moving together.", ExerciseType.Duration, Rest: 45),
        new("sled-push", "Sled Push", "sled", "Conditioning", ["quads", "glutes"], ["calves", "cardio", "front-delts"],
            "Brace into the handles and drive the ground backward with short powerful steps.", ExerciseType.Cardio, Rest: 90),
        new("sled-pull", "Backward Sled Drag", "sled", "Conditioning", ["quads"], ["calves", "cardio"],
            "Walk backward with steady steps and keep continuous tension on the straps or handles.", ExerciseType.Cardio, Rest: 90),
        new("battle-rope-waves", "Battle Rope Waves", "battle-rope", "Conditioning", ["cardio", "front-delts"], ["abs", "forearms"],
            "Maintain an athletic stance and alternate quick waves without losing trunk position.", ExerciseType.Duration, Rest: 60),
        new("medicine-ball-slam", "Medicine Ball Slam", "medicine-ball", "Conditioning", ["abs", "lats"], ["front-delts", "cardio"],
            "Reach tall, brace and drive the ball forcefully to the floor using the whole trunk.", ExerciseType.BodyweightReps, Rest: 60),
        new("wall-ball", "Wall Ball", "medicine-ball", "Conditioning", ["quads", "front-delts"], ["glutes", "triceps", "cardio"],
            "Stand from the squat and throw the ball to the target in one continuous motion.", ExerciseType.BodyweightReps, Rest: 60),
        new("box-jump", "Box Jump", "plyo-box", "Plyometrics", ["quads", "glutes"], ["calves", "cardio"],
            "Jump to a stable landing on the box and step down under control.", ExerciseType.BodyweightReps, Rest: 75),
        new("broad-jump", "Broad Jump", "bodyweight", "Plyometrics", ["glutes", "quads"], ["hamstrings", "calves"],
            "Swing the arms, jump forward and land softly with balanced knees and hips.", ExerciseType.BodyweightReps, Rest: 75),
        new("suspension-row", "Suspension Trainer Row", "suspension-trainer", "Pull", ["upper-back", "lats"], ["biceps", "abs"],
            "Keep the body rigid and pull the chest toward the handles.", ExerciseType.BodyweightReps, Rest: 60),
        new("suspension-chest-press", "Suspension Trainer Chest Press", "suspension-trainer", "Push", ["chest"], ["triceps", "front-delts", "abs"],
            "Lean into the straps, lower between the handles and press while keeping them steady.", ExerciseType.BodyweightReps, Rest: 60),
        new("ring-dip", "Ring Dip", "gymnastic-rings", "Push", ["triceps", "chest"], ["front-delts", "abs"],
            "Stabilize the rings close to the body and press from a controlled depth.", ExerciseType.WeightedBodyweight, Rest: 120),

        // ----- Cardio and mobility -----
        new("outdoor-run", "Outdoor Run", "bodyweight", "Cardio", ["cardio"], ["quads", "hamstrings", "calves"],
            "Track time and distance while maintaining a sustainable stride.", ExerciseType.Cardio, Rest: 0),
        new("brisk-walk", "Brisk Walk", "bodyweight", "Cardio", ["cardio"], ["calves", "quads"],
            "Walk at a purposeful pace and track time and distance.", ExerciseType.Cardio, Rest: 0),
        new("stair-climber", "Stair Climber", "cardio-machine", "Cardio", ["cardio", "quads"], ["glutes", "calves"],
            "Use steady steps and avoid leaning heavily on the rails.", ExerciseType.Cardio, Rest: 0),
        new("elliptical", "Elliptical Trainer", "cardio-machine", "Cardio", ["cardio"], ["quads", "glutes"],
            "Maintain a smooth cadence and record duration and distance.", ExerciseType.Cardio, Rest: 0),
        new("air-bike", "Air Bike", "cardio-machine", "Cardio", ["cardio"], ["quads", "front-delts", "triceps"],
            "Push and pull the handles while pedaling at the intended effort.", ExerciseType.Cardio, Rest: 0),
        new("ski-erg", "Ski Erg", "cardio-machine", "Cardio", ["cardio", "lats"], ["abs", "triceps"],
            "Drive the handles down with the trunk and arms, then recover smoothly.", ExerciseType.Cardio, Rest: 0),
        new("swimming", "Swimming", "bodyweight", "Cardio", ["cardio"], ["lats", "front-delts", "quads"],
            "Record pool distance and duration for the chosen stroke.", ExerciseType.Cardio, Rest: 0),
        new("hiking", "Hiking", "bodyweight", "Cardio", ["cardio"], ["quads", "glutes", "calves"],
            "Track trail time and distance, adjusting effort for terrain and elevation.", ExerciseType.Cardio, Rest: 0),
        new("dead-hang", "Dead Hang", "bodyweight", "Mobility", ["forearms", "lats"], ["traps"],
            "Hang with a comfortable shoulder position and steady breathing.", ExerciseType.Duration, Rest: 45),
        new("cat-cow", "Cat Cow", "bodyweight", "Mobility", ["lower-back", "abs"], [],
            "Move the spine slowly between rounded and extended positions with the breath.", ExerciseType.BodyweightReps, Rest: 0),
        new("childs-pose", "Child's Pose", "bodyweight", "Mobility", ["lats", "lower-back"], [],
            "Sit the hips back, reach the arms forward and breathe into the stretch.", ExerciseType.Duration, Rest: 0),
        new("cobra-stretch", "Cobra Stretch", "bodyweight", "Mobility", ["abs"], ["chest"],
            "Press the chest up only as far as comfortable while keeping the hips grounded.", ExerciseType.Duration, Rest: 0),
        new("ninety-ninety-hip-switch", "90/90 Hip Switch", "bodyweight", "Mobility", ["glutes", "adductors", "abductors"], [],
            "Rotate both knees side to side under control while keeping the torso tall.", ExerciseType.BodyweightReps, Rest: 0),
        new("couch-stretch", "Couch Stretch", "bodyweight", "Mobility", ["quads"], ["glutes"],
            "Place the rear shin against support, tuck the pelvis and hold a tall posture.", ExerciseType.Duration, Rest: 0),
        new("thoracic-rotation", "Quadruped Thoracic Rotation", "bodyweight", "Mobility", ["upper-back", "obliques"], [],
            "From hands and knees, rotate one elbow toward the ceiling without shifting the hips.", ExerciseType.BodyweightReps, Rest: 0)
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
