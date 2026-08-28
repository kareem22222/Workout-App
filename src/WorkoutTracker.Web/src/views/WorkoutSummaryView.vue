<script setup lang="ts">
import { Award, CheckCircle2, Clock3, Dumbbell, RotateCcw } from '@lucide/vue'
import { useAppStore } from '@/stores/app'
const store = useAppStore()
</script>

<template>
  <div v-if="store.lastSummary" class="summary-page">
    <div class="summary-check"><CheckCircle2 :size="32"/></div><span class="eyebrow">WORKOUT COMPLETE</span><h1>{{ store.lastSummary.title }}</h1><p>You showed up and did the work.</p>
    <section class="summary-stats"><article><Clock3/><strong>{{ store.lastSummary.duration }}</strong><span>minutes</span></article><article><Dumbbell/><strong>{{ store.lastSummary.sets }}</strong><span>sets</span></article><article><RotateCcw/><strong>{{ store.lastSummary.reps }}</strong><span>reps</span></article><article><strong>{{ Math.round(store.lastSummary.volume).toLocaleString() }}</strong><span>{{ store.profile.units }} volume</span></article></section>
    <section v-if="store.lastSummary.prs.length" class="pr-celebration"><Award :size="24"/><div><span class="eyebrow">NEW PERSONAL RECORD</span><strong v-for="pr in store.lastSummary.prs" :key="pr">{{ pr }}</strong></div></section>
    <div class="summary-actions"><RouterLink :to="`/history/${store.lastSummary.id}`" class="btn btn-quiet">View workout</RouterLink><RouterLink to="/" class="btn btn-primary">Back to home</RouterLink></div>
  </div>
  <div v-else class="empty-state"><h1>No recent summary</h1><p>Finish a workout to see your results.</p><RouterLink to="/routines" class="btn btn-primary">Start training</RouterLink></div>
</template>
