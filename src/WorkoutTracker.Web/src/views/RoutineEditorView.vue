<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ArrowDown, ArrowUp, Copy, Plus, Search, Trash2, X } from '@lucide/vue'
import { api } from '@/lib/api'
import { ApiError } from '@/lib/http'
import { useLibraryStore } from '@/stores/library'
import { useSessionStore } from '@/stores/session'
import { setTypeCycle, setTypeName, weightUnitLabel } from '@/lib/format'
import type { Exercise, SaveRoutineRequest, SupersetKind, WorkoutSetType } from '@/lib/types'

/** Editable set template row. */
interface DraftSet {
  targetReps: number
  targetRepsMax: number | null
  targetWeight: number | null
  type: WorkoutSetType
}

/** Editable exercise row. */
interface DraftExercise {
  key: string
  exerciseId: string
  exerciseName: string
  restSeconds: number
  notes: string
  supersetGroup: number | null
  supersetKind: SupersetKind
  sets: DraftSet[]
}

const library = useLibraryStore()
const session = useSessionStore()
const route = useRoute()
const router = useRouter()

const routineId = computed(() => (typeof route.params.id === 'string' ? route.params.id : null))
const isEditing = computed(() => routineId.value !== null)

const draft = reactive({
  name: '',
  description: '',
  folderId: null as string | null,
  exercises: [] as DraftExercise[],
})

const loading = ref(false)
const saving = ref(false)
const error = ref<string | null>(null)
const showPicker = ref(false)
const search = ref('')

const unit = computed(() => weightUnitLabel(session.weightUnit))
const totalSets = computed(() => draft.exercises.reduce((sum, exercise) => sum + exercise.sets.length, 0))

const pickerResults = computed(() => {
  const term = search.value.trim().toLowerCase()
  return library.exercises.filter((exercise) => exercise.name.toLowerCase().includes(term)).slice(0, 60)
})

onMounted(async () => {
  loading.value = true

  try {
    await Promise.all([
      library.exercises.length === 0 ? library.loadExercises() : Promise.resolve(),
      library.routines.length === 0 ? library.loadRoutines() : Promise.resolve(),
    ])

    if (routineId.value) await loadExisting(routineId.value)
  } finally {
    loading.value = false
  }
})

async function loadExisting(id: string) {
  try {
    const routine = await api.routines.get(id)

    draft.name = routine.name
    draft.description = routine.description
    draft.folderId = routine.folderId
    draft.exercises = routine.exercises.map((exercise) => ({
      key: exercise.id,
      exerciseId: exercise.exerciseId,
      exerciseName: exercise.exerciseName,
      restSeconds: exercise.restSeconds,
      notes: exercise.notes,
      supersetGroup: exercise.supersetGroup,
      supersetKind: exercise.supersetKind,
      sets: exercise.sets.map((set) => ({
        targetReps: set.targetReps,
        targetRepsMax: set.targetRepsMax,
        targetWeight: set.targetWeight,
        type: set.type,
      })),
    }))
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to load the routine.'
  }
}

function addExercise(exercise: Exercise) {
  draft.exercises.push({
    key: crypto.randomUUID(),
    exerciseId: exercise.id,
    exerciseName: exercise.name,
    restSeconds: exercise.defaultRestSeconds,
    notes: '',
    supersetGroup: null,
    supersetKind: 'None',
    sets: Array.from({ length: 3 }, () => ({
      targetReps: 8,
      targetRepsMax: null,
      targetWeight: null,
      type: 'Normal' as WorkoutSetType,
    })),
  })

  search.value = ''
  showPicker.value = false
}

function duplicateExercise(index: number) {
  const source = draft.exercises[index]
  if (!source) return

  draft.exercises.splice(index + 1, 0, {
    ...source,
    key: crypto.randomUUID(),
    sets: source.sets.map((set) => ({ ...set })),
  })
}

function removeExercise(index: number) {
  draft.exercises.splice(index, 1)
}

function move(index: number, direction: -1 | 1) {
  const target = index + direction
  if (target < 0 || target >= draft.exercises.length) return

  const [moved] = draft.exercises.splice(index, 1)
  draft.exercises.splice(target, 0, moved!)
}

function addSet(exercise: DraftExercise) {
  const last = exercise.sets[exercise.sets.length - 1]
  exercise.sets.push(last ? { ...last } : { targetReps: 8, targetRepsMax: null, targetWeight: null, type: 'Normal' })
}

function removeSet(exercise: DraftExercise, index: number) {
  if (exercise.sets.length <= 1) return
  exercise.sets.splice(index, 1)
}

function cycleType(set: DraftSet) {
  set.type = setTypeCycle[(setTypeCycle.indexOf(set.type) + 1) % setTypeCycle.length] as WorkoutSetType
}

/** Groups this exercise with the next one as a superset (spec US-022). */
function toggleSuperset(index: number) {
  const exercise = draft.exercises[index]
  if (!exercise) return

  if (exercise.supersetGroup !== null) {
    const group = exercise.supersetGroup
    draft.exercises
      .filter((item) => item.supersetGroup === group)
      .forEach((item) => {
        item.supersetGroup = null
        item.supersetKind = 'None'
      })
    return
  }

  const next = draft.exercises[index + 1]
  if (!next) {
    error.value = 'Add another exercise below to create a superset.'
    return
  }

  const group = Math.max(0, ...draft.exercises.map((item) => item.supersetGroup ?? -1)) + 1
  for (const item of [exercise, next]) {
    item.supersetGroup = group
    item.supersetKind = 'Superset'
  }
}

async function save() {
  error.value = null

  if (draft.name.trim().length === 0) {
    error.value = 'Give the routine a name.'
    return
  }

  if (draft.exercises.length === 0) {
    error.value = 'Add at least one exercise.'
    return
  }

  const request: SaveRoutineRequest = {
    name: draft.name.trim(),
    description: draft.description.trim(),
    folderId: draft.folderId,
    exercises: draft.exercises.map((exercise) => ({
      exerciseId: exercise.exerciseId,
      restSeconds: exercise.restSeconds,
      notes: exercise.notes,
      supersetGroup: exercise.supersetGroup,
      supersetKind: exercise.supersetKind,
      sets: exercise.sets.map((set) => ({
        targetReps: set.targetReps,
        // An empty range top means a fixed target rather than a range.
        targetRepsMax: set.targetRepsMax === null || set.targetRepsMax <= set.targetReps ? null : set.targetRepsMax,
        targetWeight: set.targetWeight,
        type: set.type,
      })),
    })),
  }

  saving.value = true

  try {
    if (routineId.value) await api.routines.update(routineId.value, request)
    else await api.routines.create(request)

    await library.loadRoutines()
    await router.push('/routines')
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to save the routine.'
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <div class="page narrow-page">
    <header class="page-head">
      <div>
        <span class="eyebrow">{{ isEditing ? 'EDIT ROUTINE' : 'NEW ROUTINE' }}</span>
        <h1>{{ isEditing ? 'Update your plan' : 'Build a routine' }}</h1>
        <p>{{ draft.exercises.length }} exercises - {{ totalSets }} sets</p>
      </div>
      <button class="btn btn-primary" :disabled="saving" @click="save">
        {{ saving ? 'Saving…' : 'Save routine' }}
      </button>
    </header>

    <p v-if="error" class="form-error" role="alert">{{ error }}</p>
    <p v-if="loading" class="small-empty">Loading…</p>

    <section class="settings-section">
      <label class="field-label">
        Name
        <input v-model="draft.name" maxlength="120" placeholder="e.g. Upper A" />
      </label>

      <label class="field-label">
        Description
        <textarea v-model="draft.description" rows="2" maxlength="2000" placeholder="What this session focuses on"></textarea>
      </label>

      <label class="field-label">
        Folder
        <select v-model="draft.folderId">
          <option :value="null">No folder</option>
          <option v-for="folder in library.folders" :key="folder.id" :value="folder.id">{{ folder.name }}</option>
        </select>
      </label>
    </section>

    <section
      v-for="(exercise, exerciseIndex) in draft.exercises"
      :key="exercise.key"
      class="exercise-card"
      :class="{ superset: exercise.supersetGroup !== null }"
    >
      <header>
        <div>
          <span class="eyebrow">
            {{ exercise.supersetGroup !== null ? `SUPERSET ${exercise.supersetGroup}` : `EXERCISE ${exerciseIndex + 1}` }}
          </span>
          <h2>{{ exercise.exerciseName }}</h2>
        </div>
        <div class="head-actions">
          <button class="icon-button" :disabled="exerciseIndex === 0" aria-label="Move up" @click="move(exerciseIndex, -1)">
            <ArrowUp :size="16" />
          </button>
          <button
            class="icon-button"
            :disabled="exerciseIndex === draft.exercises.length - 1"
            aria-label="Move down"
            @click="move(exerciseIndex, 1)"
          >
            <ArrowDown :size="16" />
          </button>
          <button class="icon-button" aria-label="Duplicate exercise" @click="duplicateExercise(exerciseIndex)">
            <Copy :size="16" />
          </button>
          <button class="icon-button danger-text" aria-label="Remove exercise" @click="removeExercise(exerciseIndex)">
            <Trash2 :size="16" />
          </button>
        </div>
      </header>

      <div class="exercise-controls">
        <input v-model="exercise.notes" placeholder="Exercise note" maxlength="2000" />
        <label>
          Rest
          <select v-model.number="exercise.restSeconds">
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

      <button class="link-button" @click="toggleSuperset(exerciseIndex)">
        {{ exercise.supersetGroup !== null ? 'Ungroup superset' : 'Superset with next exercise' }}
      </button>

      <div class="set-header template-header">
        <span>SET</span>
        <span>TYPE</span>
        <span>REPS</span>
        <span>TO</span>
        <span>{{ unit.toUpperCase() }}</span>
        <span></span>
      </div>

      <div v-for="(set, setIndex) in exercise.sets" :key="setIndex" class="set-row template-row">
        <span class="set-number">{{ setIndex + 1 }}</span>
        <button class="link-button" @click="cycleType(set)">{{ setTypeName(set.type) }}</button>
        <input v-model.number="set.targetReps" type="number" min="0" max="1000" aria-label="Target reps" />
        <input
          v-model.number="set.targetRepsMax"
          type="number"
          min="0"
          max="1000"
          placeholder="-"
          aria-label="Top of rep range"
        />
        <input
          v-model.number="set.targetWeight"
          type="number"
          min="0"
          step="0.5"
          placeholder="-"
          :aria-label="`Target weight in ${unit}`"
        />
        <button
          class="icon-button danger-text"
          :disabled="exercise.sets.length === 1"
          aria-label="Remove set"
          @click="removeSet(exercise, setIndex)"
        >
          <Trash2 :size="15" />
        </button>
      </div>

      <button class="add-set" @click="addSet(exercise)"><Plus :size="16" /> Add set</button>
    </section>

    <div v-if="draft.exercises.length === 0 && !loading" class="inline-empty">
      <strong>No exercises yet</strong>
      <span>Add movements to build this routine.</span>
    </div>

    <button class="add-exercise" @click="showPicker = true"><Plus :size="19" /> Add exercise</button>

    <div v-if="showPicker" class="sheet-backdrop" @click.self="showPicker = false">
      <section class="sheet">
        <header>
          <div>
            <span class="eyebrow">EXERCISE LIBRARY</span>
            <h2>Add exercise</h2>
          </div>
          <button class="icon-button" aria-label="Close" @click="showPicker = false"><X /></button>
        </header>

        <div class="search-field compact">
          <Search :size="17" />
          <input v-model="search" placeholder="Search exercises" aria-label="Search exercises" autofocus />
        </div>

        <button v-for="exercise in pickerResults" :key="exercise.id" class="exercise-option" @click="addExercise(exercise)">
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

        <p v-if="pickerResults.length === 0" class="small-empty">No exercises match your search.</p>
      </section>
    </div>
  </div>
</template>
