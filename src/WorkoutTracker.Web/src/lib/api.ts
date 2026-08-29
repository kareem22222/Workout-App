import { http } from './http'
import type {
  AdminUser,
  AuthResponse,
  BodyMeasurement,
  ChartRange,
  DashboardSummary,
  Equipment,
  Exercise,
  ExerciseHistoryEntry,
  ExerciseProgress,
  ImportPreview,
  ImportResult,
  Muscle,
  OverloadSuggestion,
  Paged,
  PersonalRecord,
  PlateSolution,
  ProgressPhoto,
  Routine,
  RoutineFolder,
  SaveBodyMeasurementRequest,
  SaveExerciseRequest,
  SaveRoutineRequest,
  TrainingStats,
  MuscleContribution,
  UpdateProfileRequest,
  UpdateSettingsRequest,
  UpdateWorkoutRequest,
  UserProfile,
  UserSettings,
  VolumePoint,
  WarmupSet,
  WorkoutCompletion,
  WorkoutSchedule,
  WorkoutSession,
  WorkoutStatus,
  WorkoutSummaryRow,
} from './types'

/** One typed function per endpoint, grouped by resource. */
export const api = {
  auth: {
    register: (displayName: string, email: string, password: string) =>
      http.post<AuthResponse>('/auth/register', { displayName, email, password }, { retryOnUnauthorized: false }),

    login: (email: string, password: string) =>
      http.post<AuthResponse>('/auth/login', { email, password }, { retryOnUnauthorized: false }),

    logout: () => http.post<void>('/auth/logout'),

    me: () => http.get<UserProfile>('/auth/me'),

    updateProfile: (request: UpdateProfileRequest) => http.put<UserProfile>('/auth/me', request),

    changePassword: (currentPassword: string, newPassword: string) =>
      http.post<AuthResponse>('/auth/change-password', { currentPassword, newPassword }),
  },

  reference: {
    muscles: () => http.get<Muscle[]>('/reference/muscles'),
    equipment: () => http.get<Equipment[]>('/reference/equipment'),
  },

  exercises: {
    list: (filters: {
      search?: string
      muscleId?: string | null
      equipmentId?: string | null
      category?: string | null
      includeArchived?: boolean
    } = {}) => http.get<Exercise[]>('/exercises', filters),

    get: (id: string) => http.get<Exercise>(`/exercises/${id}`),

    create: (request: SaveExerciseRequest) => http.post<Exercise>('/exercises', request),

    update: (id: string, request: SaveExerciseRequest) => http.put<Exercise>(`/exercises/${id}`, request),

    remove: (id: string) => http.delete<void>(`/exercises/${id}`),

    saveNote: (id: string, text: string) => http.put<string>(`/exercises/${id}/note`, { text }),

    history: (id: string, page = 1, pageSize = 20) =>
      http.get<Paged<ExerciseHistoryEntry>>(`/exercises/${id}/history`, { page, pageSize }),
  },

  routines: {
    list: (includeArchived = false) => http.get<Routine[]>('/routines', { includeArchived }),
    get: (id: string) => http.get<Routine>(`/routines/${id}`),
    create: (request: SaveRoutineRequest) => http.post<Routine>('/routines', request),
    update: (id: string, request: SaveRoutineRequest) => http.put<Routine>(`/routines/${id}`, request),
    remove: (id: string) => http.delete<void>(`/routines/${id}`),
    duplicate: (id: string) => http.post<Routine>(`/routines/${id}/duplicate`),
    reorder: (routineIdsInOrder: string[], folderId: string | null) =>
      http.post<void>('/routines/reorder', { routineIdsInOrder, folderId }),
  },

  folders: {
    list: () => http.get<RoutineFolder[]>('/routine-folders'),
    create: (name: string, order = 0) => http.post<RoutineFolder>('/routine-folders', { name, order }),
    rename: (id: string, name: string, order = 0) => http.put<RoutineFolder>(`/routine-folders/${id}`, { name, order }),
    remove: (id: string) => http.delete<void>(`/routine-folders/${id}`),
  },

  schedule: {
    list: () => http.get<WorkoutSchedule[]>('/schedule'),
    save: (routineId: string, dayOfWeek: number, isEnabled = true) =>
      http.put<WorkoutSchedule>('/schedule', { routineId, dayOfWeek, isEnabled }),
    remove: (id: string) => http.delete<void>(`/schedule/${id}`),
  },

  workouts: {
    list: (filters: {
      page?: number
      pageSize?: number
      from?: string | null
      to?: string | null
      routineId?: string | null
      exerciseId?: string | null
      status?: WorkoutStatus | null
    } = {}) => http.get<Paged<WorkoutSummaryRow>>('/workouts', filters),

    active: () => http.get<WorkoutSession | null>('/workouts/active'),

    get: (id: string) => http.get<WorkoutSession>(`/workouts/${id}`),

    calendar: (year: number, month: number) =>
      http.get<Record<string, WorkoutSummaryRow[]>>('/workouts/calendar', { year, month }),

    start: (request: { routineId?: string | null; title?: string | null; copyFromWorkoutId?: string | null }) =>
      http.post<WorkoutSession>('/workouts/start', request),

    update: (id: string, request: UpdateWorkoutRequest) => http.put<WorkoutSession>(`/workouts/${id}`, request),

    finish: (id: string, notes?: string | null) =>
      http.post<WorkoutCompletion>(`/workouts/${id}/finish`, { notes: notes ?? null }),

    cancel: (id: string) => http.post<void>(`/workouts/${id}/cancel`),

    remove: (id: string) => http.delete<void>(`/workouts/${id}`),
  },

  progress: {
    exercise: (exerciseId: string, range: ChartRange = '3m') =>
      http.get<ExerciseProgress>(`/progress/exercise/${exerciseId}`, { range }),

    personalRecords: (exerciseId?: string | null) =>
      http.get<PersonalRecord[]>('/progress/personal-records', { exerciseId }),

    volume: (range: ChartRange = '3m', groupBy: 'week' | 'month' = 'week') =>
      http.get<VolumePoint[]>('/progress/volume', { range, groupBy }),

    oneRepMax: () => http.get<OverloadSuggestion[]>('/progress/estimated-one-rep-max'),

    stats: (range: ChartRange = '3m', groupBy: 'week' | 'month' = 'week') =>
      http.get<TrainingStats>('/progress/stats', { range, groupBy }),

    muscles: (range = '1m') => http.get<MuscleContribution[]>('/progress/muscles', { range }),
  },

  tools: {
    plates: (targetKg: number, barKg?: number | null) =>
      http.get<PlateSolution>('/tools/plates', { targetKg, barKg }),

    warmup: (workingWeightKg: number, reps = 8) =>
      http.get<WarmupSet[]>('/tools/warmup', { workingWeightKg, reps }),

    overload: (exerciseId: string) => http.get<OverloadSuggestion>(`/tools/overload/${exerciseId}`),
  },

  measurements: {
    list: () => http.get<BodyMeasurement[]>('/measurements'),
    save: (request: SaveBodyMeasurementRequest) => http.post<BodyMeasurement>('/measurements', request),
    update: (id: string, request: SaveBodyMeasurementRequest) => http.put<BodyMeasurement>(`/measurements/${id}`, request),
    remove: (id: string) => http.delete<void>(`/measurements/${id}`),
  },

  photos: {
    list: () => http.get<ProgressPhoto[]>('/photos'),

    upload: (file: File, takenOn: string, pose: string, weightKg: number | null, notes: string) => {
      const form = new FormData()
      form.append('file', file)
      form.append('takenOn', takenOn)
      form.append('pose', pose)
      if (weightKg !== null) form.append('weightKg', String(weightKg))
      form.append('notes', notes)
      return http.postForm<ProgressPhoto>('/photos', form)
    },

    remove: (id: string) => http.delete<void>(`/photos/${id}`),

    /**
     * Fetches photo bytes through the authorized endpoint and returns an object URL.
     * A plain <img src> cannot be used because the request must carry the bearer token.
     * Callers are responsible for revoking the returned URL.
     */
    contentUrl: async (id: string) => {
      const { blob } = await http.download(`/photos/${id}/content`)
      return URL.createObjectURL(blob)
    },
  },

  dashboard: {
    summary: () => http.get<DashboardSummary>('/dashboard/summary'),
  },

  settings: {
    get: () => http.get<UserSettings>('/settings'),
    update: (request: UpdateSettingsRequest) => http.put<UserSettings>('/settings', request),
  },

  data: {
    exportJson: () => http.download('/export/json'),
    exportCsv: (dataset: 'workouts' | 'sets' | 'exercises' | 'measurements') =>
      http.download('/export/csv', { dataset }),

    previewImport: (file: File) => {
      const form = new FormData()
      form.append('file', file)
      return http.postForm<ImportPreview>('/import/preview', form)
    },

    commitImport: (file: File) => {
      const form = new FormData()
      form.append('file', file)
      return http.postForm<ImportResult>('/import/commit', form)
    },
  },

  admin: {
    users: () => http.get<AdminUser[]>('/admin/users'),
    setDisabled: (id: string, isDisabled: boolean) =>
      http.put<void>(`/admin/users/${id}/disabled`, { isDisabled }),
    info: () => http.get<{ version: string; environment: string; serverTimeUtc: string; databaseHealthy: boolean }>('/admin/info'),
  },
}
