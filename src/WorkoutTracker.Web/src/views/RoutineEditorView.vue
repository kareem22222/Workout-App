<script setup lang="ts">
import { computed, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ArrowDown, ArrowLeft, ArrowUp, Plus, Search, Trash2 } from '@lucide/vue'
import { useAppStore, type RoutineDraft } from '@/stores/app'

const store = useAppStore()
const route = useRoute()
const router = useRouter()
const existing = store.routineList.find(r => r.id === route.params.id)
const query = ref('')
const error = ref('')
const draft = reactive<RoutineDraft>({
  id: existing?.id,
  name: existing?.name ?? '',
  description: existing?.description ?? '',
  exercises: existing?.exercises.map(item => ({ exerciseId: item.exerciseId, sets: item.sets.length, reps: item.sets[0]?.reps ?? 8, weight: item.sets[0]?.weight ?? 0, rest: item.rest })) ?? [],
})
const available = computed(() => store.exerciseList.filter(item => !draft.exercises.some(e => e.exerciseId === item.id) && item.name.toLowerCase().includes(query.value.toLowerCase())))
const nameFor = (id: string) => store.exerciseList.find(item => item.id === id)?.name ?? 'Exercise'
function add(id: string) { draft.exercises.push({ exerciseId: id, sets: 3, reps: 8, weight: 0, rest: store.profile.defaultRest }) }
function move(index: number, direction: -1 | 1) { const next = index + direction; if (next >= 0 && next < draft.exercises.length) [draft.exercises[index], draft.exercises[next]] = [draft.exercises[next]!, draft.exercises[index]!] }
function save() {
  error.value = !draft.name.trim() ? 'Give your routine a name.' : !draft.exercises.length ? 'Add at least one exercise.' : draft.exercises.some(item=>!Number.isFinite(item.sets)||item.sets<1||item.sets>20||!Number.isFinite(item.reps)||item.reps<1||item.reps>100||item.weight<0) ? 'Check the set, rep and weight targets.' : ''
  if (error.value) return
  store.saveRoutine(draft)
  router.push('/routines')
}
</script>

<template>
  <div class="page editor-page">
    <header class="editor-head"><button class="icon-button" aria-label="Back to routines" @click="router.push('/routines')"><ArrowLeft /></button><div><span class="eyebrow">{{ existing ? 'EDIT ROUTINE' : 'NEW ROUTINE' }}</span><h1>{{ existing ? existing.name : 'Build a routine' }}</h1></div><button class="btn btn-primary" @click="save">Save routine</button></header>
    <p v-if="error" class="form-error" role="alert">{{ error }}</p>
    <div class="editor-layout">
      <section class="editor-main">
        <label class="field-label">Routine name<input v-model="draft.name" maxlength="60" placeholder="e.g. Push Day" /></label>
        <label class="field-label">Description<textarea v-model="draft.description" rows="2" maxlength="180" placeholder="What is this session for?"></textarea></label>
        <div class="folder-head"><span>EXERCISES</span><small>{{ draft.exercises.length }} selected</small></div>
        <div v-if="!draft.exercises.length" class="inline-empty"><strong>No exercises yet</strong><span>Choose movements from the library.</span></div>
        <article v-for="(item,index) in draft.exercises" :key="item.exerciseId" class="routine-edit-row">
          <div class="reorder-controls"><button :disabled="index===0" aria-label="Move up" @click="move(index,-1)"><ArrowUp :size="15"/></button><button :disabled="index===draft.exercises.length-1" aria-label="Move down" @click="move(index,1)"><ArrowDown :size="15"/></button></div>
          <div class="routine-edit-name"><span class="exercise-glyph">{{ nameFor(item.exerciseId)[0] }}</span><strong>{{ nameFor(item.exerciseId) }}</strong></div>
          <label>Sets<input v-model.number="item.sets" type="number" min="1" max="20" /></label>
          <label>Reps<input v-model.number="item.reps" type="number" min="1" max="100" /></label>
          <label>{{ store.profile.units }}<input v-model.number="item.weight" type="number" min="0" step="0.5" /></label>
          <label>Rest<select v-model.number="item.rest"><option :value="60">1:00</option><option :value="90">1:30</option><option :value="120">2:00</option><option :value="180">3:00</option></select></label>
          <button class="icon-button danger" aria-label="Remove exercise" @click="draft.exercises.splice(index,1)"><Trash2 :size="17"/></button>
        </article>
      </section>
      <aside class="library-panel">
        <div><span class="eyebrow">EXERCISE LIBRARY</span><h2>Add movements</h2></div>
        <div class="search-field compact"><Search :size="17"/><input v-model="query" placeholder="Search" /></div>
        <button v-for="item in available" :key="item.id" class="exercise-option" @click="add(item.id)"><span class="exercise-glyph">{{ item.name[0] }}</span><span><strong>{{ item.name }}</strong><small>{{ item.muscle }} - {{ item.equipment }}</small></span><Plus :size="18"/></button>
        <p v-if="!available.length" class="small-empty">No matching exercises.</p>
      </aside>
    </div>
  </div>
</template>
