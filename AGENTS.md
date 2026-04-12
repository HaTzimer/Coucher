# AGENTS Guide for `Coucher`

## Scope
This repository (`Coucher`) is a .NET 7 solution with:

- `Coucher.WebApi` (API host + transport layer)
- `Coucher.Lib` (business logic + DAL orchestration)
- `Coucher.Shared` (canonical domain/DAL entities + enums + shared interfaces)
- `Augustus.Infra.Core` (sibling repo project path: `..\Infra.Core\Augustus.Infra.Core\Augustus.Infra.Core.csproj`)

Dependency direction:

- `Coucher.WebApi` -> `Coucher.Lib`
- `Coucher.Lib` -> `Coucher.Shared`

## Model Architecture
`Coucher.Shared`:

- Contains EF-ready DAL/domain entities
- Entity model rules:
  - Use `get; set;` only (no `init`)
  - No default property values
  - Use `required` on mandatory DAL fields where applicable
  - Use `List<T>` for collections (do not use `IReadOnlyCollection<T>` / `IEnumerable<T>` in entities)

`Coucher.WebApi`:

- Contains request/transport DTOs only
- DTO location:
  - `Models/WebApi/Requests/Auth`
  - `Models/WebApi/Requests/Exercises`
  - `Models/WebApi/Requests/Tasks`
  - `Models/WebApi/Requests/Common`

Mapping rule:

- WebApi request DTOs are mapped to DAL/domain entities in WebApi/Lib layers.
- Do not put API request DTOs in `Coucher.Shared`.

## COACHER System Design
Source of truth is the COACHER system-spec PDF provided by the user in `C:\Users\Tasht\Downloads\`.

Core domain:

- `Exercise` root entity
- `ExerciseTask` entities under each exercise
- Participants, influencers, threat arenas, dependencies, statuses, notifications
- Role model: global (`User`, `Auditor`, `Admin`) and exercise-scoped (`ExerciseManager`, `ExerciseParticipant`, `TraineeUnitContact`)

Core flows:

1. Authentication: login, registration, first-login setup, security-question reset.
2. Exercise wizard: exercise details + participants.
3. Exercise task management: generate/filter/update tasks, dependencies, responsibility, due-state.
4. Admin management: closed lists, fixed templates, privileged users.
5. Archive lifecycle: active to archive by policy.

## Current Canonical Shared Entities
- `Models/Enums`
- `Models/DAL/Users`
- `Models/DAL/Exercises`
- `Models/DAL/Tasks`
- `Models/DAL/Admin`
- `Models/DAL/Notifications`

## Infra.Core Reuse Policy
- Treat `..\Infra.Core` as external sibling codebase; avoid editing it unless explicitly requested.
- Reuse existing `Augustus.Infra.Core` classes/methods/extensions before adding local infrastructure.
- Before adding a new local infra abstraction, verify Infra.Core does not already provide it.
- Document every Infra.Core adoption in code changes.

Primary Infra.Core DI entry points:

- `AddGenericAugustusServices(this IServiceCollection services)`
- `AddGenericPipelineServices(this IServiceCollection services)`
- `AddPooledDbContextFactory<T>(this IServiceCollection services, IConfiguration configuration)`

## Verification
- `dotnet restore Coucher.sln`
- `dotnet build Coucher.sln`
