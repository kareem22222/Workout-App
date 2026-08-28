import { computed, ref, watch } from 'vue'
import { defineStore } from 'pinia'

export type SetType = 'Normal' | 'Warmup' | 'Drop set' | 'Failure' | 'AMRAP' | 'Backoff'
export type Exercise = { id: string; name: string; muscle: string; equipment: string; instructions: string; custom?: boolean }
export type WorkoutSet = { id: string; previous: string; weight: number; reps: number; rpe: number | null; done: boolean; type: SetType }
export type WorkoutExercise = { id: string; exerciseId: string; name: string; rest: number; note: string; sets: WorkoutSet[] }
export type Routine = { id: string; name: string; description: string; exercises: WorkoutExercise[] }
export type Workout = { id: string; title: string; note: string; startedAt: string; completedAt?: string; exercises: WorkoutExercise[] }
export type Measurement = { id: string; date: string; weight: number; bodyFat: number | null; note: string }
export type Profile = { name: string; email: string; units: 'kg' | 'lb'; theme: 'dark' | 'light'; defaultRest: number; timerAlerts: boolean; timezone: string; weeklyGoal: number }
export type WorkoutSummary = { id: string; title: string; duration: number; sets: number; reps: number; volume: number; prs: string[] }
export type RoutineDraft = { id?: string; name: string; description: string; exercises: Array<{ exerciseId: string; sets: number; reps: number; weight: number; rest: number }> }

const newSet = (previous = '-', weight = 0, reps = 8, type: SetType = 'Normal'): WorkoutSet => ({ id: crypto.randomUUID(), previous, weight, reps, rpe: null, done: false, type })
const exercise = (id: string, name: string, muscle: string, equipment: string, instructions: string): Exercise => ({ id, name, muscle, equipment, instructions })

const seedExercises: Exercise[] = [
  exercise('bench', 'Bench Press', 'Chest', 'Barbell', 'Retract your shoulders, keep your feet planted, and lower the bar with control.'),
  exercise('squat', 'Back Squat', 'Quadriceps', 'Barbell', 'Brace before each rep, sit between your hips, and keep your whole foot planted.'),
  exercise('row', 'Barbell Row', 'Back', 'Barbell', 'Hinge at the hips, keep a neutral spine, and pull toward your lower ribs.'),
  exercise('ohp', 'Overhead Press', 'Shoulders', 'Barbell', 'Squeeze your glutes, brace, and press the bar in a straight path overhead.'),
  exercise('pullup', 'Pull Up', 'Back', 'Bodyweight', 'Start from a full hang and drive your elbows toward your sides.'),
  exercise('rdl', 'Romanian Deadlift', 'Hamstrings', 'Barbell', 'Push your hips back while keeping the bar close and your spine neutral.'),
  exercise('curl', 'Dumbbell Curl', 'Biceps', 'Dumbbell', 'Keep your elbows still and control the lowering phase.'),
  exercise('triceps', 'Cable Pushdown', 'Triceps', 'Cable', 'Pin your elbows to your sides and fully extend without leaning over the cable.'),
  exercise('incline', 'Incline Dumbbell Press', 'Chest', 'Dumbbell', 'Set a low incline and press with your forearms vertical.'),
  exercise('legpress', 'Leg Press', 'Quadriceps', 'Machine', 'Lower under control without letting your lower back lift from the pad.'),
  exercise('lateral', 'Lateral Raise', 'Shoulders', 'Dumbbell', 'Lead with your elbows and stop around shoulder height.'),
  exercise('pulldown', 'Lat Pulldown', 'Back', 'Cable', 'Pull your elbows down while keeping your chest tall.'),
]

const routineExercise = (exerciseId: string, name: string, rest: number, note: string, values: Array<[string, number, number]>): WorkoutExercise => ({
  id: crypto.randomUUID(), exerciseId, name, rest, note,
  sets: values.map(([previous, weight, reps]) => newSet(previous, weight, reps)),
})

const seedRoutines: Routine[] = [
  { id: 'upper', name: 'Upper A', description: 'Strength-focused upper body session.', exercises: [
    routineExercise('bench', 'Bench Press', 120, 'Retract shoulders. Controlled descent.', [['77.5 x 8',80,8],['77.5 x 8',80,8],['77.5 x 7',80,8],['75 x 9',77.5,10]]),
    routineExercise('row', 'Barbell Row', 90, 'Pause at the top.', [['65 x 10',67.5,10],['65 x 10',67.5,10],['65 x 9',67.5,10],['60 x 12',62.5,12]]),
    routineExercise('ohp', 'Overhead Press', 90, '', [['42.5 x 8',45,8],['42.5 x 7',45,8],['40 x 9',42.5,9],['40 x 8',42.5,9]]),
    routineExercise('pullup', 'Pull Up', 90, 'Full hang each rep.', [['BW x 9',0,10],['BW x 8',0,9],['BW x 7',0,8],['BW x 6',0,8]]),
  ] },
  { id: 'lower', name: 'Lower A', description: 'Squat, hinge and leg accessories.', exercises: [
    routineExercise('squat', 'Back Squat', 150, 'Brace before each rep.', [['105 x 6',107.5,6],['105 x 6',107.5,6],['100 x 8',102.5,8],['100 x 7',102.5,8]]),
    routineExercise('rdl', 'Romanian Deadlift', 120, 'Keep the bar close.', [['90 x 8',92.5,8],['90 x 8',92.5,8],['90 x 8',92.5,8],['85 x 10',87.5,10]]),
    routineExercise('legpress', 'Leg Press', 90, '', [['160 x 10',170,10],['160 x 10',170,10],['160 x 9',170,10],['150 x 12',160,12]]),
  ] },
  { id: 'full', name: 'Quick Full Body', description: 'A compact session for busy days.', exercises: [
    routineExercise('squat', 'Back Squat', 120, '', [['100 x 6',100,6],['100 x 6',100,6],['95 x 8',95,8]]),
    routineExercise('bench', 'Bench Press', 90, '', [['75 x 8',77.5,8],['75 x 8',77.5,8],['72.5 x 10',75,10]]),
    routineExercise('row', 'Barbell Row', 90, '', [['62.5 x 10',65,10],['62.5 x 10',65,10],['60 x 12',62.5,12]]),
  ] },
]

const completedClone = (routine: Routine, id: string, startedAt: string, completedAt: string): Workout => ({
  id, title: routine.name, note: '', startedAt, completedAt,
  exercises: structuredClone(routine.exercises).map(item => ({ ...item, sets: item.sets.slice(0, 3).map(set => ({ ...set, id: crypto.randomUUID(), done: true })) })),
})

const seedHistory: Workout[] = [
  completedClone(seedRoutines[0]!, 'h1', '2026-08-25T12:05:00Z', '2026-08-25T13:08:00Z'),
  completedClone(seedRoutines[1]!, 'h2', '2026-08-22T11:32:00Z', '2026-08-22T12:29:00Z'),
  completedClone(seedRoutines[2]!, 'h3', '2026-08-19T12:12:00Z', '2026-08-19T12:58:00Z'),
]

const profileDefaults: Profile = { name: 'Alex', email: 'alex@example.com', units: 'kg', theme: 'dark', defaultRest: 90, timerAlerts: true, timezone: Intl.DateTimeFormat().resolvedOptions().timeZone, weeklyGoal: 4 }
type SavedState = Partial<{ isAuthenticated: boolean; profile: Partial<Profile>; routines: Routine[]; activeWorkout: Workout | null; history: Workout[]; restEndsAt: number | null; exercises: Exercise[]; measurements: Measurement[]; lastSummary: WorkoutSummary | null }>

function loadSaved(): SavedState {
  try { return JSON.parse(localStorage.getItem('form-state') || '{}') as SavedState }
  catch { localStorage.removeItem('form-state'); return {} }
}
function normalizeSets(sets: WorkoutSet[]) { return sets.map(set => ({ ...set, type: set.type || 'Normal' as SetType })) }
function normalizeWorkout(workout: Workout) { return { ...workout, note: workout.note || '', exercises: workout.exercises.map(item => ({ ...item, sets: normalizeSets(item.sets) })) } }

export const useAppStore = defineStore('app', () => {
  const saved = loadSaved()
  const isAuthenticated = ref(saved.isAuthenticated ?? false)
  const profile = ref<Profile>({ ...profileDefaults, ...saved.profile })
  const routineList = ref<Routine[]>((saved.routines ?? seedRoutines).map(r => ({ ...r, description: r.description || '', exercises: r.exercises.map(item => ({ ...item, sets: normalizeSets(item.sets) })) })))
  const activeWorkout = ref<Workout | null>(saved.activeWorkout ? normalizeWorkout(saved.activeWorkout) : null)
  const history = ref<Workout[]>((saved.history ?? seedHistory).map(normalizeWorkout))
  const restEndsAt = ref<number | null>(saved.restEndsAt ?? null)
  const exerciseList = ref<Exercise[]>((saved.exercises ?? seedExercises).map(item => ({ ...item, instructions: item.instructions || 'No instructions added yet.' })))
  const measurements = ref<Measurement[]>(saved.measurements ?? [
    { id: 'm1', date: '2026-08-26', weight: 78.4, bodyFat: 16.8, note: '' },
    { id: 'm2', date: '2026-08-12', weight: 79.1, bodyFat: 17.1, note: '' },
    { id: 'm3', date: '2026-07-29', weight: 79.8, bodyFat: 17.4, note: '' },
  ])
  const lastSummary = ref<WorkoutSummary | null>(saved.lastSummary ?? null)

  const completedSets = computed(() => activeWorkout.value?.exercises.flatMap(e => e.sets).filter(s => s.done).length ?? 0)
  const totalSets = computed(() => activeWorkout.value?.exercises.flatMap(e => e.sets).length ?? 0)
  const workoutsThisWeek = computed(() => { const now = new Date(); const start = new Date(now); start.setDate(now.getDate() - ((now.getDay() + 6) % 7)); start.setHours(0,0,0,0); return history.value.filter(w => new Date(w.startedAt) >= start) })
  const weeklyVolume = computed(() => workoutsThisWeek.value.reduce((sum, workout) => sum + workoutVolume(workout), 0))

  function login(name: string, email: string) { profile.value.name = name.trim() || 'Athlete'; profile.value.email = email.trim(); isAuthenticated.value = true }
  function register(name: string, email: string) { login(name, email) }
  function logout() { isAuthenticated.value = false }
  function startRoutine(id: string) {
    if (activeWorkout.value) return false
    const routine = routineList.value.find(r => r.id === id); if (!routine) return false
    activeWorkout.value = { id: crypto.randomUUID(), title: routine.name, note: '', startedAt: new Date().toISOString(), exercises: structuredClone(routine.exercises).map(item => ({ ...item, id: crypto.randomUUID(), sets: item.sets.map(set => ({ ...set, id: crypto.randomUUID(), done: false })) })) }
    return true
  }
  function startEmpty() { if (activeWorkout.value) return false; activeWorkout.value = { id: crypto.randomUUID(), title: 'Quick Workout', note: '', startedAt: new Date().toISOString(), exercises: [] }; return true }
  function discardWorkout() { activeWorkout.value = null; restEndsAt.value = null }
  function toggleSet(item: WorkoutExercise, workoutSet: WorkoutSet) {
    if (workoutSet.weight < 0 || workoutSet.reps < 0 || (workoutSet.rpe !== null && (workoutSet.rpe < 1 || workoutSet.rpe > 10))) return false
    workoutSet.done = !workoutSet.done; restEndsAt.value = workoutSet.done ? Date.now() + item.rest * 1000 : null; return true
  }
  function addSet(item: WorkoutExercise) { const last = item.sets[item.sets.length - 1]; item.sets.push(newSet('-', last?.weight ?? 0, last?.reps ?? 8, last?.type ?? 'Normal')) }
  function duplicateSet(item: WorkoutExercise, setId: string) { const index = item.sets.findIndex(set => set.id === setId); if (index >= 0) item.sets.splice(index + 1, 0, { ...structuredClone(item.sets[index]!), id: crypto.randomUUID(), done: false }) }
  function removeSet(item: WorkoutExercise, setId: string) { if (item.sets.length > 1) item.sets = item.sets.filter(set => set.id !== setId) }
  function cycleSetType(workoutSet: WorkoutSet) { const types: SetType[] = ['Normal','Warmup','Drop set','Failure','AMRAP','Backoff']; workoutSet.type = types[(types.indexOf(workoutSet.type) + 1) % types.length]! }
  function addExercise(item: Exercise) { if (!activeWorkout.value || activeWorkout.value.exercises.some(e => e.exerciseId === item.id)) return false; activeWorkout.value.exercises.push({ id: crypto.randomUUID(), exerciseId: item.id, name: item.name, rest: profile.value.defaultRest, note: '', sets: [newSet(), newSet(), newSet()] }); return true }
  function removeWorkoutExercise(id: string) { if (activeWorkout.value) activeWorkout.value.exercises = activeWorkout.value.exercises.filter(e => e.id !== id) }
  function moveWorkoutExercise(id: string, direction: -1 | 1) { if (!activeWorkout.value) return; const index = activeWorkout.value.exercises.findIndex(e => e.id === id); const next = index + direction; if (index >= 0 && next >= 0 && next < activeWorkout.value.exercises.length) [activeWorkout.value.exercises[index], activeWorkout.value.exercises[next]] = [activeWorkout.value.exercises[next]!, activeWorkout.value.exercises[index]!] }
  function finishWorkout() {
    const workout = activeWorkout.value; if (!workout || !completedSets.value) return null
    workout.completedAt = new Date().toISOString()
    const summary: WorkoutSummary = { id: workout.id, title: workout.title, duration: workoutDuration(workout), sets: completedSets.value, reps: workout.exercises.flatMap(e => e.sets).filter(s => s.done).reduce((sum, set) => sum + set.reps, 0), volume: workoutVolume(workout), prs: detectPrs(workout, history.value, profile.value.units) }
    history.value.unshift(structuredClone(workout)); lastSummary.value = summary; activeWorkout.value = null; restEndsAt.value = null; return summary
  }
  function saveRoutine(draft: RoutineDraft) {
    const mapped = draft.exercises.map(item => { const source = exerciseList.value.find(e => e.id === item.exerciseId)!; const count=Math.min(20,Math.max(1,Math.round(item.sets)||1)); return { id: crypto.randomUUID(), exerciseId: source.id, name: source.name, rest: Math.min(1800,Math.max(0,item.rest||0)), note: '', sets: Array.from({ length: count }, () => newSet('-', Math.max(0,item.weight||0), Math.min(100,Math.max(1,Math.round(item.reps)||1)))) } })
    const routine: Routine = { id: draft.id ?? crypto.randomUUID(), name: draft.name.trim(), description: draft.description.trim(), exercises: mapped }
    const index = routineList.value.findIndex(r => r.id === routine.id); if (index >= 0) routineList.value[index] = routine; else routineList.value.push(routine); return routine.id
  }
  function duplicateRoutine(id: string) { const source = routineList.value.find(r => r.id === id); if (!source) return; const copy = structuredClone(source); copy.id = crypto.randomUUID(); copy.name += ' Copy'; copy.exercises.forEach(e => { e.id = crypto.randomUUID(); e.sets.forEach(s => s.id = crypto.randomUUID()) }); routineList.value.push(copy) }
  function deleteRoutine(id: string) { routineList.value = routineList.value.filter(r => r.id !== id) }
  function createCustomExercise(input: Omit<Exercise, 'id' | 'custom'>) { const item: Exercise = { ...input, id: crypto.randomUUID(), custom: true }; exerciseList.value.push(item); return item.id }
  function deleteCustomExercise(id: string) { if (!routineList.value.some(r => r.exercises.some(e => e.exerciseId === id))) exerciseList.value = exerciseList.value.filter(e => e.id !== id) }
  function updateWorkout(id: string, title: string, note: string) { const workout = history.value.find(w => w.id === id); if (workout) { workout.title = title.trim() || workout.title; workout.note = note.trim() } }
  function deleteWorkout(id: string) { history.value = history.value.filter(w => w.id !== id) }
  function addMeasurement(input: Omit<Measurement, 'id'>) { measurements.value.unshift({ ...input, id: crypto.randomUUID() }); measurements.value.sort((a,b) => b.date.localeCompare(a.date)) }
  function deleteMeasurement(id: string) { measurements.value = measurements.value.filter(m => m.id !== id) }
  function setUnits(units: Profile['units']) {
    if (units === profile.value.units) return
    const factor = units === 'lb' ? 2.2046226218 : 1 / 2.2046226218
    const convert = (value: number) => Math.round(value * factor * 10) / 10
    const convertSets = (sets: WorkoutSet[]) => sets.forEach(set => { set.weight = convert(set.weight); const value = Number(set.previous.split('x')[0]?.trim()); if (Number.isFinite(value)) set.previous = set.previous.replace(String(value), String(convert(value))) })
    routineList.value.forEach(r => r.exercises.forEach(e => convertSets(e.sets)))
    history.value.forEach(w => w.exercises.forEach(e => convertSets(e.sets)))
    activeWorkout.value?.exercises.forEach(e => convertSets(e.sets))
    measurements.value.forEach(m => m.weight = convert(m.weight))
    if (lastSummary.value) { lastSummary.value.volume = convert(lastSummary.value.volume); lastSummary.value.prs = lastSummary.value.prs.map(pr => pr.replace(/([0-9.]+) (kg|lb)$/, (_,value) => `${convert(Number(value))} ${units}`)) }
    profile.value.units = units
  }
  function resetDemo() { localStorage.removeItem('form-state'); location.reload() }

  watch([isAuthenticated, profile, routineList, activeWorkout, history, restEndsAt, exerciseList, measurements, lastSummary], () => localStorage.setItem('form-state', JSON.stringify({ isAuthenticated: isAuthenticated.value, profile: profile.value, routines: routineList.value, activeWorkout: activeWorkout.value, history: history.value, restEndsAt: restEndsAt.value, exercises: exerciseList.value, measurements: measurements.value, lastSummary: lastSummary.value })), { deep: true })
  return { isAuthenticated, profile, routineList, activeWorkout, history, restEndsAt, exerciseList, measurements, lastSummary, completedSets, totalSets, workoutsThisWeek, weeklyVolume, login, register, logout, startRoutine, startEmpty, discardWorkout, toggleSet, addSet, duplicateSet, removeSet, cycleSetType, addExercise, removeWorkoutExercise, moveWorkoutExercise, finishWorkout, saveRoutine, duplicateRoutine, deleteRoutine, createCustomExercise, deleteCustomExercise, updateWorkout, deleteWorkout, addMeasurement, deleteMeasurement, setUnits, resetDemo }
})

export function workoutVolume(workout: Workout) { return workout.exercises.flatMap(e => e.sets).filter(s => s.done).reduce((sum, set) => sum + set.weight * set.reps, 0) }
export function workoutDuration(workout: Workout) { return workout.completedAt ? Math.max(1, Math.round((new Date(workout.completedAt).getTime() - new Date(workout.startedAt).getTime()) / 60000)) : 0 }
export function routineSubtitle(routine: Routine) { return `${routine.exercises.length} exercises - ${routine.exercises.reduce((sum, e) => sum + e.sets.length, 0)} sets` }
function detectPrs(workout: Workout, history: Workout[], units: Profile['units']) {
  return workout.exercises.flatMap(item => {
    const best = Math.max(...item.sets.filter(s => s.done).map(s => s.weight), 0)
    const previous = Math.max(...history.flatMap(w => w.exercises.filter(e => e.exerciseId === item.exerciseId).flatMap(e => e.sets.filter(s => s.done).map(s => s.weight))), 0)
    return best > previous ? [`${item.name} - ${best} ${units}`] : []
  }).slice(0, 3)
}
