/**
 * Service worker for the Form workout tracker (spec Epic 29).
 *
 * Caching policy is deliberately conservative:
 *  - the app shell and build assets are cached so the UI opens offline;
 *  - only read-only reference data (muscles, equipment) is cached from the API;
 *  - anything user-owned or mutating is always fetched from the network, so private
 *    training data is never served from a stale cache and writes are never intercepted.
 *
 * Offline write durability is handled by the IndexedDB outbox in the app, not here.
 */

const VERSION = 'v2'
const SHELL_CACHE = `form-shell-${VERSION}`
const ASSET_CACHE = `form-assets-${VERSION}`
const REFERENCE_CACHE = `form-reference-${VERSION}`

/** Minimum shell needed to boot the SPA offline. */
const SHELL_URLS = ['/', '/index.html', '/manifest.webmanifest', '/icon.svg']

/** Read-only API paths that are safe to serve stale. */
const CACHEABLE_API = ['/api/reference/muscles', '/api/reference/equipment']

self.addEventListener('install', (event) => {
  event.waitUntil(
    caches
      .open(SHELL_CACHE)
      // Individual failures must not abort installation.
      .then((cache) => Promise.allSettled(SHELL_URLS.map((url) => cache.add(new Request(url, { cache: 'reload' })))))
      .then(() => self.skipWaiting()),
  )
})

self.addEventListener('activate', (event) => {
  const keep = new Set([SHELL_CACHE, ASSET_CACHE, REFERENCE_CACHE])

  event.waitUntil(
    caches
      .keys()
      .then((keys) => Promise.all(keys.filter((key) => !keep.has(key)).map((key) => caches.delete(key))))
      .then(() => self.clients.claim()),
  )
})

self.addEventListener('fetch', (event) => {
  const request = event.request

  // Never interfere with writes; a queued mutation must reach the server verbatim.
  if (request.method !== 'GET') return

  const url = new URL(request.url)
  if (url.origin !== self.location.origin) return

  if (url.pathname.startsWith('/api/')) {
    if (CACHEABLE_API.includes(url.pathname)) {
      event.respondWith(networkFirst(request, REFERENCE_CACHE))
    }
    // All other API reads intentionally fall through to the network.
    return
  }

  // Navigations resolve to the SPA shell so deep links work offline.
  if (request.mode === 'navigate') {
    event.respondWith(
      fetch(request).catch(async () => (await caches.match('/index.html')) ?? Response.error()),
    )
    return
  }

  // Build assets are content-hashed, so a cache hit is always correct.
  event.respondWith(cacheFirst(request, ASSET_CACHE))
})

/** Serves from cache when present, otherwise fetches and stores. */
async function cacheFirst(request, cacheName) {
  const cached = await caches.match(request)
  if (cached) return cached

  try {
    const response = await fetch(request)
    if (response.ok && response.type === 'basic') {
      const cache = await caches.open(cacheName)
      cache.put(request, response.clone())
    }
    return response
  } catch {
    return (await caches.match('/index.html')) ?? Response.error()
  }
}

/** Prefers fresh data but falls back to the cached copy when offline. */
async function networkFirst(request, cacheName) {
  try {
    const response = await fetch(request)

    if (response.ok) {
      const cache = await caches.open(cacheName)
      cache.put(request, response.clone())
    }

    return response
  } catch {
    const cached = await caches.match(request)
    if (cached) return cached

    return new Response(JSON.stringify({ title: 'Offline', status: 503 }), {
      status: 503,
      headers: { 'Content-Type': 'application/json' },
    })
  }
}
