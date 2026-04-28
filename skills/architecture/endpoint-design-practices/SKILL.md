---
name: endpoint-design-practices
description: Design or refactor Coucher REST endpoints and request contracts. Use when adding or changing route shape, deciding whether an update should be unified or split, choosing scalar-vs-model request bodies, or separating root-field updates from relation operations.
---

# Endpoint Design Practices

## Overview

Use this skill to keep endpoint shape consistent across `Coucher.WebApi` and `Coucher.Shared` request models.

Read `AGENTS.md` first, then use this skill together with `skills/architecture/create-update-workflows` when the task also changes service write orchestration.

## Core Rules

- Prefer one unified root update endpoint per aggregate:
  - `PUT /api/<resource>/update/{id}`
- Make unified root update request models nullable/optional:
  - only non-null values are applied
- For nullable string fields that must be clearable, add explicit clear flags:
  - `ClearDescription`
  - `ClearNotes`
- Treat `null` in unified update models as `no change`, not `clear`.
- Keep relation-based operations on dedicated endpoints:
  - participants
  - contacts
  - dependencies
  - responsible users
  - influencers
  - children
- For attached entities and relation links, allow add or remove operations, not replace-all overrides.
- Do not split root-field updates into `/name`, `/status`, `/details`, `/series`, or similar endpoints unless there is a real behavioral difference that cannot be expressed cleanly in one unified request.
- If a request body is one scalar value, accept the raw scalar body instead of a wrapper DTO.
- Keep bulk collection operations on dedicated endpoints instead of overloading the unified root update endpoint.

## Route Shape

- Use:
  - singular root resource bases such as:
    - `api/exercise-task`
    - `api/exercise`
    - `api/admin/task-template`
    - `api/admin/closed-list-item`
    - `api/admin/user-role`
  - `POST /api/<resource>/create/single` for create
  - `POST /api/<resource>/create/bulk` for bulk create
  - `PUT /api/<resource>/update/{id}` for unified root-field update
  - `DELETE /api/<resource>/delete/{id}` for explicit root deletes when the aggregate supports delete
  - explicit verb routes for relation changes such as:
    - `POST /api/<resource>/{id}/add-participant`
    - `POST /api/<resource>/{id}/add-responsible-user`
    - `DELETE /api/<resource>/remove-participant/{participantId}`
    - `DELETE /api/<resource>/remove-dependency/{dependencyId}`
    - `PUT /api/<resource>/update-contact/{contactId}` when an attached entity itself needs field edits
  - `PUT /api/<resource>/archive/{id}` with a raw `bool` body for archive state changes
- Do not create duplicate endpoint pairs where both routes only add/update the same root data in different shapes.
- Do not use generic plural relation paths when an explicit action name makes the behavior clearer.
- Do not expose replace-all routes for attached entity collections.

## Request Model Guidance

- Put request DTOs under `Coucher.Shared/Models/WebApi/Requests/...`.
- Keep unified update DTOs limited to mutable root business fields.
- Do not include:
  - ids
  - timestamps
  - actor metadata
  - related collection replacement fields
- For root string fields:
  - `string? Description`
  - `bool ClearDescription`
- Reject requests that send both a value and its clear flag in the same payload.
- If a unified update request carries no effective changes, return the current entity unchanged and do not persist or log an update.

## Decision Rule

- Split endpoints only when one of these is true:
  - the endpoint changes relations instead of root fields
  - the endpoint has materially different authorization rules
  - the endpoint has materially different side effects or workflow semantics
- Otherwise, prefer the unified root update endpoint.

## Examples

Prefer:

```csharp
[HttpPut("update/{id:guid}")]
public async Task<ActionResult<Exercise>> UpdateAsync(
    Guid id,
    [FromBody] UpdateExerciseRequest request,
    CancellationToken cancellationToken
)
```

```csharp
public sealed class UpdateExerciseRequest
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool ClearDescription { get; set; }

    public DateOnly? EndDate { get; set; }

    public Guid? StatusId { get; set; }
}
```

Do not prefer:

```csharp
[HttpPut("{id:guid}/status")]
[HttpPut("{id:guid}/details")]
[HttpPut("{id:guid}/end-date")]
```

```csharp
public sealed class UpdateExerciseDetailsRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
```

Do prefer:

```csharp
[HttpPut("archive/{id:guid}")]
public async Task<ActionResult<Exercise>> SetArchiveStateAsync(
    Guid id,
    [FromBody] bool isArchived,
    CancellationToken cancellationToken
)
```

Do not prefer:

```csharp
[HttpPost("{id:guid}/archive")]
[HttpPost("{id:guid}/unarchive")]
```

Do prefer:

```csharp
[HttpPost("{id:guid}/add-responsible-user")]
[HttpDelete("remove-responsible-user/{responsibilityId:guid}")]
```

Do not prefer:

```csharp
[HttpPost("{id:guid}/responsible-users")]
[HttpPut("{id:guid}/responsible-users")]
[HttpDelete("responsible-users/{responsibilityId:guid}")]
```

## Review Checklist

- Reject split root-field update routes when a unified nullable request would be simpler.
- Reject separate archive and unarchive endpoints when one raw-boolean archive-state endpoint would do the same job.
- Reject replace-all routes for attached entity collections.
- Reject vague relation route names when explicit `add-*`, `remove-*`, `set-*`, or `update-*` naming would describe the behavior better.
- Reject relation fields inside unified root update DTOs.
- Reject single-scalar wrapper DTOs.
- Reject use of `null` as both `no change` and `clear`.
- Reject missing clear flags for nullable string fields that need explicit clearing.
