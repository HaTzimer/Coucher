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
  - For DAL DateTime field names, prefer `...Time` (for example `CreationTime`, `LastUpdateTime`, `CompletionTime`) and avoid ending with `At` or `AtUtc`

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

## Canonical Requirements Source
Source of truth is the user-provided COACHER spec DOCX in `C:\Users\Tasht\Downloads\` (file name contains `COACHER`).

## COACHER Plan Baseline (Plan-Only)
This section is the implementation plan baseline. Do not treat it as already implemented behavior.

### Phase A Scope (from spec)
1. Authentication: login, registration, first-login setup, security-question reset.
2. Home/dashboard: personalized exercise list, search, profile card, notifications.
3. Exercise wizard: exercise details + participants + influencer-based task derivation.
4. Exercise task board: create/import/edit tasks, sub-tasks, filters, dependencies, due-state visualization.
5. Admin management: privileged users, closed lists, fixed templates.
6. Archive lifecycle: automatic archive 30 days after exercise end + admin restore/delete.

### Out of Scope (current spec maturity)
- Phase B items are placeholders only (knowledge library, gantt, external integrations, rich comments, etc.).

## Domain Model Plan (`Coucher.Shared`)
Keep canonical entities under:
- `Models/Enums`
- `Models/DAL/Users`
- `Models/DAL/Exercises`
- `Models/DAL/Tasks`
- `Models/DAL/Admin`
- `Models/DAL/Notifications`
- `Models/Internal/Projections` (read models only; do not place projections under `Models/DAL`)

### Users + Access Models
- `UserProfile`: identity and profile fields, password/first-login state, forgot-password lock state, contact details.
- `UserRole`: global roles (`User`, `Auditor`, `Admin`).
- `UserSecurityQuestion`: selected closed-list questions and answer hashes.
- `ExerciseParticipant`: exercise-scoped role (`ExerciseManager`, `ExerciseParticipant`, `TraineeUnitContact`) with optional link to `UserProfile`.

### Exercise Models
- `Exercise` remains root aggregate:
  - identity, name, start/end dates, trainer/trainee unit values, status (`Draft`, `Active`, `Archived`)
  - selected influencers and threat arenas
  - participant set and manager/contact pointers
  - archive lifecycle timestamps/policy fields (planned)
- `ExerciseSummary`: read-model projection for grid/dashboard (keep under `Models/Internal/Projections/Exercises`).
- `ExerciseInfluencerLink`, `ExerciseThreatArenaLink`: closed-list selections per exercise.

### Task + Sub-Task Models
- `ExerciseTask` remains canonical task row for exercise boards.
- Plan to model sub-tasks as first-class rows linked to parent task (hierarchical task model); each row keeps own status/responsibility/due date while inheriting contextual display values from parent.
- `TaskDependency`: logical dependency graph between task rows; dependency semantics are warnings (not hard blocks).
- `ExerciseTaskInfluencerLink`: many-to-many task-to-influencer mapping.
- Planned task metadata extensions for Phase A behavior:
  - stable ordering/reindex support
  - completed-at timestamp when status changes to done
  - template-origin metadata for manual/template import
  - due-state derivation inputs for overdue / due-soon visualization

### Admin + Template Models
- `ClosedListItem`: closed-list storage for values used by wizard/admin flows.
- `FixedTaskTemplate`: central template task bank.
- `FixedTaskTemplateDependency`, `FixedTaskTemplateInfluencerLink`: template dependency graph + influencer tags.
- Planned template-import behavior:
  - import task only
  - import task with dependency chain (dedupe-aware)
  - import with links to existing tasks only

### Notification Models
- `UserNotification`: user-facing events with type, read-state, created timestamp, and deep-link context (`ExerciseId`, `TaskId`).
- `NotificationType` plan must cover Phase A events:
  - exercise create/update/delete/archive
  - task create/update/delete/status-change
  - due-soon / overdue
  - added-to-exercise
  - sub-task add
  - fixed-template and influencer catalog changes
  - exercise start reminders

### Enums + Closed Lists Plan
- Keep enums for stable/system semantics (roles, statuses, series).
- Keep admin-managed closed lists for operational vocabularies and selectable catalogs:
  - units (trainer/trainee)
  - threat arenas
  - influencers
  - security questions
  - template categories/sub-categories (if not hard-enum)

## Web API Request DTO Plan (`Coucher.WebApi`)
Request DTOs stay only under:
- `Models/WebApi/Requests/Auth`
- `Models/WebApi/Requests/Exercises`
- `Models/WebApi/Requests/Tasks`
- `Models/WebApi/Requests/Common`

Planned request families:
1. Auth: login/register/first-login/forgot/reset with security answers.
2. Exercises: wizard step-1 details + step-2 participants + archive actions.
3. Tasks: manual create, template import mode, status/assignee/due updates, dependency and ordering operations, sub-task creation.
4. Admin: closed-list CRUD, fixed template CRUD, privileged-user management.

## Provider/Repository/Service Plan (Skill-Aligned)
Use the `provider-repository-service-pattern` skill as the architecture contract.

### Provider Responsibilities (`Coucher.Lib/DAL/Providers`)
- Own EF Core mechanics only: query shapes, includes, existence checks, persistence commands, transactional save boundaries.
- No workflow decisions, permission policy, or cross-aggregate orchestration logic.

### Repository Responsibilities (`Coucher.Lib/Repositories`)
- Provide business-facing access contracts (not thin renames).
- Translate missing data to domain/app exceptions.
- Encapsulate reusable data semantics:
  - dependency graph reads
  - dedupe-safe template imports
  - archive eligibility predicates
  - filtered task-board fetch semantics

### Service Responsibilities (`Coucher.Lib/Services`)
- Orchestrate use cases and side effects:
  - auth lifecycle (registration/first login/reset)
  - exercise wizard (create + task generation + participant assignment)
  - task board workflows (bulk edits, ordering, due calculations, completion stamping)
  - admin workflows (closed lists/templates/privileged users)
  - archive lifecycle and notifications fan-out
- Services may call providers directly only when orchestration requires it and no reusable repository contract is being bypassed.

### DI Registration Plan
- Register each layer explicitly in `Startup`:
  - provider interface -> provider implementation
  - repository interface -> repository implementation
  - service interface -> service implementation
- Keep Infra.Core bootstrap calls in place (`AddGenericAugustusServices`, `AddPooledDbContextFactory<T>`), and use `AddGenericPipelineServices` when pipeline use-cases are introduced.

## Infra.Core Reuse Policy
- Treat `..\Infra.Core` as external sibling codebase; avoid editing it unless explicitly requested.
- Reuse existing `Augustus.Infra.Core` classes/methods/extensions before adding local infrastructure.
- Before adding a new local infra abstraction, verify Infra.Core does not already provide it.
- Document every Infra.Core adoption in code changes.

## Current Canonical Shared Entities
- `Models/Enums`
- `Models/DAL/Users`
- `Models/DAL/Exercises`
- `Models/DAL/Tasks`
- `Models/DAL/Admin`
- `Models/DAL/Notifications`

Primary Infra.Core DI entry points:

- `AddGenericAugustusServices(this IServiceCollection services)`
- `AddGenericPipelineServices(this IServiceCollection services)`
- `AddPooledDbContextFactory<T>(this IServiceCollection services, IConfiguration configuration)`

## Delivery Phases (Execution Order)
1. Foundation alignment: freeze enums/closed-list strategy, finalize permissions matrix mapping, finalize task/sub-task hierarchy approach.
2. Auth + user lifecycle: registration, first-login completion, security-question setup, forgot-password lock flow.
3. Exercise wizard: step-1 exercise metadata and influencer/threat selections, step-2 participant role assignment, initial template task generation.
4. Task board: manual create, template import modes, dependency warnings, ordering/reindex, bulk operations, due-state highlighting, completion timestamping.
5. Admin: privileged users, closed lists, fixed templates, template dependency management.
6. Archive + notifications: auto-archive policy, restore/delete operations, phase-A notification matrix.

## Planning Assumptions To Validate
- Responsibility cardinality in spec is inconsistent (single vs multiple assignees); default plan: keep single owner unless clarified.
- Status vocabulary must be finalized against UI wording (`NotStarted/InProgress/Done` vs extra states).
- Phase B features remain non-blocking placeholders for current delivery.

## Verification
- `dotnet restore Coucher.sln`
- `dotnet build Coucher.sln`
