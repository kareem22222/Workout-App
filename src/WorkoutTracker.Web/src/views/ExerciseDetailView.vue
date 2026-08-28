<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Award, ChevronLeft, Lightbulb, Save } from '@lucide/vue'
import ProgressChart from '@/components/ProgressChart.vue'
import { api } from '@/lib/api'
import { ApiError } from '@/lib/http'
import { useSessionStore } from '@/stores/session'
import {
  describeRecord,
  formatDate,
  formatVolume,
  formatWeight,
  setTypeName,
  weightUnitLabel,
} from '@/lib/format'
import type {
  ChartRange,
  Exercise,
  ExerciseHistoryEntry,
  ExerciseProgress,
  OverloadSuggestion,
} from '@/lib/types'

const session = useSessionStore()
const route = useRoute()
const router = useRouter()

const exercise = ref<Exercise | null>(null)
const progress = ref<ExerciseProgress | null>(null)
const history = ref<ExerciseHistoryEntry[]>([])
const suggestion = ref<OverloadSuggestion | null>(null)

const range = ref<ChartRange>('3m')
const metric = ref<'bestWeight' | 'estimatedOneRepMax' | 'volume' | 'maxReps'>('estimatedOneRepMax')
const note = ref('')
const noteSaved = ref(false)
const loading = ref(true)
const error = ref<string | null>(null)

const ranges: Array<{ value: ChartRange; label: string }> = [
  { value: '1m', label: '1M' },
  { value: '3m', label: '3M' },
  { value: '6m', label: '6M' },
  { value: '1y', label: '1Y' },
  { value: 'all', label: 'All' },
]

const metrics = [
  { value: 'estimatedOneRepMax' as const, label: 'Estimated 1RM' },
  { value: 'bestWeight' as const, label: 'Best weight' },
  { value: 'volume' as const, label: 'Volume' },
  { value: 'maxReps' as const, label: 'Max reps' },
]

const exerciseId = computed(() => (typeof route.params.id === 'string' ? route.params.id : ''))
const unit = computed(() => weightUnitLabel(session.weightUnit))

const chartPoints = computed(() => progress.value?.[metric.value] ?? [])

/** Reps are a count, so only weight-derived metrics carry a unit label. */
const chartUnit = computed(() => (metric.value === 'maxReps' ? 'reps' : unit.value))

const activeMetricLabel = computed(() => metrics.find((item) => item.value === metric.value)?.label ?? 'Progress')

async function loadAll() {
  loading.value = true
  error.value = null

  try {
    const [loadedExercise, loadedProgress, loadedHistory] = await Promise.all([
      api.exercises.get(exerciseId.value),
      api.progress.exercise(exerciseId.value, range.value),
      api.exercises.history(exerciseId.value, 1, 20),
    ])

    exercise.value = loadedExercise
    progress.value = loadedProgress
    history.value = loadedHistory.items
    note.value = loadedExercise.persistentNote ?? ''

    // The suggestion is advisory, so a failure here must not block the page.
    try {
      suggestion.value = await api.tools.overload(exerciseId.value)
    } catch {
      suggestion.value = null
    }
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to load this exercise.'
  } finally {
    loading.value = false
  }
}

onMounted(loadAll)

watch(range, async (value) => {
  try {
    progress.value = await api.progress.exercise(exerciseId.value, value)
  } catch {
    // Keep the previous series if the refresh fails.
  }
})

async function saveNote() {
  try {
    await api.exercises.saveNote(exerciseId.value, note.value)
    noteSaved.value = true
    window.setTimeout(() => (noteSaved.value = false), 1800)
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to save the note.'
  }
}
</script>

<template>
  <div class="page narrow-page">
    <header class="page-head">
      <div>
        <button class="icon-button" aria-label="Back" @click="router.back()"><ChevronLeft /></button>
        <span class="eyebrow">
          {{ exercise?.muscles.find((m) => m.role === 'Primary')?.muscleName || exercise?.category || 'EXERCISE' }}
          <template v-if="exercise?.equipmentName"> - {{ exercise.equipmentName }}</template>
        </span>
        <h1>{{ exercise?.name ?? 'Exercise' }}</h1>
        <p v-if="exercise?.isCustom">Custom exercise</p>
      </div>
    </header>

    <p v-if="error" class="form-error" role="alert">{{ error }}</p>
    <p v-if="loading" class="small-empty">Loading…</p>

    <template v-if="exercise">
      <p v-if="exercise.instructions" class="exercise-instructions">{{ exercise.instructions }}</p>

      <section v-if="suggestion && suggestion.action !== 'NotEnoughData'" class="panel suggestion-panel">
        <div class="panel-head">
          <div>
            <span class="eyebrow">NEXT SESSION SUGGESTION</span>
            <h2>
              {{ suggestion.action === 'IncreaseLoad' ? 'Add load' : 'Hold steady' }}
              <template v-if="suggestion.suggestedWeightKg !== null">
                - {{ formatWeight(suggestion.suggestedWeightKg, session.weightUnit, true) }}
              </template>
            </h2>
          </div>
          <span class="stat-icon amber"><Lightbulb :size="19" /></span>
        </div>
        <p>{{ suggestion.rationale }}</p>
        <small class="form-note">A suggestion only. Your logged weights are never changed automatically.</small>
      </section>

      <section class="panel">
        <div class="panel-head">
          <div>
            <span class="eyebrow">PERSISTENT NOTE</span>
            <h2>Your cues</h2>
          </div>
        </div>
        <textarea
          v-model="note"
          rows="2"
          maxlength="2000"
          placeholder="e.g. Seat position 4. Neutral grip."
        ></textarea>
        <button class="btn btn-quiet" @click="saveNote">
          <Save :size="16" /> {{ noteSaved ? 'Saved' : 'Save note' }}
        </button>
      </section>

      <section class="panel">
        <div class="panel-head">
          <div>
            <span class="eyebrow">PROGRESS</span>
            <h2>{{ activeMetricLabel }}</h2>
          </div>
        </div>

        <div class="filter-chips">
          <button
            v-for="option in metrics"
            :key="option.value"
            :class="{ active: metric === option.value }"
            @click="metric = option.value"
          >
            {{ option.label }}
          </button>
        </div>

        <div class="filter-chips">
          <button
            v-for="option in ranges"
            :key="option.value"
            :class="{ active: range === option.value }"
            @click="range = option.value"
          >
            {{ option.label }}
          </button>
        </div>

        <ProgressChart :points="chartPoints" :label="activeMetricLabel" :unit="chartUnit" />
      </section>

      <section v-if="progress && progress.records.length > 0" class="panel">
        <div class="panel-head">
          <div>
            <span class="eyebrow">PERSONAL RECORDS</span>
            <h2>Your bests</h2>
          </div>
          <span class="stat-icon amber"><Award :size="19" /></span>
        </div>

        <div
          v-for="record in progress.records.filter((r) => r.type !== 'MostRepsAtWeight').slice(0, 6)"
          :key="record.id"
          class="activity-row"
        >
          <span>
            <strong>{{ describeRecord(record.type, record.value, record.atWeight, session.weightUnit).label }}</strong>
            <small>{{ formatDate(record.achievedAt, { month: 'short', day: 'numeric', year: 'numeric' }) }}</small>
          </span>
          <strong>{{ describeRecord(record.type, record.value, record.atWeight, session.weightUnit).value }}</strong>
        </div>
      </section>

      <section class="panel">
        <div class="panel-head">
          <div>
            <span class="eyebrow">HISTORY</span>
            <h2>Every session</h2>
          </div>
        </div>

        <RouterLink
          v-for="entry in history"
          :key="entry.workoutSessionId"
          :to="`/history/${entry.workoutSessionId}`"
          class="history-entry"
        >
          <div class="history-entry-head">
            <strong>{{ formatDate(entry.performedAt, { month: 'short', day: 'numeric', year: 'numeric' }) }}</strong>
            <small>
              {{ formatVolume(entry.volume, session.weightUnit) }} {{ unit }}
              <template v-if="entry.bestEstimatedOneRepMax !== null">
                - 1RM {{ formatWeight(entry.bestEstimatedOneRepMax, session.weightUnit) }}
              </template>
            </small>
          </div>
          <div class="history-entry-sets">
            <span v-for="set in entry.sets" :key="set.order" :title="setTypeName(set.type)">
              {{ formatWeight(set.weight, session.weightUnit) }} × {{ set.reps }}
            </span>
          </div>
        </RouterLink>

        <p v-if="history.length === 0 && !loading" class="small-empty">
          You have not logged this exercise yet.
        </p>
      </section>
    </template>
  </div>
</template>
