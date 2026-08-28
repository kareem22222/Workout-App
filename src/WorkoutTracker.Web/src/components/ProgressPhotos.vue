<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref } from 'vue'
import { Camera, Trash2, Upload } from '@lucide/vue'
import { api } from '@/lib/api'
import { ApiError } from '@/lib/http'
import { useSessionStore } from '@/stores/session'
import { displayToKg, formatDate, formatWeight, todayIsoDate, weightUnitLabel } from '@/lib/format'
import type { PhotoPose, ProgressPhoto } from '@/lib/types'

/**
 * Private progress photos (spec US-102).
 *
 * Only metadata is listed. Image bytes are streamed from an authorized endpoint after the
 * server re-checks ownership, so photos are never reachable by URL alone.
 */
const session = useSessionStore()

const photos = ref<ProgressPhoto[]>([])
/** Object URLs for the fetched image bytes, keyed by photo id. */
const previews = ref<Record<string, string>>({})
const loading = ref(true)
const uploading = ref(false)
const error = ref<string | null>(null)

const file = ref<File | null>(null)
const takenOn = ref(todayIsoDate())
const pose = ref<PhotoPose>('Front')
const weight = ref<number | null>(null)
const notes = ref('')

const poses: PhotoPose[] = ['Front', 'Side', 'Back']

/** Matches the server-side cap so oversized files fail before upload. */
const maxBytes = 8 * 1024 * 1024

const unit = computed(() => weightUnitLabel(session.weightUnit))

async function load() {
  loading.value = true

  try {
    photos.value = await api.photos.list()
    await loadPreviews()
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to load your photos.'
  } finally {
    loading.value = false
  }
}

/** Fetches image bytes for any photo not already previewed. */
async function loadPreviews() {
  for (const photo of photos.value) {
    if (previews.value[photo.id]) continue

    try {
      previews.value[photo.id] = await api.photos.contentUrl(photo.id)
    } catch {
      // A single unreadable image must not break the gallery.
    }
  }
}

function revokeAll() {
  for (const url of Object.values(previews.value)) URL.revokeObjectURL(url)
  previews.value = {}
}

onMounted(load)
onBeforeUnmount(revokeAll)

function pick(event: Event) {
  const input = event.target as HTMLInputElement
  const selected = input.files?.[0] ?? null
  error.value = null

  if (selected && selected.size > maxBytes) {
    error.value = 'Images must be 8 MB or smaller.'
    file.value = null
    input.value = ''
    return
  }

  file.value = selected
}

async function upload() {
  if (!file.value) {
    error.value = 'Choose an image first.'
    return
  }

  uploading.value = true
  error.value = null

  try {
    await api.photos.upload(
      file.value,
      takenOn.value,
      pose.value,
      weight.value === null ? null : displayToKg(weight.value, session.weightUnit),
      notes.value,
    )

    file.value = null
    notes.value = ''
    weight.value = null
    await load()
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to upload that image.'
  } finally {
    uploading.value = false
  }
}

async function remove(id: string) {
  if (!window.confirm('Delete this photo? This cannot be undone.')) return

  try {
    await api.photos.remove(id)

    // Release the object URL for the removed image.
    const url = previews.value[id]
    if (url) {
      URL.revokeObjectURL(url)
      delete previews.value[id]
    }

    await load()
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to delete the photo.'
  }
}
</script>

<template>
  <section class="panel">
    <div class="panel-head">
      <div>
        <span class="eyebrow">PROGRESS PHOTOS</span>
        <h2>Private to you</h2>
      </div>
      <span class="stat-icon purple"><Camera :size="19" /></span>
    </div>

    <p v-if="error" class="form-error" role="alert">{{ error }}</p>

    <div class="filter-grid">
      <label class="field-label">Date<input v-model="takenOn" type="date" /></label>

      <label class="field-label">
        Pose
        <select v-model="pose">
          <option v-for="option in poses" :key="option" :value="option">{{ option }}</option>
        </select>
      </label>

      <label class="field-label">
        Weight ({{ unit }})
        <input v-model.number="weight" type="number" min="0" step="0.1" placeholder="-" />
      </label>

      <label class="field-label">
        Image
        <input type="file" accept="image/jpeg,image/png,image/webp" @change="pick" />
      </label>
    </div>

    <label class="field-label">
      Note
      <input v-model="notes" maxlength="1000" placeholder="Optional" />
    </label>

    <button class="btn btn-quiet btn-wide" :disabled="uploading || !file" @click="upload">
      <Upload :size="16" /> {{ uploading ? 'Uploading…' : 'Upload photo' }}
    </button>

    <small class="form-note">
      JPEG, PNG or WebP up to 8 MB. Photos are stored privately and are never shared or
      shown to administrators.
    </small>

    <p v-if="loading" class="small-empty">Loading photos…</p>

    <div v-else-if="photos.length > 0" class="photo-grid">
      <figure v-for="photo in photos" :key="photo.id" class="photo-card">
        <img
          v-if="previews[photo.id]"
          :src="previews[photo.id]"
          :alt="`${photo.pose} photo from ${photo.takenOn}`"
        />
        <span v-else class="photo-placeholder"><Camera :size="20" /></span>
        <figcaption>
          <span>
            <strong>{{ photo.pose }}</strong>
            <small>
              {{ formatDate(`${photo.takenOn}T12:00:00`, { month: 'short', day: 'numeric', year: 'numeric' }) }}
              <template v-if="photo.weightKg !== null">
                - {{ formatWeight(photo.weightKg, session.weightUnit, true) }}
              </template>
            </small>
          </span>
          <button class="icon-button danger-text" aria-label="Delete photo" @click="remove(photo.id)">
            <Trash2 :size="15" />
          </button>
        </figcaption>
      </figure>
    </div>

    <p v-else class="small-empty">No progress photos yet.</p>
  </section>
</template>
