<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { CalendarDays, ChevronLeft, ChevronRight, Trash2 } from '@lucide/vue'
import { api } from '@/lib/api'
import { ApiError } from '@/lib/http'
import { useLibraryStore } from '@/stores/library'
import { useSessionStore } from '@/stores/session'
import { useWorkoutStore } from '@/stores/workout'
import { useRouter } from 'vue-router'
import { dayIndex, dayNames, formatVolume, weightUnitLabel } from '@/lib/format'
import type { WorkoutSummaryRow } from '@/lib/types'

const session = useSessionStore()
const library = useLibraryStore()
const workouts = useWorkoutStore()
const router = useRouter()

const cursor = ref(new Date())
const days = ref<Record<string, WorkoutSummaryRow[]>>({})
const selectedDate = ref<string | null>(null)
const loading = ref(false)
const error = ref<string | null>(null)

const unit = computed(() => weightUnitLabel(session.weightUnit))

const monthLabel = computed(() =>
  new Intl.DateTimeFormat(undefined, { month: 'long', year: 'numeric' }).format(cursor.value),
)

/** Weekday headers starting Monday, matching the grid layout. */
const weekdayLabels = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']

/**
 * Builds the month grid, padded with leading blanks so the first day lands under the
 * correct weekday.
 */
const grid = computed(() => {
  const year = cursor.value.getFullYear()
  const month = cursor.value.getMonth()

  const first = new Date(year, month, 1)
  const daysInMonth = new Date(year, month + 1, 0).getDate()

  // Convert Sunday-first (0) to Monday-first (0).
  const leading = (first.getDay() + 6) % 7

  const cells: Array<{ key: string; day: number | null; iso: string | null }> = []

  for (let index = 0; index < leading; index++) {
    cells.push({ key: `pad-${index}`, day: null, iso: null })
  }

  for (let day = 1; day <= daysInMonth; day++) {
    const iso = `${year}-${String(month + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`
    cells.push({ key: iso, day, iso })
  }

  return cells
})

const selectedWorkouts = computed(() => (selectedDate.value ? days.value[selectedDate.value] ?? [] : []))
const selectedSchedule = computed(() => {
  if (!selectedDate.value) return null
  const jsDay = new Date(`${selectedDate.value}T12:00:00`).getDay()
  return scheduleFor((jsDay + 6) % 7)
})

const trainedDayCount = computed(() => Object.keys(days.value).length)

async function load() {
  loading.value = true
  error.value = null

  try {
    days.value = await api.workouts.calendar(cursor.value.getFullYear(), cursor.value.getMonth() + 1)
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to load the calendar.'
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  await Promise.all([load(), library.loadRoutines(), library.loadSchedules()])
})

watch(cursor, load)

function shiftMonth(delta: number) {
  const next = new Date(cursor.value)
  next.setDate(1)
  next.setMonth(next.getMonth() + delta)
  cursor.value = next
  selectedDate.value = null
}

function select(iso: string | null) {
  if (!iso) return
  selectedDate.value = selectedDate.value === iso ? null : iso
}

/** Assigns a routine to a weekday. A schedule never creates a workout (spec US-140). */
async function assign(dayOfWeekIndex: number, routineId: string) {
  try {
    if (routineId) await api.schedule.save(routineId, dayOfWeekIndex)
    await library.loadSchedules()
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to save the schedule.'
  }
}

async function clearSchedule(id: string) {
  try {
    await api.schedule.remove(id)
    await library.loadSchedules()
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to clear the schedule.'
  }
}

function scheduleFor(dayOfWeekIndex: number) {
  return library.schedules.find((schedule) => dayIndex(schedule.dayOfWeek) === dayOfWeekIndex) ?? null
}

async function startScheduled() {
  if (!selectedSchedule.value) return
  const result = await workouts.start({ routineId: selectedSchedule.value.routineId })
  if (result.ok || result.resumed) await router.push('/workout')
  else error.value = result.message ?? 'Unable to start the scheduled workout.'
}
</script>

<template>
  <div class="page narrow-page">
    <header class="page-head">
      <div>
        <span class="eyebrow">YOUR MONTH</span>
        <h1>Calendar</h1>
        <p>{{ trainedDayCount }} training {{ trainedDayCount === 1 ? 'day' : 'days' }} this month.</p>
      </div>
    </header>

    <p v-if="error" class="form-error" role="alert">{{ error }}</p>

    <section class="panel">
      <div class="calendar-head">
        <button class="icon-button" aria-label="Previous month" @click="shiftMonth(-1)"><ChevronLeft /></button>
        <strong>{{ monthLabel }}</strong>
        <button class="icon-button" aria-label="Next month" @click="shiftMonth(1)"><ChevronRight /></button>
      </div>

      <div class="calendar-weekdays">
        <span v-for="label in weekdayLabels" :key="label">{{ label }}</span>
      </div>

      <div class="calendar-grid">
        <template v-for="cell in grid" :key="cell.key">
          <span v-if="cell.day === null" class="calendar-cell empty"></span>
          <button
            v-else
            class="calendar-cell"
            :class="{ trained: (days[cell.iso!]?.length ?? 0) > 0, selected: selectedDate === cell.iso }"
            :aria-label="`${cell.day}: ${(days[cell.iso!]?.length ?? 0)} workouts`"
            @click="select(cell.iso)"
          >
            <span>{{ cell.day }}</span>
            <i v-if="(days[cell.iso!]?.length ?? 0) > 0" class="calendar-dot"></i>
          </button>
        </template>
      </div>

      <p v-if="loading" class="small-empty">Loading…</p>
    </section>

    <section v-if="selectedDate" class="panel">
      <div class="panel-head">
        <div>
          <span class="eyebrow">{{ selectedDate }}</span>
          <h2>{{ selectedWorkouts.length }} {{ selectedWorkouts.length === 1 ? 'workout' : 'workouts' }}</h2>
        </div>
      </div>

      <RouterLink
        v-for="workout in selectedWorkouts"
        :key="workout.id"
        :to="`/history/${workout.id}`"
        class="activity-row"
      >
        <span class="stat-icon purple"><CalendarDays :size="17" /></span>
        <span>
          <strong>{{ workout.title }}</strong>
          <small>
            {{ Math.round(workout.durationSeconds / 60) }} min ·
            {{ formatVolume(workout.totalVolume, session.weightUnit) }} {{ unit }}
          </small>
        </span>
        <ChevronRight :size="18" />
      </RouterLink>

      <p v-if="selectedWorkouts.length === 0" class="small-empty">Nothing logged on this day.</p>
      <div v-if="selectedSchedule" class="scheduled-card">
        <span><small>SCHEDULED</small><strong>{{ selectedSchedule.routineName }}</strong></span>
        <button class="btn btn-primary" @click="startScheduled">Start routine</button>
      </div>
      <p v-else-if="selectedWorkouts.length === 0" class="form-note">Rest day</p>
    </section>

    <section class="panel">
      <div class="panel-head">
        <div>
          <span class="eyebrow">WEEKLY SCHEDULE</span>
          <h2>Plan your week</h2>
        </div>
      </div>

      <p class="form-note">Assigning a routine only surfaces it on your dashboard. It never starts a workout for you.</p>

      <div v-for="(name, index) in dayNames" :key="name" class="schedule-row">
        <strong>{{ name }}</strong>

        <select
          :value="scheduleFor(index)?.routineId ?? ''"
          :aria-label="`Routine for ${name}`"
          @change="assign(index, ($event.target as HTMLSelectElement).value)"
        >
          <option value="">Rest day</option>
          <option v-for="routine in library.routines" :key="routine.id" :value="routine.id">{{ routine.name }}</option>
        </select>

        <button
          v-if="scheduleFor(index)"
          class="icon-button danger-text"
          :aria-label="`Clear ${name}`"
          @click="clearSchedule(scheduleFor(index)!.id)"
        >
          <Trash2 :size="16" />
        </button>
      </div>
    </section>
  </div>
</template>
