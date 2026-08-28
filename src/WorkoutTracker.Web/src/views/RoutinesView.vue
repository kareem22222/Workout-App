<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { Copy, FolderPlus, Pencil, Play, Plus, Trash2 } from '@lucide/vue'
import { api } from '@/lib/api'
import { ApiError } from '@/lib/http'
import { useLibraryStore } from '@/stores/library'
import { useWorkoutStore } from '@/stores/workout'

const library = useLibraryStore()
const workouts = useWorkoutStore()
const router = useRouter()

const notice = ref<string | null>(null)
const busyId = ref<string | null>(null)

const totalRoutines = computed(() => library.routines.length)

onMounted(() => library.loadRoutines())

function monogram(name: string) {
  return name
    .split(' ')
    .map((word) => word.charAt(0))
    .join('')
    .slice(0, 2)
    .toUpperCase()
}

async function start(routineId: string) {
  if (workouts.hasActiveWorkout) {
    await router.push('/workout')
    return
  }

  busyId.value = routineId

  try {
    const result = await workouts.start({ routineId })
    if (result.ok || result.resumed) {
      await router.push('/workout')
      return
    }
    notice.value = result.message ?? 'Unable to start this routine.'
  } finally {
    busyId.value = null
  }
}

async function duplicate(routineId: string) {
  notice.value = null

  try {
    await library.duplicateRoutine(routineId)
  } catch (exception) {
    notice.value = exception instanceof ApiError ? exception.message : 'Unable to duplicate the routine.'
  }
}

async function remove(routineId: string, name: string) {
  if (!window.confirm(`Delete "${name}"? Your workout history will stay intact.`)) return
  notice.value = null

  try {
    await library.deleteRoutine(routineId)
  } catch (exception) {
    notice.value = exception instanceof ApiError ? exception.message : 'Unable to delete the routine.'
  }
}

async function createFolder() {
  const name = window.prompt('Folder name')
  if (!name?.trim()) return

  try {
    await api.folders.create(name.trim())
    await library.loadRoutines()
  } catch (exception) {
    notice.value = exception instanceof ApiError ? exception.message : 'Unable to create the folder.'
  }
}

async function removeFolder(folderId: string, name: string) {
  if (!window.confirm(`Delete the folder "${name}"? Routines inside it will simply become ungrouped.`)) return

  try {
    await api.folders.remove(folderId)
    await library.loadRoutines()
  } catch (exception) {
    notice.value = exception instanceof ApiError ? exception.message : 'Unable to delete the folder.'
  }
}
</script>

<template>
  <div class="page">
    <header class="page-head">
      <div>
        <span class="eyebrow">TRAIN YOUR WAY</span>
        <h1>Routines</h1>
        <p>Repeatable plans for focused sessions.</p>
      </div>
      <div class="head-actions">
        <button class="btn btn-quiet" @click="createFolder"><FolderPlus :size="18" /> New folder</button>
        <RouterLink to="/routines/new" class="btn btn-primary"><Plus :size="18" /> New routine</RouterLink>
      </div>
    </header>

    <p v-if="notice" class="form-error" role="alert">{{ notice }}</p>
    <p v-if="library.error" class="form-error" role="alert">{{ library.error }}</p>

    <p v-if="library.loadingRoutines && totalRoutines === 0" class="small-empty">Loading routines…</p>

    <!-- Folder groups first, then anything ungrouped. -->
    <template v-for="group in library.groupedRoutines.groups" :key="group.folder.id">
      <div class="folder-head">
        <span>{{ group.folder.name.toUpperCase() }}</span>
        <small>
          {{ group.routines.length }} {{ group.routines.length === 1 ? 'routine' : 'routines' }}
          <button class="link-button" @click="removeFolder(group.folder.id, group.folder.name)">Delete folder</button>
        </small>
      </div>

      <section class="routine-grid">
        <article v-for="routine in group.routines" :key="routine.id" class="routine-card">
          <header>
            <span class="routine-monogram">{{ monogram(routine.name) }}</span>
            <RouterLink :to="`/routines/${routine.id}/edit`" class="icon-button" :aria-label="`Edit ${routine.name}`">
              <Pencil :size="18" />
            </RouterLink>
          </header>
          <div>
            <h2>{{ routine.name }}</h2>
            <p>{{ routine.exerciseCount }} exercises - {{ routine.setCount }} sets</p>
            <small class="routine-description">{{ routine.description }}</small>
            <ul>
              <li v-for="item in routine.exercises.slice(0, 4)" :key="item.id">{{ item.exerciseName }}</li>
            </ul>
          </div>
          <footer>
            <details class="action-menu">
              <summary class="btn btn-quiet">More</summary>
              <div>
                <button @click="duplicate(routine.id)"><Copy :size="15" /> Duplicate</button>
                <button class="danger-text" @click="remove(routine.id, routine.name)"><Trash2 :size="15" /> Delete</button>
              </div>
            </details>
            <button class="btn btn-primary" :disabled="busyId === routine.id" @click="start(routine.id)">
              <Play :size="16" fill="currentColor" />
              {{ workouts.hasActiveWorkout ? 'Resume' : busyId === routine.id ? 'Starting…' : 'Start' }}
            </button>
          </footer>
        </article>

        <p v-if="group.routines.length === 0" class="small-empty">This folder is empty.</p>
      </section>
    </template>

    <div class="folder-head">
      <span>{{ library.groupedRoutines.groups.length > 0 ? 'UNGROUPED' : 'MY ROUTINES' }}</span>
      <small>{{ totalRoutines }} total</small>
    </div>

    <section class="routine-grid">
      <article v-for="routine in library.groupedRoutines.ungrouped" :key="routine.id" class="routine-card">
        <header>
          <span class="routine-monogram">{{ monogram(routine.name) }}</span>
          <RouterLink :to="`/routines/${routine.id}/edit`" class="icon-button" :aria-label="`Edit ${routine.name}`">
            <Pencil :size="18" />
          </RouterLink>
        </header>
        <div>
          <h2>{{ routine.name }}</h2>
          <p>{{ routine.exerciseCount }} exercises - {{ routine.setCount }} sets</p>
          <small class="routine-description">{{ routine.description }}</small>
          <ul>
            <li v-for="item in routine.exercises.slice(0, 4)" :key="item.id">{{ item.exerciseName }}</li>
          </ul>
        </div>
        <footer>
          <details class="action-menu">
            <summary class="btn btn-quiet">More</summary>
            <div>
              <button @click="duplicate(routine.id)"><Copy :size="15" /> Duplicate</button>
              <button class="danger-text" @click="remove(routine.id, routine.name)"><Trash2 :size="15" /> Delete</button>
            </div>
          </details>
          <button class="btn btn-primary" :disabled="busyId === routine.id" @click="start(routine.id)">
            <Play :size="16" fill="currentColor" />
            {{ workouts.hasActiveWorkout ? 'Resume' : busyId === routine.id ? 'Starting…' : 'Start' }}
          </button>
        </footer>
      </article>

      <RouterLink to="/routines/new" class="routine-card add-routine-card">
        <Plus :size="26" />
        <strong>Create a routine</strong>
        <span>Build your next training session.</span>
      </RouterLink>
    </section>
  </div>
</template>
