# Builds the PWA and bakes it into a Caddy image that also terminates TLS and reverse
# proxies /api to the API container. Serving both from one origin means the browser
# treats API calls as same-site, so no CORS configuration is involved at all.
#
# Build context is this directory:
#   docker build -t workouttracker-web src/WorkoutTracker.Web
#
# syntax=docker/dockerfile:1
FROM node:22-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
# `npm run build` runs vue-tsc first, so a type error fails the image build.
RUN npm run build

# ---------------------------------------------------------------------------------
FROM caddy:2-alpine AS final
COPY --from=build /app/dist /srv
# The Caddyfile is bind-mounted by compose so the domain can change without a rebuild.
