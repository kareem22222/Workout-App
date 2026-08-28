<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ArrowLeft, Clock3, Pencil, Save, Trash2 } from '@lucide/vue'
import { useAppStore, workoutDuration, workoutVolume } from '@/stores/app'
const store=useAppStore();const route=useRoute();const router=useRouter();const workout=store.history.find(w=>w.id===route.params.id);const editing=ref(false);const title=ref(workout?.title??'');const note=ref(workout?.note??'')
function save(){if(!workout)return;store.updateWorkout(workout.id,title.value,note.value);editing.value=false}
function remove(){if(workout&&confirm('Delete this workout permanently?')){store.deleteWorkout(workout.id);router.push('/history')}}
</script>
<template>
  <div v-if="workout" class="page detail-page"><header class="editor-head"><button class="icon-button" aria-label="Back" @click="router.push('/history')"><ArrowLeft/></button><div><span class="eyebrow">{{ new Date(workout.startedAt).toLocaleDateString('en',{weekday:'long',month:'long',day:'numeric'}) }}</span><h1 v-if="!editing">{{ workout.title }}</h1><input v-else v-model="title" class="title-input" maxlength="60"/></div><button v-if="!editing" class="btn btn-quiet" @click="editing=true"><Pencil :size="16"/> Edit</button><button v-else class="btn btn-primary" @click="save"><Save :size="16"/> Save</button></header>
    <section class="detail-stats"><span><Clock3 :size="17"/><strong>{{ workoutDuration(workout) }} min</strong></span><span><strong>{{ workout.exercises.flatMap(e=>e.sets).filter(s=>s.done).length }}</strong> sets</span><span><strong>{{ Math.round(workoutVolume(workout)).toLocaleString() }}</strong> {{ store.profile.units }}</span></section>
    <label v-if="editing" class="field-label">Workout note<textarea v-model="note" rows="3" placeholder="Add a note"></textarea></label><p v-else-if="workout.note" class="exercise-note">{{ workout.note }}</p>
    <section v-for="item in workout.exercises" :key="item.id" class="history-exercise"><header><div><span class="eyebrow">{{ store.exerciseList.find(e=>e.id===item.exerciseId)?.muscle }}</span><h2>{{ item.name }}</h2></div><span>{{ item.sets.filter(s=>s.done).length }} sets</span></header><div class="history-set-head"><span>SET</span><span>TYPE</span><span>{{ store.profile.units.toUpperCase() }}</span><span>REPS</span><span>RPE</span></div><div v-for="(set,index) in item.sets.filter(s=>s.done)" :key="set.id" class="history-set-row"><span>{{ index+1 }}</span><span>{{ set.type }}</span><strong>{{ set.weight || 'BW' }}</strong><strong>{{ set.reps }}</strong><span>{{ set.rpe ?? '-' }}</span></div></section>
    <button class="logout-button" @click="remove"><Trash2 :size="17"/> Delete workout</button>
  </div>
  <div v-else class="empty-state"><h1>Workout not found</h1><RouterLink to="/history" class="btn btn-primary">Back to history</RouterLink></div>
</template>
