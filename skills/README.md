# Skills Index

This repo stores local Codex-style skills under `skills/`.

Source of truth for repo conventions is `AGENTS.md`. Skills should reference it instead of duplicating rules.

## Architecture

- `skills/architecture/provider-repository-service-pattern`: project boundaries + provider/repository/service layering.
- `skills/architecture/dal-dbcontext-practices`: EF Core entity + DbContext rules and patterns.
- `skills/architecture/model-creation-practices`: canonical model creation conventions.

## Operations

- `skills/operations/configuration-use-practices`: configuration key usage and environment overrides.
- `skills/operations/augustus-logger-practices`: logging rules and Augustus logger expectations.

## Tooling

- `skills/tooling/code-organization-practices`: repository code organization conventions.

## Backward Compatibility

The original skill folder names still exist as junctions under `skills/` (for example `skills/provider-repository-service-pattern`) and point to the categorized locations above.

