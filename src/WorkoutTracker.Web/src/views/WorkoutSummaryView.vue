<script setup lang="ts">
import { computed, ref } from 'vue'
import { Award, Clock3, Dumbbell, Repeat, TrendingUp } from '@lucide/vue'
import { useSessionStore } from '@/stores/session'
import { describeRecord, formatDuration, formatVolume, weightUnitLabel } from '@/lib/format'
import type { WorkoutCompletion } from '@/lib/types'

const session = useSessionStore()

/**
 * The completion payload is handed over through history state when finishing a workout, so
 * the summary needs no extra request. A direct visit simply has nothing to show.
 */
const completion = ref<WorkoutCompletion | null>(readCompletion())

function readCompletion(): WorkoutCompletion | null {
  const raw = window.history.state?.completion
  if (typeof raw !== 'string') return null

  try {
    return JSON.parse(raw) as WorkoutCompletion
  } catch {
    return null
  }
}

const unit = computed(() => weightUnitLabel(session.weightUnit))

const topMuscles = computed(() => completion.value?.muscleBreakdown.slice(0, 5) ?? [])

/** Share of the highest-scoring muscle, used to size the bars. */
const maxMuscleScore = computed(() => Math.max(1, ...topMuscles.value.map((muscle) => muscle.score)))
</script>

<template>
  <div class="page narrow-page">
    <template v-if="completion">
      <header class="page-head">
        <div>
          <span class="eyebrow">WORKOUT COMPLETE</span>
          <h1>{{ completion.title }}</h1>
          <p>Nice work. Here is how the session went.</p>
        </div>
        <RouterLink :to="`/history/${completion.id}`" class="btn btn-quiet">View details</RouterLink>
      </header>

      <section class="stat-grid">
        <article class="stat-card">
          <span class="stat-icon purple"><Clock3 :size="19" /></span>
          <div>
            <strong>{{ formatDuration(completion.durationSeconds) }}</strong>
            <span>Duration</span>
          </div>
        </article>

        <article class="stat-card">
          <span class="stat-icon lime"><TrendingUp :size="19" /></span>
          <div>
            <strong>{{ formatVolume(completion.totalVolume, session.weightUnit) }}</strong>
            <span>Volume - {{ unit }}</span>
          </div>
        </article>

        <article class="stat-card">
          <span class="stat-icon coral"><Dumbbell :size="19" /></span>
          <div>
            <strong>{{ completion.completedSets }}</strong>
            <span>Sets</span>
          </div>
        </article>

        <article class="stat-card">
          <span class="stat-icon amber"><Repeat :size="19" /></span>
          <div>
            <strong>{{ completion.totalReps }}</strong>
            <span>Reps</span>
          </div>
        </article>
      </section>

      <section v-if="completion.newRecords.length > 0" class="panel">
        <div class="panel-head">
          <div>
            <span class="eyebrow">NEW RECORDS</span>
            <h2>{{ completion.newRecords.length }} personal {{ completion.newRecords.length === 1 ? 'record' : 'records' }}</h2>
          </div>
          <span class="stat-icon amber"><Award :size="19" /></span>
        </div>

        <div v-for="record in completion.newRecords" :key="record.id" class="activity-row">
          <span class="stat-icon amber"><Award :size="17" /></span>
          <span>
            <strong>{{ record.exerciseName }}</strong>
            <small>{{ describeRecord(record.type, record.value, record.atWeight, session.weightUnit).label }}</small>
          </span>
          <strong>{{ describeRecord(record.type, record.value, record.atWeight, session.weightUnit).value }}</strong>
        </div>
      </section>

      <section v-if="topMuscles.length > 0" class="panel">
        <div class="panel-head">
          <div>
            <span class="eyebrow">MUSCLE FOCUS</span>
            <h2>What you trained</h2>
          </div>
        </div>

        <div v-for="muscle in topMuscles" :key="muscle.muscleName" class="muscle-row">
          <span>
            <strong>{{ muscle.muscleName }}</strong>
            <small>{{ muscle.bodyRegion }} - {{ muscle.sets }} sets</small>
          </span>
          <span class="muscle-bar" role="img" :aria-label="`${muscle.muscleName}: ${muscle.sets} sets`">
            <i :style="{ width: `${(muscle.score / maxMuscleScore) * 100}%` }"></i>
          </span>
        </div>
      </section>

      <div class="empty-actions">
        <RouterLink to="/" class="btn btn-primary">Back to home</RouterLink>
        <RouterLink to="/history" class="btn btn-quiet">All history</RouterLink>
      </div>
    </template>

    <div v-else class="empty-state">
      <span class="empty-icon"><Award :size="28" /></span>
      <h1>No summary to show</h1>
      <p>Finish a workout to see its summary here.</p>
      <div class="empty-actions">
        <RouterLink to="/history" class="btn btn-primary">View history</RouterLink>
      </div>
    </div>
  </div>
</template>
