# Form - Workout Tracker

A private, mobile-first workout logging PWA built from the supplied Product & Engineering
Specification. Routines, active-workout logging with previous values, rest timing,
automatic personal records, progress charts, body tracking, data export/import and offline
support are all backed by a real API — the backend is the source of truth for every
business rule and authorization decision.

## Architecture

| Project | Responsibility |
| --- | --- |
| `src/WorkoutTracker.Domain` | Entities, enums and pure domain rules (1RM formulas, volume, PR detection, plate/warmup/overload calculators). No infrastructure dependencies. |
| `src/WorkoutTracker.Application` | Use-case services, DTOs, validation and the `IAppDbContext` / `IUserDirectory` / `IMediaStorage` abstractions. |
| `src/WorkoutTracker.Infrastructure` | EF Core 10 + PostgreSQL, ASP.NET Identity, seed data, local media storage. |
| `src/WorkoutTracker.Api` | ASP.NET Core 10 minimal APIs, JWT + rotating refresh tokens, OpenAPI, Serilog, health checks. |
| `src/WorkoutTracker.Web` | Vue 3 + TypeScript + Vite + Pinia + Chart.js PWA. |

Weights are persisted in canonical kilograms and lengths in centimeters; units are
converted only for display, so stored history stays comparable when a user switches
between kg and lb.

## Run locally

Requirements: .NET 10 SDK, Node 22+, Docker (for PostgreSQL).

**1. Start the database**

```powershell
docker compose up -d postgres
```

**2. Apply migrations**

```powershell
dotnet ef database update --project src/WorkoutTracker.Infrastructure --startup-project src/WorkoutTracker.Api
```

This creates the schema and seeds the muscle and equipment taxonomies plus a curated
library of built-in exercises. Seed identifiers are deterministic, so re-running
migrations never creates duplicates.

**3. Run the API**

```powershell
dotnet run --project src/WorkoutTracker.Api
```

The API listens on `http://localhost:5080`. OpenAPI is exposed at `/openapi/v1.json` in
Development, and `/health` reports database connectivity.

**4. Run the web app**

```powershell
cd src/WorkoutTracker.Web
npm install
npm run dev
```

Open `http://localhost:5173`. Vite proxies `/api` to the API, so no CORS configuration is
needed for local development.

**5. Create your account**

Register from the login screen. The first account to register is automatically granted the
`Admin` role, which unlocks the admin screen at `/admin`.

## Deploy to a single host

`docker-compose.prod.yml` runs the whole stack — Postgres, the API, and Caddy serving the
built PWA — on one machine. Caddy terminates TLS and reverse proxies `/api` to the API, so
the app and its API share one origin and CORS is never involved.

```bash
cp deploy/.env.prod.example .env.prod
# fill in POSTGRES_PASSWORD, JWT_KEY, WORKOUT_DOMAIN, ACME_EMAIL
docker compose -f docker-compose.prod.yml --env-file .env.prod up -d --build
```

Point an A record at the host before starting so Caddy can provision a certificate. To
check things over plain HTTP by IP first, set `WORKOUT_DOMAIN=:80`. Register the first
account immediately — it receives the `Admin` role — then set `ALLOW_REGISTRATION=false`
and re-run the command to close sign-up.

Only Caddy publishes ports. Postgres and the API are reachable on the internal Docker
network alone, which is what makes `Proxy__TrustForwardedHeaders` safe to enable: the proxy
is the sole possible source of requests, so `X-Forwarded-For` cannot be spoofed by a
client. That header is what lets the auth rate limiter partition by real client IP instead
of lumping every user into the proxy's single address.

Two settings are off by default and switched on only in the compose file:

| Setting | Effect |
| --- | --- |
| `Database:MigrateOnStartup` | Applies pending EF migrations at boot, retrying while the database wakes. Assumes a single API instance, since EF does not lock across processes. |
| `Proxy:TrustForwardedHeaders` | Honours `X-Forwarded-For`/`-Proto`, and hands HTTPS redirection and HSTS to the proxy. |

Three named volumes hold state: `postgres-data`, `media-data` (progress photos, which are
files on disk rather than rows) and `caddy-data` (issued certificates). Back up the first
two; losing `media-data` leaves photo metadata pointing at absent files.

For free hosting, an Oracle Cloud Always Free ARM VM fits this compose file directly and
has no cold starts or disk resets. The alternative — a static host plus a free container
tier plus managed Postgres — has no persistent disk, so progress photos would not survive
a redeploy unless `IMediaStorage` is pointed at object storage first.

## Configuration

Secrets are never committed. `src/WorkoutTracker.Api/appsettings.json` ships with an empty
`Jwt:Key`, and the API refuses to start unless a key of at least 32 characters is supplied.
For convenience, `appsettings.Development.json` contains a clearly labelled
development-only key that signs tokens for your local database.

For production, supply configuration through the host's secret store or environment
variables. See `.env.example` for the full list:

| Setting | Purpose |
| --- | --- |
| `ConnectionStrings__Database` | PostgreSQL connection string. |
| `Jwt__Key` | Signing key, minimum 32 characters. Generate with `openssl rand -base64 48`. |
| `Cors__AllowedOrigins__0` | Exact frontend origin. Wildcards are rejected because the refresh cookie requires credentials. |
| `Auth__AllowRegistration` | Set to `false` to make the deployment invite-only. |
| `Storage__MediaRoot` | Directory for private progress photos. Use a mounted volume. |

## Security notes

- Access tokens are short-lived and held only in memory by the client. The refresh token is
  a rotating, hashed, HttpOnly cookie scoped to `/api/auth`.
- Reusing an already-rotated refresh token revokes the entire token family, on the
  assumption that the chain has been stolen.
- Every request re-checks the account's status and Identity security stamp, so disabling a
  user or changing a password invalidates tokens that have not yet expired.
- All user-owned queries are filtered by the authenticated user id. A client-supplied owner
  id is never trusted, and another user's resource returns 404 rather than revealing that it
  exists.
- Progress photos are stored under non-public keys and streamed through an authorized
  endpoint after an ownership re-check.
- Authentication endpoints are rate limited and return generic credential errors.

## Offline behaviour

The service worker caches the app shell and read-only reference data only; user-owned data
is always fetched from the network so nothing private is served stale. The active workout
and any pending mutations are mirrored into IndexedDB. Set ids are generated client-side,
which makes replay idempotent, and each session carries a version so a stale offline write
is rejected with a conflict instead of overwriting newer server data.

## Backups

`GET /api/export/json` produces a complete, schema-versioned export of a single user's
data, and CSV exports are available per dataset. These are user conveniences, not a backup
strategy: configure automated PostgreSQL backups (for example Neon's snapshots or a
scheduled `pg_dump`) before relying on the app, and verify a restore at least once.

## Known follow-up work

- Automated tests were intentionally skipped for this pass. The spec's testing strategy
  (domain unit tests, cross-user authorization integration tests, and an E2E smoke test of
  register → routine → workout → history) remains outstanding. CI currently builds and
  type-checks only.
- Friends/sharing (Epics 32-33) is deliberately unimplemented; everything is private, which
  is the spec's stated default until an explicit privacy model exists.
- The muscle summary is the ranked textual breakdown the spec asks for; a graphical body
  heatmap is not drawn.
- Media is stored on the local filesystem via `IMediaStorage`. Swap in an object-storage
  implementation before running more than one API instance.
