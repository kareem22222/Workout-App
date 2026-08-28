import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { api } from '@/lib/api'
import { ApiError, refreshSession, setAccessToken, setSessionExpiredHandler } from '@/lib/http'
import { weightUnitLabel } from '@/lib/format'
import type { UpdateProfileRequest, UpdateSettingsRequest, UserProfile, UserSettings } from '@/lib/types'

/** Settings applied before the server responds, so the first paint is never unstyled. */
const defaultSettings: UserSettings = {
  weightUnit: 'Kilograms',
  lengthUnit: 'Centimeters',
  timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone || 'UTC',
  theme: 'Dark',
  oneRepMaxFormula: 'Epley',
  defaultRestSeconds: 90,
  autoStartRestTimer: true,
  restTimerSound: true,
  restTimerVibrate: true,
  restTimerNotifications: false,
  barWeightKg: 20,
  plateInventoryKg: [25, 20, 15, 10, 5, 2.5, 1.25],
  roundingIncrementKg: 2.5,
  overloadIncrementKg: 2.5,
  warmupPercentages: [40, 60, 80],
  weeklyWorkoutGoal: 4,
}

/**
 * Authentication and preference state.
 *
 * The access token lives only in memory. Session continuity across reloads comes from the
 * HttpOnly refresh cookie, so no credential is exposed to scripts (spec 3).
 */
export const useSessionStore = defineStore('session', () => {
  const profile = ref<UserProfile | null>(null)
  const settings = ref<UserSettings>({ ...defaultSettings })

  /** True until the initial refresh attempt completes, so the router can wait. */
  const initializing = ref(true)
  const authenticating = ref(false)
  const error = ref<string | null>(null)

  const isAuthenticated = computed(() => profile.value !== null)
  const isAdmin = computed(() => profile.value?.isAdmin === true)
  const weightUnit = computed(() => settings.value.weightUnit)
  const unitLabel = computed(() => weightUnitLabel(settings.value.weightUnit))

  /** Resolves the theme, honouring the OS preference when set to System. */
  const resolvedTheme = computed<'dark' | 'light'>(() => {
    if (settings.value.theme === 'Dark') return 'dark'
    if (settings.value.theme === 'Light') return 'light'

    return typeof window !== 'undefined' && window.matchMedia?.('(prefers-color-scheme: light)').matches
      ? 'light'
      : 'dark'
  })

  /**
   * Attempts to restore a session from the refresh cookie. Called once at startup.
   */
  async function initialize() {
    setSessionExpiredHandler(() => {
      profile.value = null
    })

    try {
      if (await refreshSession()) await loadAccount()
    } catch {
      profile.value = null
    } finally {
      initializing.value = false
    }
  }

  /** Loads the profile and settings that every screen depends on. */
  async function loadAccount() {
    const [loadedProfile, loadedSettings] = await Promise.all([api.auth.me(), api.settings.get()])
    profile.value = loadedProfile
    settings.value = loadedSettings
  }

  async function login(email: string, password: string): Promise<boolean> {
    authenticating.value = true
    error.value = null

    try {
      const response = await api.auth.login(email, password)
      setAccessToken(response.accessToken)
      await loadAccount()
      return true
    } catch (exception) {
      error.value = exception instanceof ApiError ? exception.message : 'Unable to sign in.'
      return false
    } finally {
      authenticating.value = false
    }
  }

  async function register(displayName: string, email: string, password: string): Promise<boolean> {
    authenticating.value = true
    error.value = null

    try {
      const response = await api.auth.register(displayName, email, password)
      setAccessToken(response.accessToken)
      await loadAccount()
      return true
    } catch (exception) {
      error.value = exception instanceof ApiError ? exception.message : 'Unable to create the account.'
      return false
    } finally {
      authenticating.value = false
    }
  }

  async function logout() {
    try {
      await api.auth.logout()
    } catch {
      // Even if the server call fails, the local session must end.
    } finally {
      setAccessToken(null)
      profile.value = null
      settings.value = { ...defaultSettings }
    }
  }

  async function updateProfile(request: UpdateProfileRequest) {
    profile.value = await api.auth.updateProfile(request)
  }

  async function updateSettings(request: UpdateSettingsRequest) {
    settings.value = await api.settings.update(request)
  }

  /** Convenience path for toggling one setting without rebuilding the whole payload. */
  async function patchSettings(changes: Partial<UserSettings>) {
    await updateSettings({ ...settings.value, ...changes })
  }

  async function changePassword(currentPassword: string, newPassword: string) {
    const response = await api.auth.changePassword(currentPassword, newPassword)
    setAccessToken(response.accessToken)
  }

  return {
    profile,
    settings,
    initializing,
    authenticating,
    error,
    isAuthenticated,
    isAdmin,
    weightUnit,
    unitLabel,
    resolvedTheme,
    initialize,
    loadAccount,
    login,
    register,
    logout,
    updateProfile,
    updateSettings,
    patchSettings,
    changePassword,
  }
})
