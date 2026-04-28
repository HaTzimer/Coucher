---
name: augustus-logger-practices
description: Use when adding or reviewing logging in `Coacher.WebApi`, `Coacher.Lib`, or shared infrastructure that relies on `Augustus.Infra.Core`. Covers `IAugustusLogger` injection, structured logging payloads, exception logging with `ExceptionWithParameters` and `HttpStatusCodeException`, log level selection, and logger configuration expectations.
---

# Augustus Logger Practices

Read [references/augustus-logger.md](references/augustus-logger.md) before adding new logs or reviewing existing ones.

## Core Rules

- Inject `IAugustusLogger` through the constructor for services, handlers, providers, middleware, and similar DI-managed classes.
- Use fixed short messages and put details in a `Dictionary<string, object>`.
- Put business identifiers in structured fields such as `userId`, `authorizingUserId`, `exerciseId`, `taskId`, `resourceId`, or `pageId`.
- Do not use string interpolation to build log messages.
- Use `Debug` for noisy activity-style logging.
- Use `Info` for meaningful non-error decisions or state changes.
- Use `Warn` for suspicious but non-failing conditions.
- Use `Error` for failure paths.
- Log only meaningful decisions, authorization failures, invalid state, missing data, external failures, and other real troubleshooting points.
- Do not log every method entry or exit.

## Error Pattern

For business or validation failures:

1. Create an `ExceptionWithParameters` or `HttpStatusCodeException`.
2. Put the relevant identifiers and inputs in its parameter dictionary.
3. Call `_logger.Error(exception)`.
4. Throw the same exception.

Do not log and throw plain framework exceptions from application code.
Wrap application failures as `ExceptionWithParameters` or a `HttpStatusCodeException` subclass first.

Prefer:

```csharp
var exception = new HttpStatusCodeException(
    "Unauthorized to update task.",
    new Dictionary<string, object?>
    {
        { "authorizingUserId", authorizingUserId },
        { "taskId", taskId }
    },
    statusCode: HttpStatusCode.Forbidden
);

_logger.Error(exception);

throw exception;
```

## Message Shape

Prefer:

```csharp
_logger.Info("User tried to update a missing subscription", new Dictionary<string, object>
{
    { "hadrachaPageId", updateData.HadrachaPageId ?? string.Empty }
});
```

Do not use:

```csharp
_logger.Info($"User {userId} failed to update page {pageId}");
```

## Configuration

- Expect the custom logger pipeline to read the `Logger` configuration section, not plain ASP.NET logging alone.
- Keep service-specific logger configuration in application settings when a host needs it.

## Review Checklist

- Reject classes that should log but do not inject `IAugustusLogger`.
- Reject interpolated log messages.
- Reject logs that hide identifiers in free text instead of structured fields.
- Reject thrown `ExceptionWithParameters` or `HttpStatusCodeException` objects that are not logged first when the path is expected to be logged.
- Reject verbose low-signal logs with no operational value.
- Reject logs that duplicate data already present in the exception parameter payload.
