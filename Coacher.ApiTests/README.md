# Coacher API Tests

Automated integration tests derived from `Coacher.WebApi/MANUAL_API_TEST_PLAN.md`.

## What It Covers

- mock seeding
- authentication and session lifecycle
- anonymous GraphQL surface
- representative authorization checks
- representative exercise, task, and admin write flows

## Environment

The suite targets a running API environment and does not self-host the application.

Defaults:

- `COACHER_API_BASE_URL=http://localhost:8080`
- `COACHER_API_TIMEOUT_SECONDS=30`

The expected runtime shape matches the Docker setup in `Coacher.WebApi/docker-compose.yml`.

## Run

```powershell
dotnet test Coacher.ApiTests/Coacher.ApiTests.csproj
```

The seeded tests mutate data and share one seeded fixture. Test parallelization is disabled intentionally.
