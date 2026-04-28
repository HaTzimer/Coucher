# Configuration Rules

## Source of truth

Use `ConfigurationKeys.cs` in `Coacher.Shared` as the source of truth for JSON key names.

Structure:

```csharp
public class ConfigurationKeys
{
    public const string GqlQueriesSection = "GqlQueries";
    public const string ViewAsUserIdHeader = "viewAsUserIdHeader";
    public const string HadrachaPageIdHeader = "hadrachaPageIdHeader";
}
```

## Naming convention

1. Section constants:
- suffix with `Section`
- value is the JSON section name
- example: `GqlQueriesSection = "GqlQueries"`

2. Section value constants:
- use readable TitleCase constant names
- no `Section` suffix
- value is the exact JSON property key
- example: `ResourceIdsHeader = "resourceIdsHeader"`

## Access pattern

Use `IAugustusConfiguration` for runtime configuration reads.

Preferred:

```csharp
var userActivityPath = config.GetOrThrow<string>(
    ConfigurationKeys.HadrachaUsersServiceSection,
    ConfigurationKeys.UserActivityPath
);
```

Avoid:

```csharp
var userActivityPath = configuration["HadrachaUsersService:userActivityPath"];
```

Prefer constructor injection of `IAugustusConfiguration` in services/providers that need settings:

```csharp
private readonly IAugustusConfiguration _config;

public UserActivityService(IAugustusConfiguration config)
{
    _config = config;
}
```

## What must be configurable

Put in appsettings any value expected to change without code edits, including:

- numeric limits
- retry counts and timeout values
- service endpoints and paths
- header names
- feature behavior strings and switches

If a value is operationally tunable, keep it in configuration and reference it through `ConfigurationKeys`.

## Review heuristics

Treat these as violations:

- string-literal config keys in runtime code
- missing `ConfigurationKeys` constants for new config fields
- runtime service code that reads config without `IAugustusConfiguration`
- required settings read with `Get<T>(...)` or `GetOrDefault<T>(...)` instead of `GetOrThrow<T>(...)`
- changeable operational values hard-coded in code
- constant names that do not follow section/value naming rules
