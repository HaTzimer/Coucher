# Code Organization Rules

## Layer and folder placement

- Put code in the project that owns the responsibility.
- Keep transport concerns in the Web API layer.
- Keep business logic and data orchestration in the Lib layer.
- Keep shared models, enums, interfaces, and extensions in the Shared layer when they truly belong there.
- Keep extensions in an `Extensions` folder.

## Constants

- Shared constant values must be defined in `Coucher.Shared/ConstantValues.cs`.
- Do not introduce file-local `const` values for shared semantics (for example closed-list keys); add them to `ConstantValues` instead.

## Return formatting

Always use a local variable before returning a computed value or awaited result.

Preferred:

```csharp
var items = await provider.ListAsync(cancellationToken);

return items;
```

Avoid:

```csharp
return await provider.ListAsync(cancellationToken);
```

Always leave an empty line before `return`.

Preferred:

```csharp
var exists = await provider.ExistsAsync(id, cancellationToken);

return exists;
```

Avoid:

```csharp
var exists = await provider.ExistsAsync(id, cancellationToken);
return exists;
```

When using async DbContext factory creation, always leave an empty line after the `await using` statement.

Preferred:

```csharp
await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

var entities = dbContext.Set<UserNotification>();
```

Avoid:

```csharp
await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
var entities = dbContext.Set<UserNotification>();
```

## Readability

- Prefer `var` for local variables when the right-hand side makes the type obvious.
- Avoid compressing multiple ideas into one expression when named locals improve readability.
- Keep control flow explicit.
- Make formatting decisions that optimize scanability, not terseness.

## Fluent chains

For method chains split across lines, keep each chained call aligned directly under the receiver line without extra indentation.

Preferred:

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

Avoid:

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
