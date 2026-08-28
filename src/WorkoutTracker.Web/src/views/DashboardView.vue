<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import {
  ArrowRight,
  Award,
  CalendarDays,
  ChevronRight,
  Clock3,
  Flame,
  Play,
  Plus,
  TrendingUp,
} from '@lucide/vue'
import { api } from '@/lib/api'
import { ApiError } from '@/lib/http'
import { useSessionStore } from '@/stores/session'
import { useWorkoutStore } from '@/stores/workout'
import {
  dayNames,
  describeRecord,
  formatDate,
  formatMinutes,
  formatVolume,
  formatWeight,
  weightUnitLabel,
} from '@/lib/format'
import type { DashboardSummary } from '@/lib/types'

const session = useSessionStore()
const workouts = useWorkoutStore()
const router = useRouter()

const summary = ref<DashboardSummary | null>(null)
const loading = ref(true)
const error = ref<string | null>(null)
const starting = ref(false)

const unit = computed(() => weightUnitLabel(session.weightUnit))

const greeting = computed(() => {
  const hour = new Date().getHours()
  if (hour < 12) return 'Good morning'
  if (hour < 18) return 'Good afternoon'
  return 'Good evening'
})

const today = computed(() =>
  new Intl.DateTimeFormat(undefined, { weekday: 'long', month: 'long', day: 'numeric' }).format(new Date()),
)

/** Routines still needed to hit the weekly goal. */
const remaining = computed(() => {
  if (!summary.value) return 0
  return Math.max(0, summary.value.weeklyWorkoutGoal - summary.value.workoutsThisWeek)
})

const topRecord = computed(() => summary.value?.recentRecords[0] ?? null)

const topRecordCopy = computed(() => {
  if (!topRecord.value) return null
  return describeRecord(topRecord.value.type, topRecord.value.value, topRecord.value.atWeight, session.weightUnit)
})

const nextScheduledDayName = computed(() => {
  const day = summary.value?.nextScheduledDay
  if (day === null || day === undefined) return null
  return typeof day === 'number' ? dayNames[day] : day
})

async function load() {
  loading.value = true
  error.value = null

  try {
    summary.value = await api.dashboard.summary()
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to load your dashboard.'
  } finally {
    loading.value = false
  }
}

onMounted(load)

/** Resumes an active workout, otherwise starts the next scheduled routine or an empty session. */
async function startTraining() {
  if (workouts.hasActiveWorkout) {
    await router.push('/workout')
    return
  }

  starting.value = true

  try {
    const routineId = summary.value?.nextScheduledRoutine?.id ?? null
    const result = await workouts.start({ routineId })

    if (result.ok || result.resumed) {
      await router.push('/workout')
      return
    }

    error.value = result.message ?? 'Unable to start the workout.'
  } finally {
    starting.value = false
  }
}

async function startEmpty() {
  if (workouts.hasActiveWorkout) {
    await router.push('/workout')
    return
  }

  const result = await workouts.start({ title: 'Quick workout' })
  if (result.ok || result.resumed) await router.push('/workout')
}
</script>

<template>
  <div class="page dashboard-page">
    <header class="page-head">
      <div>
        <span class="eyebrow">{{ today }}</span>
        <h1>{{ greeting }}, {{ summary?.displayName || session.profile?.displayName || 'Athlete' }}.</h1>
        <p v-if="loading">Loading your training summary…</p>
        <p v-else-if="summary && remaining === 0">Weekly goal complete. Strong work.</p>
        <p v-else-if="summary">{{ remaining }} {{ remaining === 1 ? 'session' : 'sessions' }} left to hit your weekly goal.</p>
      </div>
      <RouterLink to="/settings" class="avatar mobile-avatar">
        {{ (session.profile?.displayName || '?').charAt(0).toUpperCase() }}
      </RouterLink>
    </header>

    <p v-if="error" class="form-error" role="alert">{{ error }}</p>

    <section class="hero-card">
      <div class="hero-copy">
        <span class="hero-kicker">
          <Flame :size="16" />
          {{ workouts.hasActiveWorkout ? 'WORKOUT IN PROGRESS' : 'READY TO TRAIN' }}
        </span>
        <h2>
          {{ workouts.workout?.title || summary?.nextScheduledRoutine?.name || 'Quick Workout' }}
        </h2>
        <p v-if="workouts.hasActiveWorkout">
          {{ workouts.completedSets }} of {{ workouts.totalSets }} sets complete
        </p>
        <p v-else-if="summary?.nextScheduledRoutine">
          Scheduled for {{ nextScheduledDayName }} ·
          {{ summary.nextScheduledRoutine.exerciseCount }} exercises,
          {{ summary.nextScheduledRoutine.setCount }} sets
        </p>
        <p v-else>Start from a routine or log an empty session.</p>

        <div class="hero-meta">
          <span>
            <Clock3 :size="16" />
            {{ workouts.hasActiveWorkout ? 'Timer running' : 'Your pace' }}
          </span>
          <span>
            {{ workouts.workout?.exercises.length || summary?.nextScheduledRoutine?.exerciseCount || 0 }} exercises
          </span>
        </div>
      </div>

      <button class="start-button" :disabled="starting" @click="startTraining">
        <span>
          <Play :size="19" fill="currentColor" />
          {{ workouts.hasActiveWorkout ? 'Resume workout' : starting ? 'Starting…' : 'Start workout' }}
        </span>
        <ArrowRight :size="20" />
      </button>
    </section>

    <div class="section-title">
      <div>
        <span class="eyebrow">THIS WEEK</span>
        <h2>Your momentum</h2>
      </div>
      <RouterLink to="/progress">View progress <ChevronRight :size="16" /></RouterLink>
    </div>

    <section class="stat-grid">
      <article class="stat-card">
        <span class="stat-icon coral"><CalendarDays :size="19" /></span>
        <div>
          <strong>{{ summary?.workoutsThisWeek ?? 0 }}</strong>
          <span>Workouts</span>
        </div>
        <small>Goal {{ summary?.weeklyWorkoutGoal ?? 0 }}</small>
      </article>

      <article class="stat-card">
        <span class="stat-icon lime"><TrendingUp :size="19" /></span>
        <div>
          <strong>{{ formatVolume(summary?.volumeThisWeek ?? 0, session.weightUnit) }}</strong>
          <span>Volume - {{ unit }}</span>
        </div>
        <small>Completed work</small>
      </article>

      <article class="stat-card">
        <span class="stat-icon purple"><Clock3 :size="19" /></span>
        <div>
          <strong>{{ formatMinutes((summary?.trainingMinutesThisWeek ?? 0) * 60) }}</strong>
          <span>Training time</span>
        </div>
        <small>{{ summary?.currentStreakWeeks ?? 0 }} week streak</small>
      </article>
    </section>

    <section class="dashboard-grid">
      <div class="panel recent-panel">
        <div class="panel-head">
          <div>
            <span class="eyebrow">RECENT ACTIVITY</span>
            <h2>Last workouts</h2>
          </div>
          <RouterLink to="/history">All history</RouterLink>
        </div>

        <RouterLink
          v-for="workout in summary?.recentWorkouts ?? []"
          :key="workout.id"
          :to="`/history/${workout.id}`"
          class="activity-row"
        >
          <span class="date-tile">
            <strong>{{ new Date(workout.startedAt).getDate() }}</strong>
            <small>{{ formatDate(workout.startedAt, { month: 'short' }) }}</small>
          </span>
          <span>
            <strong>{{ workout.title }}</strong>
            <small>
              {{ Math.round(workout.durationSeconds / 60) }} min ·
              {{ formatVolume(workout.totalVolume, session.weightUnit) }} {{ unit }}
            </small>
          </span>
          <ChevronRight :size="18" />
        </RouterLink>

        <p v-if="!loading && (summary?.recentWorkouts.length ?? 0) === 0" class="small-empty">
          No completed workouts yet. Your first session will appear here.
        </p>
      </div>

      <div class="panel pr-panel">
        <div class="panel-head">
          <div>
            <span class="eyebrow">LATEST RECORD</span>
            <h2>{{ topRecord?.exerciseName || 'No record yet' }}</h2>
          </div>
          <span class="stat-icon amber"><Award :size="19" /></span>
        </div>

        <div class="pr-value">
          <strong>{{ topRecordCopy?.value ?? '--' }}</strong>
        </div>
        <p>{{ topRecordCopy?.label ?? 'Complete a workout to detect records.' }}</p>

        <div v-if="summary?.latestWeightKg !== null && summary?.latestWeightKg !== undefined" class="hero-meta">
          <span>Weight {{ formatWeight(summary.latestWeightKg, session.weightUnit, true) }}</span>
          <span v-if="summary.weightChange30DaysKg !== null">
            {{ summary.weightChange30DaysKg > 0 ? '+' : '' }}{{ formatWeight(summary.weightChange30DaysKg, session.weightUnit) }} in 30d
          </span>
        </div>
      </div>
    </section>

    <div class="quick-actions">
      <RouterLink to="/routines/new"><Plus :size="17" /> New routine</RouterLink>
      <button @click="startEmpty"><Plus :size="17" /> Empty workout</button>
      <RouterLink to="/progress"><Plus :size="17" /> Log measurement</RouterLink>
    </div>
  </div>
</template>
