<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { ShieldCheck, UserX } from '@lucide/vue'
import { api } from '@/lib/api'
import { ApiError } from '@/lib/http'
import { useSessionStore } from '@/stores/session'
import { formatDate } from '@/lib/format'
import type { AdminUser } from '@/lib/types'

const session = useSessionStore()

const users = ref<AdminUser[]>([])
const info = ref<{ version: string; environment: string; serverTimeUtc: string } | null>(null)
const loading = ref(true)
const error = ref<string | null>(null)

async function load() {
  loading.value = true
  error.value = null

  try {
    const [loadedUsers, loadedInfo] = await Promise.all([api.admin.users(), api.admin.info()])
    users.value = loadedUsers
    info.value = loadedInfo
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to load admin data.'
  } finally {
    loading.value = false
  }
}

onMounted(load)

async function toggleDisabled(user: AdminUser) {
  const next = !user.isDisabled
  const verb = next ? 'Disable' : 'Enable'

  if (!window.confirm(`${verb} ${user.displayName}? Their training data is never deleted.`)) return

  try {
    await api.admin.setDisabled(user.id, next)
    await load()
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : `Unable to ${verb.toLowerCase()} the account.`
  }
}
</script>

<template>
  <div class="page narrow-page">
    <header class="page-head">
      <div>
        <span class="eyebrow">OPERATIONS</span>
        <h1>Admin</h1>
        <p>Accounts and deployment information.</p>
      </div>
      <span class="stat-icon purple"><ShieldCheck :size="19" /></span>
    </header>

    <p v-if="error" class="form-error" role="alert">{{ error }}</p>
    <p v-if="loading" class="small-empty">Loading…</p>

    <section v-if="info" class="settings-section">
      <span class="folder-head">DEPLOYMENT</span>
      <div class="setting-row">
        <span><strong>Version</strong><small>API assembly version</small></span>
        <span>{{ info.version }}</span>
      </div>
      <div class="setting-row">
        <span><strong>Environment</strong><small>Host configuration</small></span>
        <span>{{ info.environment }}</span>
      </div>
      <div class="setting-row">
        <span><strong>Server time</strong><small>UTC</small></span>
        <span>{{ new Date(info.serverTimeUtc).toISOString().slice(0, 19).replace('T', ' ') }}</span>
      </div>
    </section>

    <section class="settings-section">
      <span class="folder-head">USERS ({{ users.length }})</span>

      <div v-for="user in users" :key="user.id" class="setting-row">
        <span class="avatar">{{ user.displayName.charAt(0).toUpperCase() }}</span>
        <span>
          <strong>
            {{ user.displayName }}
            <template v-if="user.isAdmin"> - admin</template>
            <template v-if="user.isDisabled"> - disabled</template>
          </strong>
          <small>
            {{ user.email }} · {{ user.workoutCount }} workouts
            <template v-if="user.lastWorkoutAt"> · last {{ formatDate(user.lastWorkoutAt) }}</template>
          </small>
        </span>

        <button
          class="btn btn-quiet"
          :disabled="user.id === session.profile?.userId"
          @click="toggleDisabled(user)"
        >
          <UserX :size="15" /> {{ user.isDisabled ? 'Enable' : 'Disable' }}
        </button>
      </div>
    </section>

    <p class="form-note">
      Admins cannot view passwords or private progress photos. Disabling an account revokes its
      sessions immediately without deleting any training history.
    </p>
  </div>
</template>
