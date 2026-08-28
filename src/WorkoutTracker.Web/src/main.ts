import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'
import { useSessionStore } from './stores/session'
import './style.css'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)

const session = useSessionStore(pinia)

// Attempt to restore the session from the refresh cookie before the first navigation, so
// a reload lands on the requested page instead of bouncing through the login screen.
session.initialize().finally(() => {
  app.use(router)
  app.mount('#app')
})

if (import.meta.env.PROD && 'serviceWorker' in navigator) {
  window.addEventListener('load', () => {
    navigator.serviceWorker.register('/sw.js').catch(() => {
      // A failed registration only costs offline support; the app still works online.
    })
  })
}
