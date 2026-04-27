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
- Do not return expressions directly when a value can be assigned first.
- Store returned values in a local `var` before returning them.
- Always put an empty line before a `return` statement.
- After `await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);`, always add an empty line.
- In fluent chains, keep the chained calls aligned to the left of the chain instead of adding extra indentation for each call.

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
- Reject model files that mix attributed properties with tightly packed property declarations; once one property in a model uses attributes, every property in that model must be separated by an empty line.
- Reject `return` statements that are not separated by an empty line.
- Reject fluent chains that use extra indentation before chained calls.
- Reject missing empty lines after async DbContext factory creation statements.
- Reject code placed in the wrong project, layer, or folder.
- Reject compact code that hides intent when a small local variable would make the flow clearer.
