# Code Organization Rules

## Layer and folder placement

- Put code in the project that owns the responsibility.
- Keep transport concerns in the Web API layer.
- Keep business logic and data orchestration in the Lib layer.
- Keep shared models, enums, interfaces, and extensions in the Shared layer when they truly belong there.
- Keep extensions in an `Extensions` folder.

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
