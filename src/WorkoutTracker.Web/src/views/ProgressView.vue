<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { Activity, Award, Calculator, Flame, TrendingUp } from '@lucide/vue'
import ProgressChart from '@/components/ProgressChart.vue'
import ProgressPhotos from '@/components/ProgressPhotos.vue'
import MeasurementTracker from '@/components/MeasurementTracker.vue'
import MuscleBreakdown from '@/components/MuscleBreakdown.vue'
import WeightTrend from '@/components/WeightTrend.vue'
import { api } from '@/lib/api'
import { ApiError } from '@/lib/http'
import { useSessionStore } from '@/stores/session'
import {
  describeRecord,
  displayToKg,
  formatDate,
  formatVolume,
  formatWeight,
  kgToDisplay,
  weightUnitLabel,
} from '@/lib/format'
import type {
  BodyMeasurement,
  ChartPoint,
  ChartRange,
  PersonalRecord,
  PlateSolution,
  TrainingStats,
  WarmupSet,
} from '@/lib/types'

const session = useSessionStore()

const stats = ref<TrainingStats | null>(null)
const measurements = ref<BodyMeasurement[]>([])
const records = ref<PersonalRecord[]>([])
const range = ref<ChartRange>('3m')
const loading = ref(true)
const error = ref<string | null>(null)

const weightUnit = computed(() => weightUnitLabel(session.weightUnit))

/** Plate calculator state. */
const plateTarget = ref<number | null>(null)
const plateBar = ref<number | null>(kgToDisplay(session.settings.barWeightKg, session.weightUnit))
const plateSolution = ref<PlateSolution | null>(null)

/** Warmup calculator state. */
const warmupWeight = ref<number | null>(null)
const warmupSets = ref<WarmupSet[]>([])

const ranges: Array<{ value: ChartRange; label: string }> = [
  { value: '1m', label: '1M' },
  { value: '3m', label: '3M' },
  { value: '6m', label: '6M' },
  { value: '1y', label: '1Y' },
  { value: 'all', label: 'All' },
]

const volumePoints = computed<ChartPoint[]>(
  () => stats.value?.series.map((point) => ({ date: point.periodStart, value: point.volume })) ?? [],
)

async function loadAll() {
  loading.value = true
  error.value = null

  try {
    const [loadedStats, loadedMeasurements, loadedRecords] = await Promise.all([
      api.progress.stats(range.value, 'week'),
      api.measurements.list(),
      api.progress.personalRecords(),
    ])

    stats.value = loadedStats
    measurements.value = loadedMeasurements
    records.value = loadedRecords.filter((record) => record.type !== 'MostRepsAtWeight').slice(0, 8)
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to load your progress.'
  } finally {
    loading.value = false
  }
}

onMounted(loadAll)
watch(range, loadAll)

async function calculatePlates() {
  if (plateTarget.value === null || plateTarget.value <= 0) return

  try {
    plateSolution.value = await api.tools.plates(
      displayToKg(plateTarget.value, session.weightUnit),
      plateBar.value === null ? null : displayToKg(plateBar.value, session.weightUnit),
    )
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to calculate plates.'
  }
}

async function calculateWarmup() {
  if (warmupWeight.value === null || warmupWeight.value <= 0) return

  try {
    warmupSets.value = await api.tools.warmup(displayToKg(warmupWeight.value, session.weightUnit))
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to calculate warmups.'
  }
}
</script>

<template>
  <div class="page">
    <header class="page-head">
      <div>
        <span class="eyebrow">THE LONG VIEW</span>
        <h1>Progress</h1>
        <p>Training statistics, records and body measurements.</p>
      </div>
    </header>

    <p v-if="error" class="form-error" role="alert">{{ error }}</p>

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

    <p v-if="loading" class="small-empty">Loading…</p>

    <section class="stat-grid">
      <article class="stat-card">
        <span class="stat-icon coral"><Activity :size="19" /></span>
        <div>
          <strong>{{ stats?.workouts ?? 0 }}</strong>
          <span>Workouts</span>
        </div>
        <small>{{ stats?.currentStreakWeeks ?? 0 }} week streak</small>
      </article>

      <article class="stat-card">
        <span class="stat-icon lime"><TrendingUp :size="19" /></span>
        <div>
          <strong>{{ formatVolume(stats?.totalVolume ?? 0, session.weightUnit) }}</strong>
          <span>Volume - {{ weightUnit }}</span>
        </div>
        <small>{{ stats?.totalSets ?? 0 }} sets</small>
      </article>

      <article class="stat-card">
        <span class="stat-icon purple"><Flame :size="19" /></span>
        <div>
          <strong>{{ Math.floor((stats?.trainingMinutes ?? 0) / 60) }}h</strong>
          <span>Training time</span>
        </div>
        <small>{{ stats?.distinctExercises ?? 0 }} exercises</small>
      </article>

      <article class="stat-card">
        <span class="stat-icon amber"><Award :size="19" /></span>
        <div>
          <strong>{{ stats?.personalRecords ?? 0 }}</strong>
          <span>Records</span>
        </div>
        <small>In this range</small>
      </article>
    </section>

    <section class="panel">
      <div class="panel-head">
        <div>
          <span class="eyebrow">TRAINING VOLUME</span>
          <h2>Weekly totals</h2>
        </div>
      </div>
      <ProgressChart :points="volumePoints" label="Volume" :unit="weightUnit" />
    </section>

    <section class="dashboard-grid">
      <WeightTrend :measurements="measurements" />
      <MuscleBreakdown />
    </section>

    <MeasurementTracker :measurements="measurements" @updated="measurements = $event" />

    <ProgressPhotos />

    <section v-if="records.length > 0" class="panel">
      <div class="panel-head">
        <div>
          <span class="eyebrow">PERSONAL RECORDS</span>
          <h2>Your bests</h2>
        </div>
        <span class="stat-icon amber"><Award :size="19" /></span>
      </div>

      <RouterLink
        v-for="record in records"
        :key="record.id"
        :to="`/exercises/${record.exerciseId}`"
        class="activity-row"
      >
        <span>
          <strong>{{ record.exerciseName }}</strong>
          <small>
            {{ describeRecord(record.type, record.value, record.atWeight, session.weightUnit).label }} -
            {{ formatDate(record.achievedAt) }}
          </small>
        </span>
        <strong>{{ describeRecord(record.type, record.value, record.atWeight, session.weightUnit).value }}</strong>
      </RouterLink>
    </section>

    <section class="dashboard-grid">
      <div class="panel">
        <div class="panel-head">
          <div>
            <span class="eyebrow">PLATE CALCULATOR</span>
            <h2>What to load</h2>
          </div>
          <span class="stat-icon lime"><Calculator :size="19" /></span>
        </div>

        <div class="tool-row">
          <input
            v-model.number="plateTarget"
            type="number"
            min="0"
            step="0.5"
            :placeholder="`Target ${weightUnit}`"
            :aria-label="`Target weight in ${weightUnit}`"
          />
          <input
            v-model.number="plateBar"
            type="number"
            min="0"
            step="0.5"
            :placeholder="`Bar ${weightUnit}`"
            :aria-label="`Bar weight in ${weightUnit}`"
          />
          <button class="btn btn-quiet" @click="calculatePlates">Calculate</button>
        </div>

        <template v-if="plateSolution">
          <p>
            Bar {{ formatWeight(plateSolution.barKg, session.weightUnit, true) }} - per side:
            <template v-if="plateSolution.perSide.length === 0">nothing to load</template>
          </p>
          <div class="plate-list">
            <span v-for="stack in plateSolution.perSide" :key="stack.plateKg" class="plate-chip">
              {{ formatWeight(stack.plateKg, session.weightUnit) }} × {{ stack.countPerSide }}
            </span>
          </div>
          <p v-if="!plateSolution.isExact" class="form-note">{{ plateSolution.message }}</p>
          <p v-else class="form-note">
            Total {{ formatWeight(plateSolution.achievableKg, session.weightUnit, true) }}
          </p>
        </template>
      </div>

      <div class="panel">
        <div class="panel-head">
          <div>
            <span class="eyebrow">WARM-UP CALCULATOR</span>
            <h2>Ramp up</h2>
          </div>
          <span class="stat-icon coral"><Flame :size="19" /></span>
        </div>

        <div class="tool-row">
          <input
            v-model.number="warmupWeight"
            type="number"
            min="0"
            step="0.5"
            :placeholder="`Working ${weightUnit}`"
            :aria-label="`Working weight in ${weightUnit}`"
          />
          <button class="btn btn-quiet" @click="calculateWarmup">Calculate</button>
        </div>

        <div v-for="set in warmupSets" :key="set.order" class="activity-row">
          <span>
            <strong>{{ formatWeight(set.weightKg, session.weightUnit, true) }}</strong>
            <small>{{ set.percentage }}% of working weight</small>
          </span>
          <strong>{{ set.reps }} reps</strong>
        </div>

        <p v-if="warmupSets.length === 0" class="small-empty">Enter a working weight to see a warm-up ramp.</p>
      </div>
    </section>
  </div>
</template>
