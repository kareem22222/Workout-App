import { createRouter, createWebHistory } from 'vue-router'
import DashboardView from '@/views/DashboardView.vue'
import RoutinesView from '@/views/RoutinesView.vue'
import ActiveWorkoutView from '@/views/ActiveWorkoutView.vue'
import HistoryView from '@/views/HistoryView.vue'
import ExercisesView from '@/views/ExercisesView.vue'
import ProgressView from '@/views/ProgressView.vue'
import SettingsView from '@/views/SettingsView.vue'
import LoginView from '@/views/LoginView.vue'
import RoutineEditorView from '@/views/RoutineEditorView.vue'
import WorkoutSummaryView from '@/views/WorkoutSummaryView.vue'
import HistoryDetailView from '@/views/HistoryDetailView.vue'
import CalendarView from '@/views/CalendarView.vue'

export default createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/login', component: LoginView, meta: { public: true } },
    { path: '/', component: DashboardView },
    { path: '/routines', component: RoutinesView },
    { path: '/routines/new', component: RoutineEditorView },
    { path: '/routines/:id/edit', component: RoutineEditorView },
    { path: '/workout', component: ActiveWorkoutView },
    { path: '/workout/summary', component: WorkoutSummaryView },
    { path: '/history', component: HistoryView },
    { path: '/history/:id', component: HistoryDetailView },
    { path: '/exercises', component: ExercisesView },
    { path: '/progress', component: ProgressView },
    { path: '/calendar', component: CalendarView },
    { path: '/settings', component: SettingsView },
  ],
})
