<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Dumbbell } from '@lucide/vue'
import { useSessionStore } from '@/stores/session'

const session = useSessionStore()
const router = useRouter()
const route = useRoute()

const mode = ref<'login' | 'register'>('login')
const displayName = ref('')
const email = ref('')
const password = ref('')
const localError = ref('')

const isRegister = computed(() => mode.value === 'register')
const title = computed(() => (isRegister.value ? 'Start training clearly.' : 'Ready when you are.'))
const error = computed(() => localError.value || session.error)

async function submit() {
  localError.value = ''

  if (!email.value.includes('@')) {
    localError.value = 'Enter a valid email address.'
    return
  }

  if (password.value.length < 8) {
    localError.value = 'Use at least 8 characters.'
    return
  }

  if (isRegister.value && displayName.value.trim().length === 0) {
    localError.value = 'Enter a display name.'
    return
  }

  const success = isRegister.value
    ? await session.register(displayName.value.trim(), email.value.trim(), password.value)
    : await session.login(email.value.trim(), password.value)

  if (!success) return

  const redirect = typeof route.query.redirect === 'string' ? route.query.redirect : '/'
  await router.replace(redirect)
}

function switchMode() {
  mode.value = isRegister.value ? 'login' : 'register'
  localError.value = ''
  session.error = null
}
</script>

<template>
  <main class="auth-page">
    <section class="auth-copy">
      <div class="brand">
        <span class="brand-mark"><Dumbbell :size="20" /></span>
        <span>FORM</span>
      </div>
      <div>
        <span class="eyebrow">YOUR TRAINING, CLEARLY</span>
        <h1>Think less.<br /><em>Lift more.</em></h1>
        <p>A focused workout log built to disappear between you and your next set.</p>
      </div>
      <div class="auth-proof">
        <strong>Fast by design.</strong>
        <span>Routines, history and progress - without the noise.</span>
      </div>
    </section>

    <section class="auth-panel">
      <form class="auth-card" @submit.prevent="submit">
        <span class="eyebrow">{{ isRegister ? 'CREATE ACCOUNT' : 'WELCOME BACK' }}</span>
        <h2>{{ title }}</h2>
        <p>{{ isRegister ? 'A private training space in under a minute.' : 'Your training data is ready.' }}</p>

        <p v-if="error" class="form-error" role="alert">{{ error }}</p>

        <label v-if="isRegister">
          Display name
          <input v-model="displayName" autocomplete="name" required maxlength="60" />
        </label>

        <label>
          Email
          <input v-model="email" type="email" autocomplete="email" required />
        </label>

        <label>
          Password
          <input
            v-model="password"
            type="password"
            :autocomplete="isRegister ? 'new-password' : 'current-password'"
            required
            minlength="8"
          />
        </label>

        <button class="btn btn-primary btn-wide" type="submit" :disabled="session.authenticating">
          {{ session.authenticating ? 'Please wait…' : isRegister ? 'Create account' : 'Sign in' }}
        </button>

        <button class="auth-switch" type="button" @click="switchMode">
          {{ isRegister ? 'Already have an account? Sign in' : 'New here? Create an account' }}
        </button>

        <small class="form-note">Private by default - Your data stays yours</small>
      </form>
    </section>
  </main>
</template>
