<script setup lang="ts">
import { reactive, ref } from 'vue'
import { Trash2 } from '@lucide/vue'
import { api } from '@/lib/api'
import { ApiError } from '@/lib/http'
import { useSessionStore } from '@/stores/session'
import { cmToDisplay, displayToCm, displayToKg, formatDate, formatWeight, lengthUnitLabel, todayIsoDate, weightUnitLabel } from '@/lib/format'
import type { BodyMeasurement } from '@/lib/types'

const props = defineProps<{ measurements: BodyMeasurement[] }>()
const emit = defineEmits<{ updated: [measurements: BodyMeasurement[]] }>()
const session = useSessionStore()
const open = ref(false)
const saving = ref(false)
const error = ref<string | null>(null)
const form = reactive({
  measuredOn: todayIsoDate(), weight: null as number | null, bodyFatPercent: null as number | null,
  chest: null as number | null, waist: null as number | null, hips: null as number | null,
  leftArm: null as number | null, rightArm: null as number | null,
  leftThigh: null as number | null, rightThigh: null as number | null,
  leftCalf: null as number | null, rightCalf: null as number | null,
  shoulders: null as number | null, neck: null as number | null, notes: '',
})

const lengthFields = [
  ['chest', 'Chest'], ['waist', 'Waist'], ['hips', 'Hips'], ['shoulders', 'Shoulders'], ['neck', 'Neck'],
  ['leftArm', 'Left arm'], ['rightArm', 'Right arm'], ['leftThigh', 'Left thigh'], ['rightThigh', 'Right thigh'],
  ['leftCalf', 'Left calf'], ['rightCalf', 'Right calf'],
] as const

const toCm = (value: number | null) => value === null ? null : displayToCm(value, session.settings.lengthUnit)

async function save() {
  saving.value = true
  error.value = null
  try {
    await api.measurements.save({
      measuredOn: form.measuredOn,
      weightKg: form.weight === null ? null : displayToKg(form.weight, session.weightUnit),
      bodyFatPercent: form.bodyFatPercent,
      chestCm: toCm(form.chest), waistCm: toCm(form.waist), hipsCm: toCm(form.hips),
      leftArmCm: toCm(form.leftArm), rightArmCm: toCm(form.rightArm),
      leftThighCm: toCm(form.leftThigh), rightThighCm: toCm(form.rightThigh),
      leftCalfCm: toCm(form.leftCalf), rightCalfCm: toCm(form.rightCalf),
      shouldersCm: toCm(form.shoulders), neckCm: toCm(form.neck), notes: form.notes,
    })
    emit('updated', await api.measurements.list())
    open.value = false
    Object.assign(form, { measuredOn: todayIsoDate(), weight: null, bodyFatPercent: null, chest: null, waist: null, hips: null, leftArm: null, rightArm: null, leftThigh: null, rightThigh: null, leftCalf: null, rightCalf: null, shoulders: null, neck: null, notes: '' })
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to save the measurement.'
  } finally {
    saving.value = false
  }
}

async function remove(id: string) {
  if (!window.confirm('Delete this measurement?')) return
  try {
    await api.measurements.remove(id)
    emit('updated', await api.measurements.list())
  } catch (exception) {
    error.value = exception instanceof ApiError ? exception.message : 'Unable to delete the measurement.'
  }
}
</script>

<template>
  <section class="panel">
    <div class="panel-head">
      <div><span class="eyebrow">BODY MEASUREMENTS</span><h2>Track changes</h2></div>
      <button class="btn btn-primary" @click="open = !open">{{ open ? 'Close' : 'Log measurement' }}</button>
    </div>
    <p v-if="error" class="form-error" role="alert">{{ error }}</p>

    <form v-if="open" class="measurement-form" @submit.prevent="save">
      <div class="field-pair">
        <label class="field-label">Date<input v-model="form.measuredOn" type="date" required /></label>
        <label class="field-label">Weight ({{ weightUnitLabel(session.weightUnit) }})<input v-model.number="form.weight" type="number" min="0" step="0.1" placeholder="-" /></label>
      </div>
      <div class="field-pair">
        <label class="field-label">Body fat %<input v-model.number="form.bodyFatPercent" type="number" min="0" max="70" step="0.1" placeholder="-" /></label>
        <label class="field-label">Note<input v-model="form.notes" maxlength="1000" placeholder="Optional" /></label>
      </div>
      <div class="measurement-grid">
        <label v-for="field in lengthFields" :key="field[0]" class="field-label">
          {{ field[1] }} ({{ lengthUnitLabel(session.settings.lengthUnit) }})
          <input v-model.number="form[field[0]]" type="number" min="0" step="0.1" placeholder="-" />
        </label>
      </div>
      <small class="form-note">Every field except the date is optional. Values are stored canonically in kg/cm.</small>
      <button class="btn btn-primary btn-wide" type="submit" :disabled="saving">{{ saving ? 'Saving…' : 'Save measurement' }}</button>
    </form>

    <div v-for="entry in props.measurements.slice(0, 12)" :key="entry.id" class="activity-row">
      <span class="date-tile"><strong>{{ new Date(`${entry.measuredOn}T12:00:00`).getDate() }}</strong><small>{{ formatDate(`${entry.measuredOn}T12:00:00`, { month: 'short' }) }}</small></span>
      <span>
        <strong>{{ entry.weightKg === null ? 'Measurements' : formatWeight(entry.weightKg, session.weightUnit, true) }}</strong>
        <small>
          <template v-if="entry.bodyFatPercent !== null">{{ entry.bodyFatPercent }}% body fat</template>
          <template v-if="entry.waistCm !== null"> · waist {{ cmToDisplay(entry.waistCm, session.settings.lengthUnit) }} {{ lengthUnitLabel(session.settings.lengthUnit) }}</template>
        </small>
      </span>
      <button class="icon-button danger-text" aria-label="Delete measurement" @click="remove(entry.id)"><Trash2 :size="16" /></button>
    </div>
    <div v-if="props.measurements.length === 0 && !open" class="inline-empty">
      <strong>No measurements yet</strong><span>Track your weight or measurements to see progress here.</span>
    </div>
  </section>
</template>
