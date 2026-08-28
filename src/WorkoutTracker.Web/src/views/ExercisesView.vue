<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { Plus, Search, Trash2, X } from '@lucide/vue'
import { api } from '@/lib/api'
import { ApiError } from '@/lib/http'
import { useLibraryStore } from '@/stores/library'
import type { ExerciseType, MuscleRole, SaveExerciseRequest } from '@/lib/types'

const library = useLibraryStore()

const search = ref('')
const muscleId = ref<string | null>(null)
const equipmentId = ref<string | null>(null)
const notice = ref<string | null>(null)
const dialog = ref<HTMLDialogElement>()

const exerciseTypes: Array<{ value: ExerciseType; label: string }> = [
  { value: 'WeightAndReps', label: 'Weight and reps' },
  { value: 'BodyweightReps', label: 'Bodyweight reps' },
  { value: 'WeightedBodyweight', label: 'Weighted bodyweight' },
  { value: 'Duration', label: 'Duration' },
  { value: 'Cardio', label: 'Cardio' },
]

const form = reactive({
  name: '',
  type: 'WeightAndReps' as ExerciseType,
  category: '',
  equipmentId: null as string | null,
  instructions: '',
  primaryMuscleId: null as string | null,
  secondaryMuscleId: null as string | null,
  defaultRestSeconds: 90,
  defaultIncrementKg: 2.5,
})

let searchTimer: number | null = null

onMounted(async () => {
  await Promise.all([library.loadReference(), library.loadExercises()])
})

/** Debounce so typing does not fire a request per keystroke. */
watch([search, muscleId, equipmentId], () => {
  if (searchTimer !== null) window.clearTimeout(searchTimer)

  searchTimer = window.setTimeout(() => {
    void library.loadExercises({
      search: search.value.trim() || undefined,
      muscleId: muscleId.value,
      equipmentId: equipmentId.value,
    })
  }, 250)
})

const grouped = computed(() => {
  const byRegion = new Map<string, typeof library.exercises>()

  for (const exercise of library.exercises) {
    const primary = exercise.muscles.find((muscle) => muscle.role === 'Primary')
    const region = primary?.muscleName ?? exercise.category ?? 'Other'
    if (!byRegion.has(region)) byRegion.set(region, [])
    byRegion.get(region)!.push(exercise)
  }

  return [...byRegion.entries()].sort((a, b) => a[0].localeCompare(b[0]))
})

function openCreate() {
  Object.assign(form, {
    name: '',
    type: 'WeightAndReps' as ExerciseType,
    category: '',
    equipmentId: null,
    instructions: '',
    primaryMuscleId: null,
    secondaryMuscleId: null,
    defaultRestSeconds: 90,
    defaultIncrementKg: 2.5,
  })

  notice.value = null
  dialog.value?.showModal()
}

async function create() {
  if (form.name.trim().length < 2) {
    notice.value = 'Enter a name with at least 2 characters.'
    return
  }

  const muscles: SaveExerciseRequest['muscles'] = []
  if (form.primaryMuscleId) muscles.push({ muscleId: form.primaryMuscleId, role: 'Primary' as MuscleRole })
  if (form.secondaryMuscleId && form.secondaryMuscleId !== form.primaryMuscleId) {
    muscles.push({ muscleId: form.secondaryMuscleId, role: 'Secondary' as MuscleRole })
  }

  try {
    await api.exercises.create({
      name: form.name.trim(),
      instructions: form.instructions.trim(),
      type: form.type,
      category: form.category.trim(),
      equipmentId: form.equipmentId,
      defaultRestSeconds: form.defaultRestSeconds,
      defaultIncrementKg: form.defaultIncrementKg,
      muscles,
    })

    dialog.value?.close()
    await library.loadExercises({ search: search.value.trim() || undefined })
  } catch (exception) {
    notice.value = exception instanceof ApiError ? exception.message : 'Unable to create the exercise.'
  }
}

async function remove(id: string, name: string) {
  if (!window.confirm(`Delete "${name}"? If it appears in past workouts it will be archived instead.`)) return

  try {
    await library.deleteExercise(id)
  } catch (exception) {
    notice.value = exception instanceof ApiError ? exception.message : 'Unable to delete the exercise.'
  }
}

function clearFilters() {
  search.value = ''
  muscleId.value = null
  equipmentId.value = null
}
</script>

<template>
  <div class="page">
    <header class="page-head">
      <div>
        <span class="eyebrow">MOVEMENT LIBRARY</span>
        <h1>Exercises</h1>
        <p>{{ library.exercises.length }} movements available.</p>
      </div>
      <button class="btn btn-primary" @click="openCreate"><Plus :size="18" /> Custom exercise</button>
    </header>

    <p v-if="notice" class="form-error" role="alert">{{ notice }}</p>
    <p v-if="library.error" class="form-error" role="alert">{{ library.error }}</p>

    <div class="search-field">
      <Search :size="19" />
      <input v-model="search" placeholder="Search exercises" aria-label="Search exercises" />
    </div>

    <div class="filter-grid">
      <label class="field-label">
        Muscle
        <select v-model="muscleId">
          <option :value="null">All muscles</option>
          <option v-for="muscle in library.muscles" :key="muscle.id" :value="muscle.id">
            {{ muscle.name }} ({{ muscle.bodyRegion }})
          </option>
        </select>
      </label>

      <label class="field-label">
        Equipment
        <select v-model="equipmentId">
          <option :value="null">All equipment</option>
          <option v-for="item in library.equipment" :key="item.id" :value="item.id">{{ item.name }}</option>
        </select>
      </label>

      <button class="btn btn-quiet" @click="clearFilters">Clear</button>
    </div>

    <p v-if="library.loadingExercises" class="small-empty">Loading exercises…</p>

    <section v-else class="exercise-browser">
      <div class="exercise-list">
        <template v-for="[region, items] in grouped" :key="region">
          <div class="folder-head"><span>{{ region.toUpperCase() }}</span><small>{{ items.length }}</small></div>

          <div v-for="item in items" :key="item.id" class="exercise-row">
            <RouterLink :to="`/exercises/${item.id}`" class="exercise-row-link">
              <span class="exercise-glyph">{{ item.name.charAt(0) }}</span>
              <span>
                <strong>{{ item.name }}</strong>
                <small>
                  {{ item.equipmentName || 'No equipment' }}
                  <template v-if="item.isCustom"> - Custom</template>
                  <template v-if="item.isArchived"> - Archived</template>
                </small>
              </span>
            </RouterLink>
            <button
              v-if="item.isCustom"
              class="icon-button danger-text"
              :aria-label="`Delete ${item.name}`"
              @click="remove(item.id, item.name)"
            >
              <Trash2 :size="16" />
            </button>
          </div>
        </template>

        <p v-if="library.exercises.length === 0" class="small-empty">No exercises match your search.</p>
      </div>
    </section>

    <dialog ref="dialog" class="form-dialog">
      <form method="dialog" @submit.prevent="create">
        <header>
          <div>
            <span class="eyebrow">CUSTOM MOVEMENT</span>
            <h2>Create exercise</h2>
          </div>
          <button type="button" class="icon-button" aria-label="Close" @click="dialog?.close()"><X /></button>
        </header>

        <label class="field-label">
          Name
          <input v-model="form.name" required maxlength="120" placeholder="Exercise name" />
        </label>

        <div class="field-pair">
          <label class="field-label">
            Type
            <select v-model="form.type">
              <option v-for="option in exerciseTypes" :key="option.value" :value="option.value">
                {{ option.label }}
              </option>
            </select>
          </label>

          <label class="field-label">
            Equipment
            <select v-model="form.equipmentId">
              <option :value="null">None</option>
              <option v-for="item in library.equipment" :key="item.id" :value="item.id">{{ item.name }}</option>
            </select>
          </label>
        </div>

        <div class="field-pair">
          <label class="field-label">
            Primary muscle
            <select v-model="form.primaryMuscleId">
              <option :value="null">None</option>
              <option v-for="muscle in library.muscles" :key="muscle.id" :value="muscle.id">{{ muscle.name }}</option>
            </select>
          </label>

          <label class="field-label">
            Secondary muscle
            <select v-model="form.secondaryMuscleId">
              <option :value="null">None</option>
              <option v-for="muscle in library.muscles" :key="muscle.id" :value="muscle.id">{{ muscle.name }}</option>
            </select>
          </label>
        </div>

        <div class="field-pair">
          <label class="field-label">
            Category
            <input v-model="form.category" maxlength="60" placeholder="e.g. Push" />
          </label>

          <label class="field-label">
            Default rest (seconds)
            <input v-model.number="form.defaultRestSeconds" type="number" min="0" max="3600" />
          </label>
        </div>

        <label class="field-label">
          Instructions
          <textarea v-model="form.instructions" rows="4" maxlength="4000" placeholder="Setup and technique cues"></textarea>
        </label>

        <button class="btn btn-primary btn-wide" type="submit">Create exercise</button>
      </form>
    </dialog>
  </div>
</template>
