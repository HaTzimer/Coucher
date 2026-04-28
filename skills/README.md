# Skills Index

This repo stores local Codex-style skills under `skills/`.

Source of truth for repo conventions is `AGENTS.md`. Skills should reference it instead of duplicating rules.

## Skill Selection Flow

Use this lightweight flow instead of scanning the whole repo:

1. Read `AGENTS.md` first.
2. Decide the task category from `AGENTS.md`.
3. Use this index to locate the matching skill.
4. Open only the relevant `SKILL.md` file or files.
5. Use the smallest skill set that covers the task.

Default routing for this repo:

- Endpoint shape / route design:
  - `skills/architecture/endpoint-design-practices`
- Layering/responsibility changes:
  - `skills/architecture/provider-repository-service-pattern`
- EF/DAL/entity/DbContext work:
  - `skills/architecture/dal-dbcontext-practices`
- New shared model/entity creation:
  - `skills/architecture/model-creation-practices`
- Create/update Web API request and write-flow work:
  - `skills/architecture/create-update-workflows`
- Logging changes:
  - `skills/operations/augustus-logger-practices`
- Config/appsettings/config-key changes:
  - `skills/operations/configuration-use-practices`
- Session/auth pipeline changes:
  - `skills/operations/redis-session-authentication`
- Formatting/style/code-shape changes:
  - `skills/tooling/code-organization-practices`

Do not bulk-open all skills.
Do not scan the whole repo for skill files every turn.

## Architecture

- `skills/architecture/endpoint-design-practices`: REST route shape, unified update endpoint, and request-contract design.
- `skills/architecture/provider-repository-service-pattern`: project boundaries + provider/repository/service layering.
- `skills/architecture/dal-dbcontext-practices`: EF Core entity + DbContext rules and patterns.
- `skills/architecture/model-creation-practices`: canonical model creation conventions.
- `skills/architecture/create-update-workflows`: explicit create/update request-contract and write-flow rules.

## Operations

- `skills/operations/configuration-use-practices`: configuration key usage and environment overrides.
- `skills/operations/augustus-logger-practices`: logging rules and Augustus logger expectations.
- `skills/operations/redis-session-authentication`: Redis-backed session storage and request authentication for REST and GraphQL.

## Tooling

- `skills/tooling/code-organization-practices`: repository code organization conventions.

## Backward Compatibility

The original skill folder names still exist as junctions under `skills/` (for example `skills/provider-repository-service-pattern`) and point to the categorized locations above.
