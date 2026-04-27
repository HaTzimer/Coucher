---
name: create-update-workflows
description: Implement explicit create/update request contracts and write flows in Coucher without changing DAL entities, DbContext shape, or migrations. Use when adding REST write endpoints, bulk-create flows, audit timestamp stamping, or aggregate-specific create/update methods across providers, repositories, and services.
---

# Create And Update Workflows

Use this skill when adding or reviewing write flows for Coucher.

## Core Rules

- Do not change DAL entity shape, `CoucherDbContext`, or migrations as part of this skill's normal workflow.
- Keep Web API request models under `Coucher.Shared/Models/WebApi/Requests/...`.
- Use explicit write methods such as `CreateExerciseAsync(...)`, `CreateExerciseTasksAsync(...)`, or `UpdateTaskTemplateAsync(...)` instead of routing new write endpoints through raw generic entity `AddAsync/UpdateAsync` calls.
- Keep create/update request models limited to client-owned business fields. Never accept server-owned fields from clients.
- Stamp existing time/audit fields in services with `DateTime.UtcNow`.
- Use the authenticated session user for write orchestration. Persist that user only when the existing DAL entity already has a matching field.
- Keep bulk create all-or-nothing and use `List<CreateXRequestModel>` directly as the request body for bulk endpoints in this repository.

## Layer Split

- Controller:
  - accept request models
  - apply `WebApiSessionAuthenticationFilter`
  - call a Lib service method
- Service:
  - resolve the current user
  - map request models into DAL entities
  - set ids, timestamps, and any existing actor fields
  - preserve immutable fields on update
- Repository:
  - expose aggregate-specific write contracts
  - translate missing rows into domain-friendly exceptions
- Provider:
  - execute EF Core persistence
  - own save boundaries
  - provide helper queries needed for write orchestration such as next serial number lookups

## Request Model Guidance

- Create models may include only client-owned fields needed to create the aggregate.
- Update models should be full mutable payloads for root fields, not patch payloads.
- Update models should not replace related child collections or link sets unless the feature explicitly calls for aggregate replacement.
- For bulk create, reuse the single-create request model and accept `List<CreateXRequestModel>`.

## Review Checklist

- Reject any write flow that lets clients send ids, `CreationTime`, `LastUpdateTime`, or other server-owned metadata.
- Reject write orchestration that lives in controllers or providers.
- Reject bulk-create implementations that save each item independently.
- Reject DAL, `DbContext`, or migration changes when the task is limited to create/update contract work.
