---
name: code-organization-practices
description: Enforce repository-specific C# code organization and formatting practices. Use when adding, editing, or reviewing code structure, method layout, return style, variable declarations, layering boundaries, or consistency rules in this repository.
---

# Code Organization Practices

## Overview

Use this skill to keep code changes aligned with the repository's organization and formatting rules.

Read [references/rules.md](references/rules.md) before making structural or formatting changes.

## Core Rules

- Keep code in the correct layer and folder for its responsibility.
- Shared constant values belong in `Coucher.Shared/ConstantValues.cs` (do not add file-local `const` values for shared semantics).
- Prefer small, readable methods over condensed expressions.
- In model files, if at least one property uses attributes, add an empty line after every property in that model, including properties without attributes.
- For Web API request bodies, if the payload is a single scalar value, do not create a one-property request model; accept the raw scalar body instead.
- In service and repository methods, separate distinct phases with empty lines so the flow scans as categories instead of one dense block.
  Authorization/guards
  Data load or entity construction
  Entity mutation
  Persistence call
  Logging
  Return
- For `if` statements with a single following statement, do not use braces.
- For simple collection shaping, filtering, and projection, prefer LINQ over `foreach`.
- Do not return expressions directly when a value can be assigned first.
- Store returned values in a local `var` before returning them.
- Do not write `return new ...`; first assign the created object to a local `var`, then return that variable.
- Always put an empty line before a `return` statement.
- After `await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);`, always add an empty line.
- In fluent chains, keep the chained calls aligned to the left of the chain instead of adding extra indentation for each call.
- Do not nest a fluent chain inside another method call when that would indent inner chained calls; extract the inner chain to a local variable first.

## Return Style

Prefer:

```csharp
var entity = await repository.GetByIdAsync(id, cancellationToken);

return entity;
```

Do not use:

```csharp
return await repository.GetByIdAsync(id, cancellationToken);
```

Do not use:

```csharp
var entity = await repository.GetByIdAsync(id, cancellationToken);
return entity;
```

Do not use:

```csharp
return new HeaderAuthenticationResult
{
    HeaderAuthentication = SessionAuthenticationResult.Valid,
    SessionId = sessionId
};
```

Prefer:

```csharp
var authenticationResult = new HeaderAuthenticationResult
{
    HeaderAuthentication = SessionAuthenticationResult.Valid,
    SessionId = sessionId
};

return authenticationResult;
```

## Collection Shaping Style

Prefer:

```csharp
var parameters = entries
.Where(item => item.Value is not null)
.Select(item => new KeyValuePair<string, object>(item.Key, item.Value!))
.ToDictionary(item => item.Key, item => item.Value);
```

Do not use:

```csharp
var parameters = new Dictionary<string, object>();

foreach (var (key, value) in entries)
{
    if (value is not null)
        parameters[key] = value;
}
```

## Fluent Chain Style

Prefer:

```csharp
var query = Entities
.Include(item => item.Exercise)
.Include(item => item.ResponsibleUser)
.Include(item => item.Influencers)
.Include(item => item.Dependencies)
.ThenInclude(item => item.DependsOnTask)
.Include(item => item.DependedOnByTasks)
.ThenInclude(item => item.ExerciseTask);
```

Do not use:

```csharp
var query = Entities
    .Include(item => item.Exercise)
    .Include(item => item.ResponsibleUser)
    .Include(item => item.Influencers)
    .Include(item => item.Dependencies)
        .ThenInclude(item => item.DependsOnTask)
    .Include(item => item.DependedOnByTasks)
        .ThenInclude(item => item.ExerciseTask);
```

Do not use:

```csharp
var parameters = defaults
.Concat(entries
    .Where(item => item.Value is not null)
    .Select(item => new KeyValuePair<string, object>(item.Key, item.Value!)))
.ToDictionary(item => item.Key, item => item.Value);
```

Prefer:

```csharp
var entryParameters = entries
.Where(item => item.Value is not null)
.Select(item => new KeyValuePair<string, object>(item.Key, item.Value!));

var parameters = defaults
.Concat(entryParameters)
.ToDictionary(item => item.Key, item => item.Value);
```

## DbContext Factory Spacing

Prefer:

```csharp
await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

var entities = dbContext.Set<UserNotification>();
```

Do not use:

```csharp
await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
var entities = dbContext.Set<UserNotification>();
```

## Review Checklist

- Reject direct-expression returns when a local variable should be used instead.
- Reject direct `return new ...` object creation; the object must be assigned to a local variable first.
- Reject simple `foreach` loops whose only job is to filter/project/materialize a collection when LINQ would express it more clearly.
- Reject model files that mix attributed properties with tightly packed property declarations; once one property in a model uses attributes, every property in that model must be separated by an empty line.
- Reject one-property request DTOs whose only purpose is to wrap a single scalar request body.
- Reject dense service/repository method bodies that mix authorization, mutation, persistence, logging, and return without visual separation.
- Reject single-statement `if` blocks that still use braces.
- Reject `return` statements that are not separated by an empty line.
- Reject fluent chains that use extra indentation before chained calls.
- Reject nested fluent chains inside method arguments when extracting the inner chain to a local variable would preserve left-aligned chaining.
- Reject missing empty lines after async DbContext factory creation statements.
- Reject code placed in the wrong project, layer, or folder.
- Reject compact code that hides intent when a small local variable would make the flow clearer.
