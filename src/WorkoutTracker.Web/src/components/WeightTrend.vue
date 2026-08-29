<script setup lang="ts">
import { computed } from 'vue'
import ProgressChart from '@/components/ProgressChart.vue'
import { formatWeight, kgToDisplay, weightUnitLabel } from '@/lib/format'
import { useSessionStore } from '@/stores/session'
import type { BodyMeasurement, ChartPoint } from '@/lib/types'

const props = defineProps<{ measurements: BodyMeasurement[] }>()
const session = useSessionStore()
const entries = computed(() => props.measurements.filter((item) => item.weightKg !== null).slice().sort((a, b) => a.measuredOn.localeCompare(b.measuredOn)))
const current = computed(() => entries.value[entries.value.length - 1] ?? null)
const starting = computed(() => entries.value[0] ?? null)
const change = (days?: number) => {
  if (current.value?.weightKg === null || current.value?.weightKg === undefined || entries.value.length < 2) return null
  const baseline = days
    ? (() => { const eligible = entries.value.filter((item) => new Date(item.measuredOn).getTime() <= new Date(current.value!.measuredOn).getTime() - days * 86400000); return eligible[eligible.length - 1] })()
    : starting.value
  return baseline?.weightKg === null || baseline?.weightKg === undefined ? null : current.value.weightKg - baseline.weightKg
}
const points = computed<ChartPoint[]>(() => entries.value.map((entry) => ({ date: `${entry.measuredOn}T12:00:00Z`, value: kgToDisplay(entry.weightKg!, session.weightUnit) })))
const signed = (value: number | null) => value === null ? 'Not enough data' : `${value > 0 ? '+' : ''}${formatWeight(value, session.weightUnit, true)}`
</script>

<template>
  <section class="panel">
    <div class="panel-head"><div><span class="eyebrow">BODY WEIGHT</span><h2>Weight trend</h2></div></div>
    <div class="weight-summary">
      <span><small>Current</small><strong>{{ current?.weightKg == null ? '—' : formatWeight(current.weightKg, session.weightUnit, true) }}</strong></span>
      <span><small>Starting</small><strong>{{ starting?.weightKg == null ? '—' : formatWeight(starting.weightKg, session.weightUnit, true) }}</strong></span>
      <span><small>Total change</small><strong>{{ signed(change()) }}</strong></span>
      <span><small>7-day trend</small><strong>{{ signed(change(7)) }}</strong></span>
      <span><small>30-day change</small><strong>{{ signed(change(30)) }}</strong></span>
    </div>
    <ProgressChart v-if="points.length" :points="points" label="Body weight" :unit="weightUnitLabel(session.weightUnit)" color="#a78bfa" />
    <p v-else class="small-empty">Log a body weight to start your trend.</p>
  </section>
</template>
