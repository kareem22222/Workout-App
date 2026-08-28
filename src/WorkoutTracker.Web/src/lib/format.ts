/**
 * Display helpers.
 *
 * The API stores canonical kilograms and centimeters. Conversion happens only at the
 * presentation boundary, which keeps stored data comparable regardless of the unit a user
 * happened to prefer at the time (spec 3.1).
 */

import type { LengthUnit, PersonalRecordType, WeightUnit, WorkoutSetType } from './types'

const KG_PER_LB = 0.45359237
const CM_PER_INCH = 2.54

export function kgToDisplay(kg: number, unit: WeightUnit): number {
  const value = unit === 'Pounds' ? kg / KG_PER_LB : kg
  return Math.round(value * 100) / 100
}

export function displayToKg(value: number, unit: WeightUnit): number {
  const kg = unit === 'Pounds' ? value * KG_PER_LB : value
  return Math.round(kg * 10000) / 10000
}

export function cmToDisplay(cm: number, unit: LengthUnit): number {
  const value = unit === 'Inches' ? cm / CM_PER_INCH : cm
  return Math.round(value * 100) / 100
}

export function displayToCm(value: number, unit: LengthUnit): number {
  const cm = unit === 'Inches' ? value * CM_PER_INCH : value
  return Math.round(cm * 10000) / 10000
}

export function weightUnitLabel(unit: WeightUnit): string {
  return unit === 'Pounds' ? 'lb' : 'kg'
}

export function lengthUnitLabel(unit: LengthUnit): string {
  return unit === 'Inches' ? 'in' : 'cm'
}

/** Formats a weight for display, trimming trailing zeros so 82.5 stays readable. */
export function formatWeight(kg: number, unit: WeightUnit, withUnit = false): string {
  const value = kgToDisplay(kg, unit)
  const text = Number.isInteger(value) ? String(value) : String(Math.round(value * 100) / 100)
  return withUnit ? `${text} ${weightUnitLabel(unit)}` : text
}

/** Compact volume label, e.g. 12.4k, since session volumes get large quickly. */
export function formatVolume(kg: number, unit: WeightUnit): string {
  const value = kgToDisplay(kg, unit)
  if (value >= 1000) return `${Math.round(value / 100) / 10}k`
  return String(Math.round(value))
}

/** Formats a second count as mm:ss, or h:mm:ss past an hour. */
export function formatDuration(totalSeconds: number): string {
  const seconds = Math.max(0, Math.floor(totalSeconds))
  const hours = Math.floor(seconds / 3600)
  const minutes = Math.floor((seconds % 3600) / 60)
  const remainder = seconds % 60

  const pad = (value: number) => String(value).padStart(2, '0')
  return hours > 0 ? `${hours}:${pad(minutes)}:${pad(remainder)}` : `${pad(minutes)}:${pad(remainder)}`
}

/** Rounded minutes, used where a coarse training-time figure is enough. */
export function formatMinutes(totalSeconds: number): string {
  const minutes = Math.round(totalSeconds / 60)
  if (minutes < 60) return `${minutes}m`
  return `${Math.floor(minutes / 60)}h ${minutes % 60}m`
}

export function formatDate(iso: string, options: Intl.DateTimeFormatOptions = { month: 'short', day: 'numeric' }): string {
  return new Intl.DateTimeFormat(undefined, options).format(new Date(iso))
}

export function formatDateTime(iso: string): string {
  return new Intl.DateTimeFormat(undefined, {
    month: 'short',
    day: 'numeric',
    hour: 'numeric',
    minute: '2-digit',
  }).format(new Date(iso))
}

/** Today's date as yyyy-MM-dd in the user's local timezone, for date inputs. */
export function todayIsoDate(): string {
  const now = new Date()
  const offsetCorrected = new Date(now.getTime() - now.getTimezoneOffset() * 60000)
  return offsetCorrected.toISOString().slice(0, 10)
}

/** Short label for a set type badge. Normal sets show their position instead. */
export function setTypeLabel(type: WorkoutSetType): string {
  switch (type) {
    case 'Warmup':
      return 'W'
    case 'DropSet':
      return 'D'
    case 'Failure':
      return 'F'
    case 'Amrap':
      return 'A'
    case 'Backoff':
      return 'B'
    default:
      return ''
  }
}

export function setTypeName(type: WorkoutSetType): string {
  switch (type) {
    case 'DropSet':
      return 'Drop set'
    case 'Amrap':
      return 'AMRAP'
    default:
      return type
  }
}

/** Order used when cycling a set through the available types. */
export const setTypeCycle: WorkoutSetType[] = ['Normal', 'Warmup', 'DropSet', 'Failure', 'Amrap', 'Backoff']

/** Human label plus formatted value for a personal record. */
export function describeRecord(
  type: PersonalRecordType,
  value: number,
  atWeight: number | null,
  unit: WeightUnit,
): { label: string; value: string } {
  switch (type) {
    case 'HeaviestWeight':
      return { label: 'Heaviest weight', value: formatWeight(value, unit, true) }
    case 'MostRepsAtWeight':
      return {
        label: `Most reps at ${atWeight === null ? '' : formatWeight(atWeight, unit, true)}`.trim(),
        value: `${value} reps`,
      }
    case 'BestEstimatedOneRepMax':
      return { label: 'Best estimated 1RM', value: formatWeight(value, unit, true) }
    case 'BestSetVolume':
      return { label: 'Best set volume', value: `${formatVolume(value, unit)} ${weightUnitLabel(unit)}` }
    case 'BestWorkoutVolume':
      return { label: 'Best workout volume', value: `${formatVolume(value, unit)} ${weightUnitLabel(unit)}` }
    default:
      return { label: 'Record', value: String(value) }
  }
}

/** Day names indexed to match .NET's DayOfWeek, where 0 is Sunday. */
export const dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday']

/**
 * Normalizes a day of week that may arrive as a name or a number, since enum
 * serialization differs between payloads.
 */
export function dayIndex(value: string | number | null): number | null {
  if (value === null) return null
  if (typeof value === 'number') return value
  const index = dayNames.indexOf(value)
  return index >= 0 ? index : null
}
