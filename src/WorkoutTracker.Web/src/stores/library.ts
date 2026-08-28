import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { api } from '@/lib/api'
import { ApiError } from '@/lib/http'
import type { Equipment, Exercise, Muscle, Routine, RoutineFolder, WorkoutSchedule } from '@/lib/types'

/**
 * Cached exercise library, routines and reference taxonomies.
 *
 * Reference data changes rarely, so it is fetched once per session and reused. Routines and
 * exercises are refetched on demand after any mutation.
 */
export const useLibraryStore = defineStore('library', () => {
  const exercises = ref<Exercise[]>([])
  const routines = ref<Routine[]>([])
  const folders = ref<RoutineFolder[]>([])
  const muscles = ref<Muscle[]>([])
  const equipment = ref<Equipment[]>([])
  const schedules = ref<WorkoutSchedule[]>([])

  const loadingExercises = ref(false)
  const loadingRoutines = ref(false)
  const error = ref<string | null>(null)

  let referenceLoaded = false

  /** Distinct categories present in the library, for the filter UI. */
  const categories = computed(() =>
    [...new Set(exercises.value.map((exercise) => exercise.category).filter((value) => value.length > 0))].sort(),
  )

  const exerciseById = computed(() => new Map(exercises.value.map((exercise) => [exercise.id, exercise])))

  function describe(exception: unknown, fallback: string) {
    return exception instanceof ApiError ? exception.message : fallback
  }

  /** Loads muscles and equipment once per session. */
  async function loadReference(force = false) {
    if (referenceLoaded && !force) return

    try {
      const [loadedMuscles, loadedEquipment] = await Promise.all([api.reference.muscles(), api.reference.equipment()])
      muscles.value = loadedMuscles
      equipment.value = loadedEquipment
      referenceLoaded = true
    } catch (exception) {
      error.value = describe(exception, 'Unable to load reference data.')
    }
  }

  async function loadExercises(filters: { search?: string; muscleId?: string | null; equipmentId?: string | null; category?: string | null } = {}) {
    loadingExercises.value = true
    error.value = null

    try {
      exercises.value = await api.exercises.list(filters)
    } catch (exception) {
      error.value = describe(exception, 'Unable to load exercises.')
    } finally {
      loadingExercises.value = false
    }
  }

  async function loadRoutines() {
    loadingRoutines.value = true
    error.value = null

    try {
      const [loadedRoutines, loadedFolders] = await Promise.all([api.routines.list(), api.folders.list()])
      routines.value = loadedRoutines
      folders.value = loadedFolders
    } catch (exception) {
      error.value = describe(exception, 'Unable to load routines.')
    } finally {
      loadingRoutines.value = false
    }
  }

  async function loadSchedules() {
    try {
      schedules.value = await api.schedule.list()
    } catch (exception) {
      error.value = describe(exception, 'Unable to load the schedule.')
    }
  }

  /** Routines grouped by folder, with ungrouped routines last. */
  const groupedRoutines = computed(() => {
    const groups = folders.value
      .slice()
      .sort((a, b) => a.order - b.order || a.name.localeCompare(b.name))
      .map((folder) => ({
        folder,
        routines: routines.value.filter((routine) => routine.folderId === folder.id),
      }))

    const ungrouped = routines.value.filter((routine) => routine.folderId === null)
    return { groups, ungrouped }
  })

  async function deleteRoutine(id: string) {
    await api.routines.remove(id)
    await loadRoutines()
  }

  async function duplicateRoutine(id: string) {
    const copy = await api.routines.duplicate(id)
    await loadRoutines()
    return copy
  }

  async function deleteExercise(id: string) {
    await api.exercises.remove(id)
    await loadExercises()
  }

  return {
    exercises,
    routines,
    folders,
    muscles,
    equipment,
    schedules,
    loadingExercises,
    loadingRoutines,
    error,
    categories,
    exerciseById,
    groupedRoutines,
    loadReference,
    loadExercises,
    loadRoutines,
    loadSchedules,
    deleteRoutine,
    duplicateRoutine,
    deleteExercise,
  }
})
