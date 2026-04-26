# Augustus Logger Reference

## Logger API

`IAugustusLogger` exposes:

- `Debug(string title, Dictionary<string, object>? @params = null, ...)`
- `Info(string title, Dictionary<string, object>? @params = null, ...)`
- `Warn(string title, Dictionary<string, object>? @params = null, ...)`
- `Error(string title, Dictionary<string, object>? @params = null, ...)`
- `Fatal(string title, Dictionary<string, object>? @params = null, ...)`
- `Error(string title, Exception exception, Dictionary<string, object>? @params = null, ...)`
- `Warn(string title, Exception exception, Dictionary<string, object>? @params = null, ...)`
- `Fatal(string title, Exception exception, Dictionary<string, object>? @params = null, ...)`
- `Error(ExceptionWithParameters exception, ...)`

## How AugustusLogger Writes Logs

`AugustusLogger` serializes the payload dictionary to JSON and adds common fields automatically:

- `title`
- `serviceName`
- `machineName`
- correlation identifiers when available
- correlation delay fields when available

Caller file path is also used to derive the logger name automatically, so do not manually pass type names into the message.

## Exception Types

Use `ExceptionWithParameters` for business failures that need structured data.

Use `HttpStatusCodeException` when the failure should also carry an HTTP status code.

Both types accept:

- a fixed message
- a parameter dictionary
- an optional inner exception

`_logger.Error(exception)` logs the exception message as the title and merges `exception.Parameters` into the structured payload, while also attaching the exception object.

## Patterns To Follow

### Constructor injection

Inject `IAugustusLogger` into DI-managed classes:

```csharp
private readonly IAugustusLogger _logger;

public ExerciseService(IAugustusLogger logger, IExerciseRepository repository)
{
    _logger = logger;
    _repository = repository;
}
```

### Structured informational logging

Keep the message stable and move identifiers into the payload:

```csharp
_logger.Info("User tried to update a missing subscription", new Dictionary<string, object>
{
    { "hadrachaPageId", updateData.HadrachaPageId ?? string.Empty }
});
```

### Exception logging before throw

```csharp
var exception = new ExceptionWithParameters(
    "Unauthorized to update resource authorizations.",
    new Dictionary<string, object?>
    {
        { "authorizingUserId", authorizingUserId },
        { "userIds", updateData.UserIds }
    }
);

_logger.Error(exception);

throw exception;
```

### Activity logging

Use `Debug` for high-volume activity-style events. In the reference codebase, incoming user activity payloads are mapped to `Debug` and `Error` based on a level field instead of being logged as interpolated text.

## Example Practices From The Existing Stack

From `HadrachaSiteServer`:

- `UserActivityService` injects `IAugustusLogger` and uses `Debug` for user activity events.
- `ResourceSubscriptionAuthorizationService` logs authorization failures with `ExceptionWithParameters` and logs missing subscription cases with `Info` plus structured identifiers.
- `AuthorizationUserToResourceHandler` logs authorization rejection for deleted users with a short message and a payload dictionary.
- `MicroResourceAuthorizationService` wraps duplication and hierarchy failures in `ExceptionWithParameters`, logs them, and rethrows.
- `HadrachaPageDuplicationService` logs duplication failures with a short stable message and identifiers like `pageId` and `resourceId`.

## Configuration Expectation

The custom logger pipeline expects a `Logger` configuration section. In the reference host settings, that section includes values such as:

- `ServiceName`
- `RemotePort`

Keep that expectation in mind when configuring a host in this repository.
