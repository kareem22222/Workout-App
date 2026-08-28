using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WorkoutTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExpandExerciseLibrary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Equipment",
                columns: new[] { "Id", "DefaultBarWeightKg", "Name", "Slug" },
                values: new object[,]
                {
                    { new Guid("134c1c3d-365b-71c5-fa42-e3dfdc967dd3"), null, "Gymnastic Rings", "gymnastic-rings" },
                    { new Guid("2c471d39-7446-8e2e-39f4-82a443032c3e"), null, "Battle Rope", "battle-rope" },
                    { new Guid("5dd58442-ba55-e4a5-d869-a66d601b193f"), null, "Stability Ball", "stability-ball" },
                    { new Guid("8e7b4a1b-c22e-904d-d102-7f61089ad5c3"), null, "Landmine", "landmine" },
                    { new Guid("91328842-7e67-521f-17b2-37ddff0c912e"), null, "Plyometric Box", "plyo-box" },
                    { new Guid("c9f465e3-aee6-83df-cb8e-e94a305112ad"), null, "Medicine Ball", "medicine-ball" },
                    { new Guid("d44984a4-01aa-fa50-7652-f83c8bad469e"), null, "Sled", "sled" },
                    { new Guid("fa6e4468-2e50-8944-d82d-8ae5fa6b0311"), null, "Suspension Trainer", "suspension-trainer" }
                });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "Category", "CreatedAt", "DefaultIncrementKg", "DefaultRestSeconds", "EquipmentId", "Instructions", "IsArchived", "MediaUrl", "Name", "OwnerId", "Type" },
                values: new object[,]
                {
                    { new Guid("023bc5af-3d3c-3701-ec42-66cea60be7d7"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Keep the shoulders down and lower only through a comfortable range.", false, null, "Bench Dip", null, 2 },
                    { new Guid("053ad886-7a89-d64d-8a9e-46381a60bc5c"), "Full Body", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 90, new Guid("e830f954-0176-7c23-cda7-31a84ba93e1c"), "Drive the bell from the hinge and punch the hand through to a stable overhead finish.", false, null, "Kettlebell Snatch", null, 0 },
                    { new Guid("06d5b353-c57f-3a85-7474-95481ed59182"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Drive one elbow down toward the hip and control the full return.", false, null, "Single Arm Lat Pulldown", null, 0 },
                    { new Guid("0caf5595-4ddb-9439-0b1c-26175401654e"), "Mobility", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 0, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Press the chest up only as far as comfortable while keeping the hips grounded.", false, null, "Cobra Stretch", null, 3 },
                    { new Guid("0d154558-9d0e-8138-33d9-536ffb9432e5"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 90, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Use enough assistance for a full range and control the platform on every rep.", false, null, "Assisted Pull Up", null, 0 },
                    { new Guid("0d1f80ff-fcfb-950d-f88d-493fbce1a73c"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Curl the rope with neutral wrists and separate the ends slightly at the top.", false, null, "Rope Hammer Curl", null, 0 },
                    { new Guid("1058b3f7-daaf-44b3-85d3-e2133b4e5cde"), "Mobility", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Hang with a comfortable shoulder position and steady breathing.", false, null, "Dead Hang", null, 3 },
                    { new Guid("105a15ee-60f5-ff7f-20ca-799591c0e169"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 90, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Keep the hips extended and lower the torso slowly, using the hands only as needed.", false, null, "Nordic Hamstring Curl", null, 1 },
                    { new Guid("11b3621a-9cfe-e302-f045-8995c7eda8ba"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5m, 120, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Stay against the pad and follow the machine arc through a controlled depth.", false, null, "Pendulum Squat", null, 0 },
                    { new Guid("153bd9c4-6991-0246-6c44-ed0caedd25a2"), "Full Body", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 120, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Rise from a front squat and carry the momentum into an overhead press.", false, null, "Barbell Thruster", null, 0 },
                    { new Guid("180a7bc9-69b6-dab8-2325-e8f12c917061"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Extend one hip behind you without rotating the pelvis or arching the back.", false, null, "Cable Glute Kickback", null, 0 },
                    { new Guid("1b03d6d3-b2ed-7f2e-6644-2a175efe6568"), "Full Body", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 75, new Guid("e830f954-0176-7c23-cda7-31a84ba93e1c"), "Drive the hips and guide the bell softly into the rack position.", false, null, "Kettlebell Clean", null, 0 },
                    { new Guid("202e04c4-ec37-1e25-32b0-e7ecb09ac090"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 90, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Lie prone on a raised bench and row the bar to the underside of the bench.", false, null, "Seal Row", null, 0 },
                    { new Guid("21310bf6-4257-119b-528d-d76067da068e"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Face away from the stack and curl with the working arm slightly behind the torso.", false, null, "Bayesian Cable Curl", null, 0 },
                    { new Guid("2299a568-2a3e-9a83-40e4-fdea1168a0af"), "Mobility", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 0, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Move the spine slowly between rounded and extended positions with the breath.", false, null, "Cat Cow", null, 1 },
                    { new Guid("23eca012-f379-86c2-bd2a-07e73da37e57"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 75, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Pull each handle toward the hip while keeping the torso against the pad.", false, null, "Iso-Lateral Row Machine", null, 0 },
                    { new Guid("25b9afa3-38cc-9a1e-bd89-4ee08d319413"), "Mobility", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 0, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Place the rear shin against support, tuck the pelvis and hold a tall posture.", false, null, "Couch Stretch", null, 3 },
                    { new Guid("268d8dad-43c7-5e83-24b5-a07387ae8af9"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Align the elbows with the machine pivot and curl without lifting from the pad.", false, null, "Machine Biceps Curl", null, 0 },
                    { new Guid("2863f1c1-b77c-0b4d-b2f8-ee5f451c3b37"), "Core", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Rotate through the torso and hips while moving the handle diagonally.", false, null, "Cable Wood Chop", null, 0 },
                    { new Guid("2a4d81d5-7db5-76fc-aab6-c55801183ca3"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Keep the elbow at the side and extend the handle to full lockout.", false, null, "Single Arm Triceps Pushdown", null, 0 },
                    { new Guid("2d8178ef-e61f-fe39-e3ac-7d2f4a748564"), "Cardio", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 0, new Guid("89cde677-e340-f0fb-a1ac-58aa920f33ea"), "Push and pull the handles while pedaling at the intended effort.", false, null, "Air Bike", null, 4 },
                    { new Guid("2e147828-59fb-4a5d-bf0e-d05b064c45d9"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5m, 120, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Keep the back on the pad, descend comfortably and drive through the whole foot.", false, null, "Hack Squat Machine", null, 0 },
                    { new Guid("2ea83b13-d10a-cbcc-790a-cc421110d46d"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Keep the hands close, elbows controlled and body straight as you press.", false, null, "Diamond Push Up", null, 1 },
                    { new Guid("3017d1b8-ac05-29a5-e2a0-f2f3b1057d74"), "Full Body", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 180, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Accelerate the bar from the floor and receive it overhead with locked arms.", false, null, "Barbell Snatch", null, 0 },
                    { new Guid("38ccd0eb-9476-583c-5e99-25654a59eddb"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Drive through the heels and extend the hips without arching the lower back.", false, null, "Glute Bridge", null, 2 },
                    { new Guid("396a5f31-4114-a312-60bd-96111afbc44a"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1.25m, 60, new Guid("ea46aa2f-9f02-4573-1016-018252ba8ce1"), "Lie chest-down on an incline and curl while the upper arms hang vertical.", false, null, "Spider Curl", null, 0 },
                    { new Guid("3a78770d-d22d-6e7c-f156-92c8e4e4c17c"), "Core", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Curl the ribs toward the pelvis without pulling on the neck.", false, null, "Crunch", null, 1 },
                    { new Guid("3cf3597c-0444-9b71-73d3-8c4f4c888e1a"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Place the hands on a raised surface and keep the body rigid through each rep.", false, null, "Incline Push Up", null, 1 },
                    { new Guid("3eeba987-f873-ba13-c353-c02844ef25f0"), "Full Body", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 120, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Dip straight down, drive with the legs and finish the bar overhead.", false, null, "Push Press", null, 0 },
                    { new Guid("3f3a9eac-002c-68dd-4878-d58fdad2a418"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 75, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Keep the chest supported and pull the handles toward the ribs without shrugging.", false, null, "Seated Row Machine", null, 0 },
                    { new Guid("408f53a9-3b97-90b7-8473-055a5d41a0be"), "Mobility", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 0, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Sit the hips back, reach the arms forward and breathe into the stretch.", false, null, "Child's Pose", null, 3 },
                    { new Guid("41303540-98c4-2902-eca0-5d9cb5ba4ca6"), "Cardio", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 0, new Guid("89cde677-e340-f0fb-a1ac-58aa920f33ea"), "Drive the handles down with the trunk and arms, then recover smoothly.", false, null, "Ski Erg", null, 4 },
                    { new Guid("4193dd34-4d5d-0d43-59fe-203d4f2b55a3"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("ef3dcb2c-113b-c883-1557-dad42e897bae"), "Pinch smooth plates at the sides and hold them with tall posture.", false, null, "Plate Pinch Hold", null, 3 },
                    { new Guid("421b346b-b949-be3b-7f2c-26372211df7d"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Hinge away from the stack, then drive the hips through to stand tall.", false, null, "Cable Pull Through", null, 0 },
                    { new Guid("44c28204-7fc2-435b-7ac2-579def6f21b0"), "Mobility", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 0, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "From hands and knees, rotate one elbow toward the ceiling without shifting the hips.", false, null, "Quadruped Thoracic Rotation", null, 1 },
                    { new Guid("46e9110f-fae3-5225-1f05-cc9d0abf91a1"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 90, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Push the hips back and keep the dumbbells close to the legs.", false, null, "Dumbbell Romanian Deadlift", null, 0 },
                    { new Guid("46ef2958-9adb-24d9-e556-fb1465098406"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Keep the hips extended and bend the knees forward only through a controlled range.", false, null, "Sissy Squat", null, 1 },
                    { new Guid("476cd99b-8358-13e5-8ade-84a1ed12511c"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Keep the elbows forward and extend the arms without moving the upper arms.", false, null, "Cable Overhead Triceps Extension", null, 0 },
                    { new Guid("4b589f5d-dd39-25fc-b56a-264d2f59e0cf"), "Cardio", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 0, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Track time and distance while maintaining a sustainable stride.", false, null, "Outdoor Run", null, 4 },
                    { new Guid("4bc0ad35-644a-ebf0-8b6e-979829f3a10d"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 90, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Rotate from palms-in at shoulder height to palms-forward as you press overhead.", false, null, "Arnold Press", null, 0 },
                    { new Guid("4be295d8-414f-8dcc-b97f-965e8d050343"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Keep the hips high and lower the head between the hands before pressing away.", false, null, "Pike Push Up", null, 1 },
                    { new Guid("4d1a1f2c-6f13-3ba0-ab64-fe84250d7267"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5m, 180, new Guid("9f6a10ab-38ef-84ba-acc9-4e6e4ef115a3"), "Stand centered in the frame, brace and drive through the floor to stand tall.", false, null, "Trap Bar Deadlift", null, 0 },
                    { new Guid("4d53e1ed-5372-c298-0210-38060df271fb"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1m, 45, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Brace the upper arm against the thigh and curl through a controlled range.", false, null, "Concentration Curl", null, 0 },
                    { new Guid("4d5ec6ca-97e9-5792-b849-e8d74bd2e6fe"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 90, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Set the seat for an upper-chest press path and control both ends of every rep.", false, null, "Machine Incline Press", null, 0 },
                    { new Guid("4dba253f-9f50-4e88-bda5-ef5c7071f416"), "Cardio", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 0, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Record pool distance and duration for the chosen stroke.", false, null, "Swimming", null, 4 },
                    { new Guid("4dc0f160-35df-ac8c-f1b7-af3a64bdca0b"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 75, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Plant the whole lead foot on the box and stand without pushing off the trailing leg.", false, null, "Dumbbell Step Up", null, 0 },
                    { new Guid("4ee98183-8d0f-4de5-db84-4c4cdf4b12e1"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Bring the knees together smoothly and control the return.", false, null, "Hip Adduction Machine", null, 0 },
                    { new Guid("4f1b6f1f-a4a6-bb5c-cac0-35260efd0733"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1m, 45, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Support the forearms and lift the backs of the hands through a controlled range.", false, null, "Reverse Wrist Curl", null, 0 },
                    { new Guid("4fa98450-c23d-c59c-4bf4-bba35d6fb18f"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1.25m, 60, new Guid("ea46aa2f-9f02-4573-1016-018252ba8ce1"), "Pull the bar toward the upper chest with elbows leading and stop at a comfortable height.", false, null, "Upright Row", null, 0 },
                    { new Guid("509f5530-0fde-4cde-5f56-2f2aa0fb008e"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1.25m, 60, new Guid("ea46aa2f-9f02-4573-1016-018252ba8ce1"), "Use an overhand grip, keep the wrists straight and curl without swinging.", false, null, "Reverse Curl", null, 0 },
                    { new Guid("59e649f9-941c-8011-9df7-274092cd0adb"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 30, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Keep the heels planted and lift the toes toward the shins under control.", false, null, "Tibialis Raise", null, 1 },
                    { new Guid("5b2f579c-447f-c741-4b28-5e929f4889ec"), "Conditioning", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Keep the knees low and crawl with opposite hand and foot moving together.", false, null, "Bear Crawl", null, 3 },
                    { new Guid("5b5b8066-2b3d-945b-5314-6f6b2bd7fc85"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Keep the elbows still and curl against continuous cable tension.", false, null, "Cable Curl", null, 0 },
                    { new Guid("5dfefe06-a654-16ef-ce73-afa498f92bd0"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Sweep the handles upward and inward while keeping a soft bend in the elbows.", false, null, "Low to High Cable Fly", null, 0 },
                    { new Guid("5e8dcf48-c535-ac4b-60d6-c96e4789225e"), "Core", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Rotate shoulder toward opposite knee while extending the other leg slowly.", false, null, "Bicycle Crunch", null, 1 },
                    { new Guid("5f8c79e1-81c2-a01d-821d-0d3d4c1221c8"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Brace against rotation and press one handle forward from chest height.", false, null, "Single Arm Cable Chest Press", null, 0 },
                    { new Guid("5fbcfdb8-df7f-f712-d8f0-1eb51118bb43"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 120, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Use a stable wall setup, lower under control and press without losing trunk tension.", false, null, "Handstand Push Up", null, 1 },
                    { new Guid("60585e44-151d-222c-0fa8-cba4a87d655a"), "Core", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 60, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Carry one heavy weight without leaning and switch sides evenly.", false, null, "Suitcase Carry", null, 3 },
                    { new Guid("61435e1b-14f3-a230-c3ec-2b49e61f68a1"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Keep the hips hinged while raising and lowering the heels through a full range.", false, null, "Donkey Calf Raise", null, 0 },
                    { new Guid("6c58eb05-3525-056d-5962-d77d102f9207"), "Conditioning", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Move from standing to a plank and back, adding a jump while maintaining control.", false, null, "Burpee", null, 1 },
                    { new Guid("6cd088d2-41a1-d172-975f-126ea52c2584"), "Full Body", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5m, 150, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Start above the knees, extend powerfully and receive the bar with stable feet.", false, null, "Hang Clean", null, 0 },
                    { new Guid("706ce261-b9dc-df16-9a7e-04d73eeb8912"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 120, new Guid("c2fe7433-3260-f60f-06bc-a1e9ab8044da"), "Set the bench upright and press the bar overhead without excessive back arch.", false, null, "Smith Machine Shoulder Press", null, 0 },
                    { new Guid("7164d238-0fcb-beac-2b3e-f6c0efe69015"), "Full Body", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 90, new Guid("e830f954-0176-7c23-cda7-31a84ba93e1c"), "Keep the weight stacked overhead while moving deliberately from the floor to standing and back.", false, null, "Turkish Get Up", null, 0 },
                    { new Guid("746d0e6e-7dde-717f-9aa2-69a4ff064302"), "Core", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Lift the legs and torso together, reaching toward the feet without swinging.", false, null, "V Up", null, 1 },
                    { new Guid("78533937-563f-c726-df24-c7d5d6b2aec3"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Row one handle toward the hip and resist rotating through the torso.", false, null, "Single Arm Cable Row", null, 0 },
                    { new Guid("7a5bf9ed-b8a4-73fd-6ea4-a06cb0b78219"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 90, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Hold the weight at the chest, sit between the hips and keep the whole foot planted.", false, null, "Goblet Squat", null, 0 },
                    { new Guid("7add62bb-f085-ede9-34c9-73c7d6ee3833"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1m, 60, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Keep the upper arms behind the torso and curl without moving the shoulders.", false, null, "Incline Dumbbell Curl", null, 0 },
                    { new Guid("7bd96d3e-8489-883f-7d03-161c63dda919"), "Plyometrics", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 75, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Swing the arms, jump forward and land softly with balanced knees and hips.", false, null, "Broad Jump", null, 1 },
                    { new Guid("84297ab4-d0e4-0871-4c23-85edf5956570"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 90, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Balance on one leg, descend with control and use assistance when needed.", false, null, "Pistol Squat", null, 1 },
                    { new Guid("861aaacc-7adc-fd78-c5ed-b7877b421581"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Drive the pads down in an arc while keeping the ribs controlled.", false, null, "Pullover Machine", null, 0 },
                    { new Guid("8634ffa3-94d4-1e45-4462-b719400266fd"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 75, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Lie chest-down on an incline bench and row toward the lower ribs.", false, null, "Chest Supported Dumbbell Row", null, 0 },
                    { new Guid("86c64172-40d7-86de-caa0-29a1fa44ee0e"), "Full Body", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5m, 180, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Drive from the floor, extend rapidly and receive the bar on the shoulders in a partial squat.", false, null, "Power Clean", null, 0 },
                    { new Guid("8b572c46-384e-4481-a3f4-4380487e64b5"), "Core", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Reach opposite arm and leg long without rotating the hips.", false, null, "Bird Dog", null, 1 },
                    { new Guid("8c85319c-7ef4-e722-42d0-b54f63f74157"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1m, 45, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Raise the arm out to shoulder height while keeping tension on the cable.", false, null, "Cable Lateral Raise", null, 0 },
                    { new Guid("90059af2-8ca9-1077-7bef-4b7d3c1ae9ef"), "Core", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Press the handle away from the chest while resisting rotation.", false, null, "Pallof Press", null, 0 },
                    { new Guid("93f38a2b-1c2b-31cd-c278-9cf959d21ba3"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Keep the chest against the pad and bring the arms together without shrugging.", false, null, "Pec Deck Fly", null, 0 },
                    { new Guid("96420734-efe4-d1ac-e5f6-ff6c5f089b62"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 90, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Keep a soft knee bend and hinge the hips back while the spine stays neutral.", false, null, "Good Morning", null, 0 },
                    { new Guid("9c32a506-9e62-d6a2-eae4-edb3064de189"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 75, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Step back, lower under control and drive through the lead foot to return.", false, null, "Reverse Lunge", null, 0 },
                    { new Guid("9c7f9d4d-d3b3-29f6-0fe5-841af531a446"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 120, new Guid("c2fe7433-3260-f60f-06bc-a1e9ab8044da"), "Set the feet for balance, brace and squat through a comfortable range.", false, null, "Smith Machine Squat", null, 0 },
                    { new Guid("a2082bd4-f7a4-c2f1-26f2-5f38e6e86332"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5m, 150, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Start below the knees, brace hard and extend the hips while keeping the bar close.", false, null, "Rack Pull", null, 0 },
                    { new Guid("a875020c-9955-5df6-d24a-bf3a4ed4a078"), "Core", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Curl the pelvis toward the ribs without swinging the legs.", false, null, "Reverse Crunch", null, 1 },
                    { new Guid("a9445cf7-e1ce-9d5a-7bb4-53955ea003cb"), "Core", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Keep the lower back gently pressed down while extending opposite arm and leg.", false, null, "Dead Bug", null, 1 },
                    { new Guid("ab0ce91f-e6e1-0a16-8c29-68a744ecbaca"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Rise through the toes, pause at the top and lower into a full stretch.", false, null, "Standing Calf Raise", null, 0 },
                    { new Guid("accf8b83-1d16-fa92-909a-80102ed2d3a8"), "Core", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 30, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Hold a strong plank and alternate driving the knees forward.", false, null, "Mountain Climber", null, 3 },
                    { new Guid("af179f27-3579-a9ad-0062-45d20073ab6c"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 75, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Keep the body rigid and pull the chest to the bar or handles.", false, null, "Inverted Row", null, 1 },
                    { new Guid("b2542f14-d2f5-bdea-7e66-51cf6c4dc26b"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Sweep the handles down and together without rounding the shoulders.", false, null, "High to Low Cable Fly", null, 0 },
                    { new Guid("b5163ad4-4e20-44d0-f7b7-44b5bad33770"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1m, 45, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Raise the weights to shoulder height without leaning back or swinging.", false, null, "Dumbbell Front Raise", null, 0 },
                    { new Guid("b708cd86-073e-94f5-d7ec-ebf1aa608b9e"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 90, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Lower the bar toward the upper chest by bending the elbows, then extend smoothly.", false, null, "JM Press", null, 0 },
                    { new Guid("b73c58a8-f06a-e30c-c3da-2ddbd1bda009"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Lift the handle forward to shoulder height while keeping the ribs down.", false, null, "Cable Front Raise", null, 0 },
                    { new Guid("b75cbbcf-99f5-489e-120e-32136bfebfc2"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 90, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Adjust the seat so the handles begin near shoulder height and press smoothly overhead.", false, null, "Machine Shoulder Press", null, 0 },
                    { new Guid("b8a15341-bb29-7b7c-f6fa-c5971d8b89e3"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 120, new Guid("c2fe7433-3260-f60f-06bc-a1e9ab8044da"), "Use a low incline and lower the bar toward the upper chest with controlled elbows.", false, null, "Smith Machine Incline Press", null, 0 },
                    { new Guid("b9e5b3ae-91b7-e8e4-4306-8007c4a085e8"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 90, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Take controlled steps and keep the lead knee tracking over the foot.", false, null, "Walking Lunge", null, 0 },
                    { new Guid("bc143f04-fe60-4860-1bc9-2be84803be5d"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Elevate the feet and lower the chest between the hands while keeping the trunk braced.", false, null, "Decline Push Up", null, 1 },
                    { new Guid("bd66bc8d-b61e-083a-d745-49b89e9bbaec"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Keep the hips on the pad, curl the heels up and lower slowly.", false, null, "Lying Leg Curl", null, 0 },
                    { new Guid("bf44d79e-2891-d9a8-8312-9c40942acca9"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 30, new Guid("ee0e66b0-29d5-90e7-f0fe-ef8dac0c1955"), "Pull the band across the chest until the arms form a straight line.", false, null, "Band Pull Apart", null, 1 },
                    { new Guid("c19305bc-400e-4d54-14f9-a649e1f69f17"), "Cardio", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 0, new Guid("89cde677-e340-f0fb-a1ac-58aa920f33ea"), "Use steady steps and avoid leaning heavily on the rails.", false, null, "Stair Climber", null, 4 },
                    { new Guid("c4d1770c-8e6a-ecc6-5a5f-e31ede9d62ed"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 90, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Keep the shoulder blades set and press the dumbbells from lower-chest level.", false, null, "Decline Dumbbell Press", null, 0 },
                    { new Guid("ca3d6e67-3a30-a951-d5c3-cbe10ae06400"), "Full Body", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 90, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Stand from the squat and press both dumbbells overhead in one fluid motion.", false, null, "Dumbbell Thruster", null, 0 },
                    { new Guid("cb1e7764-060c-6432-f5f2-62562e1a44b4"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 120, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Secure the legs, lower the bar to the lower chest and press over the shoulders.", false, null, "Decline Bench Press", null, 0 },
                    { new Guid("cd6b8e66-8d73-bdc3-8b56-51e74c3eeebe"), "Cardio", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 0, new Guid("89cde677-e340-f0fb-a1ac-58aa920f33ea"), "Maintain a smooth cadence and record duration and distance.", false, null, "Elliptical Trainer", null, 4 },
                    { new Guid("ceb5a4bd-cb28-433c-f2e6-9d1a0b65df75"), "Mobility", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 0, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Rotate both knees side to side under control while keeping the torso tall.", false, null, "90/90 Hip Switch", null, 1 },
                    { new Guid("cf7abb85-1437-010e-1ab1-9279e4065702"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1m, 45, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Hold the upper arm still and extend the elbow until the arm is straight.", false, null, "Dumbbell Triceps Kickback", null, 0 },
                    { new Guid("d1cfc3eb-bcc8-4e2c-9e83-8ebcf01fcd3a"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 120, new Guid("c2fe7433-3260-f60f-06bc-a1e9ab8044da"), "Position the bench so the bar reaches mid chest, then press without losing upper-back tension.", false, null, "Smith Machine Bench Press", null, 0 },
                    { new Guid("d41dc9d6-f8b9-1cd8-b3dd-b8878a560f65"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5m, 180, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Take a wide stance, brace and push the floor apart while keeping the bar close.", false, null, "Sumo Deadlift", null, 0 },
                    { new Guid("d83c8717-a4a5-7c44-a3ba-dad36745d3a1"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 75, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Hinge over one leg with square hips and return by driving the stance hip forward.", false, null, "Single Leg Romanian Deadlift", null, 0 },
                    { new Guid("d97e16d0-85bf-ee77-1fde-d97423bbd58c"), "Cardio", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 0, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Walk at a purposeful pace and track time and distance.", false, null, "Brisk Walk", null, 4 },
                    { new Guid("e3df50c2-0e26-aca4-b81a-0cf712988fcc"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 60, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Lower the dumbbell behind the head with bent elbows, then pull it back over the chest.", false, null, "Dumbbell Pullover", null, 0 },
                    { new Guid("e716c25b-65e9-2b43-b502-177bae273a96"), "Core", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Stack the body in a straight line and keep the hips lifted.", false, null, "Side Plank", null, 3 },
                    { new Guid("e868adcb-aea1-bca9-1f24-a9d524cbb20e"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Drive the pads outward with the elbows and lower slowly.", false, null, "Machine Lateral Raise", null, 0 },
                    { new Guid("eb7f4ce6-bd75-dfa7-456b-78e7d2144794"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 75, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Step wide, sit into one hip and keep the opposite leg long.", false, null, "Lateral Lunge", null, 0 },
                    { new Guid("ed003439-f256-a3cb-aa58-3efc2dc04a82"), "Cardio", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 0, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Track trail time and distance, adjusting effort for terrain and elevation.", false, null, "Hiking", null, 4 },
                    { new Guid("ed8cd75f-9214-593b-8956-62ec3b722a09"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Keep the arms nearly straight and sweep the bar from shoulder height to the thighs.", false, null, "Straight Arm Pulldown", null, 0 },
                    { new Guid("ee605ae6-5a8e-0883-d970-43935b6c59d3"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5m, 120, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Keep the torso tall and squat while the belt loads the hips.", false, null, "Belt Squat", null, 0 },
                    { new Guid("f0a5dda3-6129-bf45-fbe3-a8396e726b58"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Keep the knees under the pads and move through the largest controlled ankle range.", false, null, "Seated Calf Raise", null, 0 },
                    { new Guid("f17e30d7-b412-eb6d-caaf-4df71dbcc7ab"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Keep the chest on the pad and sweep the arms back without shrugging.", false, null, "Reverse Pec Deck", null, 0 },
                    { new Guid("f20ab66d-11dc-cae8-a67d-02470794376d"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Extend the elbows and separate the rope ends without letting the shoulders roll forward.", false, null, "Rope Triceps Pushdown", null, 0 },
                    { new Guid("f9156ba0-43ee-5905-1c38-768342eabb23"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Align the elbows with the pivot and extend without lifting from the pad.", false, null, "Machine Triceps Extension", null, 0 },
                    { new Guid("fa286022-acf1-e07a-b4d6-7d0168c5419d"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Secure the thigh pad, curl through the full range and control the return.", false, null, "Seated Leg Curl", null, 0 },
                    { new Guid("fe0d3b73-4714-17a5-ef86-354da5b1bcc7"), "Core", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Brace the feet as needed and roll the torso up under control.", false, null, "Sit Up", null, 1 },
                    { new Guid("fea8e52d-6f6c-c15d-0fac-39014417c43f"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 75, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Pull the neutral handle toward the upper chest while keeping the torso tall.", false, null, "Close Grip Lat Pulldown", null, 0 }
                });

            migrationBuilder.InsertData(
                table: "ExerciseMuscles",
                columns: new[] { "Id", "ContributionWeight", "ExerciseId", "MuscleId", "Role" },
                values: new object[,]
                {
                    { new Guid("029ede44-5c88-7469-3ef3-57414ae437dd"), 1.0m, new Guid("bc143f04-fe60-4860-1bc9-2be84803be5d"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("02aa8ccc-1122-8e37-8011-9b315e437c54"), 1.0m, new Guid("509f5530-0fde-4cde-5f56-2f2aa0fb008e"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 0 },
                    { new Guid("02b826a2-271e-344c-0d09-0c614ebf324f"), 0.4m, new Guid("4d5ec6ca-97e9-5792-b849-e8d74bd2e6fe"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("031383dc-a56b-3d3b-7800-df6d11631c30"), 0.4m, new Guid("4be295d8-414f-8dcc-b97f-965e8d050343"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 1 },
                    { new Guid("0671bfb2-0a7b-49dd-e942-97ecd39b215b"), 0.4m, new Guid("8c85319c-7ef4-e722-42d0-b54f63f74157"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 1 },
                    { new Guid("068ab489-4456-04e8-06b7-9da5c5c3aae4"), 1.0m, new Guid("7a5bf9ed-b8a4-73fd-6ea4-a06cb0b78219"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("06c512a9-070e-212e-3ae8-f9b6259f1d66"), 0.4m, new Guid("ca3d6e67-3a30-a951-d5c3-cbe10ae06400"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("072f2d87-023d-f159-ba14-cf929df8ec28"), 1.0m, new Guid("3eeba987-f873-ba13-c353-c02844ef25f0"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 0 },
                    { new Guid("07bd70d1-7158-9b8d-7648-34a22cbc48e5"), 1.0m, new Guid("9c32a506-9e62-d6a2-eae4-edb3064de189"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("09dbf87a-98df-083e-0ba9-6299903975e9"), 1.0m, new Guid("268d8dad-43c7-5e83-24b5-a07387ae8af9"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 0 },
                    { new Guid("0b2953d0-9f67-b22c-b9bb-00bc7f7573be"), 1.0m, new Guid("86c64172-40d7-86de-caa0-29a1fa44ee0e"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("0dc62918-a3f6-c102-50f7-4c9a2af165ed"), 1.0m, new Guid("e3df50c2-0e26-aca4-b81a-0cf712988fcc"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("1019ea7e-a971-3e7d-126b-de70f70f5714"), 1.0m, new Guid("86c64172-40d7-86de-caa0-29a1fa44ee0e"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("108d1f81-b52f-fe4c-11d1-17291cf791d9"), 0.4m, new Guid("861aaacc-7adc-fd78-c5ed-b7877b421581"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 1 },
                    { new Guid("10e57a7e-8736-0032-6a45-5aacb8841d72"), 1.0m, new Guid("4d53e1ed-5372-c298-0210-38060df271fb"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 0 },
                    { new Guid("111316fb-796a-98c9-b3e2-1ce3652da989"), 1.0m, new Guid("8b572c46-384e-4481-a3f4-4380487e64b5"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 0 },
                    { new Guid("1123af59-c95d-5df4-8645-b53c94f2ed68"), 0.4m, new Guid("46e9110f-fae3-5225-1f05-cc9d0abf91a1"), new Guid("983a359b-2e07-4350-bdb3-3568b0b85dda"), 1 },
                    { new Guid("112607b0-1033-c76d-16ec-5292fa53e633"), 1.0m, new Guid("421b346b-b949-be3b-7f2c-26372211df7d"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 0 },
                    { new Guid("11867af7-abba-eac6-7866-599e17b7218b"), 1.0m, new Guid("c19305bc-400e-4d54-14f9-a649e1f69f17"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 0 },
                    { new Guid("119189e7-6e01-9b45-3239-9908c9ea56d5"), 1.0m, new Guid("9c7f9d4d-d3b3-29f6-0fe5-841af531a446"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("122c89fc-3e65-193e-5e2b-900d22b03193"), 1.0m, new Guid("6cd088d2-41a1-d172-975f-126ea52c2584"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 0 },
                    { new Guid("129fa8b8-755f-bf88-5bb8-ed716c4746cf"), 1.0m, new Guid("44c28204-7fc2-435b-7ac2-579def6f21b0"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 0 },
                    { new Guid("1352f236-ae7a-a3f9-1317-b340253db7f5"), 0.4m, new Guid("0caf5595-4ddb-9439-0b1c-26175401654e"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 1 },
                    { new Guid("14366be8-4950-fd34-666b-9f8b22867c97"), 0.4m, new Guid("3f3a9eac-002c-68dd-4878-d58fdad2a418"), new Guid("8d35a1be-c85a-eac7-c2b5-757c17cebb0d"), 1 },
                    { new Guid("153276a6-3124-49f1-0e8f-ff03dc675198"), 1.0m, new Guid("8634ffa3-94d4-1e45-4462-b719400266fd"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 0 },
                    { new Guid("15ad3ca8-bd92-7ab1-71df-558ad8c13bec"), 0.4m, new Guid("5b2f579c-447f-c741-4b28-5e929f4889ec"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("193c4bc5-5481-2377-9845-1b99c1bfbecd"), 0.4m, new Guid("84297ab4-d0e4-0871-4c23-85edf5956570"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 1 },
                    { new Guid("194aa0e1-3393-fdd3-6cbd-0662fc7ada3e"), 1.0m, new Guid("1b03d6d3-b2ed-7f2e-6644-2a175efe6568"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("1bc10305-3e4f-31b2-b977-f46f041b3ab4"), 1.0m, new Guid("4193dd34-4d5d-0d43-59fe-203d4f2b55a3"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 0 },
                    { new Guid("1e03e84f-ecad-60d4-a066-450302ee229e"), 1.0m, new Guid("2ea83b13-d10a-cbcc-790a-cc421110d46d"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("1e37752c-f831-6e4e-fabd-5a14f2ffe723"), 0.4m, new Guid("38ccd0eb-9476-583c-5e99-25654a59eddb"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("1f47af5f-441b-d458-fecf-b5e6b4338229"), 0.4m, new Guid("ca3d6e67-3a30-a951-d5c3-cbe10ae06400"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("200731ab-bfe4-38d6-0c17-16728702315c"), 0.4m, new Guid("3eeba987-f873-ba13-c353-c02844ef25f0"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("2009fc9a-0123-d4f0-0228-e6ee7b82c289"), 0.4m, new Guid("bc143f04-fe60-4860-1bc9-2be84803be5d"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("2026b894-07ee-48ad-dd83-9f21565839e6"), 1.0m, new Guid("5dfefe06-a654-16ef-ce73-afa498f92bd0"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("20385932-d201-2eca-5b1a-789dea6d22fe"), 1.0m, new Guid("60585e44-151d-222c-0fa8-cba4a87d655a"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 0 },
                    { new Guid("220ce8b9-7efe-f7f2-8850-78ba3c4f3a8c"), 1.0m, new Guid("3017d1b8-ac05-29a5-e2a0-f2f3b1057d74"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 0 },
                    { new Guid("2252d74c-3b5b-25ef-ba7e-eed1e6f833e5"), 1.0m, new Guid("60585e44-151d-222c-0fa8-cba4a87d655a"), new Guid("bcb0bc9d-0d9f-89e8-f957-337f2614a9b1"), 0 },
                    { new Guid("225dec63-1227-b120-224e-6fa57d879d2f"), 1.0m, new Guid("0d1f80ff-fcfb-950d-f88d-493fbce1a73c"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 0 },
                    { new Guid("24e10edd-c7cd-81b8-63cc-516094ef2c22"), 0.4m, new Guid("eb7f4ce6-bd75-dfa7-456b-78e7d2144794"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 1 },
                    { new Guid("262659d9-336a-c563-230e-306ae4029997"), 0.4m, new Guid("84297ab4-d0e4-0871-4c23-85edf5956570"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("29317490-0fc2-b26c-99a9-d9a1f71c62d7"), 0.4m, new Guid("1b03d6d3-b2ed-7f2e-6644-2a175efe6568"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("29f7ee4e-637b-33e0-922f-e50f77c52ccb"), 0.4m, new Guid("5f8c79e1-81c2-a01d-821d-0d3d4c1221c8"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("2a398414-c661-86f2-bd7e-14dcab613708"), 0.4m, new Guid("e3df50c2-0e26-aca4-b81a-0cf712988fcc"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("2a8ebb70-ed6a-8ba2-85ef-201c28ce3732"), 0.4m, new Guid("746d0e6e-7dde-717f-9aa2-69a4ff064302"), new Guid("bcb0bc9d-0d9f-89e8-f957-337f2614a9b1"), 1 },
                    { new Guid("2b51b098-0b01-45c6-e9ca-e2b7728550f0"), 0.4m, new Guid("b8a15341-bb29-7b7c-f6fa-c5971d8b89e3"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("2c0aacc0-f1ea-b1e4-26df-a3e234bb813e"), 1.0m, new Guid("105a15ee-60f5-ff7f-20ca-799591c0e169"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 0 },
                    { new Guid("2c20b3a0-2b26-1450-90b7-f24950b967ba"), 0.4m, new Guid("d97e16d0-85bf-ee77-1fde-d97423bbd58c"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 1 },
                    { new Guid("2d5e543f-57ec-5abb-b389-100f04d06bcf"), 0.4m, new Guid("11b3621a-9cfe-e302-f045-8995c7eda8ba"), new Guid("1678d70c-25a5-db43-d7f4-a7ea30040382"), 1 },
                    { new Guid("2e897c98-1f38-d070-e12a-fd856b496c9c"), 0.4m, new Guid("4be295d8-414f-8dcc-b97f-965e8d050343"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("2e9cd74a-589d-12f2-f86a-5af9b3cf600a"), 1.0m, new Guid("4fa98450-c23d-c59c-4bf4-bba35d6fb18f"), new Guid("37a3dd84-a1db-46b4-357f-47478a17a912"), 0 },
                    { new Guid("2f211cf6-c657-b515-0549-67b184678af3"), 1.0m, new Guid("b8a15341-bb29-7b7c-f6fa-c5971d8b89e3"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("30179f63-ae43-27b0-a911-a7c337862cb8"), 0.4m, new Guid("ee605ae6-5a8e-0883-d970-43935b6c59d3"), new Guid("1678d70c-25a5-db43-d7f4-a7ea30040382"), 1 },
                    { new Guid("304a0a5c-fff4-8816-e892-84acf642b022"), 0.4m, new Guid("2d8178ef-e61f-fe39-e3ac-7d2f4a748564"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 1 },
                    { new Guid("3098f5d2-8da5-38ff-9b44-6448e92cc6d5"), 0.4m, new Guid("cd6b8e66-8d73-bdc3-8b56-51e74c3eeebe"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("30e0d0a5-83ee-40e0-19c9-6bc693a508ac"), 0.4m, new Guid("b9e5b3ae-91b7-e8e4-4306-8007c4a085e8"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("3195a3a7-c235-7596-f03b-f905da8c810d"), 1.0m, new Guid("af179f27-3579-a9ad-0062-45d20073ab6c"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("31a835e6-81e2-77a2-7c37-55f7ea39fa95"), 0.4m, new Guid("78533937-563f-c726-df24-c7d5d6b2aec3"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("3330a00c-f92a-b99c-ab46-6f69ba10a43c"), 1.0m, new Guid("5fbcfdb8-df7f-f712-d8f0-1eb51118bb43"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 0 },
                    { new Guid("345e2369-561e-0cc3-5480-ae5617d8da62"), 1.0m, new Guid("d41dc9d6-f8b9-1cd8-b3dd-b8878a560f65"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("3527d35f-31dc-e62c-22a3-512c50289b26"), 1.0m, new Guid("44c28204-7fc2-435b-7ac2-579def6f21b0"), new Guid("bcb0bc9d-0d9f-89e8-f957-337f2614a9b1"), 0 },
                    { new Guid("35aa3b69-b194-0f82-8615-3471c3a439ba"), 0.4m, new Guid("9c7f9d4d-d3b3-29f6-0fe5-841af531a446"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("36ee6068-5e70-f198-0367-250d9054c830"), 1.0m, new Guid("5e8dcf48-c535-ac4b-60d6-c96e4789225e"), new Guid("bcb0bc9d-0d9f-89e8-f957-337f2614a9b1"), 0 },
                    { new Guid("3a08a963-889a-c0d3-806e-b486ee775378"), 0.4m, new Guid("9c32a506-9e62-d6a2-eae4-edb3064de189"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("3a839884-252a-e2ad-1fc5-7288ce0e28b1"), 1.0m, new Guid("2299a568-2a3e-9a83-40e4-fdea1168a0af"), new Guid("983a359b-2e07-4350-bdb3-3568b0b85dda"), 0 },
                    { new Guid("3a9cb662-b260-5da2-c545-b9a69d591e92"), 0.4m, new Guid("5fbcfdb8-df7f-f712-d8f0-1eb51118bb43"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 1 },
                    { new Guid("3ac3cc1a-ed18-4a6e-61c1-f804049efcf9"), 1.0m, new Guid("2e147828-59fb-4a5d-bf0e-d05b064c45d9"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("3aec4630-3395-c172-fe88-860955ce3aeb"), 0.4m, new Guid("ca3d6e67-3a30-a951-d5c3-cbe10ae06400"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("3cfea475-a165-b9ca-2b0c-5c23ff308db1"), 1.0m, new Guid("b5163ad4-4e20-44d0-f7b7-44b5bad33770"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 0 },
                    { new Guid("3d7111eb-83a9-534b-eacc-94f89d47c1c1"), 0.4m, new Guid("0d154558-9d0e-8138-33d9-536ffb9432e5"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("3dcf7697-3000-b38e-3f3a-54180f1cf236"), 1.0m, new Guid("a875020c-9955-5df6-d24a-bf3a4ed4a078"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 0 },
                    { new Guid("3e28d201-5a08-b57b-25a7-19a45b9d8149"), 0.4m, new Guid("5b5b8066-2b3d-945b-5314-6f6b2bd7fc85"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 1 },
                    { new Guid("3e9b589e-ea24-7512-d9a7-21018b4cb326"), 0.4m, new Guid("3cf3597c-0444-9b71-73d3-8c4f4c888e1a"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("3ed9897e-35e3-7408-bb04-5b0018c978d9"), 0.4m, new Guid("8634ffa3-94d4-1e45-4462-b719400266fd"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("401836b8-6593-92f2-35e1-84a7d4db82de"), 0.4m, new Guid("d41dc9d6-f8b9-1cd8-b3dd-b8878a560f65"), new Guid("983a359b-2e07-4350-bdb3-3568b0b85dda"), 1 },
                    { new Guid("4053e2b6-44ac-417e-26b7-84276d0f5586"), 1.0m, new Guid("b9e5b3ae-91b7-e8e4-4306-8007c4a085e8"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("416714d5-2e86-9e7a-d654-f9ac764cc335"), 1.0m, new Guid("ab0ce91f-e6e1-0a16-8c29-68a744ecbaca"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 0 },
                    { new Guid("416a34c3-2cf1-5662-1a01-f7771c30d334"), 0.4m, new Guid("e716c25b-65e9-2b43-b502-177bae273a96"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("41b38429-acc8-bc51-035c-9e541e8ae525"), 0.4m, new Guid("053ad886-7a89-d64d-8a9e-46381a60bc5c"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("420ce2b5-8da6-7d1b-ecb9-81e372952f6e"), 1.0m, new Guid("4d1a1f2c-6f13-3ba0-ab64-fe84250d7267"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("437e214c-97bc-410f-2b88-9f35b8898111"), 1.0m, new Guid("e716c25b-65e9-2b43-b502-177bae273a96"), new Guid("bcb0bc9d-0d9f-89e8-f957-337f2614a9b1"), 0 },
                    { new Guid("4420776e-1546-4b2f-49b4-1fe66ae7e746"), 1.0m, new Guid("c4d1770c-8e6a-ecc6-5a5f-e31ede9d62ed"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("4461f0f9-7b2e-8fc5-cf56-c803ecc51dbd"), 1.0m, new Guid("861aaacc-7adc-fd78-c5ed-b7877b421581"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("44d7e72d-10db-e7e5-dc8a-fc2ddc3d8c97"), 1.0m, new Guid("bf44d79e-2891-d9a8-8312-9c40942acca9"), new Guid("8d35a1be-c85a-eac7-c2b5-757c17cebb0d"), 0 },
                    { new Guid("44efe63d-e1ab-520a-e86c-949ac8034861"), 1.0m, new Guid("93f38a2b-1c2b-31cd-c278-9cf959d21ba3"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("47b12ee6-9a4c-08b0-fba3-c0d486917f74"), 1.0m, new Guid("153bd9c4-6991-0246-6c44-ed0caedd25a2"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("47dc64db-8d39-42c7-a288-c0f152a85691"), 1.0m, new Guid("11b3621a-9cfe-e302-f045-8995c7eda8ba"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("48163976-2e29-27a9-43a6-1567d6e75d4e"), 1.0m, new Guid("96420734-efe4-d1ac-e5f6-ff6c5f089b62"), new Guid("983a359b-2e07-4350-bdb3-3568b0b85dda"), 0 },
                    { new Guid("481960d8-617a-836f-cfe3-13668661759a"), 1.0m, new Guid("4dc0f160-35df-ac8c-f1b7-af3a64bdca0b"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("488eedd1-82ba-1b2e-c184-8bd34ea20401"), 1.0m, new Guid("b73c58a8-f06a-e30c-c3da-2ddbd1bda009"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 0 },
                    { new Guid("49125acb-973d-094a-f11e-3915badfd00c"), 0.4m, new Guid("ed8cd75f-9214-593b-8956-62ec3b722a09"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("49971eaa-8085-9c96-324a-ffac7524232a"), 0.4m, new Guid("d1cfc3eb-bcc8-4e2c-9e83-8ebcf01fcd3a"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("4a55b6cd-a61d-bcee-67f1-d40b40ce37f3"), 1.0m, new Guid("5f8c79e1-81c2-a01d-821d-0d3d4c1221c8"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("4af85e9c-53e9-fbdd-3e1d-a80f6b47af00"), 0.4m, new Guid("96420734-efe4-d1ac-e5f6-ff6c5f089b62"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("4b904600-fe0c-c7b8-0150-5285ba1b7061"), 1.0m, new Guid("23eca012-f379-86c2-bd2a-07e73da37e57"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 0 },
                    { new Guid("4bd0da04-db54-95f7-a65f-367cb3eba0d6"), 1.0m, new Guid("d83c8717-a4a5-7c44-a3ba-dad36745d3a1"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 0 },
                    { new Guid("4c15287d-32bd-2e9d-1159-47bb19689a08"), 0.4m, new Guid("cb1e7764-060c-6432-f5f2-62562e1a44b4"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("4c5ae591-0510-f3ec-b341-9452524d034f"), 0.4m, new Guid("a2082bd4-f7a4-c2f1-26f2-5f38e6e86332"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 1 },
                    { new Guid("4e50ad1c-1ec9-c414-c538-2242623337f0"), 0.4m, new Guid("4dba253f-9f50-4e88-bda5-ef5c7071f416"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 1 },
                    { new Guid("4f52ba07-c2bb-57bb-ccd9-bd979a6a09e5"), 0.4m, new Guid("06d5b353-c57f-3a85-7474-95481ed59182"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("4f5d7fde-3f54-4b07-d12a-e6654a87c4f2"), 0.4m, new Guid("41303540-98c4-2902-eca0-5d9cb5ba4ca6"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("506ccd00-7925-aada-bdb5-382b4b4dde33"), 1.0m, new Guid("accf8b83-1d16-fa92-909a-80102ed2d3a8"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 0 },
                    { new Guid("531a0500-2ff4-f903-7971-f33bf63642ee"), 1.0m, new Guid("4d1a1f2c-6f13-3ba0-ab64-fe84250d7267"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("53469bf4-1f96-eae5-2d3e-b8b5bf7808e0"), 1.0m, new Guid("023bc5af-3d3c-3701-ec42-66cea60be7d7"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 0 },
                    { new Guid("54049c3f-6a4b-8a21-8468-9c6e9abb7ed3"), 0.4m, new Guid("7a5bf9ed-b8a4-73fd-6ea4-a06cb0b78219"), new Guid("1678d70c-25a5-db43-d7f4-a7ea30040382"), 1 },
                    { new Guid("54a72936-0991-c0cf-ce35-6cd1adecb886"), 0.4m, new Guid("d41dc9d6-f8b9-1cd8-b3dd-b8878a560f65"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("54db888c-bb14-13fa-9bb6-636c8310a536"), 1.0m, new Guid("2863f1c1-b77c-0b4d-b2f8-ee5f451c3b37"), new Guid("bcb0bc9d-0d9f-89e8-f957-337f2614a9b1"), 0 },
                    { new Guid("554bf92d-f7cc-6d7a-7a13-27945285c334"), 1.0m, new Guid("78533937-563f-c726-df24-c7d5d6b2aec3"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("5556482d-0b0d-3718-9dd7-214ca696eb6b"), 0.4m, new Guid("a9445cf7-e1ce-9d5a-7bb4-53955ea003cb"), new Guid("bcb0bc9d-0d9f-89e8-f957-337f2614a9b1"), 1 },
                    { new Guid("55913cb9-faa6-1c52-29a9-c6d745dd3341"), 1.0m, new Guid("8c85319c-7ef4-e722-42d0-b54f63f74157"), new Guid("37a3dd84-a1db-46b4-357f-47478a17a912"), 0 },
                    { new Guid("55c01ffd-7938-6fda-1447-ece6364c2a22"), 0.4m, new Guid("3eeba987-f873-ba13-c353-c02844ef25f0"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("55fd04d6-952c-e995-5cf7-18793c30b3cf"), 1.0m, new Guid("61435e1b-14f3-a230-c3ec-2b49e61f68a1"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 0 },
                    { new Guid("56a85416-23d0-1104-3ad4-cd2c70c2e8df"), 0.4m, new Guid("d83c8717-a4a5-7c44-a3ba-dad36745d3a1"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("57893efa-fbaa-e44d-7d71-bd942ee4eca6"), 1.0m, new Guid("ceb5a4bd-cb28-433c-f2e6-9d1a0b65df75"), new Guid("1678d70c-25a5-db43-d7f4-a7ea30040382"), 0 },
                    { new Guid("5886c00c-f3c4-0121-ae8e-89c6c1938c5e"), 1.0m, new Guid("e3df50c2-0e26-aca4-b81a-0cf712988fcc"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("59e07a00-7f80-3aae-3f5e-35441a4b7742"), 1.0m, new Guid("3f3a9eac-002c-68dd-4878-d58fdad2a418"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 0 },
                    { new Guid("5a8057de-485e-6fa2-1b99-2b1fce03dd4e"), 0.4m, new Guid("4193dd34-4d5d-0d43-59fe-203d4f2b55a3"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 1 },
                    { new Guid("5aa879b4-5e70-3825-c60c-e15a7bf89f03"), 0.4m, new Guid("4fa98450-c23d-c59c-4bf4-bba35d6fb18f"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("5ab03a10-4f6e-b18a-4164-d8279d6a8ecb"), 0.4m, new Guid("cd6b8e66-8d73-bdc3-8b56-51e74c3eeebe"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 1 },
                    { new Guid("5ae5c0c7-610b-701e-f5c6-b5fb963423c8"), 0.4m, new Guid("46ef2958-9adb-24d9-e556-fb1465098406"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("5b41f37e-48d3-93d7-086b-bfa36167ae7d"), 1.0m, new Guid("b708cd86-073e-94f5-d7ec-ebf1aa608b9e"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 0 },
                    { new Guid("5b995436-0c2d-9e2b-f85d-757ae8e89051"), 1.0m, new Guid("4f1b6f1f-a4a6-bb5c-cac0-35260efd0733"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 0 },
                    { new Guid("5bead454-b028-b742-2562-06434f58f965"), 1.0m, new Guid("4d5ec6ca-97e9-5792-b849-e8d74bd2e6fe"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("5c964c7d-46dd-ea5a-d88a-040eeef8e723"), 0.4m, new Guid("4dba253f-9f50-4e88-bda5-ef5c7071f416"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 1 },
                    { new Guid("5e0770d4-9526-9ce9-16f7-fd23d480e742"), 1.0m, new Guid("c19305bc-400e-4d54-14f9-a649e1f69f17"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("5e148b83-55c1-455e-c140-2b8452ab45f0"), 0.4m, new Guid("ed003439-f256-a3cb-aa58-3efc2dc04a82"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 1 },
                    { new Guid("5e6167d7-c25c-47d2-a44b-a3ccf21e7531"), 1.0m, new Guid("b2542f14-d2f5-bdea-7e66-51cf6c4dc26b"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("5e788418-2dc0-1b42-df42-88e58b87a8cb"), 0.4m, new Guid("5b2f579c-447f-c741-4b28-5e929f4889ec"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 1 },
                    { new Guid("5ec29e16-f242-652b-eef6-c9045bc7453e"), 1.0m, new Guid("2299a568-2a3e-9a83-40e4-fdea1168a0af"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 0 },
                    { new Guid("5f5b22df-9300-c5c2-5ce2-d7db422fd2f4"), 0.4m, new Guid("5f8c79e1-81c2-a01d-821d-0d3d4c1221c8"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("5f66365e-f70d-0618-0bb8-38cb5d1ecb4a"), 0.4m, new Guid("c19305bc-400e-4d54-14f9-a649e1f69f17"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("60f23c64-57ca-fb47-6e23-a02b5ed8b666"), 0.4m, new Guid("25b9afa3-38cc-9a1e-bd89-4ee08d319413"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("6152f3da-14c1-c516-25a4-38bf02c70a67"), 0.4m, new Guid("4dc0f160-35df-ac8c-f1b7-af3a64bdca0b"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 1 },
                    { new Guid("61e0144b-5595-108f-2e8c-47adfba8422d"), 0.4m, new Guid("b2542f14-d2f5-bdea-7e66-51cf6c4dc26b"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("63a55b0c-a1db-111e-11cb-4020133874e4"), 1.0m, new Guid("4ee98183-8d0f-4de5-db84-4c4cdf4b12e1"), new Guid("1678d70c-25a5-db43-d7f4-a7ea30040382"), 0 },
                    { new Guid("643ef453-8683-d284-fdc8-4d6bcdefd616"), 0.4m, new Guid("e716c25b-65e9-2b43-b502-177bae273a96"), new Guid("37a3dd84-a1db-46b4-357f-47478a17a912"), 1 },
                    { new Guid("645a01fa-d151-f7cb-b763-de5c739cf57f"), 0.4m, new Guid("3f3a9eac-002c-68dd-4878-d58fdad2a418"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("66711e0d-1a66-c643-ae58-494160bcf7fa"), 1.0m, new Guid("fea8e52d-6f6c-c15d-0fac-39014417c43f"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("66a774e0-03dd-bf0b-08b2-91692ea9b8ef"), 0.4m, new Guid("46e9110f-fae3-5225-1f05-cc9d0abf91a1"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 1 },
                    { new Guid("678f4713-c5ce-7de7-5c0a-a22e94ed6a17"), 0.4m, new Guid("153bd9c4-6991-0246-6c44-ed0caedd25a2"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("68d059c5-92c4-e6b3-a289-bd065249a404"), 0.4m, new Guid("421b346b-b949-be3b-7f2c-26372211df7d"), new Guid("983a359b-2e07-4350-bdb3-3568b0b85dda"), 1 },
                    { new Guid("68df2d1b-c066-423f-d972-d5c78b2dcb06"), 0.4m, new Guid("cb1e7764-060c-6432-f5f2-62562e1a44b4"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("68fbf2ce-22f2-893d-2e39-f6cc62d9aa94"), 0.4m, new Guid("9c7f9d4d-d3b3-29f6-0fe5-841af531a446"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("69ed86fe-4ca3-e8d1-d6da-3a656b57b7ad"), 1.0m, new Guid("ed003439-f256-a3cb-aa58-3efc2dc04a82"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 0 },
                    { new Guid("6ab8729b-3195-bfc4-3a8d-069c90a7f0ea"), 0.4m, new Guid("7164d238-0fcb-beac-2b3e-f6c0efe69015"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("6b3538ce-a49c-b94b-1c47-4720f22dbf6e"), 0.4m, new Guid("4d53e1ed-5372-c298-0210-38060df271fb"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 1 },
                    { new Guid("6bc58661-9f47-b6b0-321e-c149c37164b2"), 1.0m, new Guid("ed8cd75f-9214-593b-8956-62ec3b722a09"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("6bea8b4a-8807-a77d-3c4a-db58c6357d92"), 0.4m, new Guid("86c64172-40d7-86de-caa0-29a1fa44ee0e"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("6c5c3256-1c27-631b-7abe-2362e04f51d8"), 1.0m, new Guid("5b5b8066-2b3d-945b-5314-6f6b2bd7fc85"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 0 },
                    { new Guid("6ce3a64a-310d-2e55-ae8f-88963961a665"), 0.4m, new Guid("f17e30d7-b412-eb6d-caaf-4df71dbcc7ab"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 1 },
                    { new Guid("6d53eddd-abac-7909-ede6-10b5cad566ae"), 1.0m, new Guid("7a5bf9ed-b8a4-73fd-6ea4-a06cb0b78219"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("6d6f452b-1b17-6b18-2900-eb22e430f885"), 0.4m, new Guid("d41dc9d6-f8b9-1cd8-b3dd-b8878a560f65"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 1 },
                    { new Guid("6de25cb2-0395-8a17-e3e4-5b038fbdedfd"), 1.0m, new Guid("408f53a9-3b97-90b7-8473-055a5d41a0be"), new Guid("983a359b-2e07-4350-bdb3-3568b0b85dda"), 0 },
                    { new Guid("6ead8219-2fdb-580c-69d1-734780a8310f"), 0.4m, new Guid("706ce261-b9dc-df16-9a7e-04d73eeb8912"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("6ee176aa-3b28-a7e8-1f6d-03af5fde588d"), 0.4m, new Guid("60585e44-151d-222c-0fa8-cba4a87d655a"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("6f307473-d4c3-0c7d-6c0a-183c126a8e72"), 1.0m, new Guid("cb1e7764-060c-6432-f5f2-62562e1a44b4"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("7151a0c9-7204-5d85-7c1e-800835832bcb"), 0.4m, new Guid("3017d1b8-ac05-29a5-e2a0-f2f3b1057d74"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("715d15a9-5ca0-fc57-1c1b-4cf374fa6808"), 0.4m, new Guid("fea8e52d-6f6c-c15d-0fac-39014417c43f"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 1 },
                    { new Guid("717915ca-988f-d7a4-53fb-093dd0e8aeed"), 1.0m, new Guid("7164d238-0fcb-beac-2b3e-f6c0efe69015"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 0 },
                    { new Guid("73a0930c-3fec-a383-61c7-31641c689bcd"), 1.0m, new Guid("f9156ba0-43ee-5905-1c38-768342eabb23"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 0 },
                    { new Guid("73c882a7-a93f-ad67-d664-be94c03a221e"), 1.0m, new Guid("f17e30d7-b412-eb6d-caaf-4df71dbcc7ab"), new Guid("8d35a1be-c85a-eac7-c2b5-757c17cebb0d"), 0 },
                    { new Guid("74359666-7005-9a59-0b0a-54dae646b1d8"), 1.0m, new Guid("4dc0f160-35df-ac8c-f1b7-af3a64bdca0b"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("74e34693-c269-64c6-8dff-33373763f8ac"), 1.0m, new Guid("46ef2958-9adb-24d9-e556-fb1465098406"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("752971b5-070c-40ca-46a9-dbbad0848030"), 1.0m, new Guid("cf7abb85-1437-010e-1ab1-9279e4065702"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 0 },
                    { new Guid("754cac42-0ada-15c5-7b04-56d426600e4e"), 0.4m, new Guid("2e147828-59fb-4a5d-bf0e-d05b064c45d9"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("764af253-209a-16ad-fde4-30ad165aa415"), 1.0m, new Guid("59e649f9-941c-8011-9df7-274092cd0adb"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 0 },
                    { new Guid("77c0cfb0-570e-219f-7fd6-d14cb52d6ef1"), 0.4m, new Guid("153bd9c4-6991-0246-6c44-ed0caedd25a2"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("788552af-abe3-dfa4-d5c4-19a9dd0c0348"), 0.4m, new Guid("5dfefe06-a654-16ef-ce73-afa498f92bd0"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("7926cd78-202b-88f0-e39e-12b7496d1d88"), 0.4m, new Guid("b708cd86-073e-94f5-d7ec-ebf1aa608b9e"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("79801326-a670-28da-ef15-8b1d458becbf"), 0.4m, new Guid("1058b3f7-daaf-44b3-85d3-e2133b4e5cde"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 1 },
                    { new Guid("79ea9f4c-47ff-1de1-6a21-febb4a935eeb"), 0.4m, new Guid("eb7f4ce6-bd75-dfa7-456b-78e7d2144794"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("79fc1fdd-8f37-327a-23f6-66e36fc38cf8"), 1.0m, new Guid("a2082bd4-f7a4-c2f1-26f2-5f38e6e86332"), new Guid("983a359b-2e07-4350-bdb3-3568b0b85dda"), 0 },
                    { new Guid("7b2b168f-5e18-12a8-2a34-3613d96486bd"), 1.0m, new Guid("7bd96d3e-8489-883f-7d03-161c63dda919"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("7b9c69b6-f7ba-1e97-a938-2c9a14230fd0"), 1.0m, new Guid("053ad886-7a89-d64d-8a9e-46381a60bc5c"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 0 },
                    { new Guid("7bc8b93b-ca8e-2d7f-87bd-e631ed0e7f71"), 1.0m, new Guid("23eca012-f379-86c2-bd2a-07e73da37e57"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("7c9dc180-fdd8-ddfc-6f5b-28b7c9c35377"), 1.0m, new Guid("f0a5dda3-6129-bf45-fbe3-a8396e726b58"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 0 },
                    { new Guid("7cce1c9c-2b47-fc39-503a-b426cc134a67"), 0.4m, new Guid("706ce261-b9dc-df16-9a7e-04d73eeb8912"), new Guid("37a3dd84-a1db-46b4-357f-47478a17a912"), 1 },
                    { new Guid("7d7714b4-129b-a2ae-a1e6-481e4dba5168"), 1.0m, new Guid("21310bf6-4257-119b-528d-d76067da068e"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 0 },
                    { new Guid("7f54e56a-d414-0e94-08c4-9ba7547fe505"), 1.0m, new Guid("f20ab66d-11dc-cae8-a67d-02470794376d"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 0 },
                    { new Guid("7f9ce1d9-d88d-5050-7b41-73f0ce11c7b3"), 0.4m, new Guid("f17e30d7-b412-eb6d-caaf-4df71dbcc7ab"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 1 },
                    { new Guid("801826a4-26a7-8057-e8d6-3f101a000dd9"), 1.0m, new Guid("d97e16d0-85bf-ee77-1fde-d97423bbd58c"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 0 },
                    { new Guid("8063ffe5-0c54-2e66-fd8d-d4c8532fab24"), 1.0m, new Guid("3f3a9eac-002c-68dd-4878-d58fdad2a418"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("80f3fecd-a6e9-04a1-879f-a493bead385b"), 0.4m, new Guid("d83c8717-a4a5-7c44-a3ba-dad36745d3a1"), new Guid("983a359b-2e07-4350-bdb3-3568b0b85dda"), 1 },
                    { new Guid("816e4ad2-7685-223e-23e1-025cc3a4c52d"), 1.0m, new Guid("6cd088d2-41a1-d172-975f-126ea52c2584"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("82a73c26-cb77-38a3-13b1-a385df223731"), 0.4m, new Guid("2d8178ef-e61f-fe39-e3ac-7d2f4a748564"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("832db960-b5ce-2a26-92a1-6a1a4dbdb6f7"), 1.0m, new Guid("e868adcb-aea1-bca9-1f24-a9d524cbb20e"), new Guid("37a3dd84-a1db-46b4-357f-47478a17a912"), 0 },
                    { new Guid("839063bc-b137-7b72-588b-f6a49eca87c3"), 1.0m, new Guid("cd6b8e66-8d73-bdc3-8b56-51e74c3eeebe"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 0 },
                    { new Guid("840854f7-dc42-b302-d652-6352b9c100fc"), 0.4m, new Guid("7bd96d3e-8489-883f-7d03-161c63dda919"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 1 },
                    { new Guid("85a9044e-29c0-54de-6938-d5d2e374a40e"), 1.0m, new Guid("90059af2-8ca9-1077-7bef-4b7d3c1ae9ef"), new Guid("bcb0bc9d-0d9f-89e8-f957-337f2614a9b1"), 0 },
                    { new Guid("85f4932d-6485-db4a-645b-87b8eaac657f"), 1.0m, new Guid("0caf5595-4ddb-9439-0b1c-26175401654e"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 0 },
                    { new Guid("85fb9fd0-6957-01bc-f49a-8ed04355637a"), 0.4m, new Guid("86c64172-40d7-86de-caa0-29a1fa44ee0e"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("863e8186-34d1-1cf3-c571-9a12dbddfdec"), 1.0m, new Guid("ee605ae6-5a8e-0883-d970-43935b6c59d3"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("8746a586-8df4-9149-9230-3dba19871d5b"), 1.0m, new Guid("eb7f4ce6-bd75-dfa7-456b-78e7d2144794"), new Guid("1678d70c-25a5-db43-d7f4-a7ea30040382"), 0 },
                    { new Guid("88cca8f7-7e1c-ea21-5864-5c88d39c8df8"), 0.4m, new Guid("3017d1b8-ac05-29a5-e2a0-f2f3b1057d74"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("89892f6c-ead3-ec76-c0ed-9c0e406a0f97"), 0.4m, new Guid("202e04c4-ec37-1e25-32b0-e7ecb09ac090"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("8a092e16-82cf-4df2-238e-c2510cca5ec8"), 0.4m, new Guid("fa286022-acf1-e07a-b4d6-7d0168c5419d"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 1 },
                    { new Guid("8a705763-1309-361c-1018-dbbbe3b05065"), 0.4m, new Guid("4b589f5d-dd39-25fc-b56a-264d2f59e0cf"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 1 },
                    { new Guid("8aef10dd-1c20-b017-7866-4b5f2476bf0c"), 1.0m, new Guid("706ce261-b9dc-df16-9a7e-04d73eeb8912"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 0 },
                    { new Guid("8b363a7e-6f26-2927-3eb6-34518159edb1"), 0.4m, new Guid("4d1a1f2c-6f13-3ba0-ab64-fe84250d7267"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("8b437288-f774-7f63-846e-ccdc770a95ef"), 1.0m, new Guid("7164d238-0fcb-beac-2b3e-f6c0efe69015"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 0 },
                    { new Guid("8c978da1-9a49-5dc7-5ed4-130e95ce43af"), 0.4m, new Guid("6cd088d2-41a1-d172-975f-126ea52c2584"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("8d8f407b-15da-3b44-62b7-c05e04a8e55d"), 0.4m, new Guid("105a15ee-60f5-ff7f-20ca-799591c0e169"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 1 },
                    { new Guid("8df2dbdb-910e-b512-f7f5-82d20ef1fc6e"), 0.4m, new Guid("7bd96d3e-8489-883f-7d03-161c63dda919"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("8e14a5db-4e62-d0c7-d3e9-dfc880d42289"), 1.0m, new Guid("96420734-efe4-d1ac-e5f6-ff6c5f089b62"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 0 },
                    { new Guid("8e81a471-d4b0-9dbb-f725-d1fb2a9311df"), 1.0m, new Guid("746d0e6e-7dde-717f-9aa2-69a4ff064302"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 0 },
                    { new Guid("8efa044f-4372-cfce-f5b2-4a81da8c60eb"), 0.4m, new Guid("b5163ad4-4e20-44d0-f7b7-44b5bad33770"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 1 },
                    { new Guid("8f5f73ed-3b78-968b-3aa7-f956aa39661b"), 0.4m, new Guid("4b589f5d-dd39-25fc-b56a-264d2f59e0cf"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("9077ac3d-b087-f0c2-0ad3-df275941fe34"), 0.4m, new Guid("d97e16d0-85bf-ee77-1fde-d97423bbd58c"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 1 },
                    { new Guid("910dfd0d-da0b-f073-45a6-43b4705d3894"), 0.4m, new Guid("509f5530-0fde-4cde-5f56-2f2aa0fb008e"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("918dcf0e-bc60-8535-1286-9244ead2ea99"), 0.4m, new Guid("a2082bd4-f7a4-c2f1-26f2-5f38e6e86332"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 1 },
                    { new Guid("92a05a99-19c2-1383-58c2-4b2723c57cae"), 0.4m, new Guid("af179f27-3579-a9ad-0062-45d20073ab6c"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("92e7ad0b-ea5a-19c4-948a-042925525b2f"), 0.4m, new Guid("6c58eb05-3525-056d-5962-d77d102f9207"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("93325a6d-2016-6528-0af7-5b788cf3b9fb"), 1.0m, new Guid("3a78770d-d22d-6e7c-f156-92c8e4e4c17c"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 0 },
                    { new Guid("941a5573-262f-7774-c5a6-51ebda783c6a"), 1.0m, new Guid("ceb5a4bd-cb28-433c-f2e6-9d1a0b65df75"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("946886a7-8461-a782-414e-5e658e872f1d"), 0.4m, new Guid("11b3621a-9cfe-e302-f045-8995c7eda8ba"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("95166683-75c2-467f-0ef0-e03372cc6209"), 0.4m, new Guid("fe0d3b73-4714-17a5-ef86-354da5b1bcc7"), new Guid("bcb0bc9d-0d9f-89e8-f957-337f2614a9b1"), 1 },
                    { new Guid("952df4a2-56ef-6445-a9e8-2c97e571aa50"), 0.4m, new Guid("a2082bd4-f7a4-c2f1-26f2-5f38e6e86332"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("957949fb-6aff-847d-3208-2c600c3bbcc5"), 1.0m, new Guid("5b2f579c-447f-c741-4b28-5e929f4889ec"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 0 },
                    { new Guid("97cd1ba2-a95e-da50-48f7-6c89dad285d9"), 1.0m, new Guid("41303540-98c4-2902-eca0-5d9cb5ba4ca6"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("97daf3e9-81a3-275d-2e28-af1043ca8af5"), 0.4m, new Guid("202e04c4-ec37-1e25-32b0-e7ecb09ac090"), new Guid("8d35a1be-c85a-eac7-c2b5-757c17cebb0d"), 1 },
                    { new Guid("981430e0-f4d0-0c62-f2b1-3dcfd4c4fe2e"), 0.4m, new Guid("180a7bc9-69b6-dab8-2325-e8f12c917061"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("9897435a-e870-3d2f-2ee2-16e3dd2f82e4"), 1.0m, new Guid("0d1f80ff-fcfb-950d-f88d-493fbce1a73c"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 0 },
                    { new Guid("98ba0b61-3142-06d3-438c-a50688437d54"), 1.0m, new Guid("ceb5a4bd-cb28-433c-f2e6-9d1a0b65df75"), new Guid("a9eec2ef-ccde-0652-e9f7-f795f1d938c2"), 0 },
                    { new Guid("990db023-4472-b8d7-8bd3-6e358964432f"), 1.0m, new Guid("5b2f579c-447f-c741-4b28-5e929f4889ec"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 0 },
                    { new Guid("9c44eff5-3dbb-7646-4ddd-60be4d92749c"), 1.0m, new Guid("7add62bb-f085-ede9-34c9-73c7d6ee3833"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 0 },
                    { new Guid("9d2eb8ac-8b9e-8cf5-d74d-44af9f8ccf03"), 1.0m, new Guid("46e9110f-fae3-5225-1f05-cc9d0abf91a1"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("9e78c0b7-42f8-0d45-cd7c-73fec6f77adf"), 1.0m, new Guid("2a4d81d5-7db5-76fc-aab6-c55801183ca3"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 0 },
                    { new Guid("9f5bfcfe-df6a-5345-ec59-03dfedbad63b"), 0.4m, new Guid("3017d1b8-ac05-29a5-e2a0-f2f3b1057d74"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("9f635850-edfd-2d6d-c7a0-af939af71ac0"), 1.0m, new Guid("1058b3f7-daaf-44b3-85d3-e2133b4e5cde"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 0 },
                    { new Guid("9fe036ad-1585-4f5f-c307-4b99d9d003b5"), 0.4m, new Guid("6c58eb05-3525-056d-5962-d77d102f9207"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("a0e5cdee-15b8-c700-0237-67e5d743ec14"), 1.0m, new Guid("4b589f5d-dd39-25fc-b56a-264d2f59e0cf"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 0 },
                    { new Guid("a199903c-8b95-7c33-d341-5b80d481b3ec"), 0.4m, new Guid("2ea83b13-d10a-cbcc-790a-cc421110d46d"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("a5a7402b-93ff-e472-31d4-310d23fdb9ed"), 1.0m, new Guid("3017d1b8-ac05-29a5-e2a0-f2f3b1057d74"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("a5e0e305-89df-bdb8-76da-0d548f482cde"), 1.0m, new Guid("3017d1b8-ac05-29a5-e2a0-f2f3b1057d74"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("a679b243-767a-19f6-e463-562512bdf767"), 0.4m, new Guid("7a5bf9ed-b8a4-73fd-6ea4-a06cb0b78219"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("a819273c-d314-ba1f-9d97-877648d52a42"), 1.0m, new Guid("5e8dcf48-c535-ac4b-60d6-c96e4789225e"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 0 },
                    { new Guid("a851557a-2eed-07a1-1385-db070b680daf"), 0.4m, new Guid("0d154558-9d0e-8138-33d9-536ffb9432e5"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 1 },
                    { new Guid("aac0b2fa-1494-be38-9163-42f7cbf74f6d"), 1.0m, new Guid("ca3d6e67-3a30-a951-d5c3-cbe10ae06400"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("ab151f07-a347-364a-9f4f-82b758dda7c6"), 0.4m, new Guid("5b2f579c-447f-c741-4b28-5e929f4889ec"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 1 },
                    { new Guid("abcd65b4-ba50-2efe-af8e-1f43b6911c13"), 0.4m, new Guid("3cf3597c-0444-9b71-73d3-8c4f4c888e1a"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("ac3572c5-2869-428e-6850-8354fa977779"), 0.4m, new Guid("7164d238-0fcb-beac-2b3e-f6c0efe69015"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("ac378507-318e-7105-f524-99ec218756de"), 1.0m, new Guid("408f53a9-3b97-90b7-8473-055a5d41a0be"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("acea2deb-088c-e718-be88-2e14bd44910e"), 0.4m, new Guid("4d1a1f2c-6f13-3ba0-ab64-fe84250d7267"), new Guid("983a359b-2e07-4350-bdb3-3568b0b85dda"), 1 },
                    { new Guid("ad181ea1-36c9-18b7-463c-2b847c07ab0a"), 0.4m, new Guid("b8a15341-bb29-7b7c-f6fa-c5971d8b89e3"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("b1d999e1-a612-58b8-dbc7-188486478449"), 0.4m, new Guid("c4d1770c-8e6a-ecc6-5a5f-e31ede9d62ed"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("b240d57f-946e-4432-9e69-236fe6cd8507"), 0.4m, new Guid("78533937-563f-c726-df24-c7d5d6b2aec3"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("b2d7c532-8a82-dcd7-73a5-0ff24657cf7e"), 1.0m, new Guid("8634ffa3-94d4-1e45-4462-b719400266fd"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("b463bf35-0ccd-5b92-9115-40400e4c7af5"), 1.0m, new Guid("6c58eb05-3525-056d-5962-d77d102f9207"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 0 },
                    { new Guid("b46be275-90ab-3723-d6f2-9d676b29148f"), 0.4m, new Guid("053ad886-7a89-d64d-8a9e-46381a60bc5c"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 1 },
                    { new Guid("b59031cd-be6a-21c2-8b63-849cbd431c7a"), 0.4m, new Guid("4d1a1f2c-6f13-3ba0-ab64-fe84250d7267"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 1 },
                    { new Guid("b80e9faa-d861-0e4e-f94a-3b6f686d70f4"), 1.0m, new Guid("ca3d6e67-3a30-a951-d5c3-cbe10ae06400"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 0 },
                    { new Guid("b835da55-f41a-886b-c1fb-d9e5edc07fcd"), 0.4m, new Guid("93f38a2b-1c2b-31cd-c278-9cf959d21ba3"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("b923d860-17d1-8f75-12e8-6c6492cce8f7"), 1.0m, new Guid("421b346b-b949-be3b-7f2c-26372211df7d"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("b9ba6b50-0612-a386-da04-509a7322d098"), 1.0m, new Guid("8b572c46-384e-4481-a3f4-4380487e64b5"), new Guid("983a359b-2e07-4350-bdb3-3568b0b85dda"), 0 },
                    { new Guid("ba1e3ffb-10f9-f8f5-2b13-94be7f3df5c7"), 1.0m, new Guid("25b9afa3-38cc-9a1e-bd89-4ee08d319413"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("ba6bdbcd-4759-4476-0ba2-e240d080c050"), 0.4m, new Guid("b708cd86-073e-94f5-d7ec-ebf1aa608b9e"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 1 },
                    { new Guid("baf8b212-9aef-472e-6c3d-873cf5b6b75f"), 1.0m, new Guid("2d8178ef-e61f-fe39-e3ac-7d2f4a748564"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 0 },
                    { new Guid("bb5fcb56-995a-a838-a72c-fb4580b87c87"), 0.4m, new Guid("861aaacc-7adc-fd78-c5ed-b7877b421581"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("bbbe173a-239d-e76f-3508-80329c20080f"), 1.0m, new Guid("a2082bd4-f7a4-c2f1-26f2-5f38e6e86332"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("bc2ac164-9cec-a8a0-0d88-580775986490"), 0.4m, new Guid("a875020c-9955-5df6-d24a-bf3a4ed4a078"), new Guid("bcb0bc9d-0d9f-89e8-f957-337f2614a9b1"), 1 },
                    { new Guid("bd392963-6f05-a7b4-3b8c-db358921a050"), 1.0m, new Guid("396a5f31-4114-a312-60bd-96111afbc44a"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 0 },
                    { new Guid("bd4978bf-fe1b-4592-14ec-86c726616dc4"), 0.4m, new Guid("023bc5af-3d3c-3701-ec42-66cea60be7d7"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 1 },
                    { new Guid("be1d6cb8-87d5-fa50-cc0c-2cb1ca33b201"), 1.0m, new Guid("90059af2-8ca9-1077-7bef-4b7d3c1ae9ef"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 0 },
                    { new Guid("bf65bcff-ba84-572a-f6d6-b6de4608a313"), 0.4m, new Guid("6c58eb05-3525-056d-5962-d77d102f9207"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 1 },
                    { new Guid("c50c781a-90c3-57d7-e02b-876b88c9f4fe"), 0.4m, new Guid("60585e44-151d-222c-0fa8-cba4a87d655a"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 1 },
                    { new Guid("c572b015-a3a9-0599-1d7e-039e3a5be6bb"), 1.0m, new Guid("46e9110f-fae3-5225-1f05-cc9d0abf91a1"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 0 },
                    { new Guid("c591d70e-221c-8529-9834-65be4d480cb0"), 1.0m, new Guid("a9445cf7-e1ce-9d5a-7bb4-53955ea003cb"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 0 },
                    { new Guid("c7d11f00-33e7-95cf-8b4b-92ab17bf2baa"), 1.0m, new Guid("4bc0ad35-644a-ebf0-8b6e-979829f3a10d"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 0 },
                    { new Guid("c81f5c8e-74d0-9639-e5df-48b089d5110f"), 1.0m, new Guid("053ad886-7a89-d64d-8a9e-46381a60bc5c"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("c83c2fb8-d7cf-0f30-c2e0-fc94a61ca4bb"), 1.0m, new Guid("ee605ae6-5a8e-0883-d970-43935b6c59d3"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("c8cb6599-3014-5fb4-5b2b-83365752f127"), 1.0m, new Guid("9c32a506-9e62-d6a2-eae4-edb3064de189"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("cac9b94b-5ae4-861c-d6c2-bd1b4631bc24"), 0.4m, new Guid("21310bf6-4257-119b-528d-d76067da068e"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 1 },
                    { new Guid("cadad6bd-433a-d03b-b60a-19e1b6fa0015"), 1.0m, new Guid("1058b3f7-daaf-44b3-85d3-e2133b4e5cde"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("caffc399-5a5c-8d40-82f3-bc0538044c59"), 0.4m, new Guid("4b589f5d-dd39-25fc-b56a-264d2f59e0cf"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 1 },
                    { new Guid("cb040d98-d00d-d35f-f76d-fa30261a9e77"), 0.4m, new Guid("b75cbbcf-99f5-489e-120e-32136bfebfc2"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("cb5c6333-d822-df51-72c6-39a495029ad5"), 0.4m, new Guid("2d8178ef-e61f-fe39-e3ac-7d2f4a748564"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("cb905702-25d5-bb7b-d6f3-6c5481014923"), 1.0m, new Guid("4be295d8-414f-8dcc-b97f-965e8d050343"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 0 },
                    { new Guid("cd374515-dcdd-c12a-3dd8-86c5c903fec8"), 1.0m, new Guid("84297ab4-d0e4-0871-4c23-85edf5956570"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("cf161c2e-3cbf-9f52-0d1c-72c5c9ebaf0b"), 0.4m, new Guid("6c58eb05-3525-056d-5962-d77d102f9207"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 1 },
                    { new Guid("cfdaa1d8-cdbb-dd2b-6870-e549f4def64f"), 0.4m, new Guid("4d5ec6ca-97e9-5792-b849-e8d74bd2e6fe"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("d02ef767-bcff-243d-f5a7-9b05a78bc984"), 0.4m, new Guid("fea8e52d-6f6c-c15d-0fac-39014417c43f"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("d0fd1934-1e88-204f-60dc-425c2e00c958"), 1.0m, new Guid("b9e5b3ae-91b7-e8e4-4306-8007c4a085e8"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("d1d27d92-e3d8-2568-e00e-5867f979aada"), 1.0m, new Guid("1b03d6d3-b2ed-7f2e-6644-2a175efe6568"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 0 },
                    { new Guid("d30a7b26-1ceb-04af-e38b-4a49fd9db13a"), 0.4m, new Guid("1b03d6d3-b2ed-7f2e-6644-2a175efe6568"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("d4793ab1-c808-00db-c3dc-4e63795cd95b"), 1.0m, new Guid("9c7f9d4d-d3b3-29f6-0fe5-841af531a446"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("d4c90f3d-7aa5-b149-cc71-be66b247f96d"), 0.4m, new Guid("7164d238-0fcb-beac-2b3e-f6c0efe69015"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 1 },
                    { new Guid("d4f83ac8-e3ad-516b-2c7f-e4700808cdf6"), 1.0m, new Guid("2ea83b13-d10a-cbcc-790a-cc421110d46d"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 0 },
                    { new Guid("d567a277-8646-3682-0a52-f28a4613eb84"), 1.0m, new Guid("4bc0ad35-644a-ebf0-8b6e-979829f3a10d"), new Guid("37a3dd84-a1db-46b4-357f-47478a17a912"), 0 },
                    { new Guid("d590ce54-e47d-ffa9-627b-05fa8ac26260"), 0.4m, new Guid("4dc0f160-35df-ac8c-f1b7-af3a64bdca0b"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("d5b0ffe1-125c-5253-b33d-b1d3f312fe9d"), 0.4m, new Guid("023bc5af-3d3c-3701-ec42-66cea60be7d7"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("d5be9f6a-e333-a3e0-5ccc-8585971f8062"), 1.0m, new Guid("0d154558-9d0e-8138-33d9-536ffb9432e5"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("d642529e-813a-096d-5dca-f6b6648b4a53"), 1.0m, new Guid("d41dc9d6-f8b9-1cd8-b3dd-b8878a560f65"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("d67e3e1a-2041-9baf-0073-f7d86597e734"), 0.4m, new Guid("86c64172-40d7-86de-caa0-29a1fa44ee0e"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("d6e3b7e2-4eaa-5c87-3d8b-dfe08cba3520"), 0.4m, new Guid("2e147828-59fb-4a5d-bf0e-d05b064c45d9"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("d6e55250-5034-3907-d4f4-4cd4af9e9d10"), 0.4m, new Guid("accf8b83-1d16-fa92-909a-80102ed2d3a8"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 1 },
                    { new Guid("d7a4c272-182b-6de2-c4eb-19168c2025cd"), 1.0m, new Guid("7bd96d3e-8489-883f-7d03-161c63dda919"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("d849e82d-319b-f661-753f-10919fb39c2b"), 1.0m, new Guid("180a7bc9-69b6-dab8-2325-e8f12c917061"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("d92cb543-b5d5-82eb-f021-7030e5c5a4d6"), 1.0m, new Guid("d1cfc3eb-bcc8-4e2c-9e83-8ebcf01fcd3a"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("d92ce00a-d1d0-f367-379f-44dcdde344ec"), 1.0m, new Guid("41303540-98c4-2902-eca0-5d9cb5ba4ca6"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 0 },
                    { new Guid("d98f810c-a432-a12c-b95a-82e80c93de89"), 0.4m, new Guid("ed003439-f256-a3cb-aa58-3efc2dc04a82"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("d9fc7f88-6a2f-80ac-b48b-767720e75ab9"), 0.4m, new Guid("396a5f31-4114-a312-60bd-96111afbc44a"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 1 },
                    { new Guid("db1bbc6f-7a5e-94f1-f794-561f9488b7b5"), 0.4m, new Guid("accf8b83-1d16-fa92-909a-80102ed2d3a8"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("dbb3a0dc-fa5b-7b06-c07e-bd2c6fdd99e0"), 1.0m, new Guid("153bd9c4-6991-0246-6c44-ed0caedd25a2"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 0 },
                    { new Guid("dbdbe9fd-2bcf-4b9c-4921-347345392cb8"), 0.4m, new Guid("e868adcb-aea1-bca9-1f24-a9d524cbb20e"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 1 },
                    { new Guid("dbebb5df-f861-65fd-feb1-939404f69aa1"), 1.0m, new Guid("202e04c4-ec37-1e25-32b0-e7ecb09ac090"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 0 },
                    { new Guid("dcb0eaf4-34a3-b85a-1ec6-c4a684597146"), 1.0m, new Guid("38ccd0eb-9476-583c-5e99-25654a59eddb"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("dd8b8731-fbe7-6ab8-213b-f604b63d732c"), 0.4m, new Guid("105a15ee-60f5-ff7f-20ca-799591c0e169"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("dd8eb547-edcd-0b1a-237d-17af4e5cedc4"), 0.4m, new Guid("4dba253f-9f50-4e88-bda5-ef5c7071f416"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("ddbdd26d-2e1e-9f03-313a-672ebf475111"), 1.0m, new Guid("202e04c4-ec37-1e25-32b0-e7ecb09ac090"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("e0ea4b93-8546-05ca-1da0-f01b232d4b31"), 1.0m, new Guid("b75cbbcf-99f5-489e-120e-32136bfebfc2"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 0 },
                    { new Guid("e1eb5928-81d7-635b-60c9-8875695b20db"), 1.0m, new Guid("4dba253f-9f50-4e88-bda5-ef5c7071f416"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 0 },
                    { new Guid("e318d2f2-4490-bb6c-4fdc-1fa59ab03ecf"), 1.0m, new Guid("476cd99b-8358-13e5-8ade-84a1ed12511c"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 0 },
                    { new Guid("e43483bb-d542-747a-bbe3-89095a418974"), 0.4m, new Guid("c19305bc-400e-4d54-14f9-a649e1f69f17"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 1 },
                    { new Guid("e47438d8-a471-4a77-ea5c-c6f21e488a0e"), 0.4m, new Guid("4bc0ad35-644a-ebf0-8b6e-979829f3a10d"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("e64e9981-cc4f-a2bb-15e7-eee6a9e973be"), 1.0m, new Guid("3cf3597c-0444-9b71-73d3-8c4f4c888e1a"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("e6725f14-08fa-6197-2281-99ec9448efeb"), 1.0m, new Guid("bd66bc8d-b61e-083a-d745-49b89e9bbaec"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 0 },
                    { new Guid("e69a9138-62c1-b7f1-a864-4f967850e6e3"), 0.4m, new Guid("5fbcfdb8-df7f-f712-d8f0-1eb51118bb43"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("e6db71e0-ae8a-2105-b026-d390383212d4"), 0.4m, new Guid("153bd9c4-6991-0246-6c44-ed0caedd25a2"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("e6ff30f6-68a0-fbfd-b0cd-2a714c3428cd"), 0.4m, new Guid("23eca012-f379-86c2-bd2a-07e73da37e57"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("e73e7dbb-846a-69f2-8f92-29eedcbb3610"), 0.4m, new Guid("38ccd0eb-9476-583c-5e99-25654a59eddb"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("e8320af5-f897-99e9-fcd0-1573ecccf8bd"), 0.4m, new Guid("ed003439-f256-a3cb-aa58-3efc2dc04a82"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 1 },
                    { new Guid("e96afbc7-d12d-5c44-b60b-c38fefb0784f"), 1.0m, new Guid("d83c8717-a4a5-7c44-a3ba-dad36745d3a1"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("e96d6787-5c09-ee93-7bde-ef7cd87197a0"), 0.4m, new Guid("bf44d79e-2891-d9a8-8312-9c40942acca9"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 1 },
                    { new Guid("ea5ac3d6-3fb7-cc63-f018-f22d5392f611"), 1.0m, new Guid("af179f27-3579-a9ad-0062-45d20073ab6c"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 0 },
                    { new Guid("eab00fb4-8098-e018-51d6-5671e6c9d679"), 0.4m, new Guid("7add62bb-f085-ede9-34c9-73c7d6ee3833"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 1 },
                    { new Guid("eac054d6-77ab-1844-11c4-e33190da7110"), 0.4m, new Guid("8634ffa3-94d4-1e45-4462-b719400266fd"), new Guid("8d35a1be-c85a-eac7-c2b5-757c17cebb0d"), 1 },
                    { new Guid("eb070a62-2c00-2c43-4132-7ad6915b4ec5"), 1.0m, new Guid("b75cbbcf-99f5-489e-120e-32136bfebfc2"), new Guid("37a3dd84-a1db-46b4-357f-47478a17a912"), 0 },
                    { new Guid("ed13cbdd-6395-719d-2bf5-63d8821e491e"), 1.0m, new Guid("bf44d79e-2891-d9a8-8312-9c40942acca9"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 0 },
                    { new Guid("ee253c1a-c129-a978-66d6-9622948cc34d"), 0.4m, new Guid("d1cfc3eb-bcc8-4e2c-9e83-8ebcf01fcd3a"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("ee9d13ba-88c1-009b-ba10-f35208d1f2e5"), 0.4m, new Guid("3eeba987-f873-ba13-c353-c02844ef25f0"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("ef646d03-715a-9f90-3083-daca8ee67343"), 0.4m, new Guid("1b03d6d3-b2ed-7f2e-6644-2a175efe6568"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 1 },
                    { new Guid("efbf477a-4f6a-ef95-de99-7248c803c727"), 0.4m, new Guid("268d8dad-43c7-5e83-24b5-a07387ae8af9"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 1 },
                    { new Guid("f0d6f290-0d41-8907-c39a-b1c32723a5b3"), 0.4m, new Guid("af179f27-3579-a9ad-0062-45d20073ab6c"), new Guid("8d35a1be-c85a-eac7-c2b5-757c17cebb0d"), 1 },
                    { new Guid("f1074ba0-ffaa-1593-413a-743f1e473e54"), 0.4m, new Guid("23eca012-f379-86c2-bd2a-07e73da37e57"), new Guid("8d35a1be-c85a-eac7-c2b5-757c17cebb0d"), 1 },
                    { new Guid("f3183ddd-bf61-32f3-3c2b-7ac0e84c583d"), 1.0m, new Guid("d41dc9d6-f8b9-1cd8-b3dd-b8878a560f65"), new Guid("1678d70c-25a5-db43-d7f4-a7ea30040382"), 0 },
                    { new Guid("f3765833-ce1d-5d71-d1ba-a0004b5f26d4"), 0.4m, new Guid("41303540-98c4-2902-eca0-5d9cb5ba4ca6"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("f37d0e82-0b87-bee8-4332-261c421da9cb"), 1.0m, new Guid("fa286022-acf1-e07a-b4d6-7d0168c5419d"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 0 },
                    { new Guid("f4e4fee1-8519-2a66-3c23-bfc0daeeb6b5"), 1.0m, new Guid("3eeba987-f873-ba13-c353-c02844ef25f0"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("f4f92658-dccb-1758-e7d1-1c2f85cd1624"), 1.0m, new Guid("84297ab4-d0e4-0871-4c23-85edf5956570"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("f5827d6e-a14b-99d7-5182-a5e0170ae940"), 0.4m, new Guid("2863f1c1-b77c-0b4d-b2f8-ee5f451c3b37"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("f5ba457b-21b6-3cb1-02b0-d60c5231678c"), 0.4m, new Guid("6cd088d2-41a1-d172-975f-126ea52c2584"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("f5bf2784-2707-00f6-82eb-97b0200ca57d"), 1.0m, new Guid("78533937-563f-c726-df24-c7d5d6b2aec3"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 0 },
                    { new Guid("f609895d-29b3-9e2d-dbfd-b7647cd0ffac"), 1.0m, new Guid("06d5b353-c57f-3a85-7474-95481ed59182"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("f6625983-75f6-6c3c-7224-1384018032ad"), 0.4m, new Guid("bc143f04-fe60-4860-1bc9-2be84803be5d"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("f6c20ee2-a118-4729-8011-06ec34fb5e62"), 1.0m, new Guid("fe0d3b73-4714-17a5-ef86-354da5b1bcc7"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 0 },
                    { new Guid("f77320a1-1770-36bc-7c24-78da39e04e0d"), 0.4m, new Guid("bd66bc8d-b61e-083a-d745-49b89e9bbaec"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 1 },
                    { new Guid("f86fea58-0196-367c-f753-7648e41898e5"), 0.4m, new Guid("053ad886-7a89-d64d-8a9e-46381a60bc5c"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("fa464bc4-5105-e388-ee02-a098e6941330"), 1.0m, new Guid("eb7f4ce6-bd75-dfa7-456b-78e7d2144794"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("fc117b37-614b-1e16-1595-4837fa4f2c14"), 1.0m, new Guid("4fa98450-c23d-c59c-4bf4-bba35d6fb18f"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 0 },
                    { new Guid("fda31caa-51f8-900e-5cd9-25ef549e4cc6"), 0.4m, new Guid("c4d1770c-8e6a-ecc6-5a5f-e31ede9d62ed"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("fdde750c-c767-6d16-e693-76648aa903de"), 0.4m, new Guid("b9e5b3ae-91b7-e8e4-4306-8007c4a085e8"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 1 },
                    { new Guid("fea6c0ec-0f0a-4bed-d270-561b74f3b9ef"), 1.0m, new Guid("6cd088d2-41a1-d172-975f-126ea52c2584"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("fedafe17-98a1-2919-6966-e83d1418b6b9"), 0.4m, new Guid("8b572c46-384e-4481-a3f4-4380487e64b5"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("ff0490d4-5718-e959-89ad-4e1b44b7e49b"), 1.0m, new Guid("86c64172-40d7-86de-caa0-29a1fa44ee0e"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 0 }
                });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "Category", "CreatedAt", "DefaultIncrementKg", "DefaultRestSeconds", "EquipmentId", "Instructions", "IsArchived", "MediaUrl", "Name", "OwnerId", "Type" },
                values: new object[,]
                {
                    { new Guid("171ccca6-7123-c9f2-b94e-b32fc12d097b"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 75, new Guid("8e7b4a1b-c22e-904d-d102-7f61089ad5c3"), "Brace in a half-kneeling stance and press the bar up and forward.", false, null, "Half Kneeling Landmine Press", null, 0 },
                    { new Guid("1dfad4d7-5270-7ad9-bd72-54e45b62a7d8"), "Conditioning", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 90, new Guid("d44984a4-01aa-fa50-7652-f83c8bad469e"), "Walk backward with steady steps and keep continuous tension on the straps or handles.", false, null, "Backward Sled Drag", null, 4 },
                    { new Guid("380497ab-2e98-e211-c7c8-53f58eda71c1"), "Plyometrics", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 75, new Guid("91328842-7e67-521f-17b2-37ddff0c912e"), "Jump to a stable landing on the box and step down under control.", false, null, "Box Jump", null, 1 },
                    { new Guid("4f199800-8c8d-8153-7729-07e8e6b73a1f"), "Conditioning", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 90, new Guid("d44984a4-01aa-fa50-7652-f83c8bad469e"), "Brace into the handles and drive the ground backward with short powerful steps.", false, null, "Sled Push", null, 4 },
                    { new Guid("6618c194-1d6f-a382-2ada-75f6936a4596"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 75, new Guid("134c1c3d-365b-71c5-fa42-e3dfdc967dd3"), "Keep the rings steady and pull the chest between them with a rigid body line.", false, null, "Ring Row", null, 1 },
                    { new Guid("7280bfe2-90f8-736c-295b-6a206e9360ec"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("fa6e4468-2e50-8944-d82d-8ae5fa6b0311"), "Keep the body rigid and pull the chest toward the handles.", false, null, "Suspension Trainer Row", null, 1 },
                    { new Guid("7ccae516-396a-7d31-1420-d97c3167f6d1"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 120, new Guid("134c1c3d-365b-71c5-fa42-e3dfdc967dd3"), "Stabilize the rings close to the body and press from a controlled depth.", false, null, "Ring Dip", null, 2 },
                    { new Guid("99b1260b-9d15-1a42-792b-3d57f62a5fc6"), "Conditioning", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("2c471d39-7446-8e2e-39f4-82a443032c3e"), "Maintain an athletic stance and alternate quick waves without losing trunk position.", false, null, "Battle Rope Waves", null, 3 },
                    { new Guid("b9d5355a-accc-c4af-e1ae-e158d22eac1d"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("fa6e4468-2e50-8944-d82d-8ae5fa6b0311"), "Lean into the straps, lower between the handles and press while keeping them steady.", false, null, "Suspension Trainer Chest Press", null, 1 },
                    { new Guid("ba158b19-5a08-250d-7de5-83ee05762d49"), "Core", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("5dd58442-ba55-e4a5-d869-a66d601b193f"), "Support the lower back on the ball and curl the ribs toward the pelvis.", false, null, "Stability Ball Crunch", null, 1 },
                    { new Guid("e711c52a-9f07-5780-4bb7-3353ccb9854f"), "Conditioning", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("c9f465e3-aee6-83df-cb8e-e94a305112ad"), "Reach tall, brace and drive the ball forcefully to the floor using the whole trunk.", false, null, "Medicine Ball Slam", null, 1 },
                    { new Guid("f264e63e-b975-e4fb-e57b-6ed4eeaea5e0"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 75, new Guid("8e7b4a1b-c22e-904d-d102-7f61089ad5c3"), "Stand across the bar and row the sleeve toward the hip with minimal torso rotation.", false, null, "Meadows Row", null, 0 },
                    { new Guid("f84f3a6a-9d58-2f85-97a8-46d88b59a683"), "Conditioning", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("c9f465e3-aee6-83df-cb8e-e94a305112ad"), "Stand from the squat and throw the ball to the target in one continuous motion.", false, null, "Wall Ball", null, 1 },
                    { new Guid("f860fb57-a804-f4fa-2fed-25061b682cbb"), "Core", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("5dd58442-ba55-e4a5-d869-a66d601b193f"), "Roll the forearms forward while maintaining a braced trunk, then pull back.", false, null, "Stability Ball Rollout", null, 1 }
                });

            migrationBuilder.InsertData(
                table: "ExerciseMuscles",
                columns: new[] { "Id", "ContributionWeight", "ExerciseId", "MuscleId", "Role" },
                values: new object[,]
                {
                    { new Guid("0452c58b-ab1d-9c11-42b7-f5128e492498"), 1.0m, new Guid("f84f3a6a-9d58-2f85-97a8-46d88b59a683"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("089383ac-d51b-04e1-7b7c-400b33ea469d"), 0.4m, new Guid("f860fb57-a804-f4fa-2fed-25061b682cbb"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("0b64584b-de31-9e5a-d20c-bd837c4a0026"), 1.0m, new Guid("ba158b19-5a08-250d-7de5-83ee05762d49"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 0 },
                    { new Guid("0c4cb86c-7e3a-44e0-6473-c95a7b01e8ae"), 0.4m, new Guid("b9d5355a-accc-c4af-e1ae-e158d22eac1d"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("10f8959e-61b4-6a9b-8a38-9de647b3646f"), 0.4m, new Guid("ba158b19-5a08-250d-7de5-83ee05762d49"), new Guid("bcb0bc9d-0d9f-89e8-f957-337f2614a9b1"), 1 },
                    { new Guid("158f36bd-cb0d-71d7-4050-cb29d250d074"), 0.4m, new Guid("99b1260b-9d15-1a42-792b-3d57f62a5fc6"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 1 },
                    { new Guid("1b6ca3a8-7573-37f9-09db-b6c46563c96c"), 1.0m, new Guid("6618c194-1d6f-a382-2ada-75f6936a4596"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 0 },
                    { new Guid("2369cfd8-5a33-9cba-d188-5e0fbcf732e7"), 1.0m, new Guid("7280bfe2-90f8-736c-295b-6a206e9360ec"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("2f181a8d-b9d5-217f-2f85-8be07c4741a4"), 1.0m, new Guid("f264e63e-b975-e4fb-e57b-6ed4eeaea5e0"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("31200f31-b9db-df94-30d2-e71cfee096cc"), 0.4m, new Guid("f84f3a6a-9d58-2f85-97a8-46d88b59a683"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("32c39c7c-4494-c64a-9844-946c5ed918e0"), 0.4m, new Guid("f264e63e-b975-e4fb-e57b-6ed4eeaea5e0"), new Guid("8d35a1be-c85a-eac7-c2b5-757c17cebb0d"), 1 },
                    { new Guid("34744e61-0385-330c-1f99-d96b86c6b5a4"), 1.0m, new Guid("4f199800-8c8d-8153-7729-07e8e6b73a1f"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("4234a571-1c6d-079f-a5dd-9613d17a061c"), 1.0m, new Guid("380497ab-2e98-e211-c7c8-53f58eda71c1"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("424d9b5e-09ad-bfae-e1e0-af0970caeec5"), 1.0m, new Guid("f84f3a6a-9d58-2f85-97a8-46d88b59a683"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 0 },
                    { new Guid("4305ba8e-fcb1-2139-ddad-dee735bef74a"), 1.0m, new Guid("e711c52a-9f07-5780-4bb7-3353ccb9854f"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("49d99458-694f-b902-941a-6577dd1b12f0"), 1.0m, new Guid("99b1260b-9d15-1a42-792b-3d57f62a5fc6"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 0 },
                    { new Guid("4a2cd423-13ec-018c-3ddf-5265519bfdbe"), 0.4m, new Guid("6618c194-1d6f-a382-2ada-75f6936a4596"), new Guid("8d35a1be-c85a-eac7-c2b5-757c17cebb0d"), 1 },
                    { new Guid("4a608f9e-6316-457e-880a-a687f374acfc"), 0.4m, new Guid("171ccca6-7123-c9f2-b94e-b32fc12d097b"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 1 },
                    { new Guid("53574ab3-88dc-9f3f-6621-d8f8df7ab928"), 0.4m, new Guid("e711c52a-9f07-5780-4bb7-3353ccb9854f"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 1 },
                    { new Guid("57e7d03a-8b4c-6638-e258-feb2d73c54b4"), 0.4m, new Guid("4f199800-8c8d-8153-7729-07e8e6b73a1f"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 1 },
                    { new Guid("592da3a7-2101-ed9d-b51e-73fa068fdbcc"), 0.4m, new Guid("7280bfe2-90f8-736c-295b-6a206e9360ec"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("599fc55e-0b91-eb33-f03a-0f53ea84c215"), 0.4m, new Guid("1dfad4d7-5270-7ad9-bd72-54e45b62a7d8"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 1 },
                    { new Guid("610eab68-de4d-93ca-46c3-618c647a0ac7"), 0.4m, new Guid("f84f3a6a-9d58-2f85-97a8-46d88b59a683"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 1 },
                    { new Guid("696851e7-8b95-cbc2-228d-b585128f4db0"), 0.4m, new Guid("4f199800-8c8d-8153-7729-07e8e6b73a1f"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("6f76b13e-27fe-098e-76ea-888f73acb3b1"), 0.4m, new Guid("171ccca6-7123-c9f2-b94e-b32fc12d097b"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("70e9accb-cec2-f1f7-1898-e5939031c918"), 0.4m, new Guid("6618c194-1d6f-a382-2ada-75f6936a4596"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("73b7bc66-0039-86d0-8b82-d4763a92711a"), 0.4m, new Guid("380497ab-2e98-e211-c7c8-53f58eda71c1"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 1 },
                    { new Guid("74be9467-2f9c-ad6c-238e-f10d8defbf35"), 0.4m, new Guid("7ccae516-396a-7d31-1420-d97c3167f6d1"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("7b75ad14-7512-488d-5986-37ea2798cbb1"), 1.0m, new Guid("4f199800-8c8d-8153-7729-07e8e6b73a1f"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("7e9d3402-866b-7c9c-ce8c-fcdb47bfd693"), 0.4m, new Guid("1dfad4d7-5270-7ad9-bd72-54e45b62a7d8"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 1 },
                    { new Guid("7ea3a435-af40-1366-b65f-8f4efcf55e58"), 0.4m, new Guid("f84f3a6a-9d58-2f85-97a8-46d88b59a683"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("80d15ed2-2cf3-7e5e-f880-0853e9817237"), 1.0m, new Guid("380497ab-2e98-e211-c7c8-53f58eda71c1"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("8aa19e3f-5ee3-9a0e-2e53-803db3d4485d"), 1.0m, new Guid("f264e63e-b975-e4fb-e57b-6ed4eeaea5e0"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 0 },
                    { new Guid("901b20a4-c6dd-d998-ef02-18ddbf7d7dee"), 0.4m, new Guid("f860fb57-a804-f4fa-2fed-25061b682cbb"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 1 },
                    { new Guid("917fbcb1-dcb7-91c4-f0e6-c438bcde27fd"), 0.4m, new Guid("e711c52a-9f07-5780-4bb7-3353ccb9854f"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("959caff7-ecb9-eafe-018d-e3e1bd39943d"), 0.4m, new Guid("b9d5355a-accc-c4af-e1ae-e158d22eac1d"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("9dd8d4f4-e93d-2823-bd28-36a52eee5ffa"), 1.0m, new Guid("6618c194-1d6f-a382-2ada-75f6936a4596"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("a5dcc2e9-1a24-1711-0a6b-f847c658e9c3"), 1.0m, new Guid("f860fb57-a804-f4fa-2fed-25061b682cbb"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 0 },
                    { new Guid("aac0e1ec-ab9c-5dcf-fb27-70a25cd85cad"), 0.4m, new Guid("4f199800-8c8d-8153-7729-07e8e6b73a1f"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 1 },
                    { new Guid("aca7998e-f8d1-994c-4509-d434a89d52d2"), 0.4m, new Guid("6618c194-1d6f-a382-2ada-75f6936a4596"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("acfce72e-db98-1b0c-256a-431df6deb14a"), 0.4m, new Guid("380497ab-2e98-e211-c7c8-53f58eda71c1"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 1 },
                    { new Guid("b37874be-ee51-f7fd-442f-200462b55ea0"), 1.0m, new Guid("99b1260b-9d15-1a42-792b-3d57f62a5fc6"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 0 },
                    { new Guid("b3f5223d-9d73-2602-375b-8b10f5a044c1"), 1.0m, new Guid("1dfad4d7-5270-7ad9-bd72-54e45b62a7d8"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("b664bcf2-d427-d076-76dc-bf81692c2493"), 1.0m, new Guid("b9d5355a-accc-c4af-e1ae-e158d22eac1d"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("b6749451-a70a-5f1f-0baa-432dcdcf6531"), 0.4m, new Guid("f264e63e-b975-e4fb-e57b-6ed4eeaea5e0"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("b8cb6c6f-2bd3-5117-66eb-de5a927fa3c8"), 1.0m, new Guid("7280bfe2-90f8-736c-295b-6a206e9360ec"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 0 },
                    { new Guid("b905d91b-e526-d5c4-5cd4-fb49abc850d8"), 0.4m, new Guid("7280bfe2-90f8-736c-295b-6a206e9360ec"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("bc793aa7-10b5-99df-a6f6-de77b7c7b511"), 0.4m, new Guid("b9d5355a-accc-c4af-e1ae-e158d22eac1d"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("c1400000-0c88-03fb-56f8-bcfdd71cae42"), 1.0m, new Guid("7ccae516-396a-7d31-1420-d97c3167f6d1"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 0 },
                    { new Guid("ca75b87d-622d-3f82-af10-b8f9a3b3506d"), 0.4m, new Guid("99b1260b-9d15-1a42-792b-3d57f62a5fc6"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("d3102c09-2fd1-d073-d531-6a05347c83ea"), 1.0m, new Guid("171ccca6-7123-c9f2-b94e-b32fc12d097b"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 0 },
                    { new Guid("d655bd93-250f-33f5-d7b3-5a3550f3c8e4"), 0.4m, new Guid("171ccca6-7123-c9f2-b94e-b32fc12d097b"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("e40c0725-2570-fc47-7e86-a2a445c4e6be"), 0.4m, new Guid("7ccae516-396a-7d31-1420-d97c3167f6d1"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("e9eb3810-1317-6340-8ae3-68293e3c979a"), 1.0m, new Guid("7ccae516-396a-7d31-1420-d97c3167f6d1"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("f42cb167-1a6c-5120-d3b4-005766f05574"), 1.0m, new Guid("e711c52a-9f07-5780-4bb7-3353ccb9854f"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("029ede44-5c88-7469-3ef3-57414ae437dd"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("02aa8ccc-1122-8e37-8011-9b315e437c54"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("02b826a2-271e-344c-0d09-0c614ebf324f"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("031383dc-a56b-3d3b-7800-df6d11631c30"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("0452c58b-ab1d-9c11-42b7-f5128e492498"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("0671bfb2-0a7b-49dd-e942-97ecd39b215b"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("068ab489-4456-04e8-06b7-9da5c5c3aae4"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("06c512a9-070e-212e-3ae8-f9b6259f1d66"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("072f2d87-023d-f159-ba14-cf929df8ec28"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("07bd70d1-7158-9b8d-7648-34a22cbc48e5"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("089383ac-d51b-04e1-7b7c-400b33ea469d"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("09dbf87a-98df-083e-0ba9-6299903975e9"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("0b2953d0-9f67-b22c-b9bb-00bc7f7573be"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("0b64584b-de31-9e5a-d20c-bd837c4a0026"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("0c4cb86c-7e3a-44e0-6473-c95a7b01e8ae"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("0dc62918-a3f6-c102-50f7-4c9a2af165ed"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("1019ea7e-a971-3e7d-126b-de70f70f5714"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("108d1f81-b52f-fe4c-11d1-17291cf791d9"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("10e57a7e-8736-0032-6a45-5aacb8841d72"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("10f8959e-61b4-6a9b-8a38-9de647b3646f"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("111316fb-796a-98c9-b3e2-1ce3652da989"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("1123af59-c95d-5df4-8645-b53c94f2ed68"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("112607b0-1033-c76d-16ec-5292fa53e633"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("11867af7-abba-eac6-7866-599e17b7218b"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("119189e7-6e01-9b45-3239-9908c9ea56d5"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("122c89fc-3e65-193e-5e2b-900d22b03193"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("129fa8b8-755f-bf88-5bb8-ed716c4746cf"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("1352f236-ae7a-a3f9-1317-b340253db7f5"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("14366be8-4950-fd34-666b-9f8b22867c97"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("153276a6-3124-49f1-0e8f-ff03dc675198"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("158f36bd-cb0d-71d7-4050-cb29d250d074"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("15ad3ca8-bd92-7ab1-71df-558ad8c13bec"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("193c4bc5-5481-2377-9845-1b99c1bfbecd"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("194aa0e1-3393-fdd3-6cbd-0662fc7ada3e"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("1b6ca3a8-7573-37f9-09db-b6c46563c96c"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("1bc10305-3e4f-31b2-b977-f46f041b3ab4"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("1e03e84f-ecad-60d4-a066-450302ee229e"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("1e37752c-f831-6e4e-fabd-5a14f2ffe723"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("1f47af5f-441b-d458-fecf-b5e6b4338229"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("200731ab-bfe4-38d6-0c17-16728702315c"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("2009fc9a-0123-d4f0-0228-e6ee7b82c289"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("2026b894-07ee-48ad-dd83-9f21565839e6"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("20385932-d201-2eca-5b1a-789dea6d22fe"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("220ce8b9-7efe-f7f2-8850-78ba3c4f3a8c"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("2252d74c-3b5b-25ef-ba7e-eed1e6f833e5"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("225dec63-1227-b120-224e-6fa57d879d2f"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("2369cfd8-5a33-9cba-d188-5e0fbcf732e7"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("24e10edd-c7cd-81b8-63cc-516094ef2c22"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("262659d9-336a-c563-230e-306ae4029997"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("29317490-0fc2-b26c-99a9-d9a1f71c62d7"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("29f7ee4e-637b-33e0-922f-e50f77c52ccb"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("2a398414-c661-86f2-bd7e-14dcab613708"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("2a8ebb70-ed6a-8ba2-85ef-201c28ce3732"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("2b51b098-0b01-45c6-e9ca-e2b7728550f0"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("2c0aacc0-f1ea-b1e4-26df-a3e234bb813e"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("2c20b3a0-2b26-1450-90b7-f24950b967ba"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("2d5e543f-57ec-5abb-b389-100f04d06bcf"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("2e897c98-1f38-d070-e12a-fd856b496c9c"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("2e9cd74a-589d-12f2-f86a-5af9b3cf600a"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("2f181a8d-b9d5-217f-2f85-8be07c4741a4"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("2f211cf6-c657-b515-0549-67b184678af3"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("30179f63-ae43-27b0-a911-a7c337862cb8"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("304a0a5c-fff4-8816-e892-84acf642b022"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("3098f5d2-8da5-38ff-9b44-6448e92cc6d5"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("30e0d0a5-83ee-40e0-19c9-6bc693a508ac"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("31200f31-b9db-df94-30d2-e71cfee096cc"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("3195a3a7-c235-7596-f03b-f905da8c810d"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("31a835e6-81e2-77a2-7c37-55f7ea39fa95"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("32c39c7c-4494-c64a-9844-946c5ed918e0"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("3330a00c-f92a-b99c-ab46-6f69ba10a43c"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("345e2369-561e-0cc3-5480-ae5617d8da62"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("34744e61-0385-330c-1f99-d96b86c6b5a4"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("3527d35f-31dc-e62c-22a3-512c50289b26"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("35aa3b69-b194-0f82-8615-3471c3a439ba"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("36ee6068-5e70-f198-0367-250d9054c830"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("3a08a963-889a-c0d3-806e-b486ee775378"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("3a839884-252a-e2ad-1fc5-7288ce0e28b1"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("3a9cb662-b260-5da2-c545-b9a69d591e92"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("3ac3cc1a-ed18-4a6e-61c1-f804049efcf9"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("3aec4630-3395-c172-fe88-860955ce3aeb"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("3cfea475-a165-b9ca-2b0c-5c23ff308db1"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("3d7111eb-83a9-534b-eacc-94f89d47c1c1"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("3dcf7697-3000-b38e-3f3a-54180f1cf236"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("3e28d201-5a08-b57b-25a7-19a45b9d8149"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("3e9b589e-ea24-7512-d9a7-21018b4cb326"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("3ed9897e-35e3-7408-bb04-5b0018c978d9"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("401836b8-6593-92f2-35e1-84a7d4db82de"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("4053e2b6-44ac-417e-26b7-84276d0f5586"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("416714d5-2e86-9e7a-d654-f9ac764cc335"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("416a34c3-2cf1-5662-1a01-f7771c30d334"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("41b38429-acc8-bc51-035c-9e541e8ae525"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("420ce2b5-8da6-7d1b-ecb9-81e372952f6e"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("4234a571-1c6d-079f-a5dd-9613d17a061c"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("424d9b5e-09ad-bfae-e1e0-af0970caeec5"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("4305ba8e-fcb1-2139-ddad-dee735bef74a"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("437e214c-97bc-410f-2b88-9f35b8898111"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("4420776e-1546-4b2f-49b4-1fe66ae7e746"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("4461f0f9-7b2e-8fc5-cf56-c803ecc51dbd"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("44d7e72d-10db-e7e5-dc8a-fc2ddc3d8c97"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("44efe63d-e1ab-520a-e86c-949ac8034861"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("47b12ee6-9a4c-08b0-fba3-c0d486917f74"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("47dc64db-8d39-42c7-a288-c0f152a85691"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("48163976-2e29-27a9-43a6-1567d6e75d4e"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("481960d8-617a-836f-cfe3-13668661759a"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("488eedd1-82ba-1b2e-c184-8bd34ea20401"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("49125acb-973d-094a-f11e-3915badfd00c"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("49971eaa-8085-9c96-324a-ffac7524232a"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("49d99458-694f-b902-941a-6577dd1b12f0"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("4a2cd423-13ec-018c-3ddf-5265519bfdbe"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("4a55b6cd-a61d-bcee-67f1-d40b40ce37f3"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("4a608f9e-6316-457e-880a-a687f374acfc"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("4af85e9c-53e9-fbdd-3e1d-a80f6b47af00"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("4b904600-fe0c-c7b8-0150-5285ba1b7061"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("4bd0da04-db54-95f7-a65f-367cb3eba0d6"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("4c15287d-32bd-2e9d-1159-47bb19689a08"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("4c5ae591-0510-f3ec-b341-9452524d034f"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("4e50ad1c-1ec9-c414-c538-2242623337f0"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("4f52ba07-c2bb-57bb-ccd9-bd979a6a09e5"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("4f5d7fde-3f54-4b07-d12a-e6654a87c4f2"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("506ccd00-7925-aada-bdb5-382b4b4dde33"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("531a0500-2ff4-f903-7971-f33bf63642ee"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("53469bf4-1f96-eae5-2d3e-b8b5bf7808e0"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("53574ab3-88dc-9f3f-6621-d8f8df7ab928"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("54049c3f-6a4b-8a21-8468-9c6e9abb7ed3"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("54a72936-0991-c0cf-ce35-6cd1adecb886"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("54db888c-bb14-13fa-9bb6-636c8310a536"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("554bf92d-f7cc-6d7a-7a13-27945285c334"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("5556482d-0b0d-3718-9dd7-214ca696eb6b"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("55913cb9-faa6-1c52-29a9-c6d745dd3341"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("55c01ffd-7938-6fda-1447-ece6364c2a22"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("55fd04d6-952c-e995-5cf7-18793c30b3cf"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("56a85416-23d0-1104-3ad4-cd2c70c2e8df"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("57893efa-fbaa-e44d-7d71-bd942ee4eca6"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("57e7d03a-8b4c-6638-e258-feb2d73c54b4"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("5886c00c-f3c4-0121-ae8e-89c6c1938c5e"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("592da3a7-2101-ed9d-b51e-73fa068fdbcc"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("599fc55e-0b91-eb33-f03a-0f53ea84c215"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("59e07a00-7f80-3aae-3f5e-35441a4b7742"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("5a8057de-485e-6fa2-1b99-2b1fce03dd4e"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("5aa879b4-5e70-3825-c60c-e15a7bf89f03"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("5ab03a10-4f6e-b18a-4164-d8279d6a8ecb"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("5ae5c0c7-610b-701e-f5c6-b5fb963423c8"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("5b41f37e-48d3-93d7-086b-bfa36167ae7d"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("5b995436-0c2d-9e2b-f85d-757ae8e89051"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("5bead454-b028-b742-2562-06434f58f965"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("5c964c7d-46dd-ea5a-d88a-040eeef8e723"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("5e0770d4-9526-9ce9-16f7-fd23d480e742"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("5e148b83-55c1-455e-c140-2b8452ab45f0"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("5e6167d7-c25c-47d2-a44b-a3ccf21e7531"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("5e788418-2dc0-1b42-df42-88e58b87a8cb"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("5ec29e16-f242-652b-eef6-c9045bc7453e"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("5f5b22df-9300-c5c2-5ce2-d7db422fd2f4"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("5f66365e-f70d-0618-0bb8-38cb5d1ecb4a"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("60f23c64-57ca-fb47-6e23-a02b5ed8b666"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("610eab68-de4d-93ca-46c3-618c647a0ac7"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("6152f3da-14c1-c516-25a4-38bf02c70a67"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("61e0144b-5595-108f-2e8c-47adfba8422d"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("63a55b0c-a1db-111e-11cb-4020133874e4"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("643ef453-8683-d284-fdc8-4d6bcdefd616"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("645a01fa-d151-f7cb-b763-de5c739cf57f"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("66711e0d-1a66-c643-ae58-494160bcf7fa"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("66a774e0-03dd-bf0b-08b2-91692ea9b8ef"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("678f4713-c5ce-7de7-5c0a-a22e94ed6a17"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("68d059c5-92c4-e6b3-a289-bd065249a404"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("68df2d1b-c066-423f-d972-d5c78b2dcb06"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("68fbf2ce-22f2-893d-2e39-f6cc62d9aa94"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("696851e7-8b95-cbc2-228d-b585128f4db0"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("69ed86fe-4ca3-e8d1-d6da-3a656b57b7ad"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("6ab8729b-3195-bfc4-3a8d-069c90a7f0ea"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("6b3538ce-a49c-b94b-1c47-4720f22dbf6e"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("6bc58661-9f47-b6b0-321e-c149c37164b2"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("6bea8b4a-8807-a77d-3c4a-db58c6357d92"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("6c5c3256-1c27-631b-7abe-2362e04f51d8"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("6ce3a64a-310d-2e55-ae8f-88963961a665"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("6d53eddd-abac-7909-ede6-10b5cad566ae"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("6d6f452b-1b17-6b18-2900-eb22e430f885"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("6de25cb2-0395-8a17-e3e4-5b038fbdedfd"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("6ead8219-2fdb-580c-69d1-734780a8310f"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("6ee176aa-3b28-a7e8-1f6d-03af5fde588d"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("6f307473-d4c3-0c7d-6c0a-183c126a8e72"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("6f76b13e-27fe-098e-76ea-888f73acb3b1"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("70e9accb-cec2-f1f7-1898-e5939031c918"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("7151a0c9-7204-5d85-7c1e-800835832bcb"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("715d15a9-5ca0-fc57-1c1b-4cf374fa6808"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("717915ca-988f-d7a4-53fb-093dd0e8aeed"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("73a0930c-3fec-a383-61c7-31641c689bcd"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("73b7bc66-0039-86d0-8b82-d4763a92711a"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("73c882a7-a93f-ad67-d664-be94c03a221e"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("74359666-7005-9a59-0b0a-54dae646b1d8"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("74be9467-2f9c-ad6c-238e-f10d8defbf35"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("74e34693-c269-64c6-8dff-33373763f8ac"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("752971b5-070c-40ca-46a9-dbbad0848030"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("754cac42-0ada-15c5-7b04-56d426600e4e"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("764af253-209a-16ad-fde4-30ad165aa415"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("77c0cfb0-570e-219f-7fd6-d14cb52d6ef1"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("788552af-abe3-dfa4-d5c4-19a9dd0c0348"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("7926cd78-202b-88f0-e39e-12b7496d1d88"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("79801326-a670-28da-ef15-8b1d458becbf"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("79ea9f4c-47ff-1de1-6a21-febb4a935eeb"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("79fc1fdd-8f37-327a-23f6-66e36fc38cf8"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("7b2b168f-5e18-12a8-2a34-3613d96486bd"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("7b75ad14-7512-488d-5986-37ea2798cbb1"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("7b9c69b6-f7ba-1e97-a938-2c9a14230fd0"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("7bc8b93b-ca8e-2d7f-87bd-e631ed0e7f71"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("7c9dc180-fdd8-ddfc-6f5b-28b7c9c35377"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("7cce1c9c-2b47-fc39-503a-b426cc134a67"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("7d7714b4-129b-a2ae-a1e6-481e4dba5168"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("7e9d3402-866b-7c9c-ce8c-fcdb47bfd693"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("7ea3a435-af40-1366-b65f-8f4efcf55e58"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("7f54e56a-d414-0e94-08c4-9ba7547fe505"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("7f9ce1d9-d88d-5050-7b41-73f0ce11c7b3"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("801826a4-26a7-8057-e8d6-3f101a000dd9"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("8063ffe5-0c54-2e66-fd8d-d4c8532fab24"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("80d15ed2-2cf3-7e5e-f880-0853e9817237"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("80f3fecd-a6e9-04a1-879f-a493bead385b"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("816e4ad2-7685-223e-23e1-025cc3a4c52d"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("82a73c26-cb77-38a3-13b1-a385df223731"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("832db960-b5ce-2a26-92a1-6a1a4dbdb6f7"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("839063bc-b137-7b72-588b-f6a49eca87c3"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("840854f7-dc42-b302-d652-6352b9c100fc"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("85a9044e-29c0-54de-6938-d5d2e374a40e"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("85f4932d-6485-db4a-645b-87b8eaac657f"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("85fb9fd0-6957-01bc-f49a-8ed04355637a"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("863e8186-34d1-1cf3-c571-9a12dbddfdec"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("8746a586-8df4-9149-9230-3dba19871d5b"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("88cca8f7-7e1c-ea21-5864-5c88d39c8df8"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("89892f6c-ead3-ec76-c0ed-9c0e406a0f97"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("8a092e16-82cf-4df2-238e-c2510cca5ec8"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("8a705763-1309-361c-1018-dbbbe3b05065"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("8aa19e3f-5ee3-9a0e-2e53-803db3d4485d"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("8aef10dd-1c20-b017-7866-4b5f2476bf0c"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("8b363a7e-6f26-2927-3eb6-34518159edb1"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("8b437288-f774-7f63-846e-ccdc770a95ef"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("8c978da1-9a49-5dc7-5ed4-130e95ce43af"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("8d8f407b-15da-3b44-62b7-c05e04a8e55d"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("8df2dbdb-910e-b512-f7f5-82d20ef1fc6e"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("8e14a5db-4e62-d0c7-d3e9-dfc880d42289"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("8e81a471-d4b0-9dbb-f725-d1fb2a9311df"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("8efa044f-4372-cfce-f5b2-4a81da8c60eb"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("8f5f73ed-3b78-968b-3aa7-f956aa39661b"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("901b20a4-c6dd-d998-ef02-18ddbf7d7dee"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("9077ac3d-b087-f0c2-0ad3-df275941fe34"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("910dfd0d-da0b-f073-45a6-43b4705d3894"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("917fbcb1-dcb7-91c4-f0e6-c438bcde27fd"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("918dcf0e-bc60-8535-1286-9244ead2ea99"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("92a05a99-19c2-1383-58c2-4b2723c57cae"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("92e7ad0b-ea5a-19c4-948a-042925525b2f"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("93325a6d-2016-6528-0af7-5b788cf3b9fb"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("941a5573-262f-7774-c5a6-51ebda783c6a"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("946886a7-8461-a782-414e-5e658e872f1d"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("95166683-75c2-467f-0ef0-e03372cc6209"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("952df4a2-56ef-6445-a9e8-2c97e571aa50"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("957949fb-6aff-847d-3208-2c600c3bbcc5"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("959caff7-ecb9-eafe-018d-e3e1bd39943d"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("97cd1ba2-a95e-da50-48f7-6c89dad285d9"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("97daf3e9-81a3-275d-2e28-af1043ca8af5"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("981430e0-f4d0-0c62-f2b1-3dcfd4c4fe2e"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("9897435a-e870-3d2f-2ee2-16e3dd2f82e4"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("98ba0b61-3142-06d3-438c-a50688437d54"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("990db023-4472-b8d7-8bd3-6e358964432f"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("9c44eff5-3dbb-7646-4ddd-60be4d92749c"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("9d2eb8ac-8b9e-8cf5-d74d-44af9f8ccf03"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("9dd8d4f4-e93d-2823-bd28-36a52eee5ffa"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("9e78c0b7-42f8-0d45-cd7c-73fec6f77adf"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("9f5bfcfe-df6a-5345-ec59-03dfedbad63b"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("9f635850-edfd-2d6d-c7a0-af939af71ac0"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("9fe036ad-1585-4f5f-c307-4b99d9d003b5"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("a0e5cdee-15b8-c700-0237-67e5d743ec14"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("a199903c-8b95-7c33-d341-5b80d481b3ec"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("a5a7402b-93ff-e472-31d4-310d23fdb9ed"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("a5dcc2e9-1a24-1711-0a6b-f847c658e9c3"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("a5e0e305-89df-bdb8-76da-0d548f482cde"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("a679b243-767a-19f6-e463-562512bdf767"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("a819273c-d314-ba1f-9d97-877648d52a42"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("a851557a-2eed-07a1-1385-db070b680daf"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("aac0b2fa-1494-be38-9163-42f7cbf74f6d"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("aac0e1ec-ab9c-5dcf-fb27-70a25cd85cad"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("ab151f07-a347-364a-9f4f-82b758dda7c6"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("abcd65b4-ba50-2efe-af8e-1f43b6911c13"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("ac3572c5-2869-428e-6850-8354fa977779"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("ac378507-318e-7105-f524-99ec218756de"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("aca7998e-f8d1-994c-4509-d434a89d52d2"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("acea2deb-088c-e718-be88-2e14bd44910e"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("acfce72e-db98-1b0c-256a-431df6deb14a"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("ad181ea1-36c9-18b7-463c-2b847c07ab0a"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("b1d999e1-a612-58b8-dbc7-188486478449"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("b240d57f-946e-4432-9e69-236fe6cd8507"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("b2d7c532-8a82-dcd7-73a5-0ff24657cf7e"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("b37874be-ee51-f7fd-442f-200462b55ea0"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("b3f5223d-9d73-2602-375b-8b10f5a044c1"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("b463bf35-0ccd-5b92-9115-40400e4c7af5"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("b46be275-90ab-3723-d6f2-9d676b29148f"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("b59031cd-be6a-21c2-8b63-849cbd431c7a"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("b664bcf2-d427-d076-76dc-bf81692c2493"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("b6749451-a70a-5f1f-0baa-432dcdcf6531"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("b80e9faa-d861-0e4e-f94a-3b6f686d70f4"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("b835da55-f41a-886b-c1fb-d9e5edc07fcd"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("b8cb6c6f-2bd3-5117-66eb-de5a927fa3c8"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("b905d91b-e526-d5c4-5cd4-fb49abc850d8"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("b923d860-17d1-8f75-12e8-6c6492cce8f7"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("b9ba6b50-0612-a386-da04-509a7322d098"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("ba1e3ffb-10f9-f8f5-2b13-94be7f3df5c7"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("ba6bdbcd-4759-4476-0ba2-e240d080c050"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("baf8b212-9aef-472e-6c3d-873cf5b6b75f"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("bb5fcb56-995a-a838-a72c-fb4580b87c87"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("bbbe173a-239d-e76f-3508-80329c20080f"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("bc2ac164-9cec-a8a0-0d88-580775986490"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("bc793aa7-10b5-99df-a6f6-de77b7c7b511"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("bd392963-6f05-a7b4-3b8c-db358921a050"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("bd4978bf-fe1b-4592-14ec-86c726616dc4"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("be1d6cb8-87d5-fa50-cc0c-2cb1ca33b201"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("bf65bcff-ba84-572a-f6d6-b6de4608a313"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("c1400000-0c88-03fb-56f8-bcfdd71cae42"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("c50c781a-90c3-57d7-e02b-876b88c9f4fe"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("c572b015-a3a9-0599-1d7e-039e3a5be6bb"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("c591d70e-221c-8529-9834-65be4d480cb0"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("c7d11f00-33e7-95cf-8b4b-92ab17bf2baa"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("c81f5c8e-74d0-9639-e5df-48b089d5110f"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("c83c2fb8-d7cf-0f30-c2e0-fc94a61ca4bb"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("c8cb6599-3014-5fb4-5b2b-83365752f127"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("ca75b87d-622d-3f82-af10-b8f9a3b3506d"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("cac9b94b-5ae4-861c-d6c2-bd1b4631bc24"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("cadad6bd-433a-d03b-b60a-19e1b6fa0015"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("caffc399-5a5c-8d40-82f3-bc0538044c59"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("cb040d98-d00d-d35f-f76d-fa30261a9e77"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("cb5c6333-d822-df51-72c6-39a495029ad5"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("cb905702-25d5-bb7b-d6f3-6c5481014923"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("cd374515-dcdd-c12a-3dd8-86c5c903fec8"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("cf161c2e-3cbf-9f52-0d1c-72c5c9ebaf0b"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("cfdaa1d8-cdbb-dd2b-6870-e549f4def64f"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d02ef767-bcff-243d-f5a7-9b05a78bc984"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d0fd1934-1e88-204f-60dc-425c2e00c958"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d1d27d92-e3d8-2568-e00e-5867f979aada"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d30a7b26-1ceb-04af-e38b-4a49fd9db13a"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d3102c09-2fd1-d073-d531-6a05347c83ea"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d4793ab1-c808-00db-c3dc-4e63795cd95b"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d4c90f3d-7aa5-b149-cc71-be66b247f96d"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d4f83ac8-e3ad-516b-2c7f-e4700808cdf6"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d567a277-8646-3682-0a52-f28a4613eb84"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d590ce54-e47d-ffa9-627b-05fa8ac26260"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d5b0ffe1-125c-5253-b33d-b1d3f312fe9d"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d5be9f6a-e333-a3e0-5ccc-8585971f8062"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d642529e-813a-096d-5dca-f6b6648b4a53"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d655bd93-250f-33f5-d7b3-5a3550f3c8e4"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d67e3e1a-2041-9baf-0073-f7d86597e734"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d6e3b7e2-4eaa-5c87-3d8b-dfe08cba3520"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d6e55250-5034-3907-d4f4-4cd4af9e9d10"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d7a4c272-182b-6de2-c4eb-19168c2025cd"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d849e82d-319b-f661-753f-10919fb39c2b"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d92cb543-b5d5-82eb-f021-7030e5c5a4d6"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d92ce00a-d1d0-f367-379f-44dcdde344ec"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d98f810c-a432-a12c-b95a-82e80c93de89"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("d9fc7f88-6a2f-80ac-b48b-767720e75ab9"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("db1bbc6f-7a5e-94f1-f794-561f9488b7b5"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("dbb3a0dc-fa5b-7b06-c07e-bd2c6fdd99e0"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("dbdbe9fd-2bcf-4b9c-4921-347345392cb8"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("dbebb5df-f861-65fd-feb1-939404f69aa1"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("dcb0eaf4-34a3-b85a-1ec6-c4a684597146"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("dd8b8731-fbe7-6ab8-213b-f604b63d732c"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("dd8eb547-edcd-0b1a-237d-17af4e5cedc4"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("ddbdd26d-2e1e-9f03-313a-672ebf475111"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("e0ea4b93-8546-05ca-1da0-f01b232d4b31"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("e1eb5928-81d7-635b-60c9-8875695b20db"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("e318d2f2-4490-bb6c-4fdc-1fa59ab03ecf"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("e40c0725-2570-fc47-7e86-a2a445c4e6be"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("e43483bb-d542-747a-bbe3-89095a418974"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("e47438d8-a471-4a77-ea5c-c6f21e488a0e"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("e64e9981-cc4f-a2bb-15e7-eee6a9e973be"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("e6725f14-08fa-6197-2281-99ec9448efeb"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("e69a9138-62c1-b7f1-a864-4f967850e6e3"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("e6db71e0-ae8a-2105-b026-d390383212d4"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("e6ff30f6-68a0-fbfd-b0cd-2a714c3428cd"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("e73e7dbb-846a-69f2-8f92-29eedcbb3610"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("e8320af5-f897-99e9-fcd0-1573ecccf8bd"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("e96afbc7-d12d-5c44-b60b-c38fefb0784f"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("e96d6787-5c09-ee93-7bde-ef7cd87197a0"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("e9eb3810-1317-6340-8ae3-68293e3c979a"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("ea5ac3d6-3fb7-cc63-f018-f22d5392f611"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("eab00fb4-8098-e018-51d6-5671e6c9d679"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("eac054d6-77ab-1844-11c4-e33190da7110"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("eb070a62-2c00-2c43-4132-7ad6915b4ec5"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("ed13cbdd-6395-719d-2bf5-63d8821e491e"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("ee253c1a-c129-a978-66d6-9622948cc34d"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("ee9d13ba-88c1-009b-ba10-f35208d1f2e5"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("ef646d03-715a-9f90-3083-daca8ee67343"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("efbf477a-4f6a-ef95-de99-7248c803c727"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("f0d6f290-0d41-8907-c39a-b1c32723a5b3"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("f1074ba0-ffaa-1593-413a-743f1e473e54"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("f3183ddd-bf61-32f3-3c2b-7ac0e84c583d"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("f3765833-ce1d-5d71-d1ba-a0004b5f26d4"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("f37d0e82-0b87-bee8-4332-261c421da9cb"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("f42cb167-1a6c-5120-d3b4-005766f05574"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("f4e4fee1-8519-2a66-3c23-bfc0daeeb6b5"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("f4f92658-dccb-1758-e7d1-1c2f85cd1624"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("f5827d6e-a14b-99d7-5182-a5e0170ae940"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("f5ba457b-21b6-3cb1-02b0-d60c5231678c"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("f5bf2784-2707-00f6-82eb-97b0200ca57d"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("f609895d-29b3-9e2d-dbfd-b7647cd0ffac"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("f6625983-75f6-6c3c-7224-1384018032ad"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("f6c20ee2-a118-4729-8011-06ec34fb5e62"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("f77320a1-1770-36bc-7c24-78da39e04e0d"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("f86fea58-0196-367c-f753-7648e41898e5"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("fa464bc4-5105-e388-ee02-a098e6941330"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("fc117b37-614b-1e16-1595-4837fa4f2c14"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("fda31caa-51f8-900e-5cd9-25ef549e4cc6"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("fdde750c-c767-6d16-e693-76648aa903de"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("fea6c0ec-0f0a-4bed-d270-561b74f3b9ef"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("fedafe17-98a1-2919-6966-e83d1418b6b9"));

            migrationBuilder.DeleteData(
                table: "ExerciseMuscles",
                keyColumn: "Id",
                keyValue: new Guid("ff0490d4-5718-e959-89ad-4e1b44b7e49b"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("023bc5af-3d3c-3701-ec42-66cea60be7d7"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("053ad886-7a89-d64d-8a9e-46381a60bc5c"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("06d5b353-c57f-3a85-7474-95481ed59182"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("0caf5595-4ddb-9439-0b1c-26175401654e"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("0d154558-9d0e-8138-33d9-536ffb9432e5"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("0d1f80ff-fcfb-950d-f88d-493fbce1a73c"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("1058b3f7-daaf-44b3-85d3-e2133b4e5cde"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("105a15ee-60f5-ff7f-20ca-799591c0e169"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("11b3621a-9cfe-e302-f045-8995c7eda8ba"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("153bd9c4-6991-0246-6c44-ed0caedd25a2"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("171ccca6-7123-c9f2-b94e-b32fc12d097b"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("180a7bc9-69b6-dab8-2325-e8f12c917061"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("1b03d6d3-b2ed-7f2e-6644-2a175efe6568"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("1dfad4d7-5270-7ad9-bd72-54e45b62a7d8"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("202e04c4-ec37-1e25-32b0-e7ecb09ac090"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("21310bf6-4257-119b-528d-d76067da068e"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("2299a568-2a3e-9a83-40e4-fdea1168a0af"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("23eca012-f379-86c2-bd2a-07e73da37e57"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("25b9afa3-38cc-9a1e-bd89-4ee08d319413"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("268d8dad-43c7-5e83-24b5-a07387ae8af9"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("2863f1c1-b77c-0b4d-b2f8-ee5f451c3b37"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("2a4d81d5-7db5-76fc-aab6-c55801183ca3"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("2d8178ef-e61f-fe39-e3ac-7d2f4a748564"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("2e147828-59fb-4a5d-bf0e-d05b064c45d9"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("2ea83b13-d10a-cbcc-790a-cc421110d46d"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("3017d1b8-ac05-29a5-e2a0-f2f3b1057d74"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("380497ab-2e98-e211-c7c8-53f58eda71c1"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("38ccd0eb-9476-583c-5e99-25654a59eddb"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("396a5f31-4114-a312-60bd-96111afbc44a"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("3a78770d-d22d-6e7c-f156-92c8e4e4c17c"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("3cf3597c-0444-9b71-73d3-8c4f4c888e1a"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("3eeba987-f873-ba13-c353-c02844ef25f0"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("3f3a9eac-002c-68dd-4878-d58fdad2a418"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("408f53a9-3b97-90b7-8473-055a5d41a0be"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("41303540-98c4-2902-eca0-5d9cb5ba4ca6"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("4193dd34-4d5d-0d43-59fe-203d4f2b55a3"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("421b346b-b949-be3b-7f2c-26372211df7d"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("44c28204-7fc2-435b-7ac2-579def6f21b0"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("46e9110f-fae3-5225-1f05-cc9d0abf91a1"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("46ef2958-9adb-24d9-e556-fb1465098406"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("476cd99b-8358-13e5-8ade-84a1ed12511c"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("4b589f5d-dd39-25fc-b56a-264d2f59e0cf"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("4bc0ad35-644a-ebf0-8b6e-979829f3a10d"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("4be295d8-414f-8dcc-b97f-965e8d050343"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("4d1a1f2c-6f13-3ba0-ab64-fe84250d7267"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("4d53e1ed-5372-c298-0210-38060df271fb"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("4d5ec6ca-97e9-5792-b849-e8d74bd2e6fe"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("4dba253f-9f50-4e88-bda5-ef5c7071f416"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("4dc0f160-35df-ac8c-f1b7-af3a64bdca0b"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("4ee98183-8d0f-4de5-db84-4c4cdf4b12e1"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("4f199800-8c8d-8153-7729-07e8e6b73a1f"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("4f1b6f1f-a4a6-bb5c-cac0-35260efd0733"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("4fa98450-c23d-c59c-4bf4-bba35d6fb18f"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("509f5530-0fde-4cde-5f56-2f2aa0fb008e"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("59e649f9-941c-8011-9df7-274092cd0adb"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("5b2f579c-447f-c741-4b28-5e929f4889ec"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("5b5b8066-2b3d-945b-5314-6f6b2bd7fc85"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("5dfefe06-a654-16ef-ce73-afa498f92bd0"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("5e8dcf48-c535-ac4b-60d6-c96e4789225e"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("5f8c79e1-81c2-a01d-821d-0d3d4c1221c8"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("5fbcfdb8-df7f-f712-d8f0-1eb51118bb43"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("60585e44-151d-222c-0fa8-cba4a87d655a"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("61435e1b-14f3-a230-c3ec-2b49e61f68a1"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("6618c194-1d6f-a382-2ada-75f6936a4596"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("6c58eb05-3525-056d-5962-d77d102f9207"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("6cd088d2-41a1-d172-975f-126ea52c2584"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("706ce261-b9dc-df16-9a7e-04d73eeb8912"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("7164d238-0fcb-beac-2b3e-f6c0efe69015"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("7280bfe2-90f8-736c-295b-6a206e9360ec"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("746d0e6e-7dde-717f-9aa2-69a4ff064302"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("78533937-563f-c726-df24-c7d5d6b2aec3"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("7a5bf9ed-b8a4-73fd-6ea4-a06cb0b78219"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("7add62bb-f085-ede9-34c9-73c7d6ee3833"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("7bd96d3e-8489-883f-7d03-161c63dda919"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("7ccae516-396a-7d31-1420-d97c3167f6d1"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("84297ab4-d0e4-0871-4c23-85edf5956570"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("861aaacc-7adc-fd78-c5ed-b7877b421581"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("8634ffa3-94d4-1e45-4462-b719400266fd"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("86c64172-40d7-86de-caa0-29a1fa44ee0e"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("8b572c46-384e-4481-a3f4-4380487e64b5"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("8c85319c-7ef4-e722-42d0-b54f63f74157"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("90059af2-8ca9-1077-7bef-4b7d3c1ae9ef"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("93f38a2b-1c2b-31cd-c278-9cf959d21ba3"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("96420734-efe4-d1ac-e5f6-ff6c5f089b62"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("99b1260b-9d15-1a42-792b-3d57f62a5fc6"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("9c32a506-9e62-d6a2-eae4-edb3064de189"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("9c7f9d4d-d3b3-29f6-0fe5-841af531a446"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("a2082bd4-f7a4-c2f1-26f2-5f38e6e86332"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("a875020c-9955-5df6-d24a-bf3a4ed4a078"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("a9445cf7-e1ce-9d5a-7bb4-53955ea003cb"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("ab0ce91f-e6e1-0a16-8c29-68a744ecbaca"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("accf8b83-1d16-fa92-909a-80102ed2d3a8"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("af179f27-3579-a9ad-0062-45d20073ab6c"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("b2542f14-d2f5-bdea-7e66-51cf6c4dc26b"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("b5163ad4-4e20-44d0-f7b7-44b5bad33770"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("b708cd86-073e-94f5-d7ec-ebf1aa608b9e"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("b73c58a8-f06a-e30c-c3da-2ddbd1bda009"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("b75cbbcf-99f5-489e-120e-32136bfebfc2"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("b8a15341-bb29-7b7c-f6fa-c5971d8b89e3"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("b9d5355a-accc-c4af-e1ae-e158d22eac1d"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("b9e5b3ae-91b7-e8e4-4306-8007c4a085e8"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("ba158b19-5a08-250d-7de5-83ee05762d49"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("bc143f04-fe60-4860-1bc9-2be84803be5d"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("bd66bc8d-b61e-083a-d745-49b89e9bbaec"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("bf44d79e-2891-d9a8-8312-9c40942acca9"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("c19305bc-400e-4d54-14f9-a649e1f69f17"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("c4d1770c-8e6a-ecc6-5a5f-e31ede9d62ed"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("ca3d6e67-3a30-a951-d5c3-cbe10ae06400"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("cb1e7764-060c-6432-f5f2-62562e1a44b4"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("cd6b8e66-8d73-bdc3-8b56-51e74c3eeebe"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("ceb5a4bd-cb28-433c-f2e6-9d1a0b65df75"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("cf7abb85-1437-010e-1ab1-9279e4065702"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("d1cfc3eb-bcc8-4e2c-9e83-8ebcf01fcd3a"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("d41dc9d6-f8b9-1cd8-b3dd-b8878a560f65"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("d83c8717-a4a5-7c44-a3ba-dad36745d3a1"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("d97e16d0-85bf-ee77-1fde-d97423bbd58c"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("e3df50c2-0e26-aca4-b81a-0cf712988fcc"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("e711c52a-9f07-5780-4bb7-3353ccb9854f"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("e716c25b-65e9-2b43-b502-177bae273a96"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("e868adcb-aea1-bca9-1f24-a9d524cbb20e"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("eb7f4ce6-bd75-dfa7-456b-78e7d2144794"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("ed003439-f256-a3cb-aa58-3efc2dc04a82"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("ed8cd75f-9214-593b-8956-62ec3b722a09"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("ee605ae6-5a8e-0883-d970-43935b6c59d3"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("f0a5dda3-6129-bf45-fbe3-a8396e726b58"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("f17e30d7-b412-eb6d-caaf-4df71dbcc7ab"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("f20ab66d-11dc-cae8-a67d-02470794376d"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("f264e63e-b975-e4fb-e57b-6ed4eeaea5e0"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("f84f3a6a-9d58-2f85-97a8-46d88b59a683"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("f860fb57-a804-f4fa-2fed-25061b682cbb"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("f9156ba0-43ee-5905-1c38-768342eabb23"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("fa286022-acf1-e07a-b4d6-7d0168c5419d"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("fe0d3b73-4714-17a5-ef86-354da5b1bcc7"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("fea8e52d-6f6c-c15d-0fac-39014417c43f"));

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "Id",
                keyValue: new Guid("134c1c3d-365b-71c5-fa42-e3dfdc967dd3"));

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "Id",
                keyValue: new Guid("2c471d39-7446-8e2e-39f4-82a443032c3e"));

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "Id",
                keyValue: new Guid("5dd58442-ba55-e4a5-d869-a66d601b193f"));

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "Id",
                keyValue: new Guid("8e7b4a1b-c22e-904d-d102-7f61089ad5c3"));

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "Id",
                keyValue: new Guid("91328842-7e67-521f-17b2-37ddff0c912e"));

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "Id",
                keyValue: new Guid("c9f465e3-aee6-83df-cb8e-e94a305112ad"));

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "Id",
                keyValue: new Guid("d44984a4-01aa-fa50-7652-f83c8bad469e"));

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "Id",
                keyValue: new Guid("fa6e4468-2e50-8944-d82d-8ae5fa6b0311"));
        }
    }
}
