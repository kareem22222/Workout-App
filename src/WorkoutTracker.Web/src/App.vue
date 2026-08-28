<script setup lang="ts">
import { computed, watchEffect } from 'vue'
import { RouterLink, RouterView, useRoute, useRouter } from 'vue-router'
import { Activity, BarChart3, BookOpen, CalendarDays, Dumbbell, History, LayoutDashboard, Settings } from '@lucide/vue'
import { useAppStore } from '@/stores/app'

const store = useAppStore()
const route = useRoute()
const router = useRouter()
const publicPage = computed(() => route.meta.public)
const nav = [
  { to: '/', label: 'Home', icon: LayoutDashboard },
  { to: '/routines', label: 'Routines', icon: BookOpen },
  { to: '/history', label: 'History', icon: History },
  { to: '/progress', label: 'Progress', icon: BarChart3 },
]

watchEffect(() => {
  document.documentElement.dataset.theme = store.profile.theme
  if (!store.isAuthenticated && !publicPage.value) router.replace('/login')
  if (store.isAuthenticated && route.path === '/login') router.replace('/')
})
</script>

<template>
  <RouterView v-if="publicPage" />
  <div v-else class="app-shell">
    <aside class="sidebar">
      <RouterLink to="/" class="brand"><span class="brand-mark"><Dumbbell :size="20" /></span><span>FORM</span></RouterLink>
      <nav class="side-nav" aria-label="Primary navigation">
        <RouterLink v-for="item in nav" :key="item.to" :to="item.to"><component :is="item.icon" :size="19" /><span>{{ item.label }}</span></RouterLink>
        <RouterLink to="/exercises"><Activity :size="19" /><span>Exercises</span></RouterLink>
        <RouterLink to="/calendar"><CalendarDays :size="19" /><span>Calendar</span></RouterLink>
      </nav>
      <RouterLink to="/settings" class="profile-link"><span class="avatar">{{ store.profile.name[0] }}</span><span><strong>{{ store.profile.name }}</strong><small>Settings</small></span><Settings :size="18" /></RouterLink>
    </aside>

    <main class="main-content"><RouterView /></main>

    <RouterLink v-if="store.activeWorkout && route.path !== '/workout'" to="/workout" class="active-pill"><span class="pulse"></span><span><strong>{{ store.activeWorkout.title }}</strong><small>{{ store.completedSets }}/{{ store.totalSets }} sets</small></span><span>Resume</span></RouterLink>

    <nav class="bottom-nav" aria-label="Mobile navigation">
      <RouterLink v-for="item in nav" :key="item.to" :to="item.to"><component :is="item.icon" :size="21" /><span>{{ item.label }}</span></RouterLink>
    </nav>
  </div>
</template>
