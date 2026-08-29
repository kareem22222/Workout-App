<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { api } from '@/lib/api'
import type { MuscleContribution } from '@/lib/types'

const period = ref('week')
const muscles = ref<MuscleContribution[]>([])
const loading = ref(false)
const error = ref<string | null>(null)
const periods = [
  ['workout', 'This workout'], ['week', 'This week'], ['7d', 'Last 7 days'], ['30d', 'Last 30 days'],
]
const max = computed(() => Math.max(1, ...muscles.value.map((item) => item.score)))
async function load() {
  loading.value = true
  error.value = null
  try { muscles.value = (await api.progress.muscles(period.value)).slice(0, 10) }
  catch { error.value = 'Unable to load the muscle breakdown.' }
  finally { loading.value = false }
}
onMounted(load)
watch(period, load)
</script>

<template>
  <section class="panel">
    <div class="panel-head"><div><span class="eyebrow">MUSCLE FOCUS</span><h2>Where the work went</h2></div></div>
    <div class="filter-chips compact-chips">
      <button v-for="option in periods" :key="option[0]" :class="{ active: period === option[0] }" @click="period = option[0]!">{{ option[1] }}</button>
    </div>
    <div v-for="muscle in muscles" :key="muscle.muscleName" class="muscle-row">
      <span><strong>{{ muscle.muscleName }}</strong><small>{{ muscle.bodyRegion }} · {{ muscle.sets }} sets</small></span>
      <span class="muscle-bar" role="img" :aria-label="`${muscle.muscleName}: ${muscle.sets} sets`"><i :style="{ width: `${muscle.score / max * 100}%` }"></i></span>
    </div>
    <p v-if="loading" class="small-empty">Loading muscle breakdown…</p>
    <p v-else-if="error" class="form-error" role="alert">{{ error }}</p>
    <p v-else-if="muscles.length === 0" class="small-empty">No completed sets in this period.</p>
  </section>
</template>
