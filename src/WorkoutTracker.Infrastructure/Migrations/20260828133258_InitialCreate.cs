using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WorkoutTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Slug = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    DefaultBarWeightKg = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Muscles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Slug = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    BodyRegion = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Muscles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BodyMeasurements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeasuredOn = table.Column<DateOnly>(type: "date", nullable: false),
                    WeightKg = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true),
                    BodyFatPercent = table.Column<decimal>(type: "numeric(4,1)", precision: 4, scale: 1, nullable: true),
                    ChestCm = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: true),
                    WaistCm = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: true),
                    HipsCm = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: true),
                    LeftArmCm = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: true),
                    RightArmCm = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: true),
                    LeftThighCm = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: true),
                    RightThighCm = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: true),
                    LeftCalfCm = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: true),
                    RightCalfCm = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: true),
                    ShouldersCm = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: true),
                    NeckCm = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyMeasurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BodyMeasurements_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonalRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<decimal>(type: "numeric(12,3)", precision: 12, scale: 3, nullable: false),
                    AtWeight = table.Column<decimal>(type: "numeric(8,3)", precision: 8, scale: 3, nullable: true),
                    WorkoutSetId = table.Column<Guid>(type: "uuid", nullable: true),
                    WorkoutSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AchievedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalRecords_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgressPhotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TakenOn = table.Column<DateOnly>(type: "date", nullable: false),
                    Pose = table.Column<int>(type: "integer", nullable: false),
                    StorageKey = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    WeightKg = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgressPhotos_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    FamilyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReplacedByTokenId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoutineFolders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutineFolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutineFolders_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    Gender = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    HeightCm = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: true),
                    AvatarStorageKey = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Goal = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfiles_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeightUnit = table.Column<int>(type: "integer", nullable: false),
                    LengthUnit = table.Column<int>(type: "integer", nullable: false),
                    TimeZone = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Theme = table.Column<int>(type: "integer", nullable: false),
                    OneRepMaxFormula = table.Column<int>(type: "integer", nullable: false),
                    DefaultRestSeconds = table.Column<int>(type: "integer", nullable: false),
                    AutoStartRestTimer = table.Column<bool>(type: "boolean", nullable: false),
                    RestTimerSound = table.Column<bool>(type: "boolean", nullable: false),
                    RestTimerVibrate = table.Column<bool>(type: "boolean", nullable: false),
                    RestTimerNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    BarWeightKg = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    PlateInventoryKg = table.Column<List<decimal>>(type: "numeric[]", nullable: false),
                    RoundingIncrementKg = table.Column<decimal>(type: "numeric(6,3)", precision: 6, scale: 3, nullable: false),
                    OverloadIncrementKg = table.Column<decimal>(type: "numeric(6,3)", precision: 6, scale: 3, nullable: false),
                    WarmupPercentages = table.Column<List<int>>(type: "integer[]", nullable: false),
                    WeeklyWorkoutGoal = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettings_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Instructions = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    MediaUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DefaultRestSeconds = table.Column<int>(type: "integer", nullable: false),
                    DefaultIncrementKg = table.Column<decimal>(type: "numeric(6,3)", precision: 6, scale: 3, nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercises_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exercises_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Routines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    FolderId = table.Column<Guid>(type: "uuid", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Routines_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Routines_RoutineFolders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "RoutineFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseMuscles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    MuscleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    ContributionWeight = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseMuscles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseMuscles_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExerciseMuscles_Muscles_MuscleId",
                        column: x => x.MuscleId,
                        principalTable: "Muscles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseNotes_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExerciseNotes_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoutineExercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoutineId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    RestSeconds = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    SupersetGroup = table.Column<int>(type: "integer", nullable: true),
                    SupersetKind = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutineExercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutineExercises_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutineExercises_Routines_RoutineId",
                        column: x => x.RoutineId,
                        principalTable: "Routines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoutineId = table.Column<Guid>(type: "uuid", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutSchedules_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutSchedules_Routines_RoutineId",
                        column: x => x.RoutineId,
                        principalTable: "Routines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoutineId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutSessions_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutSessions_Routines_RoutineId",
                        column: x => x.RoutineId,
                        principalTable: "Routines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RoutineSetTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoutineExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    TargetReps = table.Column<int>(type: "integer", nullable: false),
                    TargetRepsMax = table.Column<int>(type: "integer", nullable: true),
                    TargetWeight = table.Column<decimal>(type: "numeric(8,3)", precision: 8, scale: 3, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutineSetTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutineSetTemplates_RoutineExercises_RoutineExerciseId",
                        column: x => x.RoutineExerciseId,
                        principalTable: "RoutineExercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutExercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkoutSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    ExerciseType = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    RestSeconds = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    SupersetGroup = table.Column<int>(type: "integer", nullable: true),
                    SupersetKind = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutExercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutExercises_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkoutExercises_WorkoutSessions_WorkoutSessionId",
                        column: x => x.WorkoutSessionId,
                        principalTable: "WorkoutSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutSets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkoutExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric(8,3)", precision: 8, scale: 3, nullable: false),
                    Reps = table.Column<int>(type: "integer", nullable: false),
                    Rpe = table.Column<decimal>(type: "numeric(3,1)", precision: 3, scale: 1, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    DurationSeconds = table.Column<int>(type: "integer", nullable: true),
                    DistanceMeters = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutSets_WorkoutExercises_WorkoutExerciseId",
                        column: x => x.WorkoutExerciseId,
                        principalTable: "WorkoutExercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Equipment",
                columns: new[] { "Id", "DefaultBarWeightKg", "Name", "Slug" },
                values: new object[,]
                {
                    { new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), null, "Dumbbell", "dumbbell" },
                    { new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), null, "Cable", "cable" },
                    { new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), null, "Bodyweight", "bodyweight" },
                    { new Guid("89cde677-e340-f0fb-a1ac-58aa920f33ea"), null, "Cardio Machine", "cardio-machine" },
                    { new Guid("9f6a10ab-38ef-84ba-acc9-4e6e4ef115a3"), 25m, "Trap Bar", "trap-bar" },
                    { new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), 20m, "Barbell", "barbell" },
                    { new Guid("c2fe7433-3260-f60f-06bc-a1e9ab8044da"), 20m, "Smith Machine", "smith-machine" },
                    { new Guid("e830f954-0176-7c23-cda7-31a84ba93e1c"), null, "Kettlebell", "kettlebell" },
                    { new Guid("ea46aa2f-9f02-4573-1016-018252ba8ce1"), 10m, "EZ Bar", "ez-bar" },
                    { new Guid("ee0e66b0-29d5-90e7-f0fe-ef8dac0c1955"), null, "Resistance Band", "resistance-band" },
                    { new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), null, "Machine", "machine" },
                    { new Guid("ef3dcb2c-113b-c883-1557-dad42e897bae"), null, "Weight Plate", "plate" }
                });

            migrationBuilder.InsertData(
                table: "Muscles",
                columns: new[] { "Id", "BodyRegion", "Name", "Slug" },
                values: new object[,]
                {
                    { new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), "Arms", "Triceps", "triceps" },
                    { new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), "Legs", "Glutes", "glutes" },
                    { new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), "Back", "Lats", "lats" },
                    { new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), "Back", "Trapezius", "traps" },
                    { new Guid("1678d70c-25a5-db43-d7f4-a7ea30040382"), "Legs", "Adductors", "adductors" },
                    { new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), "Chest", "Chest", "chest" },
                    { new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), "Back", "Upper Back", "upper-back" },
                    { new Guid("37a3dd84-a1db-46b4-357f-47478a17a912"), "Shoulders", "Side Deltoids", "side-delts" },
                    { new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), "Legs", "Quadriceps", "quads" },
                    { new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), "Arms", "Biceps", "biceps" },
                    { new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), "Shoulders", "Front Deltoids", "front-delts" },
                    { new Guid("8d35a1be-c85a-eac7-c2b5-757c17cebb0d"), "Shoulders", "Rear Deltoids", "rear-delts" },
                    { new Guid("983a359b-2e07-4350-bdb3-3568b0b85dda"), "Back", "Lower Back", "lower-back" },
                    { new Guid("a9eec2ef-ccde-0652-e9f7-f795f1d938c2"), "Legs", "Abductors", "abductors" },
                    { new Guid("bcb0bc9d-0d9f-89e8-f957-337f2614a9b1"), "Core", "Obliques", "obliques" },
                    { new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), "Legs", "Hamstrings", "hamstrings" },
                    { new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), "Legs", "Calves", "calves" },
                    { new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), "Arms", "Forearms", "forearms" },
                    { new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), "Full Body", "Cardiovascular", "cardio" },
                    { new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), "Core", "Abdominals", "abs" }
                });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "Category", "CreatedAt", "DefaultIncrementKg", "DefaultRestSeconds", "EquipmentId", "Instructions", "IsArchived", "MediaUrl", "Name", "OwnerId", "Type" },
                values: new object[,]
                {
                    { new Guid("0d8f361e-6b85-c865-db5f-7f5cdea192cd"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Pull the rope toward the forehead while separating the hands and rotating the shoulders back.", false, null, "Face Pull", null, 0 },
                    { new Guid("10b96a6a-cf80-96b1-9b5e-b79e15cf167c"), "Core", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Kneel below the cable and flex the spine to bring the elbows toward the knees.", false, null, "Cable Crunch", null, 0 },
                    { new Guid("12c04756-7d48-a105-e28b-0fb993391b61"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 120, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Start from a full hang, drive the elbows down toward the sides and clear the chin above the bar.", false, null, "Pull Up", null, 2 },
                    { new Guid("13642092-77b7-3ff9-16da-745dc66c368c"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1.25m, 60, new Guid("ea46aa2f-9f02-4573-1016-018252ba8ce1"), "Lower the bar toward the forehead with the upper arms nearly vertical, then extend.", false, null, "Skullcrusher", null, 0 },
                    { new Guid("138513e0-dd7e-c591-cd90-ea2fbeaa0d72"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 90, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Sit tall, pull the handle to the abdomen and avoid rocking the torso for momentum.", false, null, "Seated Cable Row", null, 0 },
                    { new Guid("16188d54-c1c9-5571-ad08-91be35559b8c"), "Core", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Hang from a bar and raise the legs without swinging, curling the pelvis at the top.", false, null, "Hanging Leg Raise", null, 2 },
                    { new Guid("16713313-d70e-a844-e6de-dfd481ec0cc4"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 60, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Keep a soft elbow bend and open the arms in a wide arc until you feel a stretch across the chest, then squeeze back together.", false, null, "Dumbbell Fly", null, 0 },
                    { new Guid("1a407dc5-ad9a-bf79-7803-017232a87766"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 120, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Hinge forward and row the handle toward the sternum, keeping the spine neutral.", false, null, "T-Bar Row", null, 0 },
                    { new Guid("1e06c8df-e6ba-60d4-ebdf-6bc34297992c"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 90, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Press from shoulder height to overhead without excessive lower back arch.", false, null, "Dumbbell Shoulder Press", null, 0 },
                    { new Guid("2a9ebe9e-c643-ce00-2606-806bbf63eac6"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1m, 45, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Let the wrists extend fully, then curl the weight using the forearms only.", false, null, "Wrist Curl", null, 0 },
                    { new Guid("2b96abda-e80b-13d9-8f5f-66c6984db3a6"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 120, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Set the bench to roughly 30 degrees. Lower the bar to the upper chest and press without letting the elbows flare excessively.", false, null, "Incline Bench Press", null, 0 },
                    { new Guid("2ccbd251-cf07-6210-6442-f9c7e28df9c6"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Pin the elbows to the sides and extend fully without leaning over the cable.", false, null, "Triceps Pushdown", null, 0 },
                    { new Guid("2d9b2af6-c29b-7318-0413-2764b164d0db"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5m, 120, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Tuck the ribs down, drive the hips up until the torso is level and squeeze at the top.", false, null, "Hip Thrust", null, 0 },
                    { new Guid("2fd873ee-3b62-70ae-caa7-49aad166dd63"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1.25m, 60, new Guid("ea46aa2f-9f02-4573-1016-018252ba8ce1"), "Keep the upper arms flat on the pad and avoid letting the elbows drift.", false, null, "Preacher Curl", null, 0 },
                    { new Guid("39045745-690e-80cf-d278-ac1edf676a4c"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 120, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Use a shoulder-width grip, keep the elbows tucked and press to full extension.", false, null, "Close Grip Bench Press", null, 0 },
                    { new Guid("3bbfab26-84a9-9e4a-07ef-f8f0f00f9673"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5m, 180, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Brace before each rep, sit down between the hips, keep the whole foot planted and drive up evenly.", false, null, "Back Squat", null, 0 },
                    { new Guid("41a196b2-ea98-162f-90dd-b35a0305c0e5"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Keep a straight line from head to heels, lower until the chest is just above the floor, then press away.", false, null, "Push Up", null, 1 },
                    { new Guid("5a7cea54-b47e-13d5-8ec7-0477d5e5cac5"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 90, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Step forward and lower until the front thigh is roughly parallel, then push back to standing.", false, null, "Lunge", null, 0 },
                    { new Guid("5b109901-5cc1-9055-5abc-6a5845f2f6fd"), "Full Body", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("e830f954-0176-7c23-cda7-31a84ba93e1c"), "Hinge at the hips and snap them forward to float the bell to chest height.", false, null, "Kettlebell Swing", null, 0 },
                    { new Guid("5d47db27-ddd9-7e65-2c80-4318dafc57f7"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Step forward slightly, keep a small elbow bend and bring the handles together in front of the chest.", false, null, "Cable Crossover", null, 0 },
                    { new Guid("5dcf8302-731d-4d21-711c-a00f684c65f1"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 90, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Press both dumbbells from chest level to arms extended, keeping the forearms vertical throughout.", false, null, "Dumbbell Bench Press", null, 0 },
                    { new Guid("6090f78a-f7b9-ce45-d2ab-3ccee1b71714"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Extend the knees fully without slamming the weight, and lower under control.", false, null, "Leg Extension", null, 0 },
                    { new Guid("6657cf47-c7e0-b672-d828-911cb5c27097"), "Cardio", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 0, new Guid("89cde677-e340-f0fb-a1ac-58aa920f33ea"), "Drive with the legs, then the back, then the arms, and reverse the order on the recovery.", false, null, "Rowing Machine", null, 4 },
                    { new Guid("66919704-1f0f-9614-351b-f266f367f142"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5m, 180, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Set the bar over the mid foot, take the slack out of the bar, then stand up by driving the floor away while keeping the bar close.", false, null, "Deadlift", null, 0 },
                    { new Guid("6d620f7a-37df-3023-326e-f5c015583cc3"), "Core", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1.25m, 45, new Guid("ef3dcb2c-113b-c883-1557-dad42e897bae"), "Rotate the torso side to side under control, keeping the chest tall.", false, null, "Russian Twist", null, 0 },
                    { new Guid("7049f0f3-3360-db4a-d6c0-471c0b8c38ad"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 120, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Push the hips back with a near-straight knee, keep the bar in contact with the legs and stop when the hamstrings limit the range.", false, null, "Romanian Deadlift", null, 0 },
                    { new Guid("78fd0059-e0a9-a60b-15f7-cb63d1fc0314"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 60, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Keep the elbows close to the head and lower until you feel a stretch, then extend.", false, null, "Overhead Triceps Extension", null, 0 },
                    { new Guid("7a9ce924-fed9-b4b2-8196-6571bfe25c82"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1m, 60, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Lead with the elbows and raise to about shoulder height. Keep the motion controlled rather than swinging.", false, null, "Lateral Raise", null, 0 },
                    { new Guid("7eda7e00-8e80-97c1-75c8-a602f6e9693b"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Curl the heels toward the glutes and resist the return.", false, null, "Leg Curl", null, 0 },
                    { new Guid("7f9de13d-be87-d98e-30fa-2a060f91a8fd"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 150, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Keep the elbows high and the torso upright, descending under control.", false, null, "Front Squat", null, 0 },
                    { new Guid("8a7d80e6-4aec-a10c-dad8-59c46f9688ba"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 120, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Hinge to roughly 45 degrees, keep a neutral spine and pull the bar toward the lower ribs.", false, null, "Barbell Row", null, 0 },
                    { new Guid("8c0a4eb4-e93a-7c42-c098-7427d51178a8"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1m, 60, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Hold a neutral grip throughout and keep the wrists straight.", false, null, "Hammer Curl", null, 0 },
                    { new Guid("8fde6a5d-dcfc-607f-298a-62213b335c08"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1.25m, 60, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Keep the elbows pinned at the sides and avoid swinging the torso.", false, null, "Barbell Curl", null, 0 },
                    { new Guid("9eef453a-7a8b-b882-0bce-f33bef17a9d3"), "Cardio", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 0, new Guid("89cde677-e340-f0fb-a1ac-58aa920f33ea"), "Maintain a steady effort. Record duration and distance rather than load.", false, null, "Treadmill Run", null, 4 },
                    { new Guid("a2d58546-3cfc-ead2-5910-922b8271d1dd"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 120, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Lean slightly forward for chest emphasis or stay upright for triceps. Lower until the upper arms are roughly parallel to the floor.", false, null, "Dip", null, 2 },
                    { new Guid("a92c21aa-6df6-1ea3-d98a-6f29ef4595b9"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 150, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Set your shoulder blades back and down, keep your feet planted, lower the bar to the mid chest under control, then press back to full extension.", false, null, "Bench Press", null, 0 },
                    { new Guid("ac5f7688-7fd6-ee4c-b2bf-c81d38ed9f85"), "Full Body", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5m, 180, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Pull the bar from the floor to the shoulders, then press overhead in one controlled sequence.", false, null, "Clean and Press", null, 0 },
                    { new Guid("b88dc589-e2a5-1233-868c-fdee59b917f8"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 90, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Support yourself with one hand, pull the dumbbell toward the hip and avoid twisting the torso.", false, null, "Dumbbell Row", null, 0 },
                    { new Guid("b954092d-34a5-91fe-de80-49c4ffb59298"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1m, 60, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Hinge forward, keep a slight elbow bend and open the arms wide while keeping the neck relaxed.", false, null, "Rear Delt Fly", null, 0 },
                    { new Guid("b968a043-a331-103d-741b-1b9960d8388d"), "Cardio", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 0, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Stay light on the balls of the feet with relaxed shoulders. Record duration.", false, null, "Jump Rope", null, 3 },
                    { new Guid("bec5ecd0-69fc-5b69-e4ef-b209303d8a65"), "Cardio", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 0, new Guid("89cde677-e340-f0fb-a1ac-58aa920f33ea"), "Keep a consistent cadence and record duration and distance.", false, null, "Stationary Bike", null, 4 },
                    { new Guid("c4600c5a-30c2-447e-6913-1c04fa0dd5b0"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Rise onto the toes through a full range and lower until you feel a stretch.", false, null, "Calf Raise", null, 0 },
                    { new Guid("cb6af2bf-0956-b705-db44-eae52b158429"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5m, 120, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Lower under control without letting the lower back lift from the pad, then press without locking harshly.", false, null, "Leg Press", null, 0 },
                    { new Guid("cc3cbb7b-332f-6f40-7ccf-b0cd7551f647"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Elevate the shoulders straight up without rolling them, pause briefly at the top and lower under control.", false, null, "Shrug", null, 0 },
                    { new Guid("ce548bbf-6d81-99e6-f073-68e79cd7bc91"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 90, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Elevate the rear foot, descend straight down and drive through the front foot.", false, null, "Bulgarian Split Squat", null, 0 },
                    { new Guid("dc489571-9119-c10b-4b12-e6e8e6ec0873"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Hinge at the hips and extend until the torso lines up with the legs. Avoid hyperextending.", false, null, "Back Extension", null, 2 },
                    { new Guid("e02c3e2e-3cfa-6041-6169-dd1a3565e195"), "Core", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Roll out only as far as you can keep the lower back from arching, then pull back.", false, null, "Ab Wheel Rollout", null, 1 },
                    { new Guid("e162e2dd-6402-0ca0-ccab-533aefc3f471"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 90, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Use a low incline and press with the forearms vertical, stopping just short of the dumbbells touching.", false, null, "Incline Dumbbell Press", null, 0 },
                    { new Guid("e606a152-f1ba-7fd4-4098-6ff6358137c0"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 90, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Align the handles with the mid chest and press without letting the shoulders roll forward.", false, null, "Chest Press Machine", null, 0 },
                    { new Guid("e63f539e-2321-faab-0bfb-d9b29ed506a0"), "Push", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 150, new Guid("a9ed280d-d3bc-1181-e070-05b48f57c163"), "Brace your core and glutes, press the bar overhead in a straight line and finish with the bar over the mid foot.", false, null, "Overhead Press", null, 0 },
                    { new Guid("ece329e3-5bd8-76ac-5838-5a15ed6979b0"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 120, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Use a supinated grip and pull until the chin passes the bar, controlling the descent.", false, null, "Chin Up", null, 2 },
                    { new Guid("f2ff6a9c-3e72-bf7e-3307-33c152e942ae"), "Pull", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 90, new Guid("4a92681f-f3fb-e3c4-1551-640198caa81c"), "Keep the chest tall and pull the bar to the upper chest, then let it rise under control.", false, null, "Lat Pulldown", null, 0 },
                    { new Guid("f3555423-c64e-674e-d195-d158113686ab"), "Core", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 60, new Guid("752fe9ad-fd8d-3ce1-a920-e35f9329ee25"), "Hold a straight line from head to heels, squeezing the glutes and bracing the abs.", false, null, "Plank", null, 3 },
                    { new Guid("f56b23d1-5f53-178a-795e-c2a2633ce93e"), "Arms", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1m, 60, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Curl without moving the elbows forward, and control the lowering phase.", false, null, "Dumbbell Curl", null, 0 },
                    { new Guid("f6474f74-08d7-6f16-afca-bd7fda20d91c"), "Legs", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2.5m, 45, new Guid("eee72c52-2cbf-1e4c-2676-11add3a889e2"), "Press the knees outward under control and resist on the way back in.", false, null, "Hip Abduction", null, 0 },
                    { new Guid("f932cdb6-1c56-a507-8e81-0069b3e48860"), "Full Body", new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2m, 90, new Guid("0bc6a473-f645-2ae0-919f-782ffce81458"), "Carry a heavy load at the sides with the chest tall and ribs down.", false, null, "Farmer's Walk", null, 3 }
                });

            migrationBuilder.InsertData(
                table: "ExerciseMuscles",
                columns: new[] { "Id", "ContributionWeight", "ExerciseId", "MuscleId", "Role" },
                values: new object[,]
                {
                    { new Guid("01e95ad3-e068-c04e-bd4e-8e38e5bf126d"), 1.0m, new Guid("5a7cea54-b47e-13d5-8ec7-0477d5e5cac5"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("022c00f6-cd4d-e1e5-eca8-65c06ef7a561"), 0.4m, new Guid("ce548bbf-6d81-99e6-f073-68e79cd7bc91"), new Guid("1678d70c-25a5-db43-d7f4-a7ea30040382"), 1 },
                    { new Guid("03a11d77-44d4-d1d1-8fd0-d139cf55dc1d"), 1.0m, new Guid("66919704-1f0f-9614-351b-f266f367f142"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 0 },
                    { new Guid("08370011-0bac-c86a-f491-0d094600008b"), 0.4m, new Guid("f2ff6a9c-3e72-bf7e-3307-33c152e942ae"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("08c2f6da-21e9-a683-749c-e7e9f2a88ec9"), 0.4m, new Guid("12c04756-7d48-a105-e28b-0fb993391b61"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("0c540e2a-a3ec-9383-e6d0-c16d558360b9"), 0.4m, new Guid("f2ff6a9c-3e72-bf7e-3307-33c152e942ae"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 1 },
                    { new Guid("0eb87c4b-90dd-1bf0-d055-f80571974e99"), 1.0m, new Guid("6090f78a-f7b9-ce45-d2ab-3ccee1b71714"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("0f3d4975-746e-8e06-50bb-0863f8188e97"), 0.4m, new Guid("e606a152-f1ba-7fd4-4098-6ff6358137c0"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("1013846c-0094-7532-0b01-c8ed73707a01"), 0.4m, new Guid("39045745-690e-80cf-d278-ac1edf676a4c"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 1 },
                    { new Guid("104c4082-99ec-2012-5fc2-28185376adf6"), 0.4m, new Guid("b954092d-34a5-91fe-de80-49c4ffb59298"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 1 },
                    { new Guid("10c73d8a-5f62-d45a-a2f1-6ac7a9b786b8"), 1.0m, new Guid("0d8f361e-6b85-c865-db5f-7f5cdea192cd"), new Guid("8d35a1be-c85a-eac7-c2b5-757c17cebb0d"), 0 },
                    { new Guid("13178f90-8246-85d2-c785-dd67709caede"), 0.4m, new Guid("e162e2dd-6402-0ca0-ccab-533aefc3f471"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("161c091d-0676-3fee-7ba5-06f14d3d49f8"), 0.4m, new Guid("8a7d80e6-4aec-a10c-dad8-59c46f9688ba"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("16f104cc-8f9f-eeaf-b20f-6f12d171e1ba"), 0.4m, new Guid("6657cf47-c7e0-b672-d828-911cb5c27097"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("1868e972-eedb-eaef-1a63-1ac532e979ea"), 0.4m, new Guid("16188d54-c1c9-5571-ad08-91be35559b8c"), new Guid("bcb0bc9d-0d9f-89e8-f957-337f2614a9b1"), 1 },
                    { new Guid("1adb9ec1-faef-661d-ed80-5ddd65f26e6e"), 0.4m, new Guid("e02c3e2e-3cfa-6041-6169-dd1a3565e195"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 1 },
                    { new Guid("1b1573eb-6255-e70e-6e1b-e80ab10ff094"), 0.4m, new Guid("e63f539e-2321-faab-0bfb-d9b29ed506a0"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("1b837118-62d8-1f78-4ace-00786825d74a"), 1.0m, new Guid("39045745-690e-80cf-d278-ac1edf676a4c"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 0 },
                    { new Guid("1d002586-1f48-e9a7-6ff3-c000c546637a"), 0.4m, new Guid("5dcf8302-731d-4d21-711c-a00f684c65f1"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("1d2519d3-93e6-9b79-5bec-167abef2caa5"), 0.4m, new Guid("3bbfab26-84a9-9e4a-07ef-f8f0f00f9673"), new Guid("1678d70c-25a5-db43-d7f4-a7ea30040382"), 1 },
                    { new Guid("1e1a2c4f-d2ef-847b-93c8-5875aa605a57"), 1.0m, new Guid("3bbfab26-84a9-9e4a-07ef-f8f0f00f9673"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("1edcafdf-7b2b-bf0e-157f-a14e2bdf3e9a"), 0.4m, new Guid("3bbfab26-84a9-9e4a-07ef-f8f0f00f9673"), new Guid("983a359b-2e07-4350-bdb3-3568b0b85dda"), 1 },
                    { new Guid("2093fb1e-b0ad-a335-b15b-c09ce7dec75a"), 0.4m, new Guid("2d9b2af6-c29b-7318-0413-2764b164d0db"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("2137d9d6-4feb-e0cd-05e2-2b6fdfcdd8d5"), 1.0m, new Guid("8fde6a5d-dcfc-607f-298a-62213b335c08"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 0 },
                    { new Guid("2183ef97-ab25-9948-7cf8-1d5494e7248f"), 0.4m, new Guid("ac5f7688-7fd6-ee4c-b2bf-c81d38ed9f85"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("225323c4-d9ab-1319-9acb-568191d90c03"), 0.4m, new Guid("cb6af2bf-0956-b705-db44-eae52b158429"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("236eeb1f-2f41-f05b-ad80-7a0a85698492"), 1.0m, new Guid("ac5f7688-7fd6-ee4c-b2bf-c81d38ed9f85"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("25708860-29d4-b71e-09bf-6e7147cd0dfa"), 1.0m, new Guid("ece329e3-5bd8-76ac-5838-5a15ed6979b0"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("28a4b184-0b90-b050-d1b6-449482f9e4cc"), 1.0m, new Guid("5dcf8302-731d-4d21-711c-a00f684c65f1"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("29ab6c8d-47dd-7ee6-c71b-2032e2ff2d9e"), 0.4m, new Guid("2d9b2af6-c29b-7318-0413-2764b164d0db"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("2a7a81c3-3047-d3de-fd9c-db34709d6e01"), 1.0m, new Guid("138513e0-dd7e-c591-cd90-ea2fbeaa0d72"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("2b5c6bf0-d9d0-99bf-fbc8-8bb059a5ddd7"), 0.4m, new Guid("e63f539e-2321-faab-0bfb-d9b29ed506a0"), new Guid("37a3dd84-a1db-46b4-357f-47478a17a912"), 1 },
                    { new Guid("2bfed153-2e8e-b85a-e052-3e2883358bec"), 0.4m, new Guid("12c04756-7d48-a105-e28b-0fb993391b61"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 1 },
                    { new Guid("2c07a85f-0d20-9fcd-b73a-c76b233a56d6"), 1.0m, new Guid("ce548bbf-6d81-99e6-f073-68e79cd7bc91"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("2e278a44-28f5-53fd-009f-01c59a9b6563"), 1.0m, new Guid("13642092-77b7-3ff9-16da-745dc66c368c"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 0 },
                    { new Guid("2e7fb47f-d7a0-bac9-7709-b5c0f825371d"), 1.0m, new Guid("ac5f7688-7fd6-ee4c-b2bf-c81d38ed9f85"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 0 },
                    { new Guid("2edf52a5-e790-c188-0784-0b52c7977a72"), 0.4m, new Guid("ce548bbf-6d81-99e6-f073-68e79cd7bc91"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("3059bb5b-a207-329d-c611-537b91dd2c1a"), 1.0m, new Guid("1a407dc5-ad9a-bf79-7803-017232a87766"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("314c1853-8cbc-502c-c28e-c1b55c2bdf36"), 0.4m, new Guid("f3555423-c64e-674e-d195-d158113686ab"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("32020326-9388-75b3-244c-fa98153400ad"), 0.4m, new Guid("f3555423-c64e-674e-d195-d158113686ab"), new Guid("bcb0bc9d-0d9f-89e8-f957-337f2614a9b1"), 1 },
                    { new Guid("33481ce8-b67e-ecc5-d4bd-348e7f563f45"), 1.0m, new Guid("ce548bbf-6d81-99e6-f073-68e79cd7bc91"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("34424dd7-e301-3745-cc47-be3c07b976fd"), 0.4m, new Guid("6657cf47-c7e0-b672-d828-911cb5c27097"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 1 },
                    { new Guid("351b6ed9-1198-688c-92ac-926ca50ed57f"), 0.4m, new Guid("138513e0-dd7e-c591-cd90-ea2fbeaa0d72"), new Guid("8d35a1be-c85a-eac7-c2b5-757c17cebb0d"), 1 },
                    { new Guid("35c33ca0-07b0-e495-5157-271da74dd885"), 0.4m, new Guid("2b96abda-e80b-13d9-8f5f-66c6984db3a6"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("36237bfd-616a-18d6-232c-ed5f070cf08f"), 1.0m, new Guid("7f9de13d-be87-d98e-30fa-2a060f91a8fd"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("390af031-4558-10af-7bc7-9f6a1d3c4390"), 1.0m, new Guid("1e06c8df-e6ba-60d4-ebdf-6bc34297992c"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 0 },
                    { new Guid("3a05367c-5a60-97b6-f077-89bab5015391"), 1.0m, new Guid("5b109901-5cc1-9055-5abc-6a5845f2f6fd"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("3ae6d21c-b2ee-8981-ff02-e5712c4f1261"), 0.4m, new Guid("cc3cbb7b-332f-6f40-7ccf-b0cd7551f647"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 1 },
                    { new Guid("3c13746d-b3a0-7b5c-a650-aa275480cf8d"), 0.4m, new Guid("1e06c8df-e6ba-60d4-ebdf-6bc34297992c"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("3c379f3f-e99c-b547-35b0-f5c418f09dce"), 1.0m, new Guid("e606a152-f1ba-7fd4-4098-6ff6358137c0"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("3cc588ae-4922-4db6-98e9-16f0e535d7be"), 1.0m, new Guid("2ccbd251-cf07-6210-6442-f9c7e28df9c6"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 0 },
                    { new Guid("3d048539-b597-7a27-d32f-a5045357ac10"), 0.4m, new Guid("7049f0f3-3360-db4a-d6c0-471c0b8c38ad"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 1 },
                    { new Guid("3e93082c-3e9e-3902-341b-aa84a95a1e8c"), 0.4m, new Guid("ece329e3-5bd8-76ac-5838-5a15ed6979b0"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 1 },
                    { new Guid("3f94c5c3-bbeb-bfa5-6d90-55eb3530011e"), 0.4m, new Guid("f6474f74-08d7-6f16-afca-bd7fda20d91c"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("40eb18db-396d-105d-ea59-83b10f16e5fb"), 0.4m, new Guid("7f9de13d-be87-d98e-30fa-2a060f91a8fd"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("41549099-bdf3-6dce-4edd-fb6f6f4b8149"), 0.4m, new Guid("66919704-1f0f-9614-351b-f266f367f142"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 1 },
                    { new Guid("41763600-5cdc-87db-5df8-83a33cbf5814"), 0.4m, new Guid("dc489571-9119-c10b-4b12-e6e8e6ec0873"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("42583f7d-ee49-dc7b-617a-0a5ee8d88526"), 0.4m, new Guid("f56b23d1-5f53-178a-795e-c2a2633ce93e"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 1 },
                    { new Guid("4504fe4b-1e9e-017e-24e2-cd2d3a34e7e7"), 1.0m, new Guid("8a7d80e6-4aec-a10c-dad8-59c46f9688ba"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 0 },
                    { new Guid("464e79ce-b3dc-4476-0179-7c51d47f1d6d"), 1.0m, new Guid("1a407dc5-ad9a-bf79-7803-017232a87766"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 0 },
                    { new Guid("4683a8f1-efcb-92b7-26b4-060f5601f91a"), 1.0m, new Guid("138513e0-dd7e-c591-cd90-ea2fbeaa0d72"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 0 },
                    { new Guid("4851e26d-6e21-ced4-2cbf-5ba9596b4e06"), 1.0m, new Guid("2a9ebe9e-c643-ce00-2606-806bbf63eac6"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 0 },
                    { new Guid("4aee3242-f195-0f1f-31f8-bc97e70db9cb"), 0.4m, new Guid("66919704-1f0f-9614-351b-f266f367f142"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 1 },
                    { new Guid("4c23b451-969f-8b53-f442-3e6195543fb0"), 0.4m, new Guid("0d8f361e-6b85-c865-db5f-7f5cdea192cd"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 1 },
                    { new Guid("4d2a0639-7a0d-4300-9a09-f4053b4f4551"), 0.4m, new Guid("39045745-690e-80cf-d278-ac1edf676a4c"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("4e301c39-3580-a5fc-f584-307008467690"), 1.0m, new Guid("f56b23d1-5f53-178a-795e-c2a2633ce93e"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 0 },
                    { new Guid("4eb0efd0-753c-f751-c303-3eb20a5f0fa5"), 0.4m, new Guid("ac5f7688-7fd6-ee4c-b2bf-c81d38ed9f85"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 1 },
                    { new Guid("4f166a47-34d9-7c65-90fb-ef74a2555c7a"), 1.0m, new Guid("b88dc589-e2a5-1233-868c-fdee59b917f8"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 0 },
                    { new Guid("5694b61b-208f-f580-84db-00d89e77a702"), 0.4m, new Guid("a92c21aa-6df6-1ea3-d98a-6f29ef4595b9"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("5894bc05-119b-a8a6-23bd-48e3fa5db7fe"), 1.0m, new Guid("2b96abda-e80b-13d9-8f5f-66c6984db3a6"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("59cb6831-c225-3cab-d01d-7e1ab73b2c3c"), 0.4m, new Guid("138513e0-dd7e-c591-cd90-ea2fbeaa0d72"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("5f0947e6-511a-ac49-51b6-4354c9544ece"), 1.0m, new Guid("f6474f74-08d7-6f16-afca-bd7fda20d91c"), new Guid("a9eec2ef-ccde-0652-e9f7-f795f1d938c2"), 0 },
                    { new Guid("5fc61095-c1ff-bc2a-013e-7267343918dd"), 1.0m, new Guid("dc489571-9119-c10b-4b12-e6e8e6ec0873"), new Guid("983a359b-2e07-4350-bdb3-3568b0b85dda"), 0 },
                    { new Guid("60bd9f62-fd03-333d-a011-7bbbc782e284"), 0.4m, new Guid("41a196b2-ea98-162f-90dd-b35a0305c0e5"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("631a9d03-22e5-7559-d433-7e26a4134080"), 0.4m, new Guid("2fd873ee-3b62-70ae-caa7-49aad166dd63"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 1 },
                    { new Guid("6613a27c-979a-41ae-33fd-ec3c9b44f1f7"), 0.4m, new Guid("f932cdb6-1c56-a507-8e81-0069b3e48860"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("67185c66-51c1-e48d-063a-b059712afff6"), 1.0m, new Guid("8c0a4eb4-e93a-7c42-c098-7427d51178a8"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 0 },
                    { new Guid("6910584f-1360-2900-5986-fd75f4588355"), 1.0m, new Guid("6d620f7a-37df-3023-326e-f5c015583cc3"), new Guid("bcb0bc9d-0d9f-89e8-f957-337f2614a9b1"), 0 },
                    { new Guid("6a469bf0-3ce9-5d89-d130-93f8d675547c"), 1.0m, new Guid("ece329e3-5bd8-76ac-5838-5a15ed6979b0"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 0 },
                    { new Guid("7102ad7a-1192-c038-b449-b1eced24fcae"), 0.4m, new Guid("5b109901-5cc1-9055-5abc-6a5845f2f6fd"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("725089c1-23bc-2e00-197a-03d1923512a6"), 1.0m, new Guid("3bbfab26-84a9-9e4a-07ef-f8f0f00f9673"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("728ee150-d6f5-7430-b4a4-12171a610be6"), 1.0m, new Guid("8a7d80e6-4aec-a10c-dad8-59c46f9688ba"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("73ea8f88-b20c-c6c4-73ca-ba5ed1e9e26d"), 0.4m, new Guid("7049f0f3-3360-db4a-d6c0-471c0b8c38ad"), new Guid("983a359b-2e07-4350-bdb3-3568b0b85dda"), 1 },
                    { new Guid("77111c20-125e-d3b1-fe90-64b4a8eaa684"), 0.4m, new Guid("7f9de13d-be87-d98e-30fa-2a060f91a8fd"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("7a1e2b76-e567-e27a-9375-f0fced264968"), 0.4m, new Guid("5d47db27-ddd9-7e65-2c80-4318dafc57f7"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("7b8b8fdc-a627-3410-3d8b-e0453a6a1c8b"), 1.0m, new Guid("a2d58546-3cfc-ead2-5910-922b8271d1dd"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("7c4f6c18-5637-19bb-7097-69f01a6c21ab"), 1.0m, new Guid("e63f539e-2321-faab-0bfb-d9b29ed506a0"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 0 },
                    { new Guid("85323032-ea2c-1b6a-08c2-0e691e27be2b"), 1.0m, new Guid("f932cdb6-1c56-a507-8e81-0069b3e48860"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 0 },
                    { new Guid("8a48cc95-ff31-1023-2cf3-5968f09ba0a7"), 1.0m, new Guid("b954092d-34a5-91fe-de80-49c4ffb59298"), new Guid("8d35a1be-c85a-eac7-c2b5-757c17cebb0d"), 0 },
                    { new Guid("8ad82938-f00b-722e-7a13-4b527f9d0133"), 1.0m, new Guid("66919704-1f0f-9614-351b-f266f367f142"), new Guid("983a359b-2e07-4350-bdb3-3568b0b85dda"), 0 },
                    { new Guid("8cf96257-5699-5425-6f01-c0c6c16f302d"), 1.0m, new Guid("7049f0f3-3360-db4a-d6c0-471c0b8c38ad"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 0 },
                    { new Guid("8f1a5ce4-4c8a-a04c-4fbc-c9c2140c6bc7"), 0.4m, new Guid("9eef453a-7a8b-b882-0bce-f33bef17a9d3"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("8f51fa33-0899-6dee-e09b-f7e5b0e7001a"), 0.4m, new Guid("10b96a6a-cf80-96b1-9b5e-b79e15cf167c"), new Guid("bcb0bc9d-0d9f-89e8-f957-337f2614a9b1"), 1 },
                    { new Guid("90d8039b-2c23-829d-85bd-c14b3d9f2f70"), 1.0m, new Guid("e162e2dd-6402-0ca0-ccab-533aefc3f471"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("9275fb75-9c23-54ca-9051-c6a3cf1c09ab"), 0.4m, new Guid("5a7cea54-b47e-13d5-8ec7-0477d5e5cac5"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("92a79e9b-e202-84d5-21ea-9ecafbd3ee6b"), 1.0m, new Guid("cc3cbb7b-332f-6f40-7ccf-b0cd7551f647"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 0 },
                    { new Guid("92e7fd6d-ad73-d9ce-bf81-36a64c8a26c4"), 1.0m, new Guid("7eda7e00-8e80-97c1-75c8-a602f6e9693b"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 0 },
                    { new Guid("94ea3705-cbc8-dddc-b8f0-6483a4ba033c"), 1.0m, new Guid("b88dc589-e2a5-1233-868c-fdee59b917f8"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("962af47d-812c-88c2-abf8-f0e187c6478c"), 0.4m, new Guid("1a407dc5-ad9a-bf79-7803-017232a87766"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("974a2fbe-5fea-1a6a-f478-0a8bbb8cf143"), 0.4m, new Guid("5dcf8302-731d-4d21-711c-a00f684c65f1"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("97cac6e3-c5a4-85b3-b965-7b876ccc9edc"), 0.4m, new Guid("16188d54-c1c9-5571-ad08-91be35559b8c"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 1 },
                    { new Guid("985196a9-ad4d-e24f-06c6-fd096392545f"), 0.4m, new Guid("2b96abda-e80b-13d9-8f5f-66c6984db3a6"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("9a446758-c23f-047b-6bb1-c9d40c73e51c"), 0.4m, new Guid("5b109901-5cc1-9055-5abc-6a5845f2f6fd"), new Guid("983a359b-2e07-4350-bdb3-3568b0b85dda"), 1 },
                    { new Guid("9b37a1e6-3439-8ef4-f50b-70618ace1cf2"), 1.0m, new Guid("c4600c5a-30c2-447e-6913-1c04fa0dd5b0"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 0 },
                    { new Guid("9bae834a-df71-7cf6-dcb4-d1959faac589"), 0.4m, new Guid("8fde6a5d-dcfc-607f-298a-62213b335c08"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 1 },
                    { new Guid("9d3bb211-fa4a-efcb-5aec-33e4c3db673f"), 0.4m, new Guid("41a196b2-ea98-162f-90dd-b35a0305c0e5"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("9d3dd0e0-cab7-3c99-bea0-b4427324f527"), 1.0m, new Guid("2fd873ee-3b62-70ae-caa7-49aad166dd63"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 0 },
                    { new Guid("9faa0a05-dde8-192d-a79b-f75432ed12ee"), 0.4m, new Guid("9eef453a-7a8b-b882-0bce-f33bef17a9d3"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 1 },
                    { new Guid("a0c8c69c-0e71-bff2-2f67-a2c3fd9c56b1"), 0.4m, new Guid("1a407dc5-ad9a-bf79-7803-017232a87766"), new Guid("983a359b-2e07-4350-bdb3-3568b0b85dda"), 1 },
                    { new Guid("a4bc5a42-51c7-cedb-f8fb-735fb479da66"), 1.0m, new Guid("bec5ecd0-69fc-5b69-e4ef-b209303d8a65"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 0 },
                    { new Guid("a6584511-d566-9d61-ec2a-32f15b22fb41"), 1.0m, new Guid("a2d58546-3cfc-ead2-5910-922b8271d1dd"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 0 },
                    { new Guid("a7fa0b9b-9925-9ad6-4c43-c4154d607a13"), 0.4m, new Guid("e606a152-f1ba-7fd4-4098-6ff6358137c0"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("aacf0aec-528f-8546-2214-92288591dbc4"), 0.4m, new Guid("b88dc589-e2a5-1233-868c-fdee59b917f8"), new Guid("8d35a1be-c85a-eac7-c2b5-757c17cebb0d"), 1 },
                    { new Guid("aafb2dbf-706b-28e0-affe-e158279c993a"), 0.4m, new Guid("ac5f7688-7fd6-ee4c-b2bf-c81d38ed9f85"), new Guid("983a359b-2e07-4350-bdb3-3568b0b85dda"), 1 },
                    { new Guid("ae68f091-4752-bc0c-56a8-f29432878712"), 1.0m, new Guid("7049f0f3-3360-db4a-d6c0-471c0b8c38ad"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("af4cd2ae-ee57-290e-8fd8-365dd86b2879"), 1.0m, new Guid("12c04756-7d48-a105-e28b-0fb993391b61"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("b0e0ffb0-f5a9-f745-735b-df626fdd8b63"), 0.4m, new Guid("9eef453a-7a8b-b882-0bce-f33bef17a9d3"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 1 },
                    { new Guid("b193438f-69de-6081-acad-6f7c3980a856"), 0.4m, new Guid("3bbfab26-84a9-9e4a-07ef-f8f0f00f9673"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 1 },
                    { new Guid("b19a1431-4a7f-d68f-b245-7cfc195a5205"), 0.4m, new Guid("b88dc589-e2a5-1233-868c-fdee59b917f8"), new Guid("672447c0-bca7-f7ae-6e23-d69f0f101e8f"), 1 },
                    { new Guid("b248f8a6-b280-34b5-1572-b30b3b1e99f5"), 0.4m, new Guid("7f9de13d-be87-d98e-30fa-2a060f91a8fd"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 1 },
                    { new Guid("b35495ba-5e86-3b2f-d32f-87cc95db706a"), 0.4m, new Guid("7a9ce924-fed9-b4b2-8196-6571bfe25c82"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 1 },
                    { new Guid("b5025e3c-252b-d7f7-700b-0fde054f83d3"), 1.0m, new Guid("41a196b2-ea98-162f-90dd-b35a0305c0e5"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("b687c704-9417-1872-49d7-1ce5e4109119"), 1.0m, new Guid("6657cf47-c7e0-b672-d828-911cb5c27097"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 0 },
                    { new Guid("b805d617-9f75-d90c-ce7b-215d2bd905bd"), 0.4m, new Guid("66919704-1f0f-9614-351b-f266f367f142"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 1 },
                    { new Guid("ba1e33ed-43b9-63fd-c72b-e94255f70428"), 1.0m, new Guid("5d47db27-ddd9-7e65-2c80-4318dafc57f7"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("bc4f154f-3969-8668-495e-85252bed8b41"), 0.4m, new Guid("1e06c8df-e6ba-60d4-ebdf-6bc34297992c"), new Guid("37a3dd84-a1db-46b4-357f-47478a17a912"), 1 },
                    { new Guid("bd44b470-6eff-2946-e6c9-5acc9789b1eb"), 0.4m, new Guid("cb6af2bf-0956-b705-db44-eae52b158429"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("bebba0f0-b340-be62-bf51-e00e14391e44"), 0.4m, new Guid("7eda7e00-8e80-97c1-75c8-a602f6e9693b"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 1 },
                    { new Guid("c07d3532-07fb-8962-88f9-8e2a06a2258f"), 1.0m, new Guid("b968a043-a331-103d-741b-1b9960d8388d"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 0 },
                    { new Guid("c1eb3ba0-0cb2-ba53-1d25-4f9bb73baf25"), 1.0m, new Guid("f3555423-c64e-674e-d195-d158113686ab"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 0 },
                    { new Guid("c24e1074-6447-4f2b-4f70-42400a666523"), 1.0m, new Guid("5a7cea54-b47e-13d5-8ec7-0477d5e5cac5"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("c26d3744-465d-d8bf-128d-dec68a16ebb3"), 1.0m, new Guid("78fd0059-e0a9-a60b-15f7-cb63d1fc0314"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 0 },
                    { new Guid("c4314fbc-bd0f-c8a7-3dae-18ab92818be3"), 0.4m, new Guid("6657cf47-c7e0-b672-d828-911cb5c27097"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 1 },
                    { new Guid("c43cff58-f371-3830-63f6-746913d6179a"), 0.4m, new Guid("5b109901-5cc1-9055-5abc-6a5845f2f6fd"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("c5a2b88f-cfbc-1e9c-b2c1-953e6ded1780"), 1.0m, new Guid("a92c21aa-6df6-1ea3-d98a-6f29ef4595b9"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("c80a31ed-bdb4-d28d-1758-f742ec0d01fc"), 1.0m, new Guid("7a9ce924-fed9-b4b2-8196-6571bfe25c82"), new Guid("37a3dd84-a1db-46b4-357f-47478a17a912"), 0 },
                    { new Guid("c8588da6-e5cb-9939-131d-47e4f5de9e90"), 1.0m, new Guid("10b96a6a-cf80-96b1-9b5e-b79e15cf167c"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 0 },
                    { new Guid("c8d3bb07-c5f7-98d0-8aa4-9b354a44b41d"), 0.4m, new Guid("12c04756-7d48-a105-e28b-0fb993391b61"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 1 },
                    { new Guid("cafcc60c-c270-e515-efe4-857624e4ae3c"), 0.4m, new Guid("bec5ecd0-69fc-5b69-e4ef-b209303d8a65"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 1 },
                    { new Guid("cc8c3811-93e3-8aa2-6374-f0739ad5c70a"), 0.4m, new Guid("8a7d80e6-4aec-a10c-dad8-59c46f9688ba"), new Guid("983a359b-2e07-4350-bdb3-3568b0b85dda"), 1 },
                    { new Guid("cd9c2f7a-2f08-904d-8021-eb4809fcf16d"), 1.0m, new Guid("5b109901-5cc1-9055-5abc-6a5845f2f6fd"), new Guid("c80506ed-dbfd-5cba-d0ba-7edce9309e62"), 0 },
                    { new Guid("ce00a740-6a31-28de-19b1-39db69692e7a"), 1.0m, new Guid("f932cdb6-1c56-a507-8e81-0069b3e48860"), new Guid("0e45dd32-fabd-b1ec-b113-c9a3f9171d09"), 0 },
                    { new Guid("d11cd144-ef9c-addb-f255-f9719be1f2cf"), 0.4m, new Guid("66919704-1f0f-9614-351b-f266f367f142"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 1 },
                    { new Guid("d2ac1697-b31b-ed3c-d566-7415e4ceb467"), 1.0m, new Guid("16713313-d70e-a844-e6de-dfd481ec0cc4"), new Guid("21537ed2-9d76-dfa0-582c-4618304e9eff"), 0 },
                    { new Guid("d3462221-e684-21fa-b42c-1ecf10496a7a"), 0.4m, new Guid("f932cdb6-1c56-a507-8e81-0069b3e48860"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("d408ca25-ff88-7a43-a3b2-afe74b667e0b"), 1.0m, new Guid("2d9b2af6-c29b-7318-0413-2764b164d0db"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("d62a587f-b35d-d69b-9fb4-b29a1217cc4e"), 0.4m, new Guid("e162e2dd-6402-0ca0-ccab-533aefc3f471"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("d6be8fc0-0c74-7eaf-8e3d-ad3049166b8b"), 0.4m, new Guid("6d620f7a-37df-3023-326e-f5c015583cc3"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("d91a9057-76bf-df18-c32d-9397f599f5dd"), 1.0m, new Guid("9eef453a-7a8b-b882-0bce-f33bef17a9d3"), new Guid("f2c8e47c-9c78-4fb2-31ab-66ef2bd23f3e"), 0 },
                    { new Guid("d9829f93-cd93-eca8-6c0f-521fb561cbe0"), 1.0m, new Guid("66919704-1f0f-9614-351b-f266f367f142"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 0 },
                    { new Guid("dd8b3fdb-b4c2-8e9d-bf1e-d168c9bb34d5"), 0.4m, new Guid("8a7d80e6-4aec-a10c-dad8-59c46f9688ba"), new Guid("8d35a1be-c85a-eac7-c2b5-757c17cebb0d"), 1 },
                    { new Guid("e016638e-fe2c-48a3-857e-d99229819a9e"), 0.4m, new Guid("16713313-d70e-a844-e6de-dfd481ec0cc4"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("e08e2d42-c886-a866-e7e5-c68095425d1b"), 0.4m, new Guid("ece329e3-5bd8-76ac-5838-5a15ed6979b0"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 1 },
                    { new Guid("e18e4029-f52e-3fcb-5e7e-36af474d6650"), 1.0m, new Guid("8c0a4eb4-e93a-7c42-c098-7427d51178a8"), new Guid("ed092e1a-da33-ba1d-bd08-df0835c21659"), 0 },
                    { new Guid("e44cf822-83c1-8a56-2293-cc70c91802e6"), 0.4m, new Guid("bec5ecd0-69fc-5b69-e4ef-b209303d8a65"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 1 },
                    { new Guid("e57ab143-dd6d-cfb8-5cf5-586d74a073e5"), 0.4m, new Guid("e63f539e-2321-faab-0bfb-d9b29ed506a0"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("e668815a-2a8f-3e22-5319-90251dc0a9f1"), 0.4m, new Guid("ac5f7688-7fd6-ee4c-b2bf-c81d38ed9f85"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 },
                    { new Guid("e8f1e641-11af-4aed-130e-d327320a692c"), 0.4m, new Guid("e02c3e2e-3cfa-6041-6169-dd1a3565e195"), new Guid("bcb0bc9d-0d9f-89e8-f957-337f2614a9b1"), 1 },
                    { new Guid("ec15db70-9a0b-5d9e-9c59-d37695b562b8"), 0.4m, new Guid("a2d58546-3cfc-ead2-5910-922b8271d1dd"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("ee04e0ea-6d01-c428-ecaa-aac17e0d24a3"), 1.0m, new Guid("f2ff6a9c-3e72-bf7e-3307-33c152e942ae"), new Guid("0caae589-7f78-7576-394d-464dfb8c0795"), 0 },
                    { new Guid("f0ab6835-86d3-597d-28f3-e561662392d0"), 0.4m, new Guid("0d8f361e-6b85-c865-db5f-7f5cdea192cd"), new Guid("30fa87ca-83ed-79d7-6bf5-efb651391042"), 1 },
                    { new Guid("f1908511-23e3-ffb8-8ebe-43d06efa233d"), 0.4m, new Guid("a92c21aa-6df6-1ea3-d98a-6f29ef4595b9"), new Guid("8a90322a-be94-7894-068d-ac4ae7c18e5e"), 1 },
                    { new Guid("f27e32bf-4a4c-e11c-e064-df3409bff1dd"), 0.4m, new Guid("3bbfab26-84a9-9e4a-07ef-f8f0f00f9673"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 1 },
                    { new Guid("f587a658-61bf-5259-86a2-feb21cf82d4e"), 0.4m, new Guid("41a196b2-ea98-162f-90dd-b35a0305c0e5"), new Guid("034001c2-18d5-f61e-1648-387dc8f7575a"), 1 },
                    { new Guid("fa6e9bbc-a11b-a8f4-7305-7da792861a20"), 0.4m, new Guid("6657cf47-c7e0-b672-d828-911cb5c27097"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 1 },
                    { new Guid("fc345a23-ef5b-92aa-526d-72460a20391b"), 1.0m, new Guid("cb6af2bf-0956-b705-db44-eae52b158429"), new Guid("6699c745-e75c-ebed-8395-74a08ce3e976"), 0 },
                    { new Guid("fca70bdd-c14d-98e2-cfd2-a47574318712"), 0.4m, new Guid("5a7cea54-b47e-13d5-8ec7-0477d5e5cac5"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 1 },
                    { new Guid("fd2d736c-387a-d7c8-e18a-23546407ae7b"), 1.0m, new Guid("e02c3e2e-3cfa-6041-6169-dd1a3565e195"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 0 },
                    { new Guid("fd928a44-4728-0a96-58f7-f6be3354d5c2"), 1.0m, new Guid("16188d54-c1c9-5571-ad08-91be35559b8c"), new Guid("f917dd1c-a93b-2cea-ea06-6f944679c2b6"), 0 },
                    { new Guid("fdd6c264-c93d-95c0-bd90-54651a7b8ec5"), 0.4m, new Guid("b968a043-a331-103d-741b-1b9960d8388d"), new Guid("e7c4bded-7302-c31c-8533-8c21a546f0ba"), 1 },
                    { new Guid("fe773749-0630-6fea-3b36-889b76ac79cb"), 0.4m, new Guid("dc489571-9119-c10b-4b12-e6e8e6ec0873"), new Guid("0bf4a24e-d178-5100-f86d-4b4b95229e9d"), 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BodyMeasurements_OwnerId_MeasuredOn",
                table: "BodyMeasurements",
                columns: new[] { "OwnerId", "MeasuredOn" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_Slug",
                table: "Equipment",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseMuscles_ExerciseId_MuscleId",
                table: "ExerciseMuscles",
                columns: new[] { "ExerciseId", "MuscleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseMuscles_MuscleId",
                table: "ExerciseMuscles",
                column: "MuscleId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseNotes_ExerciseId",
                table: "ExerciseNotes",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseNotes_OwnerId_ExerciseId",
                table: "ExerciseNotes",
                columns: new[] { "OwnerId", "ExerciseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_EquipmentId",
                table: "Exercises",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_Name",
                table: "Exercises",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_OwnerId_Name",
                table: "Exercises",
                columns: new[] { "OwnerId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Muscles_Slug",
                table: "Muscles",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonalRecords_OwnerId_AchievedAt",
                table: "PersonalRecords",
                columns: new[] { "OwnerId", "AchievedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PersonalRecords_OwnerId_ExerciseId_Type",
                table: "PersonalRecords",
                columns: new[] { "OwnerId", "ExerciseId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_ProgressPhotos_OwnerId_TakenOn",
                table: "ProgressPhotos",
                columns: new[] { "OwnerId", "TakenOn" });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId_FamilyId",
                table: "RefreshTokens",
                columns: new[] { "UserId", "FamilyId" });

            migrationBuilder.CreateIndex(
                name: "IX_RoutineExercises_ExerciseId",
                table: "RoutineExercises",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutineExercises_RoutineId_Order",
                table: "RoutineExercises",
                columns: new[] { "RoutineId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_RoutineFolders_OwnerId_Name",
                table: "RoutineFolders",
                columns: new[] { "OwnerId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Routines_FolderId",
                table: "Routines",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Routines_OwnerId_Order",
                table: "Routines",
                columns: new[] { "OwnerId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_RoutineSetTemplates_RoutineExerciseId_Order",
                table: "RoutineSetTemplates",
                columns: new[] { "RoutineExerciseId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_OwnerId",
                table: "UserProfiles",
                column: "OwnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_OwnerId",
                table: "UserSettings",
                column: "OwnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_ExerciseId",
                table: "WorkoutExercises",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_WorkoutSessionId_Order",
                table: "WorkoutExercises",
                columns: new[] { "WorkoutSessionId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSchedules_OwnerId_DayOfWeek",
                table: "WorkoutSchedules",
                columns: new[] { "OwnerId", "DayOfWeek" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSchedules_RoutineId",
                table: "WorkoutSchedules",
                column: "RoutineId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_OwnerId_StartedAt",
                table: "WorkoutSessions",
                columns: new[] { "OwnerId", "StartedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_OwnerId_Status",
                table: "WorkoutSessions",
                columns: new[] { "OwnerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_RoutineId",
                table: "WorkoutSessions",
                column: "RoutineId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSets_WorkoutExerciseId_Order",
                table: "WorkoutSets",
                columns: new[] { "WorkoutExerciseId", "Order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BodyMeasurements");

            migrationBuilder.DropTable(
                name: "ExerciseMuscles");

            migrationBuilder.DropTable(
                name: "ExerciseNotes");

            migrationBuilder.DropTable(
                name: "PersonalRecords");

            migrationBuilder.DropTable(
                name: "ProgressPhotos");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RoutineSetTemplates");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "WorkoutSchedules");

            migrationBuilder.DropTable(
                name: "WorkoutSets");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Muscles");

            migrationBuilder.DropTable(
                name: "RoutineExercises");

            migrationBuilder.DropTable(
                name: "WorkoutExercises");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "WorkoutSessions");

            migrationBuilder.DropTable(
                name: "Equipment");

            migrationBuilder.DropTable(
                name: "Routines");

            migrationBuilder.DropTable(
                name: "RoutineFolders");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
