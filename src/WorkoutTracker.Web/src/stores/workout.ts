import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { api } from '@/lib/api'
import { ApiError } from '@/lib/http'
import {
  cacheActiveWorkout,
  enqueueMutation,
  outboxSize,
  readCachedWorkout,
  readOutbox,
  removeFromOutbox,
} from '@/lib/offline'
import { setTypeCycle } from '@/lib/format'
import type {
  Exercise,
  UpdateWorkoutRequest,
  WorkoutExercise,
  WorkoutSession,
  WorkoutSet,
  WorkoutSetType,
} from '@/lib/types'
import { useSessionStore } from './session'

/** How long to coalesce rapid edits before saving. Keeps set entry responsive. */
const SAVE_DEBOUNCE_MS = 900

export type SyncState = 'idle' | 'saving' | 'offline' | 'synced' | 'error' | 'conflict'

/**
 * Active workout state.
 *
 * Edits apply to local state immediately and are persisted asynchronously, so set entry
 * never blocks on the network and a failed save cannot erase what was just typed
 * (spec 3.1, 8.1). Pending changes are mirrored into IndexedDB and replayed on reconnect.
 */
export const useWorkoutStore = defineStore('workout', () => {
  const session = useSessionStore()

  const workout = ref<WorkoutSession | null>(null)
  const loading = ref(false)
  const syncState = ref<SyncState>('idle')
  const syncMessage = ref<string | null>(null)
  const serverConflict = ref<WorkoutSession | null>(null)
  const pendingCount = ref(0)

  /** Absolute end timestamp so a backgrounded tab still reports the correct time (spec 7.5). */
  const restEndsAt = ref<number | null>(null)
  /** Remaining seconds captured while the timer is paused. */
  const restPaused = ref(0)
  /** Ticks every second to drive elapsed and rest countdowns. */
  const now = ref(Date.now())

  let saveTimer: number | null = null
  let tickTimer: number | null = null
  let saveInFlight: Promise<void> | null = null

  const hasActiveWorkout = computed(() => workout.value !== null && workout.value.status === 'Active')

  const allSets = computed<WorkoutSet[]>(() =>
    workout.value ? workout.value.exercises.flatMap((exercise) => exercise.sets) : [],
  )

  const completedSets = computed(() => allSets.value.filter((set) => set.completed && set.type !== 'Warmup').length)
  const totalSets = computed(() => allSets.value.length)

  const elapsedSeconds = computed(() => {
    if (!workout.value) return 0
    return Math.max(0, Math.floor((now.value - new Date(workout.value.startedAt).getTime()) / 1000))
  })

  const restSecondsLeft = computed(() => {
    if (restPaused.value > 0) return restPaused.value
    if (restEndsAt.value === null) return 0
    return Math.max(0, Math.ceil((restEndsAt.value - now.value) / 1000))
  })

  const restActive = computed(() => restSecondsLeft.value > 0)

  /** Local volume estimate so the UI updates without waiting for a server round trip. */
  const localVolume = computed(() =>
    workout.value
      ? workout.value.exercises.reduce((total, exercise) => {
          if (exercise.exerciseType !== 'WeightAndReps' && exercise.exerciseType !== 'WeightedBodyweight') return total
          return (
            total +
            exercise.sets.reduce(
              (sum, set) => (set.completed && set.type !== 'Warmup' ? sum + set.weight * set.reps : sum),
              0,
            )
          )
        }, 0)
      : 0,
  )

  function startTicking() {
    if (tickTimer !== null) return
    tickTimer = window.setInterval(() => {
      now.value = Date.now()
    }, 1000)
  }

  function stopTicking() {
    if (tickTimer === null) return
    window.clearInterval(tickTimer)
    tickTimer = null
  }

  /**
   * Loads the active session from the server, falling back to the IndexedDB copy when
   * offline so training can continue.
   */
  async function loadActive() {
    loading.value = true

    try {
      const active = await api.workouts.active()
      workout.value = active
      await cacheActiveWorkout(active)
      syncState.value = 'idle'
      syncMessage.value = null
      await flushOutbox()
    } catch (exception) {
      if (exception instanceof ApiError && exception.isNetworkError) {
        workout.value = await readCachedWorkout()
        syncState.value = 'offline'
        syncMessage.value = 'Offline. Your changes are saved on this device.'
      } else {
        workout.value = null
      }
    } finally {
      loading.value = false
      pendingCount.value = await outboxSize()
      if (workout.value) startTicking()
    }
  }

  async function start(options: { routineId?: string | null; copyFromWorkoutId?: string | null; title?: string | null }) {
    try {
      workout.value = await api.workouts.start(options)
      await cacheActiveWorkout(workout.value)
      startTicking()
      return { ok: true as const }
    } catch (exception) {
      // A conflict means an active session already exists; adopt it and offer resume.
      if (exception instanceof ApiError && exception.isConflict && exception.conflict) {
        workout.value = exception.conflict as WorkoutSession
        await cacheActiveWorkout(workout.value)
        startTicking()
        return { ok: false as const, resumed: true, message: exception.message }
      }

      return {
        ok: false as const,
        resumed: false,
        message: exception instanceof ApiError ? exception.message : 'Unable to start the workout.',
      }
    }
  }

  // -------------------------------------------------------------------------------------
  // Set and exercise editing
  // -------------------------------------------------------------------------------------

  function newSetId() {
    // Client-generated ids make offline replays idempotent.
    return crypto.randomUUID()
  }

  function addSet(exercise: WorkoutExercise) {
    const last = exercise.sets[exercise.sets.length - 1]

    exercise.sets.push({
      id: newSetId(),
      order: exercise.sets.length,
      weight: last?.weight ?? 0,
      reps: last?.reps ?? 8,
      rpe: null,
      type: last?.type ?? 'Normal',
      durationSeconds: null,
      distanceMeters: null,
      notes: '',
      completed: false,
      completedAt: null,
      previous: null,
    })

    queueSave()
  }

  function duplicateSet(exercise: WorkoutExercise, setId: string) {
    const index = exercise.sets.findIndex((set) => set.id === setId)
    if (index < 0) return

    const source = exercise.sets[index]!
    exercise.sets.splice(index + 1, 0, {
      ...source,
      id: newSetId(),
      completed: false,
      completedAt: null,
    })

    reindex(exercise)
    queueSave()
  }

  function removeSet(exercise: WorkoutExercise, setId: string) {
    if (exercise.sets.length <= 1) return
    exercise.sets = exercise.sets.filter((set) => set.id !== setId)
    reindex(exercise)
    queueSave()
  }

  function cycleSetType(set: WorkoutSet) {
    const next = (setTypeCycle.indexOf(set.type) + 1) % setTypeCycle.length
    set.type = setTypeCycle[next] as WorkoutSetType
    queueSave()
  }

  /** Copies the previous performance into the inputs without marking the set complete. */
  function copyPrevious(set: WorkoutSet) {
    if (!set.previous) return
    set.weight = set.previous.weight
    set.reps = set.previous.reps
    queueSave()
  }

  /**
   * Toggles completion and starts the rest timer.
   * Returns a validation message when the values cannot be accepted.
   */
  function toggleSetComplete(exercise: WorkoutExercise, set: WorkoutSet): string | null {
    if (set.weight < 0) return 'Weight cannot be negative.'
    if (set.reps < 0) return 'Reps cannot be negative.'
    if (set.rpe !== null && (set.rpe < 1 || set.rpe > 10)) return 'RPE must be between 1 and 10.'

    set.completed = !set.completed
    set.completedAt = set.completed ? new Date().toISOString() : null

    if (set.completed && session.settings.autoStartRestTimer) {
      // Supersets rest after the group, not after each exercise (spec US-050).
      const restSeconds = resolveRestSeconds(exercise)
      if (restSeconds > 0) startRest(restSeconds)
    }

    queueSave()
    return null
  }

  /**
   * Rest duration for an exercise, preferring the most specific configured value.
   * Within a superset, rest only applies once the last exercise of the group is reached.
   */
  function resolveRestSeconds(exercise: WorkoutExercise): number {
    if (exercise.supersetGroup !== null && workout.value) {
      const group = workout.value.exercises.filter((item) => item.supersetGroup === exercise.supersetGroup)
      const isLastInGroup = group[group.length - 1]?.id === exercise.id
      if (!isLastInGroup) return 0
    }

    return exercise.restSeconds > 0 ? exercise.restSeconds : session.settings.defaultRestSeconds
  }

  function addExercise(exercise: Exercise) {
    if (!workout.value) return

    workout.value.exercises.push({
      id: crypto.randomUUID(),
      exerciseId: exercise.id,
      exerciseName: exercise.name,
      exerciseType: exercise.type,
      order: workout.value.exercises.length,
      restSeconds: exercise.defaultRestSeconds,
      notes: '',
      supersetGroup: null,
      supersetKind: 'None',
      persistentNote: exercise.persistentNote,
      volume: 0,
      sets: Array.from({ length: 3 }, (_, index) => ({
        id: newSetId(),
        order: index,
        weight: 0,
        reps: 8,
        rpe: null,
        type: 'Normal' as WorkoutSetType,
        durationSeconds: null,
        distanceMeters: null,
        notes: '',
        completed: false,
        completedAt: null,
        previous: null,
      })),
    })

    queueSave()
  }

  function removeExercise(exerciseId: string) {
    if (!workout.value) return
    workout.value.exercises = workout.value.exercises.filter((exercise) => exercise.id !== exerciseId)
    workout.value.exercises.forEach((exercise, index) => (exercise.order = index))
    queueSave()
  }

  function moveExercise(exerciseId: string, direction: -1 | 1) {
    if (!workout.value) return

    const exercises = workout.value.exercises
    const index = exercises.findIndex((exercise) => exercise.id === exerciseId)
    const target = index + direction

    if (index < 0 || target < 0 || target >= exercises.length) return

    const [moved] = exercises.splice(index, 1)
    exercises.splice(target, 0, moved!)
    exercises.forEach((exercise, position) => (exercise.order = position))

    queueSave()
  }

  /** Inserts suggested warmup sets ahead of the working sets (spec US-060). */
  async function insertWarmup(exercise: WorkoutExercise, workingWeightKg: number, reps: number) {
    const suggestions = await api.tools.warmup(workingWeightKg, reps)
    if (suggestions.length === 0) return 0

    const warmupSets: WorkoutSet[] = suggestions.map((suggestion, index) => ({
      id: newSetId(),
      order: index,
      weight: suggestion.weightKg,
      reps: suggestion.reps,
      rpe: null,
      type: 'Warmup',
      durationSeconds: null,
      distanceMeters: null,
      notes: '',
      completed: false,
      completedAt: null,
      previous: null,
    }))

    exercise.sets = [...warmupSets, ...exercise.sets]
    reindex(exercise)
    queueSave()

    return warmupSets.length
  }

  function reindex(exercise: WorkoutExercise) {
    exercise.sets.forEach((set, index) => (set.order = index))
  }

  // -------------------------------------------------------------------------------------
  // Rest timer
  // -------------------------------------------------------------------------------------

  function startRest(seconds: number) {
    restPaused.value = 0
    restEndsAt.value = Date.now() + seconds * 1000
    startTicking()
  }

  function adjustRest(deltaSeconds: number) {
    if (restPaused.value > 0) {
      restPaused.value = Math.max(1, restPaused.value + deltaSeconds)
      return
    }

    if (restEndsAt.value === null) return
    restEndsAt.value = Math.max(Date.now() + 1000, restEndsAt.value + deltaSeconds * 1000)
  }

  function toggleRestPause() {
    if (restPaused.value > 0) {
      restEndsAt.value = Date.now() + restPaused.value * 1000
      restPaused.value = 0
      return
    }

    restPaused.value = restSecondsLeft.value
    restEndsAt.value = null
  }

  function skipRest() {
    restPaused.value = 0
    restEndsAt.value = null
  }

  // -------------------------------------------------------------------------------------
  // Persistence
  // -------------------------------------------------------------------------------------

  function buildUpdateRequest(target: WorkoutSession): UpdateWorkoutRequest {
    return {
      title: target.title,
      notes: target.notes,
      version: target.version,
      exercises: target.exercises.map((exercise, index) => ({
        id: exercise.id,
        exerciseId: exercise.exerciseId,
        order: index,
        restSeconds: exercise.restSeconds,
        notes: exercise.notes,
        supersetGroup: exercise.supersetGroup,
        supersetKind: exercise.supersetKind,
        sets: exercise.sets.map((set, setIndex) => ({
          id: set.id,
          order: setIndex,
          weight: set.weight,
          reps: set.reps,
          rpe: set.rpe,
          type: set.type,
          durationSeconds: set.durationSeconds,
          distanceMeters: set.distanceMeters,
          notes: set.notes,
          completed: set.completed,
        })),
      })),
    }
  }

  /** Debounces a save so typing does not produce a request per keystroke. */
  function queueSave() {
    if (!workout.value) return

    void cacheActiveWorkout(workout.value)

    if (saveTimer !== null) window.clearTimeout(saveTimer)
    saveTimer = window.setTimeout(() => void save(), SAVE_DEBOUNCE_MS)
  }

  /** Forces an immediate save, used before navigating away or finishing. */
  async function saveNow() {
    if (saveTimer !== null) {
      window.clearTimeout(saveTimer)
      saveTimer = null
    }
    await save()
  }

  async function save(): Promise<void> {
    if (!workout.value) return

    // Serialize saves so a slower earlier request cannot overwrite a newer one.
    if (saveInFlight) {
      await saveInFlight
      return
    }

    const target = workout.value
    const request = buildUpdateRequest(target)

    syncState.value = 'saving'

    saveInFlight = (async () => {
      try {
        const updated = await api.workouts.update(target.id, request)
        // Preserve the version and server-resolved fields without discarding in-flight edits.
        applyServerState(updated)
        syncState.value = 'idle'
        syncMessage.value = null
        pendingCount.value = await outboxSize()
      } catch (exception) {
        if (exception instanceof ApiError && exception.isNetworkError) {
          await enqueueMutation(target.id, request)
          pendingCount.value = await outboxSize()
          syncState.value = 'offline'
          syncMessage.value = 'Offline. Changes are queued and will sync automatically.'
          return
        }

        if (exception instanceof ApiError && exception.isConflict) {
          syncState.value = 'conflict'
          syncMessage.value = 'This workout changed on another device. Reload the server version before continuing.'
          serverConflict.value = (exception.conflict as WorkoutSession | undefined) ?? null
          return
        }

        syncState.value = 'error'
        syncMessage.value = exception instanceof ApiError ? exception.message : 'Unable to save changes.'
      } finally {
        saveInFlight = null
      }
    })()

    await saveInFlight
  }

  /**
   * Merges authoritative server state into the local copy. The version and previous-value
   * hints must come from the server; local values are otherwise kept intact.
   */
  function applyServerState(updated: WorkoutSession) {
    if (!workout.value || workout.value.id !== updated.id) {
      workout.value = updated
      void cacheActiveWorkout(updated)
      return
    }

    workout.value.version = updated.version
    workout.value.totalVolume = updated.totalVolume
    workout.value.completedSets = updated.completedSets
    workout.value.totalReps = updated.totalReps
    workout.value.durationSeconds = updated.durationSeconds

    for (const serverExercise of updated.exercises) {
      const local = workout.value.exercises.find((exercise) => exercise.id === serverExercise.id)
      if (!local) continue

      local.persistentNote = serverExercise.persistentNote
      local.volume = serverExercise.volume

      for (const serverSet of serverExercise.sets) {
        const localSet = local.sets.find((set) => set.id === serverSet.id)
        if (!localSet) continue
        localSet.previous = serverSet.previous
        localSet.completedAt = serverSet.completedAt
      }
    }

    void cacheActiveWorkout(workout.value)
  }

  /** Replays queued mutations in insertion order (spec US-250). */
  async function flushOutbox() {
    const entries = await readOutbox()
    if (entries.length === 0) {
      pendingCount.value = 0
      return
    }

    for (const entry of entries) {
      if (entry.id === undefined) continue

      try {
        const updated = await api.workouts.update(entry.workoutId, entry.request)
        await removeFromOutbox(entry.id)
        if (workout.value?.id === updated.id) applyServerState(updated)
      } catch (exception) {
        if (exception instanceof ApiError && exception.isNetworkError) break

        if (exception instanceof ApiError && exception.isConflict) {
          syncState.value = 'conflict'
          syncMessage.value = 'Queued changes conflict with a newer server version. Reload the server version to continue.'
          serverConflict.value = (exception.conflict as WorkoutSession | undefined) ?? null
          break
        }

        // A permanent failure must not block the queue forever.
        await removeFromOutbox(entry.id)
        syncState.value = 'error'
        syncMessage.value = exception instanceof ApiError ? exception.message : 'Some queued changes could not sync.'
      }
    }

    pendingCount.value = await outboxSize()
    if (pendingCount.value === 0 && entries.length > 0) {
      syncState.value = 'synced'
      syncMessage.value = 'Offline workout changes synced.'
      window.setTimeout(() => {
        if (syncState.value === 'synced') {
          syncState.value = 'idle'
          syncMessage.value = null
        }
      }, 2500)
    }
  }

  /** Discards conflicted local edits only after the user explicitly chooses server data. */
  async function reloadServerVersion() {
    if (!serverConflict.value) return

    workout.value = serverConflict.value
    serverConflict.value = null
    for (const entry of await readOutbox()) {
      if (entry.id !== undefined && entry.workoutId === workout.value.id) await removeFromOutbox(entry.id)
    }
    await cacheActiveWorkout(workout.value)
    pendingCount.value = await outboxSize()
    syncState.value = 'idle'
    syncMessage.value = null
  }

  async function finish(notes?: string | null) {
    if (!workout.value) return { ok: false as const, message: 'No active workout.' }

    await saveNow()

    const improvedExercises = workout.value.exercises.flatMap((exercise) => {
      const improved = exercise.sets
        .filter((set) => set.completed && set.type !== 'Warmup' && set.previous &&
          (set.weight > set.previous.weight || (set.weight === set.previous.weight && set.reps > set.previous.reps)))
        .sort((a, b) => b.weight - a.weight || b.reps - a.reps)[0]
      return improved?.previous ? [{
        exerciseId: exercise.exerciseId,
        exerciseName: exercise.exerciseName,
        weightKg: improved.weight,
        reps: improved.reps,
        previousWeightKg: improved.previous.weight,
        previousReps: improved.previous.reps,
      }] : []
    })

    try {
      const completion = { ...await api.workouts.finish(workout.value.id, notes), improvedExercises }
      workout.value = null
      restEndsAt.value = null
      restPaused.value = 0
      await cacheActiveWorkout(null)
      stopTicking()
      return { ok: true as const, completion }
    } catch (exception) {
      return {
        ok: false as const,
        message: exception instanceof ApiError ? exception.message : 'Unable to finish the workout.',
      }
    }
  }

  async function cancel() {
    if (!workout.value) return

    try {
      await api.workouts.cancel(workout.value.id)
    } finally {
      workout.value = null
      restEndsAt.value = null
      restPaused.value = 0
      await cacheActiveWorkout(null)
      stopTicking()
    }
  }

  return {
    workout,
    loading,
    syncState,
    syncMessage,
    serverConflict,
    pendingCount,
    restEndsAt,
    restPaused,
    hasActiveWorkout,
    completedSets,
    totalSets,
    elapsedSeconds,
    restSecondsLeft,
    restActive,
    localVolume,
    loadActive,
    start,
    addSet,
    duplicateSet,
    removeSet,
    cycleSetType,
    copyPrevious,
    toggleSetComplete,
    addExercise,
    removeExercise,
    moveExercise,
    insertWarmup,
    startRest,
    adjustRest,
    toggleRestPause,
    skipRest,
    queueSave,
    saveNow,
    flushOutbox,
    reloadServerVersion,
    finish,
    cancel,
    startTicking,
    stopTicking,
  }
})
