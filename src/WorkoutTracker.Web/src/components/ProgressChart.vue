<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref, watch } from 'vue'
import {
  Chart,
  Filler,
  LineController,
  LineElement,
  LinearScale,
  PointElement,
  TimeSeriesScale,
  Tooltip,
  CategoryScale,
} from 'chart.js'
import type { ChartPoint } from '@/lib/types'

/**
 * Small line chart wrapper.
 *
 * Only the pieces actually used are registered, which keeps the Chart.js bundle small on
 * mobile. Every chart is paired with a text table so the data is available without relying
 * on colour or canvas rendering (spec US-098, 3.1 accessibility).
 */
Chart.register(LineController, LineElement, PointElement, LinearScale, CategoryScale, TimeSeriesScale, Tooltip, Filler)

const props = defineProps<{
  points: ChartPoint[]
  label: string
  /** Suffix appended to values in the tooltip and text table, e.g. kg. */
  unit?: string
  color?: string
}>()

const canvas = ref<HTMLCanvasElement | null>(null)
const showTable = ref(false)
let chart: Chart | null = null

function labelFor(iso: string) {
  return new Intl.DateTimeFormat(undefined, { month: 'short', day: 'numeric' }).format(new Date(iso))
}

function render() {
  if (!canvas.value) return

  const context = canvas.value.getContext('2d')
  if (!context) return

  chart?.destroy()

  // Matches the --lime design token used across the stat cards.
  const accent = props.color ?? '#b7f36b'

  chart = new Chart(context, {
    type: 'line',
    data: {
      labels: props.points.map((point) => labelFor(point.date)),
      datasets: [
        {
          label: props.label,
          data: props.points.map((point) => point.value),
          borderColor: accent,
          backgroundColor: `${accent}22`,
          borderWidth: 2.5,
          pointRadius: props.points.length > 30 ? 0 : 3,
          pointHoverRadius: 5,
          tension: 0.3,
          fill: true,
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      // Touch targets are imprecise, so the nearest point wins rather than an exact hit.
      interaction: { mode: 'nearest', intersect: false },
      plugins: {
        legend: { display: false },
        tooltip: {
          callbacks: {
            label: (item) => `${item.formattedValue}${props.unit ? ` ${props.unit}` : ''}`,
          },
        },
      },
      scales: {
        x: {
          grid: { display: false },
          ticks: { maxTicksLimit: 6, color: 'rgba(255,255,255,0.45)' },
        },
        y: {
          grid: { color: 'rgba(255,255,255,0.07)' },
          ticks: { maxTicksLimit: 5, color: 'rgba(255,255,255,0.45)' },
        },
      },
    },
  })
}

onMounted(render)
watch(() => props.points, render, { deep: true })
onBeforeUnmount(() => chart?.destroy())
</script>

<template>
  <div class="chart-block">
    <div class="chart-head">
      <span class="eyebrow">{{ label }}</span>
      <button v-if="points.length > 0" class="link-button" @click="showTable = !showTable">
        {{ showTable ? 'Hide values' : 'Show values' }}
      </button>
    </div>

    <div v-if="points.length === 0" class="small-empty">No data in this range yet.</div>

    <div v-else class="chart-canvas">
      <canvas ref="canvas" :aria-label="`${label} chart`" role="img"></canvas>
    </div>

    <!-- Accessible textual equivalent of the plotted series. -->
    <table v-if="showTable && points.length > 0" class="chart-table">
      <caption class="sr-only">{{ label }} values</caption>
      <thead>
        <tr><th scope="col">Date</th><th scope="col">{{ label }}</th></tr>
      </thead>
      <tbody>
        <tr v-for="point in points" :key="point.date">
          <td>{{ labelFor(point.date) }}</td>
          <td>{{ point.value }}{{ unit ? ` ${unit}` : '' }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
