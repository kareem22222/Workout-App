import { createRouter, createWebHistory } from 'vue-router'
import { useSessionStore } from '@/stores/session'

/**
 * Routes are lazily loaded so the initial bundle stays small on mobile connections.
 * The login route is the only public one.
 */
const router = createRouter({
  history: createWebHistory(),
  scrollBehavior: (to, _from, savedPosition) => savedPosition ?? { top: 0 },
  routes: [
    { path: '/login', name: 'login', component: () => import('@/views/LoginView.vue'), meta: { public: true } },
    { path: '/', name: 'dashboard', component: () => import('@/views/DashboardView.vue') },
    { path: '/routines', name: 'routines', component: () => import('@/views/RoutinesView.vue') },
    { path: '/routines/new', name: 'routine-new', component: () => import('@/views/RoutineEditorView.vue') },
    { path: '/routines/:id/edit', name: 'routine-edit', component: () => import('@/views/RoutineEditorView.vue') },
    { path: '/workout', name: 'workout', component: () => import('@/views/ActiveWorkoutView.vue') },
    { path: '/workout/summary', name: 'workout-summary', component: () => import('@/views/WorkoutSummaryView.vue') },
    { path: '/history', name: 'history', component: () => import('@/views/HistoryView.vue') },
    { path: '/history/:id', name: 'history-detail', component: () => import('@/views/HistoryDetailView.vue') },
    { path: '/exercises', name: 'exercises', component: () => import('@/views/ExercisesView.vue') },
    { path: '/exercises/:id', name: 'exercise-detail', component: () => import('@/views/ExerciseDetailView.vue') },
    { path: '/progress', name: 'progress', component: () => import('@/views/ProgressView.vue') },
    { path: '/calendar', name: 'calendar', component: () => import('@/views/CalendarView.vue') },
    { path: '/settings', name: 'settings', component: () => import('@/views/SettingsView.vue') },
    { path: '/admin', name: 'admin', component: () => import('@/views/AdminView.vue'), meta: { admin: true } },
    // Unknown paths fall back to the dashboard rather than showing a dead end.
    { path: '/:pathMatch(.*)*', redirect: '/' },
  ],
})

router.beforeEach(async (to) => {
  const session = useSessionStore()

  // A direct navigation can outrun session restoration, so wait for it to settle.
  if (session.initializing) {
    await new Promise<void>((resolve) => {
      const check = () => (session.initializing ? setTimeout(check, 20) : resolve())
      check()
    })
  }

  if (!session.isAuthenticated && !to.meta.public) {
    return { name: 'login', query: to.fullPath === '/' ? undefined : { redirect: to.fullPath } }
  }

  if (session.isAuthenticated && to.meta.public) return { name: 'dashboard' }

  // Admin routes are guarded here for navigation only; the API enforces authorization.
  if (to.meta.admin && !session.isAdmin) return { name: 'dashboard' }

  return true
})

export default router
