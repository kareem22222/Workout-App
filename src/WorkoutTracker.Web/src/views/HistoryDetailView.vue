<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ChevronLeft, Clock3, Dumbbell, Pencil, Repeat, Trash2, TrendingUp } from '@lucide/vue'
import { api } from '@/lib/api'
import { ApiError } from '@/lib/http'
import { useSessionStore } from '@/stores/session'
import {
  displayToKg,
  formatDateTime,
  formatDuration,
  formatVolume,
  formatWeight,
  kgToDisplay,
  setTypeName,
  weightUnitLabel,
} from '@/lib/format'
import type { UpdateWorkoutRequest, WorkoutSession } from '@/lib/types'

const session = useSessionStore()
const route = useRoute()
const router = useRouter()

const workout = ref<WorkoutSession | null>(null)
const loading = ref(true)
const saving = ref(false)
const error = ref<string | null>(null)

/**
 * Editing is opt-in. Historical data is read-only by default so a stray tap cannot
 * silently rewrite a past session (spec US-082).
 */
const editing = ref(false)

const unit = computed(() => weightUnitLabel(session.weightUnit))
const workoutId = computed(() => (typeof route.params.id === 'string' ? route.params.id : ''))

async function load() {
  loading.value = true
  error.value = null

  try {
    workout.value = await api.workouts.get(workoutId.value)
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to load this workout.'
  } finally {
    loading.value = false
  }
}

onMounted(load)

function buildRequest(target: WorkoutSession): UpdateWorkoutRequest {
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

/** Saving recalculates derived volume and records server-side. */
async function save() {
  if (!workout.value) return

  saving.value = true
  error.value = null

  try {
    workout.value = await api.workouts.update(workout.value.id, buildRequest(workout.value))
    editing.value = false
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to save your changes.'
  } finally {
    saving.value = false
  }
}

function removeSet(exerciseIndex: number, setId: string) {
  const exercise = workout.value?.exercises[exerciseIndex]
  if (!exercise || exercise.sets.length <= 1) return
  if (!window.confirm('Delete this set?')) return
  exercise.sets = exercise.sets.filter((set) => set.id !== setId)
}

function setDisplayWeight(set: { weight: number }, event: Event) {
  const value = Number((event.target as HTMLInputElement).value)
  set.weight = displayToKg(Number.isFinite(value) ? value : 0, session.weightUnit)
}

async function removeWorkout() {
  if (!workout.value) return
  if (!window.confirm('Delete this workout permanently? Records will be recalculated.')) return

  try {
    await api.workouts.remove(workout.value.id)
    await router.push('/history')
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to delete this workout.'
  }
}
</script>

<template>
  <div class="page narrow-page">
    <header class="page-head">
      <div>
        <button class="icon-button" aria-label="Back to history" @click="router.push('/history')">
          <ChevronLeft />
        </button>
        <span class="eyebrow">{{ workout ? formatDateTime(workout.startedAt) : 'WORKOUT' }}</span>
        <h1>
          <input v-if="editing && workout" v-model="workout.title" class="workout-title-input" maxlength="120" />
          <template v-else>{{ workout?.title ?? 'Workout' }}</template>
        </h1>
        <p v-if="editing" class="editing-hint">Edit mode - changes recalculate your volume and records.</p>
      </div>

      <div class="head-actions">
        <button v-if="!editing" class="btn btn-quiet" @click="editing = true"><Pencil :size="16" /> Edit</button>
        <template v-else>
          <button class="btn btn-quiet" @click="load(); editing = false">Cancel</button>
          <button class="btn btn-primary" :disabled="saving" @click="save">{{ saving ? 'Saving…' : 'Save' }}</button>
        </template>
      </div>
    </header>

    <p v-if="error" class="form-error" role="alert">{{ error }}</p>
    <p v-if="loading" class="small-empty">Loading…</p>

    <template v-if="workout">
      <section class="stat-grid">
        <article class="stat-card">
          <span class="stat-icon purple"><Clock3 :size="19" /></span>
          <div>
            <strong>{{ formatDuration(workout.durationSeconds) }}</strong>
            <span>Duration</span>
          </div>
        </article>
        <article class="stat-card">
          <span class="stat-icon lime"><TrendingUp :size="19" /></span>
          <div>
            <strong>{{ formatVolume(workout.totalVolume, session.weightUnit) }}</strong>
            <span>Volume - {{ unit }}</span>
          </div>
        </article>
        <article class="stat-card">
          <span class="stat-icon coral"><Dumbbell :size="19" /></span>
          <div>
            <strong>{{ workout.completedSets }}</strong>
            <span>Sets</span>
          </div>
        </article>
        <article class="stat-card">
          <span class="stat-icon amber"><Repeat :size="19" /></span>
          <div>
            <strong>{{ workout.totalReps }}</strong>
            <span>Reps</span>
          </div>
        </article>
      </section>

      <label v-if="editing" class="field-label">
        Workout note
        <textarea v-model="workout.notes" rows="2" maxlength="4000"></textarea>
      </label>
      <p v-else-if="workout.notes" class="workout-note-display">{{ workout.notes }}</p>

      <section v-for="(exercise, exerciseIndex) in workout.exercises" :key="exercise.id" class="exercise-card">
        <header>
          <div>
            <span class="eyebrow">{{ exercise.supersetGroup !== null ? `SUPERSET ${exercise.supersetGroup}` : 'EXERCISE' }}</span>
            <h2>{{ exercise.exerciseName }}</h2>
          </div>
          <RouterLink :to="`/exercises/${exercise.exerciseId}`" class="link-button">View exercise</RouterLink>
        </header>

        <p v-if="exercise.notes" class="exercise-note-display">{{ exercise.notes }}</p>

        <div class="set-header history-header">
          <span>SET</span>
          <span>TYPE</span>
          <span>{{ unit.toUpperCase() }}</span>
          <span>REPS</span>
          <span>RPE</span>
          <span></span>
        </div>

        <div v-for="(set, setIndex) in exercise.sets" :key="set.id" class="set-row history-row">
          <span class="set-number">{{ setIndex + 1 }}</span>
          <span class="set-type-label">{{ setTypeName(set.type) }}</span>

          <template v-if="editing">
            <input :value="kgToDisplay(set.weight, session.weightUnit)" type="number" min="0" step="0.5" :aria-label="`Weight in ${unit}`" @change="setDisplayWeight(set, $event)" />
            <input v-model.number="set.reps" type="number" min="0" aria-label="Repetitions" />
            <input v-model.number="set.rpe" type="number" min="1" max="10" step="0.5" placeholder="-" aria-label="RPE" />
            <button
              class="icon-button danger-text"
              :disabled="exercise.sets.length === 1"
              aria-label="Delete set"
              @click="removeSet(exerciseIndex, set.id)"
            >
              <Trash2 :size="15" />
            </button>
          </template>

          <template v-else>
            <span>{{ formatWeight(set.weight, session.weightUnit) }}</span>
            <span>{{ set.reps }}</span>
            <span>{{ set.rpe ?? '-' }}</span>
            <span></span>
          </template>
        </div>
      </section>

      <button v-if="editing" class="discard-workout" @click="removeWorkout">
        <Trash2 :size="16" /> Delete this workout
      </button>
    </template>
  </div>
</template>
