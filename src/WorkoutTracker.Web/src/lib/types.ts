/**
 * TypeScript mirror of the API contract (WorkoutTracker.Application.Contracts).
 * Enums are serialized as strings by the API, so they are modelled as string unions.
 */

export type WorkoutStatus = 'Active' | 'Completed' | 'Cancelled'

export type WorkoutSetType = 'Normal' | 'Warmup' | 'DropSet' | 'Failure' | 'Amrap' | 'Backoff'

export type ExerciseType =
  | 'WeightAndReps'
  | 'BodyweightReps'
  | 'WeightedBodyweight'
  | 'Duration'
  | 'Cardio'

export type MuscleRole = 'Primary' | 'Secondary'

export type PersonalRecordType =
  | 'HeaviestWeight'
  | 'MostRepsAtWeight'
  | 'BestEstimatedOneRepMax'
  | 'BestSetVolume'
  | 'BestWorkoutVolume'

export type OneRepMaxFormula = 'Epley' | 'Brzycki' | 'Lombardi'

export type TrainingGoal = 'GeneralFitness' | 'FatLoss' | 'Strength' | 'Hypertrophy' | 'Endurance'

export type WeightUnit = 'Kilograms' | 'Pounds'

export type LengthUnit = 'Centimeters' | 'Inches'

export type ThemePreference = 'System' | 'Dark' | 'Light'

export type PhotoPose = 'Front' | 'Side' | 'Back'

export type SupersetKind = 'None' | 'Superset' | 'TriSet' | 'GiantSet' | 'Circuit'

export type OverloadAction = 'Maintain' | 'IncreaseLoad' | 'ReduceLoad' | 'NotEnoughData'

/** Chart ranges accepted by the progress endpoints. */
export type ChartRange = '1m' | '3m' | '6m' | '1y' | 'all'

// ---------------------------------------------------------------------------------------
// Reference data
// ---------------------------------------------------------------------------------------

export interface Muscle {
  id: string
  slug: string
  name: string
  bodyRegion: string
}

export interface Equipment {
  id: string
  slug: string
  name: string
  defaultBarWeightKg: number | null
}

export interface ExerciseMuscle {
  muscleId: string
  muscleName: string
  role: MuscleRole
  contributionWeight: number
}

// ---------------------------------------------------------------------------------------
// Exercises
// ---------------------------------------------------------------------------------------

export interface Exercise {
  id: string
  name: string
  instructions: string
  type: ExerciseType
  category: string
  equipmentId: string | null
  equipmentName: string | null
  mediaUrl: string | null
  defaultRestSeconds: number
  defaultIncrementKg: number
  isCustom: boolean
  isArchived: boolean
  muscles: ExerciseMuscle[]
  persistentNote: string | null
}

export interface SaveExerciseRequest {
  name: string
  instructions?: string | null
  type: ExerciseType
  category?: string | null
  equipmentId?: string | null
  mediaUrl?: string | null
  defaultRestSeconds?: number
  defaultIncrementKg?: number
  muscles?: Array<{ muscleId: string; role: MuscleRole; contributionWeight?: number | null }>
}

export interface LoggedSet {
  order: number
  weight: number
  reps: number
  rpe: number | null
  type: WorkoutSetType
  durationSeconds: number | null
  distanceMeters: number | null
  notes: string
}

export interface ExerciseHistoryEntry {
  workoutSessionId: string
  workoutTitle: string
  performedAt: string
  sets: LoggedSet[]
  volume: number
  bestEstimatedOneRepMax: number | null
}

// ---------------------------------------------------------------------------------------
// Routines
// ---------------------------------------------------------------------------------------

export interface RoutineFolder {
  id: string
  name: string
  order: number
  routineCount: number
}

export interface RoutineSetTemplate {
  id: string
  order: number
  targetReps: number
  targetRepsMax: number | null
  targetWeight: number | null
  type: WorkoutSetType
}

export interface RoutineExercise {
  id: string
  exerciseId: string
  exerciseName: string
  exerciseType: ExerciseType
  order: number
  restSeconds: number
  notes: string
  supersetGroup: number | null
  supersetKind: SupersetKind
  sets: RoutineSetTemplate[]
}

export interface Routine {
  id: string
  name: string
  description: string
  folderId: string | null
  folderName: string | null
  order: number
  createdAt: string
  updatedAt: string
  exerciseCount: number
  setCount: number
  exercises: RoutineExercise[]
}

export interface SaveRoutineRequest {
  name: string
  description?: string | null
  folderId?: string | null
  exercises: Array<{
    exerciseId: string
    restSeconds: number
    notes?: string | null
    supersetGroup?: number | null
    supersetKind: SupersetKind
    sets: Array<{
      targetReps: number
      targetRepsMax?: number | null
      targetWeight?: number | null
      type: WorkoutSetType
    }>
  }>
}

// ---------------------------------------------------------------------------------------
// Workouts
// ---------------------------------------------------------------------------------------

export interface PreviousSet {
  weight: number
  reps: number
  performedAt: string
}

export interface WorkoutSet {
  id: string
  order: number
  weight: number
  reps: number
  rpe: number | null
  type: WorkoutSetType
  durationSeconds: number | null
  distanceMeters: number | null
  notes: string
  completed: boolean
  completedAt: string | null
  previous: PreviousSet | null
}

export interface WorkoutExercise {
  id: string
  exerciseId: string
  exerciseName: string
  exerciseType: ExerciseType
  order: number
  restSeconds: number
  notes: string
  supersetGroup: number | null
  supersetKind: SupersetKind
  persistentNote: string | null
  volume: number
  sets: WorkoutSet[]
}

export interface WorkoutSession {
  id: string
  routineId: string | null
  title: string
  status: WorkoutStatus
  startedAt: string
  completedAt: string | null
  notes: string
  version: number
  durationSeconds: number
  totalVolume: number
  completedSets: number
  totalSets: number
  totalReps: number
  exercises: WorkoutExercise[]
}

export interface WorkoutSummaryRow {
  id: string
  title: string
  status: WorkoutStatus
  startedAt: string
  completedAt: string | null
  durationSeconds: number
  totalVolume: number
  completedSets: number
  exerciseCount: number
  exerciseNames: string[]
}

export interface UpdateWorkoutRequest {
  title?: string | null
  notes?: string | null
  version: number
  exercises: Array<{
    id: string
    exerciseId: string
    order: number
    restSeconds: number
    notes?: string | null
    supersetGroup?: number | null
    supersetKind: SupersetKind
    sets: Array<{
      id: string
      order: number
      weight: number
      reps: number
      rpe: number | null
      type: WorkoutSetType
      durationSeconds?: number | null
      distanceMeters?: number | null
      notes?: string | null
      completed: boolean
    }>
  }>
}

export interface WorkoutCompletion {
  id: string
  title: string
  startedAt: string
  completedAt: string
  durationSeconds: number
  completedSets: number
  totalReps: number
  totalVolume: number
  newRecords: PersonalRecord[]
  muscleBreakdown: MuscleContribution[]
}

// ---------------------------------------------------------------------------------------
// Progress
// ---------------------------------------------------------------------------------------

export interface PersonalRecord {
  id: string
  exerciseId: string
  exerciseName: string
  type: PersonalRecordType
  value: number
  atWeight: number | null
  workoutSessionId: string
  achievedAt: string
}

export interface ChartPoint {
  date: string
  value: number
}

export interface ExerciseProgress {
  exerciseId: string
  exerciseName: string
  range: string
  bestWeight: ChartPoint[]
  estimatedOneRepMax: ChartPoint[]
  volume: ChartPoint[]
  maxReps: ChartPoint[]
  records: PersonalRecord[]
}

export interface VolumePoint {
  periodStart: string
  volume: number
  workouts: number
  sets: number
}

export interface TrainingStats {
  from: string
  to: string
  workouts: number
  trainingMinutes: number
  totalVolume: number
  totalSets: number
  totalReps: number
  distinctExercises: number
  personalRecords: number
  currentStreakWeeks: number
  series: VolumePoint[]
}

export interface MuscleContribution {
  muscleName: string
  bodyRegion: string
  score: number
  sets: number
}

// ---------------------------------------------------------------------------------------
// Measurements
// ---------------------------------------------------------------------------------------

export interface BodyMeasurement {
  id: string
  measuredOn: string
  weightKg: number | null
  bodyFatPercent: number | null
  chestCm: number | null
  waistCm: number | null
  hipsCm: number | null
  leftArmCm: number | null
  rightArmCm: number | null
  leftThighCm: number | null
  rightThighCm: number | null
  leftCalfCm: number | null
  rightCalfCm: number | null
  shouldersCm: number | null
  neckCm: number | null
  notes: string
}

export type SaveBodyMeasurementRequest = Omit<BodyMeasurement, 'id'>

export interface ProgressPhoto {
  id: string
  takenOn: string
  pose: PhotoPose
  weightKg: number | null
  notes: string
  sizeBytes: number
  contentType: string
}

// ---------------------------------------------------------------------------------------
// Profile, settings, dashboard
// ---------------------------------------------------------------------------------------

export interface UserProfile {
  userId: string
  displayName: string
  email: string
  isAdmin: boolean
  dateOfBirth: string | null
  gender: string | null
  heightCm: number | null
  goal: TrainingGoal
  hasAvatar: boolean
  latestWeightKg: number | null
}

export interface UpdateProfileRequest {
  displayName: string
  dateOfBirth: string | null
  gender: string | null
  heightCm: number | null
  goal: TrainingGoal
}

export interface UserSettings {
  weightUnit: WeightUnit
  lengthUnit: LengthUnit
  timeZone: string
  theme: ThemePreference
  oneRepMaxFormula: OneRepMaxFormula
  defaultRestSeconds: number
  autoStartRestTimer: boolean
  restTimerSound: boolean
  restTimerVibrate: boolean
  restTimerNotifications: boolean
  barWeightKg: number
  plateInventoryKg: number[]
  roundingIncrementKg: number
  overloadIncrementKg: number
  warmupPercentages: number[]
  weeklyWorkoutGoal: number
}

export type UpdateSettingsRequest = UserSettings

export interface WorkoutSchedule {
  id: string
  routineId: string
  routineName: string
  dayOfWeek: number | string
  isEnabled: boolean
}

export interface DashboardSummary {
  displayName: string
  activeWorkout: WorkoutSession | null
  nextScheduledRoutine: Routine | null
  nextScheduledDay: string | number | null
  workoutsThisWeek: number
  weeklyWorkoutGoal: number
  volumeThisWeek: number
  trainingMinutesThisWeek: number
  currentStreakWeeks: number
  recentRecords: PersonalRecord[]
  recentWorkouts: WorkoutSummaryRow[]
  latestWeightKg: number | null
  latestWeightOn: string | null
  weightChange30DaysKg: number | null
}

// ---------------------------------------------------------------------------------------
// Tools
// ---------------------------------------------------------------------------------------

export interface PlateStack {
  plateKg: number
  countPerSide: number
}

export interface PlateSolution {
  requestedKg: number
  achievableKg: number
  barKg: number
  perSide: PlateStack[]
  isExact: boolean
  message: string | null
}

export interface WarmupSet {
  order: number
  percentage: number
  weightKg: number
  reps: number
}

export interface OverloadSuggestion {
  exerciseId: string
  exerciseName: string
  action: OverloadAction
  suggestedWeightKg: number | null
  previousWeightKg: number | null
  rationale: string
}

// ---------------------------------------------------------------------------------------
// Import and admin
// ---------------------------------------------------------------------------------------

export interface ImportRowPreview {
  rowNumber: number
  date: string
  exercise: string
  weight: string
  reps: string
  error: string | null
}

export interface ImportPreview {
  totalRows: number
  validRows: number
  invalidRows: number
  canCommit: boolean
  rows: ImportRowPreview[]
}

export interface ImportResult {
  workoutsCreated: number
  setsCreated: number
  rowsSkipped: number
}

export interface AdminUser {
  id: string
  displayName: string
  email: string
  isAdmin: boolean
  isDisabled: boolean
  createdAt: string
  workoutCount: number
  lastWorkoutAt: string | null
}

// ---------------------------------------------------------------------------------------
// Shared shapes
// ---------------------------------------------------------------------------------------

export interface Paged<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasMore: boolean
}

export interface AuthResponse {
  accessToken: string
  expiresAt: string
  user?: UserProfile
}
