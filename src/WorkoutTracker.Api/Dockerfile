# Build context is the REPOSITORY ROOT, not this directory, because the API csproj
# references its sibling projects:
#   docker build -f src/WorkoutTracker.Api/Dockerfile -t workouttracker-api .
#
# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Copy only the project files first so `restore` stays cached until a dependency changes.
COPY WorkoutTracker.slnx ./
COPY src/WorkoutTracker.Domain/WorkoutTracker.Domain.csproj src/WorkoutTracker.Domain/
COPY src/WorkoutTracker.Application/WorkoutTracker.Application.csproj src/WorkoutTracker.Application/
COPY src/WorkoutTracker.Infrastructure/WorkoutTracker.Infrastructure.csproj src/WorkoutTracker.Infrastructure/
COPY src/WorkoutTracker.Api/WorkoutTracker.Api.csproj src/WorkoutTracker.Api/
RUN dotnet restore src/WorkoutTracker.Api/WorkoutTracker.Api.csproj

COPY src/ src/
RUN dotnet publish src/WorkoutTracker.Api/WorkoutTracker.Api.csproj \
    -c Release \
    -o /app \
    --no-restore

# ---------------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final

# Alpine ships without ICU. The app formats dates and weights for display, so real
# culture data is preferred over invariant mode. curl backs the healthcheck below.
RUN apk add --no-cache icu-libs curl
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app
COPY --from=build /app ./

# Media root is a mounted volume so progress photos survive image rebuilds. It must be
# writable by the non-root user ($APP_UID is 1654 in the official .NET images).
RUN mkdir -p /var/lib/workouttracker/media \
 && chown -R $APP_UID:$APP_UID /var/lib/workouttracker

ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    Storage__MediaRoot=/var/lib/workouttracker/media

EXPOSE 8080
USER $APP_UID

HEALTHCHECK --interval=30s --timeout=5s --start-period=45s --retries=5 \
    CMD curl -fsS http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "WorkoutTracker.Api.dll"]
