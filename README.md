# Form Workout Tracker

A private, mobile-first workout logger built from the supplied product specification. The web app includes editable routines, custom exercises, active set logging, rest timing, workout summaries, history, progress measurements, calendar, themes, export, offline caching, and durable browser storage. The .NET API provides the PostgreSQL/Identity foundation for server deployment.

## Run locally

Requirements: .NET 10, Node 22+, npm, Docker.

For the complete browser experience, run this from the web project folder:

```powershell
cd "C:\Users\sakareem\Workout App\src\WorkoutTracker.Web"
npm.cmd install
npm.cmd run dev
```

To run the API too, open a second PowerShell window at the repository root:

```powershell
docker compose up -d postgres
dotnet ef database update --project src/WorkoutTracker.Infrastructure --startup-project src/WorkoutTracker.Api
dotnet run --project src/WorkoutTracker.Api
```

Open `http://localhost:5173`. The frontend ships with local demo data so the core interaction can be reviewed without starting PostgreSQL; the API is available at `http://localhost:5080` for real persistence integration.

## Production

Set `ConnectionStrings__Database` and `Jwt__Key` using the host's secret store. Serve the frontend over HTTPS, restrict CORS to its exact origin, and apply migrations during deployment.
