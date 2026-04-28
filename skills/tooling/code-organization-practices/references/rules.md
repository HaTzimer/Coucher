# Code Organization Rules

## Layer and folder placement

- Put code in the project that owns the responsibility.
- Keep transport concerns in the Web API layer.
- Keep business logic and data orchestration in the Lib layer.
- Keep shared models, enums, interfaces, and extensions in the Shared layer when they truly belong there.
- Keep extensions in an `Extensions` folder.

## Constants

- Shared constant values must be defined in `Coacher.Shared/ConstantValues.cs`.
- Do not introduce file-local `const` values for shared semantics (for example closed-list keys); add them to `ConstantValues` instead.

## Return formatting

Always use a local variable before returning a computed value or awaited result.
Also use a local variable before returning a newly created object.

Preferred:

```csharp
var items = await provider.ListAsync(cancellationToken);

return items;
```

Avoid:

```csharp
return await provider.ListAsync(cancellationToken);
```

Avoid:

```csharp
return new HeaderAuthenticationResult
{
    HeaderAuthentication = SessionAuthenticationResult.Valid,
    SessionId = sessionId
};
```

Preferred:

```csharp
var authenticationResult = new HeaderAuthenticationResult
{
    HeaderAuthentication = SessionAuthenticationResult.Valid,
    SessionId = sessionId
};

return authenticationResult;
```

In application code, throw only `ExceptionWithParameters` or `HttpStatusCodeException` subclasses.
Do not throw plain framework exceptions such as `InvalidOperationException`, `KeyNotFoundException`, `NotSupportedException`, or `AuthenticationException`.

For application exceptions that should be logged with structured parameters, create the exception in a local variable, log that same instance with `_logger.Error(exception)`, then throw it.

Preferred:

```csharp
var exception = new ExceptionWithParameters(
    "User with personal id or military id already exists.",
    new Dictionary<string, object?>()
    {
        { "personalId", registerUserModel.PersonalId! },
        { "militaryId", registerUserModel.MilitaryId! }
    });

_logger.Error(exception);

throw exception;
```

Preferred:

```csharp
var exception = new DataConflictException(
    "A task cannot depend on itself.",
    parameters: new Dictionary<string, object?>
    {
        { "taskId", taskId }
    }
);

_logger.Error(exception);

throw exception;
```

Preferred:

```csharp
var exception = new HttpStatusCodeException(
    "ManagerUserId must be a valid GUID string.",
    new Dictionary<string, object?>
    {
        { "managerUserId", managerUserId }
    },
    HttpStatusCode.BadRequest
);

_logger.Error(exception);

throw exception;
```

Avoid:

```csharp
throw new ExceptionWithParameters(
    "User with personal id or military id already exists.",
    new Dictionary<string, object?>()
    {
        { "personalId", registerUserModel.PersonalId! },
        { "militaryId", registerUserModel.MilitaryId! }
    });
```

Avoid:

```csharp
var exception = new InvalidOperationException("...");

_logger.Error(exception.Message.TrimEnd('.'), exception);

throw exception;
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
- In service/repository methods, visually split distinct phases with blank lines instead of packing them into one block.
- For `if` statements with only one following statement, omit braces.
- When an `if` appears in the middle of a method body instead of immediately after the opening `{`, leave an empty line before it.
- When an `if` appears in the middle of a method body instead of immediately after the opening `{`, leave an empty line before it.
- For simple collection filtering, projection, or materialization, prefer LINQ over `foreach`.

Preferred:

```csharp
await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);

var entity = await _repository.GetRequiredByIdAsync(id, cancellationToken);
entity.Description = description;
entity.LastUpdateTime = DateTime.UtcNow;

var updatedEntity = await _repository.UpdateAsync(entity, cancellationToken);

return updatedEntity;
```

Also prefer:

```csharp
await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);

var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
var entity = await _repository.GetRequiredByIdAsync(id, cancellationToken);

entity.Value = value;
entity.LastUpdateTime = DateTime.UtcNow;

var updatedEntity = await _repository.UpdateAsync(entity, cancellationToken);

_logger.Info("Entity updated", new Dictionary<string, object>
{
    { "userId", currentUserId },
    { "id", id }
});

return updatedEntity;
```

Avoid:

```csharp
await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);
var entity = await _repository.GetRequiredByIdAsync(id, cancellationToken);
entity.Description = description;
entity.LastUpdateTime = DateTime.UtcNow;
var updatedEntity = await _repository.UpdateAsync(entity, cancellationToken);
return updatedEntity;
```

Preferred:

```csharp
if (ShouldLogMockSeed())
    _logger.Info("Mock seed started", new Dictionary<string, object>
    {
        { "resetExisting", options.ResetExisting },
        { "result", "started" }
    });
```

Also prefer:

```csharp
if (options.ResetExisting)
    await ResetAsync(dbContext, cancellationToken);
```

Avoid:

```csharp
if (ShouldLogMockSeed())
{
    _logger.Info("Mock seed started", new Dictionary<string, object>
    {
        { "resetExisting", options.ResetExisting },
        { "result", "started" }
    });
}
```

Preferred:

```csharp
var parameters = entries
.Where(item => item.Value is not null)
.Select(item => new KeyValuePair<string, object>(item.Key, item.Value!))
.ToDictionary(item => item.Key, item => item.Value);
```

Avoid:

```csharp
var parameters = new Dictionary<string, object>();

foreach (var (key, value) in entries)
{
    if (value is not null)
        parameters[key] = value;
}
```

## Single-scalar request bodies

For Web API request bodies, if the payload contains only one scalar value, do not wrap it in a one-property request model. Use the scalar body directly.

Preferred:

```csharp
[HttpPut("update/{id:guid}")]
public async Task<ActionResult<Exercise>> UpdateStatusAsync(
    Guid id,
    [FromBody] Guid statusId,
    CancellationToken cancellationToken
)
```

Also prefer:

```csharp
[HttpPost("{id:guid}/add-participants")]
public async Task<ActionResult<ExerciseParticipant>> AddParticipantAsync(
    Guid id,
    [FromBody] string userId,
    CancellationToken cancellationToken
)
```

Avoid:

```csharp
public sealed class SingleValueRequest
{
    public required string Value { get; set; }
}
```

## Model property spacing

For model files, if at least one property in the model has one or more attributes directly above it, every property in that model must be followed by an empty line.

Preferred:

```csharp
[GraphQLDescription("When the entry was created.")]
public required DateTime CreationTime { get; set; }

[GraphQLDescription("When the entry was last updated.")]
public required DateTime LastUpdateTime { get; set; }
```

Also prefer:

```csharp
[GraphQLDescription("The unique identifier of the unit.")]
public Guid Id { get; set; }

public required string Name { get; set; }
```

Avoid:

```csharp
[GraphQLDescription("When the entry was created.")]
public required DateTime CreationTime { get; set; }
[GraphQLDescription("When the entry was last updated.")]
public required DateTime LastUpdateTime { get; set; }
```

## Fluent chains

For method chains split across lines, keep each chained call aligned directly under the receiver line without extra indentation.
If an inner chain would need to be nested inside another call and become indented, extract the inner chain to a local variable first.

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

Avoid:

```csharp
var parameters = defaults
.Concat(entries
    .Where(item => item.Value is not null)
    .Select(item => new KeyValuePair<string, object>(item.Key, item.Value!)))
.ToDictionary(item => item.Key, item => item.Value);
```

Preferred:

```csharp
var entryParameters = entries
.Where(item => item.Value is not null)
.Select(item => new KeyValuePair<string, object>(item.Key, item.Value!));

var parameters = defaults
.Concat(entryParameters)
.ToDictionary(item => item.Key, item => item.Value);
```
