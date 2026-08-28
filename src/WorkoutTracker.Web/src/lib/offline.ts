/**
 * Offline support for the active workout (spec Epic 30).
 *
 * Two concerns live here:
 *  - a cached copy of the active session so a refresh or connectivity loss never
 *    discards what the user just entered;
 *  - an append-only outbox of pending mutations, replayed in insertion order once the
 *    connection returns.
 *
 * Set ids are generated client-side, so a replayed mutation targets the same rows and
 * cannot create duplicates.
 */

import type { UpdateWorkoutRequest, WorkoutSession } from './types'

const DB_NAME = 'workouttracker'
const DB_VERSION = 1
const CACHE_STORE = 'activeWorkout'
const OUTBOX_STORE = 'outbox'

/** A queued mutation awaiting replay. */
export interface OutboxEntry {
  /** Monotonic key assigned by IndexedDB, which defines replay order. */
  id?: number
  workoutId: string
  request: UpdateWorkoutRequest
  queuedAt: number
}

let dbPromise: Promise<IDBDatabase | null> | null = null

/** Opens the database, resolving to null when IndexedDB is unavailable (e.g. private mode). */
function openDatabase(): Promise<IDBDatabase | null> {
  dbPromise ??= new Promise((resolve) => {
    if (typeof indexedDB === 'undefined') {
      resolve(null)
      return
    }

    const request = indexedDB.open(DB_NAME, DB_VERSION)

    request.onupgradeneeded = () => {
      const db = request.result
      if (!db.objectStoreNames.contains(CACHE_STORE)) db.createObjectStore(CACHE_STORE)
      if (!db.objectStoreNames.contains(OUTBOX_STORE)) {
        db.createObjectStore(OUTBOX_STORE, { keyPath: 'id', autoIncrement: true })
      }
    }

    request.onsuccess = () => resolve(request.result)
    request.onerror = () => resolve(null)
  })

  return dbPromise
}

/** Runs a store operation, swallowing storage failures so the UI keeps working. */
async function withStore<T>(
  storeName: string,
  mode: IDBTransactionMode,
  operation: (store: IDBObjectStore) => IDBRequest,
): Promise<T | null> {
  const db = await openDatabase()
  if (!db) return null

  return new Promise((resolve) => {
    try {
      const transaction = db.transaction(storeName, mode)
      const request = operation(transaction.objectStore(storeName))

      request.onsuccess = () => resolve(request.result as T)
      request.onerror = () => resolve(null)
    } catch {
      resolve(null)
    }
  })
}

// ---------------------------------------------------------------------------------------
// Active workout cache
// ---------------------------------------------------------------------------------------

export async function cacheActiveWorkout(session: WorkoutSession | null): Promise<void> {
  if (session === null) {
    await withStore(CACHE_STORE, 'readwrite', (store) => store.delete('current'))
    return
  }

  await withStore(CACHE_STORE, 'readwrite', (store) => store.put(session, 'current'))
}

export async function readCachedWorkout(): Promise<WorkoutSession | null> {
  return (await withStore<WorkoutSession>(CACHE_STORE, 'readonly', (store) => store.get('current'))) ?? null
}

// ---------------------------------------------------------------------------------------
// Outbox
// ---------------------------------------------------------------------------------------

/**
 * Queues a mutation for later replay.
 * Only the newest state per workout is useful because updates are full replacements, so
 * earlier queued entries for the same workout are dropped to keep replay cheap.
 */
export async function enqueueMutation(workoutId: string, request: UpdateWorkoutRequest): Promise<void> {
  const pending = await readOutbox()
  const superseded = pending.filter((entry) => entry.workoutId === workoutId && entry.id !== undefined)

  for (const entry of superseded) {
    await withStore(OUTBOX_STORE, 'readwrite', (store) => store.delete(entry.id!))
  }

  const entry: OutboxEntry = { workoutId, request, queuedAt: Date.now() }
  await withStore(OUTBOX_STORE, 'readwrite', (store) => store.add(entry))
}

export async function readOutbox(): Promise<OutboxEntry[]> {
  const entries = await withStore<OutboxEntry[]>(OUTBOX_STORE, 'readonly', (store) => store.getAll())
  return (entries ?? []).sort((a, b) => (a.id ?? 0) - (b.id ?? 0))
}

export async function removeFromOutbox(id: number): Promise<void> {
  await withStore(OUTBOX_STORE, 'readwrite', (store) => store.delete(id))
}

export async function clearOutbox(): Promise<void> {
  await withStore(OUTBOX_STORE, 'readwrite', (store) => store.clear())
}

export async function outboxSize(): Promise<number> {
  return (await withStore<number>(OUTBOX_STORE, 'readonly', (store) => store.count())) ?? 0
}
