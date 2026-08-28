<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import { onBeforeRouteLeave, useRouter } from 'vue-router'
import {
  ArrowDown,
  ArrowUp,
  Check,
  ChevronLeft,
  Clock3,
  Copy,
  Flame,
  MoreHorizontal,
  Plus,
  Search,
  TimerReset,
  Trash2,
  X,
} from '@lucide/vue'
import { api } from '@/lib/api'
import { useSessionStore } from '@/stores/session'
import { useWorkoutStore } from '@/stores/workout'
import { useLibraryStore } from '@/stores/library'
import { formatDuration, formatWeight, setTypeLabel, setTypeName, weightUnitLabel } from '@/lib/format'
import type { Exercise, WorkoutExercise, WorkoutSet } from '@/lib/types'

const session = useSessionStore()
const workouts = useWorkoutStore()
const library = useLibraryStore()
const router = useRouter()

const showExercisePicker = ref(false)
const search = ref('')
const notice = ref<string | null>(null)
const finishing = ref(false)

const unit = computed(() => weightUnitLabel(session.weightUnit))
const workout = computed(() => workouts.workout)

/** Exercises not already in the workout, filtered by the search box. */
const availableExercises = computed(() => {
  const term = search.value.trim().toLowerCase()
  const present = new Set(workout.value?.exercises.map((exercise) => exercise.exerciseId) ?? [])

  return library.exercises.filter(
    (exercise) => !present.has(exercise.id) && exercise.name.toLowerCase().includes(term),
  )
})

const restLabel = computed(() => formatDuration(workouts.restSecondsLeft))

onMounted(async () => {
  workouts.startTicking()
  if (!workout.value) await workouts.loadActive()
  if (library.exercises.length === 0) await library.loadExercises()
})

onBeforeUnmount(() => {
  // Flush pending edits so navigating away never loses input.
  void workouts.saveNow()
})

// Warn before a full page unload while a workout is in progress (spec US-230).
function beforeUnload(event: BeforeUnloadEvent) {
  if (workouts.hasActiveWorkout && workouts.syncState !== 'idle') {
    event.preventDefault()
    event.returnValue = ''
  }
}

onMounted(() => window.addEventListener('beforeunload', beforeUnload))
onBeforeUnmount(() => window.removeEventListener('beforeunload', beforeUnload))

onBeforeRouteLeave(async () => {
  await workouts.saveNow()
  return true
})

/** Alert when the rest timer reaches zero, honouring the user's opt-in preferences. */
watch(
  () => workouts.restSecondsLeft,
  (value, previous) => {
    if (previous <= 0 || value !== 0) return

    if (session.settings.restTimerVibrate) navigator.vibrate?.([180, 80, 180])

    if (session.settings.restTimerNotifications && 'Notification' in window && Notification.permission === 'granted') {
      new Notification('Rest complete', { body: 'Time for your next set.' })
    }
  },
)

async function toggleComplete(exercise: WorkoutExercise, set: WorkoutSet) {
  const problem = workouts.toggleSetComplete(exercise, set)
  if (problem) {
    notice.value = problem
    return
  }

  notice.value = null

  // Ask for notification permission at the moment it becomes useful, never on load.
  if (
    set.completed &&
    session.settings.restTimerNotifications &&
    'Notification' in window &&
    Notification.permission === 'default'
  ) {
    await Notification.requestPermission()
  }
}

function addExercise(exercise: Exercise) {
  workouts.addExercise(exercise)
  search.value = ''
  showExercisePicker.value = false
}

function removeExercise(exercise: WorkoutExercise) {
  const hasCompleted = exercise.sets.some((set) => set.completed)
  if (hasCompleted && !window.confirm(`Remove ${exercise.exerciseName} and its completed sets?`)) return
  workouts.removeExercise(exercise.id)
}

/** Deleting a completed set is confirmed to avoid losing logged work (spec US-034). */
function removeSet(exercise: WorkoutExercise, set: WorkoutSet) {
  if (set.completed && !window.confirm('Delete this completed set?')) return
  workouts.removeSet(exercise, set.id)
}

async function addWarmup(exercise: WorkoutExercise) {
  const working = exercise.sets.find((set) => set.type !== 'Warmup' && set.weight > 0)

  if (!working) {
    notice.value = 'Enter a working weight first so warmups can be calculated.'
    return
  }

  const inserted = await workouts.insertWarmup(exercise, working.weight, working.reps)
  notice.value = inserted > 0 ? `Added ${inserted} warmup sets.` : 'No warmup sets are needed for that weight.'
}

/** Toggles superset grouping with the following exercise (spec US-050). */
function toggleSuperset(exercise: WorkoutExercise, index: number) {
  if (!workout.value) return

  if (exercise.supersetGroup !== null) {
    const group = exercise.supersetGroup
    workout.value.exercises
      .filter((item) => item.supersetGroup === group)
      .forEach((item) => {
        item.supersetGroup = null
        item.supersetKind = 'None'
      })
  } else {
    const next = workout.value.exercises[index + 1]
    if (!next) {
      notice.value = 'Add another exercise below to create a superset.'
      return
    }

    const group = Math.max(0, ...workout.value.exercises.map((item) => item.supersetGroup ?? -1)) + 1
    for (const item of [exercise, next]) {
      item.supersetGroup = group
      item.supersetKind = 'Superset'
    }
  }

  workouts.queueSave()
}

async function saveNote(exercise: WorkoutExercise) {
  // Persist the reusable note so it reappears next session (spec US-150).
  try {
    await api.exercises.saveNote(exercise.exerciseId, exercise.persistentNote ?? '')
  } catch {
    notice.value = 'The exercise note could not be saved.'
  }
}

async function finish() {
  if (workouts.completedSets === 0) {
    notice.value = 'Complete at least one set before finishing.'
    return
  }

  if (
    workouts.completedSets < workouts.totalSets &&
    !window.confirm('Some sets are not complete. They will be discarded. Finish anyway?')
  ) {
    return
  }

  finishing.value = true

  try {
    const result = await workouts.finish(workout.value?.notes ?? null)

    if (!result.ok) {
      notice.value = result.message ?? 'Unable to finish the workout.'
      return
    }

    // Hand the summary to the next screen through history state.
    await router.push({ name: 'workout-summary', state: { completion: JSON.stringify(result.completion) } })
  } finally {
    finishing.value = false
  }
}

async function cancel() {
  if (!window.confirm('Discard this workout? Logged sets will not be saved.')) return
  await workouts.cancel()
  await router.push('/')
}

async function startEmpty() {
  const result = await workouts.start({ title: 'Quick workout' })
  if (!result.ok && !result.resumed) notice.value = result.message ?? null
}
</script>

<template>
  <div v-if="workouts.loading && !workout" class="empty-state">
    <span class="empty-icon"><TimerReset :size="28" /></span>
    <h1>Loading your workout…</h1>
  </div>

  <div v-else-if="!workout" class="empty-state">
    <span class="empty-icon"><TimerReset :size="28" /></span>
    <h1>No workout in progress</h1>
    <p>Pick a routine or start an empty workout.</p>
    <div class="empty-actions">
      <RouterLink to="/routines" class="btn btn-primary">Browse routines</RouterLink>
      <button class="btn btn-quiet" @click="startEmpty">Start empty</button>
    </div>
  </div>

  <div v-else class="workout-page">
    <header class="workout-top">
      <button class="icon-button" aria-label="Back to dashboard" @click="router.push('/')">
        <ChevronLeft />
      </button>
      <div>
        <input
          v-model="workout.title"
          class="workout-title-input"
          maxlength="120"
          aria-label="Workout title"
          @change="workouts.queueSave()"
        />
        <span><span class="live-dot"></span>{{ formatDuration(workouts.elapsedSeconds) }}</span>
      </div>
      <button class="btn btn-finish" :disabled="finishing" @click="finish">
        {{ finishing ? 'Saving…' : 'Finish' }}
      </button>
    </header>

    <div class="workout-progress">
      <span :style="{ width: `${workouts.totalSets ? (workouts.completedSets / workouts.totalSets) * 100 : 0}%` }"></span>
    </div>

    <main class="workout-content">
      <p v-if="notice" class="form-error" role="status">{{ notice }}</p>

      <label class="workout-note-field">
        <input
          v-model="workout.notes"
          placeholder="Add a workout note"
          maxlength="4000"
          @change="workouts.queueSave()"
        />
      </label>

      <section
        v-for="(exercise, exerciseIndex) in workout.exercises"
        :key="exercise.id"
        class="exercise-card"
        :class="{ superset: exercise.supersetGroup !== null }"
      >
        <header>
          <div>
            <span class="eyebrow">
              {{ exercise.supersetGroup !== null ? `SUPERSET ${exercise.supersetGroup}` : exercise.exerciseType === 'Cardio' ? 'CARDIO' : 'EXERCISE' }}
            </span>
            <h2>{{ exercise.exerciseName }}</h2>
          </div>

          <details class="action-menu exercise-actions">
            <summary class="icon-button"><MoreHorizontal :size="20" /></summary>
            <div>
              <button :disabled="exerciseIndex === 0" @click="workouts.moveExercise(exercise.id, -1)">
                <ArrowUp :size="15" /> Move up
              </button>
              <button
                :disabled="exerciseIndex === workout.exercises.length - 1"
                @click="workouts.moveExercise(exercise.id, 1)"
              >
                <ArrowDown :size="15" /> Move down
              </button>
              <button @click="addWarmup(exercise)"><Flame :size="15" /> Add warmup</button>
              <button @click="toggleSuperset(exercise, exerciseIndex)">
                <Copy :size="15" /> {{ exercise.supersetGroup !== null ? 'Ungroup superset' : 'Superset with next' }}
              </button>
              <button class="danger-text" @click="removeExercise(exercise)"><Trash2 :size="15" /> Remove</button>
            </div>
          </details>
        </header>

        <div class="exercise-controls">
          <input
            v-model="exercise.persistentNote"
            placeholder="Exercise note (saved for next time)"
            maxlength="2000"
            @change="saveNote(exercise)"
          />
          <label>
            Rest
            <select v-model.number="exercise.restSeconds" @change="workouts.queueSave()">
              <option :value="0">Off</option>
              <option :value="30">0:30</option>
              <option :value="60">1:00</option>
              <option :value="90">1:30</option>
              <option :value="120">2:00</option>
              <option :value="150">2:30</option>
              <option :value="180">3:00</option>
              <option :value="240">4:00</option>
            </select>
          </label>
        </div>

        <div class="set-header">
          <span>SET</span>
          <span>PREVIOUS</span>
          <span>{{ unit.toUpperCase() }}</span>
          <span>REPS</span>
          <span>RPE</span>
          <span></span>
        </div>

        <div
          v-for="(set, setIndex) in exercise.sets"
          :key="set.id"
          class="set-row"
          :class="{ complete: set.completed, warmup: set.type === 'Warmup' }"
        >
          <details class="set-menu">
            <summary class="set-number" :title="setTypeName(set.type)">
              {{ setTypeLabel(set.type) || setIndex + 1 }}
            </summary>
            <div>
              <strong>{{ setTypeName(set.type) }}</strong>
              <button @click="workouts.cycleSetType(set)">Change type</button>
              <button @click="workouts.duplicateSet(exercise, set.id)"><Copy :size="13" /> Duplicate</button>
              <button
                :disabled="exercise.sets.length === 1"
                class="danger-text"
                @click="removeSet(exercise, set)"
              >
                <Trash2 :size="13" /> Delete
              </button>
            </div>
          </details>

          <button
            class="previous"
            :disabled="!set.previous"
            :aria-label="set.previous ? `Copy previous ${formatWeight(set.previous.weight, session.weightUnit)} by ${set.previous.reps}` : 'No previous performance'"
            @click="workouts.copyPrevious(set)"
          >
            <template v-if="set.previous">
              {{ formatWeight(set.previous.weight, session.weightUnit) }} × {{ set.previous.reps }}
              <Copy :size="12" />
            </template>
            <template v-else>—</template>
          </button>

          <input
            v-model.number="set.weight"
            type="number"
            inputmode="decimal"
            min="0"
            step="0.5"
            :aria-label="`Weight in ${unit}`"
            @change="workouts.queueSave()"
          />
          <input
            v-model.number="set.reps"
            type="number"
            inputmode="numeric"
            min="0"
            aria-label="Repetitions"
            @change="workouts.queueSave()"
          />
          <input
            v-model.number="set.rpe"
            type="number"
            inputmode="decimal"
            min="1"
            max="10"
            step="0.5"
            placeholder="-"
            aria-label="Rate of perceived exertion"
            @change="workouts.queueSave()"
          />

          <button
            class="check-button"
            :aria-label="set.completed ? 'Mark set incomplete' : 'Complete set'"
            :aria-pressed="set.completed"
            @click="toggleComplete(exercise, set)"
          >
            <Check :size="19" stroke-width="3" />
          </button>
        </div>

        <button class="add-set" @click="workouts.addSet(exercise)"><Plus :size="16" /> Add set</button>
      </section>

      <div v-if="workout.exercises.length === 0" class="inline-empty workout-empty">
        <strong>Your workout is empty</strong>
        <span>Add an exercise to begin logging.</span>
      </div>

      <button class="add-exercise" @click="showExercisePicker = true"><Plus :size="19" /> Add exercise</button>
      <button class="cancel-workout" @click="router.push('/')">Save and close</button>
      <button class="discard-workout" @click="cancel">Discard workout</button>
    </main>

    <div v-if="workouts.restActive" class="rest-timer">
      <span>
        <Clock3 :size="20" />
        <span>
          <small>{{ workouts.restPaused > 0 ? 'PAUSED' : 'REST TIMER' }}</small>
          <strong>{{ restLabel }}</strong>
        </span>
      </span>
      <button @click="workouts.adjustRest(-30)">-30s</button>
      <button @click="workouts.toggleRestPause()">{{ workouts.restPaused > 0 ? 'Resume' : 'Pause' }}</button>
      <button @click="workouts.adjustRest(30)">+30s</button>
      <button aria-label="Skip rest timer" @click="workouts.skipRest()"><X :size="18" /></button>
    </div>

    <div v-if="showExercisePicker" class="sheet-backdrop" @click.self="showExercisePicker = false">
      <section class="sheet">
        <header>
          <div>
            <span class="eyebrow">EXERCISE LIBRARY</span>
            <h2>Add exercise</h2>
          </div>
          <button class="icon-button" aria-label="Close" @click="showExercisePicker = false"><X /></button>
        </header>

        <div class="search-field compact">
          <Search :size="17" />
          <input v-model="search" placeholder="Search exercises" aria-label="Search exercises" autofocus />
        </div>

        <button
          v-for="exercise in availableExercises.slice(0, 60)"
          :key="exercise.id"
          class="exercise-option"
          @click="addExercise(exercise)"
        >
          <span class="exercise-glyph">{{ exercise.name.charAt(0) }}</span>
          <span>
            <strong>{{ exercise.name }}</strong>
            <small>
              {{ exercise.muscles.find((m) => m.role === 'Primary')?.muscleName || exercise.category || 'Exercise' }}
              <template v-if="exercise.equipmentName"> - {{ exercise.equipmentName }}</template>
            </small>
          </span>
          <Plus :size="19" />
        </button>

        <p v-if="availableExercises.length === 0" class="small-empty">No more matching exercises.</p>
      </section>
    </div>
  </div>
</template>
