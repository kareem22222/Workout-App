/**
 * Typed HTTP client for the WorkoutTracker API.
 *
 * The access token is held in memory only. The long-lived refresh token lives in an
 * HttpOnly cookie the browser manages, so a successful refresh works without any
 * JavaScript-readable credential (spec 3).
 */

const BASE_URL = '/api'

/** Normalized API failure, thrown by every request helper. */
export class ApiError extends Error {
  readonly status: number
  readonly validationErrors: Record<string, string[]>
  /** Payload attached to a 409, e.g. the current server state of a workout. */
  readonly conflict: unknown

  constructor(status: number, message: string, validationErrors: Record<string, string[]> = {}, conflict: unknown = null) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.validationErrors = validationErrors
    this.conflict = conflict
  }

  /** True when the request failed because the browser is offline or the API is unreachable. */
  get isNetworkError() {
    return this.status === 0
  }

  get isUnauthorized() {
    return this.status === 401
  }

  get isConflict() {
    return this.status === 409
  }

  /** First field-level message, if the server returned one. */
  get firstValidationMessage(): string | null {
    const first = Object.values(this.validationErrors)[0]
    return first?.[0] ?? null
  }
}

let accessToken: string | null = null

/** Invoked when the session cannot be recovered, so the app can route to login. */
let onSessionExpired: (() => void) | null = null

/** De-duplicates concurrent refresh attempts into a single in-flight request. */
let refreshInFlight: Promise<boolean> | null = null

export function setAccessToken(token: string | null) {
  accessToken = token
}

export function getAccessToken() {
  return accessToken
}

export function setSessionExpiredHandler(handler: () => void) {
  onSessionExpired = handler
}

/**
 * Exchanges the refresh cookie for a new access token.
 * Concurrent callers share one request so a burst of 401s cannot trigger a refresh storm.
 */
export function refreshSession(): Promise<boolean> {
  refreshInFlight ??= (async () => {
    try {
      const response = await fetch(`${BASE_URL}/auth/refresh`, {
        method: 'POST',
        credentials: 'include',
      })

      if (!response.ok) return false

      const payload = (await response.json()) as { accessToken: string }
      accessToken = payload.accessToken
      return true
    } catch {
      return false
    } finally {
      // Cleared in a microtask so callers awaiting this promise all observe the result.
      queueMicrotask(() => {
        refreshInFlight = null
      })
    }
  })()

  return refreshInFlight
}

interface RequestOptions {
  method?: string
  body?: unknown
  /** Sent as multipart/form-data instead of JSON. */
  form?: FormData
  query?: Record<string, string | number | boolean | null | undefined>
  /** Set false for endpoints that must not attempt a token refresh, e.g. login. */
  retryOnUnauthorized?: boolean
  signal?: AbortSignal
}

function buildUrl(path: string, query?: RequestOptions['query']) {
  const url = `${BASE_URL}${path}`
  if (!query) return url

  const params = new URLSearchParams()
  for (const [key, value] of Object.entries(query)) {
    if (value === null || value === undefined || value === '') continue
    params.append(key, String(value))
  }

  const serialized = params.toString()
  return serialized ? `${url}?${serialized}` : url
}

/** Converts a non-2xx response into an ApiError, tolerating non-JSON bodies. */
async function toApiError(response: Response): Promise<ApiError> {
  let message = response.statusText || 'Request failed.'
  let validationErrors: Record<string, string[]> = {}
  let conflict: unknown = null

  try {
    const payload = await response.json()

    if (payload && typeof payload === 'object') {
      const body = payload as Record<string, unknown>
      if (typeof body.detail === 'string' && body.detail) message = body.detail
      else if (typeof body.title === 'string' && body.title) message = body.title

      if (body.errors && typeof body.errors === 'object') {
        validationErrors = body.errors as Record<string, string[]>
        // Surface the first field error, which is more actionable than a generic title.
        const first = Object.values(validationErrors)[0]
        if (first?.[0]) message = first[0]
      }

      if ('current' in body) conflict = body.current
    }
  } catch {
    // Non-JSON error body; the status-derived message is the best available.
  }

  if (response.status === 401 && message === 'Unauthorized') {
    message = 'Your session has expired. Please sign in again.'
  }

  return new ApiError(response.status, message, validationErrors, conflict)
}

async function send<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const { method = 'GET', body, form, query, retryOnUnauthorized = true, signal } = options

  const execute = async (): Promise<Response> => {
    const headers: Record<string, string> = {}
    if (accessToken) headers.Authorization = `Bearer ${accessToken}`
    if (body !== undefined) headers['Content-Type'] = 'application/json'

    return fetch(buildUrl(path, query), {
      method,
      headers,
      // Required so the refresh cookie is sent to the auth routes.
      credentials: 'include',
      body: form ?? (body !== undefined ? JSON.stringify(body) : undefined),
      signal,
    })
  }

  let response: Response
  try {
    response = await execute()
  } catch (error) {
    if (error instanceof DOMException && error.name === 'AbortError') throw error
    throw new ApiError(0, 'You appear to be offline. Changes will sync when the connection returns.')
  }

  // A short-lived access token expiring mid-session is expected, so refresh once and retry.
  if (response.status === 401 && retryOnUnauthorized) {
    const refreshed = await refreshSession()

    if (!refreshed) {
      accessToken = null
      onSessionExpired?.()
      throw await toApiError(response)
    }

    try {
      response = await execute()
    } catch {
      throw new ApiError(0, 'You appear to be offline. Changes will sync when the connection returns.')
    }
  }

  if (!response.ok) throw await toApiError(response)

  if (response.status === 204) return undefined as T

  const text = await response.text()
  return (text ? JSON.parse(text) : undefined) as T
}

/** Downloads a file, returning the blob and the server-provided filename. */
async function download(path: string, query?: RequestOptions['query']): Promise<{ blob: Blob; fileName: string }> {
  const headers: Record<string, string> = {}
  if (accessToken) headers.Authorization = `Bearer ${accessToken}`

  let response = await fetch(buildUrl(path, query), { headers, credentials: 'include' })

  if (response.status === 401 && (await refreshSession())) {
    headers.Authorization = `Bearer ${accessToken}`
    response = await fetch(buildUrl(path, query), { headers, credentials: 'include' })
  }

  if (!response.ok) throw await toApiError(response)

  const disposition = response.headers.get('Content-Disposition') ?? ''
  const match = /filename\*?=(?:UTF-8'')?"?([^";]+)"?/i.exec(disposition)

  return { blob: await response.blob(), fileName: match?.[1] ?? 'export' }
}

export const http = {
  get: <T>(path: string, query?: RequestOptions['query'], signal?: AbortSignal) =>
    send<T>(path, { query, signal }),
  post: <T>(path: string, body?: unknown, options?: Omit<RequestOptions, 'method' | 'body'>) =>
    send<T>(path, { ...options, method: 'POST', body }),
  put: <T>(path: string, body?: unknown, options?: Omit<RequestOptions, 'method' | 'body'>) =>
    send<T>(path, { ...options, method: 'PUT', body }),
  delete: <T>(path: string, options?: Omit<RequestOptions, 'method'>) =>
    send<T>(path, { ...options, method: 'DELETE' }),
  postForm: <T>(path: string, form: FormData) => send<T>(path, { method: 'POST', form }),
  download,
}
