# Model Creation Rules

## Shared entity rules

For DAL and shared models in this repository:

- Use plain `get; set;` properties.
- Do not use `init`.
- Use `required` for mandatory fields when applicable.
- Do not assign default values to properties.
- Use `List<T>` for collection properties on entities.

Preferred:

```csharp
public sealed class Exercise
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required List<ExerciseTask> Tasks { get; set; }
}
```

Avoid:

```csharp
public sealed class Exercise
{
    public Guid Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public IEnumerable<ExerciseTask> Tasks { get; set; } = Array.Empty<ExerciseTask>();
}
```

## Required usage

If a field is mandatory for a valid model instance, prefer `required`.

Use it for:

- Required strings
- Required navigation properties when they must exist
- Required collections that must be populated by the caller

Do not omit `required` when the property is conceptually mandatory and nullable state should not carry that burden alone.

## No defaults

Do not assign default values on model properties.

That includes:

- `= string.Empty`
- `= new List<T>()`
- `= Array.Empty<T>()`
- `= default`
- any other inline default initializer

Initialization belongs to the creator of the model, not the model declaration itself.

## Interface rule

Do not introduce or depend on marker interfaces like `IHasId<Guid>` for normal model creation.

If a model has an `Id`, declare the property directly on the model.
