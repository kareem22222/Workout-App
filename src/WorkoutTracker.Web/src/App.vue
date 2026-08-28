<script setup lang="ts">
import { computed, onMounted, onUnmounted, watch } from 'vue'
import { RouterLink, RouterView, useRoute } from 'vue-router'
import {
  Activity,
  BarChart3,
  BookOpen,
  CalendarDays,
  CloudOff,
  Dumbbell,
  History,
  LayoutDashboard,
  RefreshCw,
  Settings,
  ShieldCheck,
} from '@lucide/vue'
import { useSessionStore } from '@/stores/session'
import { useWorkoutStore } from '@/stores/workout'
import { formatDuration } from '@/lib/format'

const session = useSessionStore()
const workouts = useWorkoutStore()
const route = useRoute()

const isPublicPage = computed(() => route.meta.public === true)

const nav = [
  { to: '/', label: 'Home', icon: LayoutDashboard },
  { to: '/routines', label: 'Routines', icon: BookOpen },
  { to: '/history', label: 'History', icon: History },
  { to: '/progress', label: 'Progress', icon: BarChart3 },
]

/** Apply the resolved theme to the document so CSS variables switch. */
watch(
  () => session.resolvedTheme,
  (theme) => {
    document.documentElement.dataset.theme = theme
  },
  { immediate: true },
)

/** Load the active workout once signed in so the resume pill is accurate everywhere. */
watch(
  () => session.isAuthenticated,
  (authenticated) => {
    if (authenticated) void workouts.loadActive()
  },
  { immediate: true },
)

/** Replay any queued offline mutations as soon as the connection returns (spec US-250). */
function handleOnline() {
  void workouts.flushOutbox()
}

onMounted(() => window.addEventListener('online', handleOnline))
onUnmounted(() => window.removeEventListener('online', handleOnline))

const showSyncBanner = computed(
  () => workouts.syncState === 'offline' || workouts.syncState === 'error' || workouts.syncState === 'conflict',
)
</script>

<template>
  <RouterView v-if="isPublicPage" />

  <div v-else class="app-shell">
    <aside class="sidebar">
      <RouterLink to="/" class="brand">
        <span class="brand-mark"><Dumbbell :size="20" /></span>
        <span>FORM</span>
      </RouterLink>

      <nav class="side-nav" aria-label="Primary navigation">
        <RouterLink v-for="item in nav" :key="item.to" :to="item.to">
          <component :is="item.icon" :size="19" />
          <span>{{ item.label }}</span>
        </RouterLink>
        <RouterLink to="/exercises"><Activity :size="19" /><span>Exercises</span></RouterLink>
        <RouterLink to="/calendar"><CalendarDays :size="19" /><span>Calendar</span></RouterLink>
        <RouterLink v-if="session.isAdmin" to="/admin"><ShieldCheck :size="19" /><span>Admin</span></RouterLink>
      </nav>

      <RouterLink to="/settings" class="profile-link">
        <span class="avatar">{{ (session.profile?.displayName || '?').charAt(0).toUpperCase() }}</span>
        <span>
          <strong>{{ session.profile?.displayName || 'Athlete' }}</strong>
          <small>Settings</small>
        </span>
        <Settings :size="18" />
      </RouterLink>
    </aside>

    <main class="main-content">
      <!-- Offline and sync state is always visible rather than failing silently (spec US-250). -->
      <div v-if="showSyncBanner" class="sync-banner" :class="workouts.syncState" role="status">
        <CloudOff v-if="workouts.syncState === 'offline'" :size="16" />
        <RefreshCw v-else :size="16" />
        <span>{{ workouts.syncMessage }}</span>
        <small v-if="workouts.pendingCount > 0">{{ workouts.pendingCount }} pending</small>
      </div>

      <RouterView />
    </main>

    <RouterLink
      v-if="workouts.hasActiveWorkout && route.path !== '/workout'"
      to="/workout"
      class="active-pill"
    >
      <span class="pulse"></span>
      <span>
        <strong>{{ workouts.workout?.title }}</strong>
        <small>{{ formatDuration(workouts.elapsedSeconds) }} · {{ workouts.completedSets }}/{{ workouts.totalSets }} sets</small>
      </span>
      <span>Resume</span>
    </RouterLink>

    <nav class="bottom-nav" aria-label="Mobile navigation">
      <RouterLink v-for="item in nav" :key="item.to" :to="item.to">
        <component :is="item.icon" :size="21" />
        <span>{{ item.label }}</span>
      </RouterLink>
    </nav>
  </div>
</template>
