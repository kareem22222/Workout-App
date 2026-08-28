<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { Activity, Award, Calculator, Flame, Plus, Trash2, TrendingUp } from '@lucide/vue'
import ProgressChart from '@/components/ProgressChart.vue'
import ProgressPhotos from '@/components/ProgressPhotos.vue'
import { api } from '@/lib/api'
import { ApiError } from '@/lib/http'
import { useSessionStore } from '@/stores/session'
import {
  cmToDisplay,
  describeRecord,
  displayToCm,
  displayToKg,
  formatDate,
  formatVolume,
  formatWeight,
  kgToDisplay,
  lengthUnitLabel,
  todayIsoDate,
  weightUnitLabel,
} from '@/lib/format'
import type {
  BodyMeasurement,
  ChartPoint,
  ChartRange,
  MuscleContribution,
  PersonalRecord,
  PlateSolution,
  TrainingStats,
  WarmupSet,
} from '@/lib/types'

const session = useSessionStore()

const stats = ref<TrainingStats | null>(null)
const measurements = ref<BodyMeasurement[]>([])
const records = ref<PersonalRecord[]>([])
const muscles = ref<MuscleContribution[]>([])
const range = ref<ChartRange>('3m')
const loading = ref(true)
const error = ref<string | null>(null)

const weightUnit = computed(() => weightUnitLabel(session.weightUnit))
const lengthUnit = computed(() => lengthUnitLabel(session.settings.lengthUnit))

/** Measurement form. Values are entered in display units and converted on save. */
const form = reactive({
  measuredOn: todayIsoDate(),
  weight: null as number | null,
  bodyFatPercent: null as number | null,
  chest: null as number | null,
  waist: null as number | null,
  hips: null as number | null,
  leftArm: null as number | null,
  rightArm: null as number | null,
  leftThigh: null as number | null,
  rightThigh: null as number | null,
  shoulders: null as number | null,
  neck: null as number | null,
  notes: '',
})

const showMeasurementForm = ref(false)
const savingMeasurement = ref(false)

/** Plate calculator state. */
const plateTarget = ref<number | null>(null)
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

const weightPoints = computed<ChartPoint[]>(() =>
  measurements.value
    .filter((entry) => entry.weightKg !== null)
    .slice()
    .reverse()
    .map((entry) => ({
      date: `${entry.measuredOn}T12:00:00Z`,
      value: kgToDisplay(entry.weightKg as number, session.weightUnit),
    })),
)

const maxMuscleScore = computed(() => Math.max(1, ...muscles.value.map((muscle) => muscle.score)))

async function loadAll() {
  loading.value = true
  error.value = null

  try {
    const [loadedStats, loadedMeasurements, loadedRecords, loadedMuscles] = await Promise.all([
      api.progress.stats(range.value, 'week'),
      api.measurements.list(),
      api.progress.personalRecords(),
      api.progress.muscles(range.value),
    ])

    stats.value = loadedStats
    measurements.value = loadedMeasurements
    records.value = loadedRecords.filter((record) => record.type !== 'MostRepsAtWeight').slice(0, 8)
    muscles.value = loadedMuscles.slice(0, 8)
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to load your progress.'
  } finally {
    loading.value = false
  }
}

onMounted(loadAll)
watch(range, loadAll)

async function saveMeasurement() {
  savingMeasurement.value = true
  error.value = null

  try {
    // Convert display units back to the canonical kg/cm the API stores.
    await api.measurements.save({
      measuredOn: form.measuredOn,
      weightKg: form.weight === null ? null : displayToKg(form.weight, session.weightUnit),
      bodyFatPercent: form.bodyFatPercent,
      chestCm: form.chest === null ? null : displayToCm(form.chest, session.settings.lengthUnit),
      waistCm: form.waist === null ? null : displayToCm(form.waist, session.settings.lengthUnit),
      hipsCm: form.hips === null ? null : displayToCm(form.hips, session.settings.lengthUnit),
      leftArmCm: form.leftArm === null ? null : displayToCm(form.leftArm, session.settings.lengthUnit),
      rightArmCm: form.rightArm === null ? null : displayToCm(form.rightArm, session.settings.lengthUnit),
      leftThighCm: form.leftThigh === null ? null : displayToCm(form.leftThigh, session.settings.lengthUnit),
      rightThighCm: form.rightThigh === null ? null : displayToCm(form.rightThigh, session.settings.lengthUnit),
      leftCalfCm: null,
      rightCalfCm: null,
      shouldersCm: form.shoulders === null ? null : displayToCm(form.shoulders, session.settings.lengthUnit),
      neckCm: form.neck === null ? null : displayToCm(form.neck, session.settings.lengthUnit),
      notes: form.notes,
    })

    measurements.value = await api.measurements.list()
    showMeasurementForm.value = false
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to save the measurement.'
  } finally {
    savingMeasurement.value = false
  }
}

async function removeMeasurement(id: string) {
  if (!window.confirm('Delete this measurement?')) return

  try {
    await api.measurements.remove(id)
    measurements.value = await api.measurements.list()
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to delete the measurement.'
  }
}

async function calculatePlates() {
  if (plateTarget.value === null || plateTarget.value <= 0) return

  try {
    plateSolution.value = await api.tools.plates(displayToKg(plateTarget.value, session.weightUnit))
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
      <button class="btn btn-primary" @click="showMeasurementForm = !showMeasurementForm">
        <Plus :size="18" /> Log measurement
      </button>
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
      <div class="panel">
        <div class="panel-head">
          <div>
            <span class="eyebrow">BODY WEIGHT</span>
            <h2>Weight trend</h2>
          </div>
        </div>
        <ProgressChart :points="weightPoints" label="Body weight" :unit="weightUnit" color="#a78bfa" />
      </div>

      <div class="panel">
        <div class="panel-head">
          <div>
            <span class="eyebrow">MUSCLE FOCUS</span>
            <h2>Where the work went</h2>
          </div>
        </div>

        <div v-for="muscle in muscles" :key="muscle.muscleName" class="muscle-row">
          <span>
            <strong>{{ muscle.muscleName }}</strong>
            <small>{{ muscle.bodyRegion }} - {{ muscle.sets }} sets</small>
          </span>
          <span class="muscle-bar" role="img" :aria-label="`${muscle.muscleName}: ${muscle.sets} sets`">
            <i :style="{ width: `${(muscle.score / maxMuscleScore) * 100}%` }"></i>
          </span>
        </div>

        <p v-if="muscles.length === 0" class="small-empty">No completed sets in this range.</p>
      </div>
    </section>

    <section v-if="showMeasurementForm" class="panel">
      <div class="panel-head">
        <div>
          <span class="eyebrow">NEW ENTRY</span>
          <h2>Log measurement</h2>
        </div>
      </div>

      <form @submit.prevent="saveMeasurement">
        <div class="field-pair">
          <label class="field-label">Date<input v-model="form.measuredOn" type="date" required /></label>
          <label class="field-label">
            Weight ({{ weightUnit }})
            <input v-model.number="form.weight" type="number" min="0" step="0.1" placeholder="-" />
          </label>
        </div>

        <div class="field-pair">
          <label class="field-label">
            Body fat %
            <input v-model.number="form.bodyFatPercent" type="number" min="0" max="70" step="0.1" placeholder="-" />
          </label>
          <label class="field-label">
            Chest ({{ lengthUnit }})
            <input v-model.number="form.chest" type="number" min="0" step="0.1" placeholder="-" />
          </label>
        </div>

        <div class="field-pair">
          <label class="field-label">
            Waist ({{ lengthUnit }})
            <input v-model.number="form.waist" type="number" min="0" step="0.1" placeholder="-" />
          </label>
          <label class="field-label">
            Hips ({{ lengthUnit }})
            <input v-model.number="form.hips" type="number" min="0" step="0.1" placeholder="-" />
          </label>
        </div>

        <div class="field-pair">
          <label class="field-label">
            Left arm ({{ lengthUnit }})
            <input v-model.number="form.leftArm" type="number" min="0" step="0.1" placeholder="-" />
          </label>
          <label class="field-label">
            Right arm ({{ lengthUnit }})
            <input v-model.number="form.rightArm" type="number" min="0" step="0.1" placeholder="-" />
          </label>
        </div>

        <div class="field-pair">
          <label class="field-label">
            Left thigh ({{ lengthUnit }})
            <input v-model.number="form.leftThigh" type="number" min="0" step="0.1" placeholder="-" />
          </label>
          <label class="field-label">
            Right thigh ({{ lengthUnit }})
            <input v-model.number="form.rightThigh" type="number" min="0" step="0.1" placeholder="-" />
          </label>
        </div>

        <div class="field-pair">
          <label class="field-label">
            Shoulders ({{ lengthUnit }})
            <input v-model.number="form.shoulders" type="number" min="0" step="0.1" placeholder="-" />
          </label>
          <label class="field-label">
            Neck ({{ lengthUnit }})
            <input v-model.number="form.neck" type="number" min="0" step="0.1" placeholder="-" />
          </label>
        </div>

        <label class="field-label">
          Note
          <input v-model="form.notes" maxlength="1000" placeholder="Optional" />
        </label>

        <small class="form-note">Every field except the date is optional.</small>

        <button class="btn btn-primary btn-wide" type="submit" :disabled="savingMeasurement">
          {{ savingMeasurement ? 'Saving…' : 'Save measurement' }}
        </button>
      </form>
    </section>

    <section class="panel">
      <div class="panel-head">
        <div>
          <span class="eyebrow">MEASUREMENTS</span>
          <h2>History</h2>
        </div>
      </div>

      <div v-for="entry in measurements.slice(0, 12)" :key="entry.id" class="activity-row">
        <span class="date-tile">
          <strong>{{ new Date(`${entry.measuredOn}T12:00:00`).getDate() }}</strong>
          <small>{{ formatDate(`${entry.measuredOn}T12:00:00`, { month: 'short' }) }}</small>
        </span>
        <span>
          <strong>
            <template v-if="entry.weightKg !== null">{{ formatWeight(entry.weightKg, session.weightUnit, true) }}</template>
            <template v-else>No weight</template>
          </strong>
          <small>
            <template v-if="entry.bodyFatPercent !== null">{{ entry.bodyFatPercent }}% body fat</template>
            <template v-if="entry.waistCm !== null">
              - waist {{ cmToDisplay(entry.waistCm, session.settings.lengthUnit) }} {{ lengthUnit }}
            </template>
          </small>
        </span>
        <button class="icon-button danger-text" aria-label="Delete measurement" @click="removeMeasurement(entry.id)">
          <Trash2 :size="16" />
        </button>
      </div>

      <p v-if="measurements.length === 0" class="small-empty">No measurements logged yet.</p>
    </section>

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
