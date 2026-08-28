<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import {
  Bell,
  Download,
  KeyRound,
  LogOut,
  Moon,
  Ruler,
  Timer,
  Upload,
  UserRound,
  Weight,
} from '@lucide/vue'
import { api } from '@/lib/api'
import { ApiError } from '@/lib/http'
import { useSessionStore } from '@/stores/session'
import { weightUnitLabel } from '@/lib/format'
import type { ImportPreview, LengthUnit, OneRepMaxFormula, ThemePreference, TrainingGoal, WeightUnit } from '@/lib/types'

const session = useSessionStore()
const router = useRouter()

const notice = ref<string | null>(null)
const error = ref<string | null>(null)
const savingProfile = ref(false)
const savingSettings = ref(false)

/** Local copy of the profile so edits can be cancelled by navigating away. */
const profileForm = reactive({
  displayName: '',
  goal: 'GeneralFitness' as TrainingGoal,
  heightCm: null as number | null,
  gender: '' as string,
  dateOfBirth: '' as string,
})

const passwordForm = reactive({ current: '', next: '' })

/** Plate inventory is edited as a comma-separated list, which is easier on mobile. */
const plateText = ref('')
const warmupText = ref('')

const importPreview = ref<ImportPreview | null>(null)
const importFile = ref<File | null>(null)

const goals: Array<{ value: TrainingGoal; label: string }> = [
  { value: 'GeneralFitness', label: 'General fitness' },
  { value: 'FatLoss', label: 'Fat loss' },
  { value: 'Strength', label: 'Strength' },
  { value: 'Hypertrophy', label: 'Hypertrophy' },
  { value: 'Endurance', label: 'Endurance' },
]

const formulas: OneRepMaxFormula[] = ['Epley', 'Brzycki', 'Lombardi']

const unit = computed(() => weightUnitLabel(session.weightUnit))

/** Timezones the browser knows about, so the value always round-trips. */
const timezones = computed(() => {
  const supported = (Intl as unknown as { supportedValuesOf?: (key: string) => string[] }).supportedValuesOf
  const list = supported ? supported('timeZone') : []
  const current = session.settings.timeZone
  return list.length > 0 ? list : [current, 'UTC'].filter((value, index, all) => all.indexOf(value) === index)
})

onMounted(() => {
  if (session.profile) {
    profileForm.displayName = session.profile.displayName
    profileForm.goal = session.profile.goal
    profileForm.heightCm = session.profile.heightCm
    profileForm.gender = session.profile.gender ?? ''
    profileForm.dateOfBirth = session.profile.dateOfBirth ?? ''
  }

  plateText.value = session.settings.plateInventoryKg.join(', ')
  warmupText.value = session.settings.warmupPercentages.join(', ')
})

function flash(message: string) {
  notice.value = message
  error.value = null
  window.setTimeout(() => (notice.value = null), 2000)
}

function describe(exception: unknown, fallback: string) {
  return exception instanceof ApiError ? exception.message : fallback
}

async function saveProfile() {
  savingProfile.value = true
  error.value = null

  try {
    await session.updateProfile({
      displayName: profileForm.displayName.trim(),
      goal: profileForm.goal,
      heightCm: profileForm.heightCm,
      gender: profileForm.gender.trim() || null,
      dateOfBirth: profileForm.dateOfBirth || null,
    })
    flash('Profile saved')
  } catch (exception) {
    error.value = describe(exception, 'Unable to save your profile.')
  } finally {
    savingProfile.value = false
  }
}

/** Parses the comma-separated numeric lists, ignoring anything unparseable. */
function parseNumbers(text: string) {
  return text
    .split(',')
    .map((part) => Number(part.trim()))
    .filter((value) => Number.isFinite(value) && value > 0)
}

async function saveSettings(changes: Partial<typeof session.settings> = {}) {
  savingSettings.value = true
  error.value = null

  try {
    await session.updateSettings({
      ...session.settings,
      plateInventoryKg: parseNumbers(plateText.value),
      warmupPercentages: parseNumbers(warmupText.value).filter((value) => value < 100),
      ...changes,
    })
    flash('Settings saved')
  } catch (exception) {
    error.value = describe(exception, 'Unable to save your settings.')
  } finally {
    savingSettings.value = false
  }
}

async function changePassword() {
  error.value = null

  if (passwordForm.next.length < 8) {
    error.value = 'New password must be at least 8 characters.'
    return
  }

  try {
    await session.changePassword(passwordForm.current, passwordForm.next)
    passwordForm.current = ''
    passwordForm.next = ''
    flash('Password changed. Other sessions were signed out.')
  } catch (exception) {
    error.value = describe(exception, 'Unable to change your password.')
  }
}

function saveBlob(blob: Blob, fileName: string) {
  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = fileName
  link.click()
  URL.revokeObjectURL(url)
}

async function exportJson() {
  try {
    const { blob, fileName } = await api.data.exportJson()
    saveBlob(blob, fileName)
  } catch (exception) {
    error.value = describe(exception, 'Unable to export your data.')
  }
}

async function exportCsv(dataset: 'workouts' | 'sets' | 'exercises' | 'measurements') {
  try {
    const { blob, fileName } = await api.data.exportCsv(dataset)
    saveBlob(blob, fileName)
  } catch (exception) {
    error.value = describe(exception, 'Unable to export your data.')
  }
}

/** Import always previews first so nothing is written without review (spec US-210). */
async function previewImport(event: Event) {
  const input = event.target as HTMLInputElement
  const file = input.files?.[0] ?? null
  importFile.value = file
  importPreview.value = null

  if (!file) return

  try {
    importPreview.value = await api.data.previewImport(file)
  } catch (exception) {
    error.value = describe(exception, 'Unable to read that CSV file.')
  }
}

async function commitImport() {
  if (!importFile.value) return

  try {
    const result = await api.data.commitImport(importFile.value)
    importPreview.value = null
    importFile.value = null
    flash(`Imported ${result.workoutsCreated} workouts and ${result.setsCreated} sets.`)
  } catch (exception) {
    error.value = describe(exception, 'Unable to import that file.')
  }
}

async function signOut() {
  await session.logout()
  await router.push('/login')
}
</script>

<template>
  <div class="page narrow-page">
    <header class="page-head">
      <div>
        <span class="eyebrow">MAKE IT YOURS</span>
        <h1>Settings</h1>
        <p>Preferences for training and your account.</p>
      </div>
    </header>

    <p v-if="notice" class="form-note success" role="status">{{ notice }}</p>
    <p v-if="error" class="form-error" role="alert">{{ error }}</p>

    <section class="settings-section">
      <span class="folder-head">PROFILE</span>

      <label class="setting-row">
        <span class="stat-icon coral"><UserRound :size="19" /></span>
        <span><strong>Display name</strong><small>Shown across your training space</small></span>
        <input v-model="profileForm.displayName" maxlength="60" />
      </label>

      <div class="setting-row">
        <span class="stat-icon coral"><UserRound :size="19" /></span>
        <span><strong>Email</strong><small>Your account address</small></span>
        <span>{{ session.profile?.email }}</span>
      </div>

      <label class="setting-row">
        <span class="stat-icon lime"><Ruler :size="19" /></span>
        <span><strong>Height (cm)</strong><small>Stored in centimeters</small></span>
        <input v-model.number="profileForm.heightCm" type="number" min="50" max="260" step="0.5" placeholder="-" />
      </label>

      <label class="setting-row">
        <span class="stat-icon purple"><UserRound :size="19" /></span>
        <span><strong>Training goal</strong><small>Descriptive only, never changes your programs</small></span>
        <select v-model="profileForm.goal">
          <option v-for="goal in goals" :key="goal.value" :value="goal.value">{{ goal.label }}</option>
        </select>
      </label>

      <label class="setting-row">
        <span class="stat-icon amber"><UserRound :size="19" /></span>
        <span><strong>Date of birth</strong><small>Optional</small></span>
        <input v-model="profileForm.dateOfBirth" type="date" />
      </label>

      <button class="btn btn-primary btn-wide" :disabled="savingProfile" @click="saveProfile">
        {{ savingProfile ? 'Saving…' : 'Save profile' }}
      </button>
    </section>

    <section class="settings-section">
      <span class="folder-head">UNITS AND TIME</span>

      <label class="setting-row">
        <span class="stat-icon lime"><Weight :size="19" /></span>
        <span><strong>Weight units</strong><small>Stored in kilograms, shown in your choice</small></span>
        <select
          :value="session.settings.weightUnit"
          @change="saveSettings({ weightUnit: ($event.target as HTMLSelectElement).value as WeightUnit })"
        >
          <option value="Kilograms">Kilograms</option>
          <option value="Pounds">Pounds</option>
        </select>
      </label>

      <label class="setting-row">
        <span class="stat-icon lime"><Ruler :size="19" /></span>
        <span><strong>Length units</strong><small>For body measurements</small></span>
        <select
          :value="session.settings.lengthUnit"
          @change="saveSettings({ lengthUnit: ($event.target as HTMLSelectElement).value as LengthUnit })"
        >
          <option value="Centimeters">Centimeters</option>
          <option value="Inches">Inches</option>
        </select>
      </label>

      <label class="setting-row">
        <span class="stat-icon purple"><Timer :size="19" /></span>
        <span><strong>Timezone</strong><small>Groups your history by local date</small></span>
        <select
          :value="session.settings.timeZone"
          @change="saveSettings({ timeZone: ($event.target as HTMLSelectElement).value })"
        >
          <option v-for="zone in timezones" :key="zone" :value="zone">{{ zone }}</option>
        </select>
      </label>
    </section>

    <section class="settings-section">
      <span class="folder-head">TRAINING</span>

      <label class="setting-row">
        <span class="stat-icon purple"><Timer :size="19" /></span>
        <span><strong>Default rest timer</strong><small>Used when an exercise has no override</small></span>
        <select
          :value="session.settings.defaultRestSeconds"
          @change="saveSettings({ defaultRestSeconds: Number(($event.target as HTMLSelectElement).value) })"
        >
          <option :value="60">1 minute</option>
          <option :value="90">1 min 30 sec</option>
          <option :value="120">2 minutes</option>
          <option :value="180">3 minutes</option>
        </select>
      </label>

      <button class="setting-row" @click="saveSettings({ autoStartRestTimer: !session.settings.autoStartRestTimer })">
        <span class="stat-icon amber"><Timer :size="19" /></span>
        <span><strong>Auto-start rest timer</strong><small>Starts when you complete a set</small></span>
        <span class="toggle" :class="{ on: session.settings.autoStartRestTimer }"><i></i></span>
      </button>

      <button class="setting-row" @click="saveSettings({ restTimerVibrate: !session.settings.restTimerVibrate })">
        <span class="stat-icon amber"><Bell :size="19" /></span>
        <span><strong>Vibrate on finish</strong><small>Where the device supports it</small></span>
        <span class="toggle" :class="{ on: session.settings.restTimerVibrate }"><i></i></span>
      </button>

      <button
        class="setting-row"
        @click="saveSettings({ restTimerNotifications: !session.settings.restTimerNotifications })"
      >
        <span class="stat-icon amber"><Bell :size="19" /></span>
        <span><strong>Browser notifications</strong><small>Opt-in, asked when first needed</small></span>
        <span class="toggle" :class="{ on: session.settings.restTimerNotifications }"><i></i></span>
      </button>

      <label class="setting-row">
        <span class="stat-icon lime"><Weight :size="19" /></span>
        <span><strong>1RM formula</strong><small>Used for estimated strength trends</small></span>
        <select
          :value="session.settings.oneRepMaxFormula"
          @change="saveSettings({ oneRepMaxFormula: ($event.target as HTMLSelectElement).value as OneRepMaxFormula })"
        >
          <option v-for="formula in formulas" :key="formula" :value="formula">{{ formula }}</option>
        </select>
      </label>

      <label class="setting-row">
        <span class="stat-icon coral"><Weight :size="19" /></span>
        <span><strong>Weekly goal</strong><small>Target workout frequency</small></span>
        <input
          :value="session.settings.weeklyWorkoutGoal"
          type="number"
          min="0"
          max="14"
          @change="saveSettings({ weeklyWorkoutGoal: Number(($event.target as HTMLInputElement).value) })"
        />
      </label>
    </section>

    <section class="settings-section">
      <span class="folder-head">EQUIPMENT</span>

      <label class="field-label">
        Bar weight (kg)
        <input
          :value="session.settings.barWeightKg"
          type="number"
          min="0"
          max="100"
          step="0.5"
          @change="saveSettings({ barWeightKg: Number(($event.target as HTMLInputElement).value) })"
        />
      </label>

      <label class="field-label">
        Available plates per side (kg, comma separated)
        <input v-model="plateText" placeholder="25, 20, 15, 10, 5, 2.5, 1.25" />
      </label>

      <label class="field-label">
        Warm-up percentages
        <input v-model="warmupText" placeholder="40, 60, 80" />
      </label>

      <label class="field-label">
        Rounding increment (kg)
        <input
          :value="session.settings.roundingIncrementKg"
          type="number"
          min="0.5"
          max="50"
          step="0.25"
          @change="saveSettings({ roundingIncrementKg: Number(($event.target as HTMLInputElement).value) })"
        />
      </label>

      <label class="field-label">
        Overload increment (kg)
        <input
          :value="session.settings.overloadIncrementKg"
          type="number"
          min="0"
          max="50"
          step="0.25"
          @change="saveSettings({ overloadIncrementKg: Number(($event.target as HTMLInputElement).value) })"
        />
      </label>

      <button class="btn btn-quiet btn-wide" :disabled="savingSettings" @click="saveSettings()">
        {{ savingSettings ? 'Saving…' : 'Save equipment settings' }}
      </button>
    </section>

    <section class="settings-section">
      <span class="folder-head">APPEARANCE</span>

      <label class="setting-row">
        <span class="stat-icon purple"><Moon :size="19" /></span>
        <span><strong>Theme</strong><small>Dark is optimized for gym use</small></span>
        <select
          :value="session.settings.theme"
          @change="saveSettings({ theme: ($event.target as HTMLSelectElement).value as ThemePreference })"
        >
          <option value="Dark">Dark</option>
          <option value="Light">Light</option>
          <option value="System">System</option>
        </select>
      </label>
    </section>

    <section class="settings-section">
      <span class="folder-head">SECURITY</span>

      <label class="field-label">
        Current password
        <input v-model="passwordForm.current" type="password" autocomplete="current-password" />
      </label>

      <label class="field-label">
        New password
        <input v-model="passwordForm.next" type="password" autocomplete="new-password" minlength="8" />
      </label>

      <button class="btn btn-quiet btn-wide" @click="changePassword"><KeyRound :size="16" /> Change password</button>
    </section>

    <section class="settings-section">
      <span class="folder-head">YOUR DATA</span>

      <button class="setting-row" @click="exportJson">
        <span class="stat-icon lime"><Download :size="19" /></span>
        <span><strong>Export everything</strong><small>Complete JSON with schema version</small></span>
        <span>JSON</span>
      </button>

      <button class="setting-row" @click="exportCsv('sets')">
        <span class="stat-icon lime"><Download :size="19" /></span>
        <span><strong>Export sets</strong><small>Every logged set</small></span>
        <span>CSV</span>
      </button>

      <button class="setting-row" @click="exportCsv('workouts')">
        <span class="stat-icon lime"><Download :size="19" /></span>
        <span><strong>Export workouts</strong><small>One row per session</small></span>
        <span>CSV</span>
      </button>

      <button class="setting-row" @click="exportCsv('measurements')">
        <span class="stat-icon lime"><Download :size="19" /></span>
        <span><strong>Export measurements</strong><small>Body tracking history</small></span>
        <span>CSV</span>
      </button>

      <label class="setting-row">
        <span class="stat-icon amber"><Upload :size="19" /></span>
        <span><strong>Import CSV</strong><small>Requires date, exercise, weight and reps columns</small></span>
        <input type="file" accept=".csv,text/csv" @change="previewImport" />
      </label>

      <div v-if="importPreview" class="import-preview">
        <p>
          {{ importPreview.validRows }} of {{ importPreview.totalRows }} rows are valid.
          <template v-if="importPreview.invalidRows > 0">{{ importPreview.invalidRows }} will be skipped.</template>
        </p>

        <ul>
          <li v-for="row in importPreview.rows.filter((r) => r.error).slice(0, 8)" :key="row.rowNumber">
            Row {{ row.rowNumber }}: {{ row.error }}
          </li>
        </ul>

        <button class="btn btn-primary btn-wide" :disabled="!importPreview.canCommit" @click="commitImport">
          Import {{ importPreview.validRows }} rows
        </button>
      </div>
    </section>

    <RouterLink v-if="session.isAdmin" to="/admin" class="btn btn-quiet btn-wide">Admin tools</RouterLink>

    <button class="logout-button" @click="signOut"><LogOut :size="18" /> Sign out</button>
  </div>
</template>
