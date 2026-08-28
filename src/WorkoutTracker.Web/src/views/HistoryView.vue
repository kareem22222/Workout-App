<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { ChevronRight, Filter, History } from '@lucide/vue'
import { api } from '@/lib/api'
import { ApiError } from '@/lib/http'
import { useLibraryStore } from '@/stores/library'
import { useSessionStore } from '@/stores/session'
import { formatDate, formatVolume, weightUnitLabel } from '@/lib/format'
import type { WorkoutSummaryRow } from '@/lib/types'

const session = useSessionStore()
const library = useLibraryStore()

const rows = ref<WorkoutSummaryRow[]>([])
const page = ref(1)
const hasMore = ref(false)
const total = ref(0)
const loading = ref(false)
const error = ref<string | null>(null)

const filters = ref({
  from: '' as string,
  to: '' as string,
  routineId: null as string | null,
  exerciseId: null as string | null,
})

const unit = computed(() => weightUnitLabel(session.weightUnit))

async function load(reset = false) {
  if (reset) {
    page.value = 1
    rows.value = []
  }

  loading.value = true
  error.value = null

  try {
    const result = await api.workouts.list({
      page: page.value,
      pageSize: 20,
      // Empty strings must not be sent as date filters.
      from: filters.value.from ? new Date(filters.value.from).toISOString() : null,
      to: filters.value.to ? new Date(`${filters.value.to}T23:59:59`).toISOString() : null,
      routineId: filters.value.routineId,
      exerciseId: filters.value.exerciseId,
      status: 'Completed',
    })

    rows.value = page.value === 1 ? result.items : [...rows.value, ...result.items]
    hasMore.value = result.hasMore
    total.value = result.totalCount
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to load your history.'
  } finally {
    loading.value = false
  }
}

async function loadMore() {
  page.value += 1
  await load()
}

onMounted(async () => {
  await Promise.all([load(true), library.loadRoutines(), library.loadExercises()])
})

// Refetch from the first page whenever a filter changes.
watch(filters, () => void load(true), { deep: true })

function clearFilters() {
  filters.value = { from: '', to: '', routineId: null, exerciseId: null }
}
</script>

<template>
  <div class="page">
    <header class="page-head">
      <div>
        <span class="eyebrow">EVERY SESSION</span>
        <h1>History</h1>
        <p>{{ total }} completed {{ total === 1 ? 'workout' : 'workouts' }}.</p>
      </div>
    </header>

    <p v-if="error" class="form-error" role="alert">{{ error }}</p>

    <details class="filter-panel">
      <summary class="btn btn-quiet"><Filter :size="16" /> Filters</summary>
      <div class="filter-grid">
        <label class="field-label">From<input v-model="filters.from" type="date" /></label>
        <label class="field-label">To<input v-model="filters.to" type="date" /></label>
        <label class="field-label">
          Routine
          <select v-model="filters.routineId">
            <option :value="null">Any routine</option>
            <option v-for="routine in library.routines" :key="routine.id" :value="routine.id">{{ routine.name }}</option>
          </select>
        </label>
        <label class="field-label">
          Exercise
          <select v-model="filters.exerciseId">
            <option :value="null">Any exercise</option>
            <option v-for="exercise in library.exercises" :key="exercise.id" :value="exercise.id">
              {{ exercise.name }}
            </option>
          </select>
        </label>
        <button class="btn btn-quiet" @click="clearFilters">Clear filters</button>
      </div>
    </details>

    <section class="panel">
      <RouterLink v-for="workout in rows" :key="workout.id" :to="`/history/${workout.id}`" class="activity-row">
        <span class="date-tile">
          <strong>{{ new Date(workout.startedAt).getDate() }}</strong>
          <small>{{ formatDate(workout.startedAt, { month: 'short' }) }}</small>
        </span>
        <span>
          <strong>{{ workout.title }}</strong>
          <small>
            {{ Math.round(workout.durationSeconds / 60) }} min ·
            {{ workout.completedSets }} sets ·
            {{ formatVolume(workout.totalVolume, session.weightUnit) }} {{ unit }}
          </small>
          <small class="muted">{{ workout.exerciseNames.slice(0, 3).join(', ') }}</small>
        </span>
        <ChevronRight :size="18" />
      </RouterLink>

      <p v-if="loading" class="small-empty">Loading…</p>

      <div v-else-if="rows.length === 0" class="inline-empty">
        <span class="empty-icon"><History :size="24" /></span>
        <strong>No workouts found</strong>
        <span>Complete a workout, or widen your filters.</span>
      </div>

      <button v-if="hasMore && !loading" class="btn btn-quiet btn-wide" @click="loadMore">Load more</button>
    </section>
  </div>
</template>
