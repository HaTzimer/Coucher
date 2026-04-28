---
name: configuration-use-practices
description: Enforce configuration access and key-definition rules in this repository. Use when adding or editing appsettings values, configuration retrieval code, service/controller settings, feature toggles, numeric limits, and any runtime-changeable value.
---

# Configuration Use Practices

Read [references/configuration-rules.md](references/configuration-rules.md) before adding or changing configuration code.

## Core Rules

- Define JSON key constants in `Coacher.Shared` in a dedicated `ConfigurationKeys.cs` file.
- Read configuration through `IAugustusConfiguration` (not raw `IConfiguration` in runtime services).
- Use Augustus throw-on-missing access with section + key constants (`config.GetOrThrow<T>(section, key)`).
- Do not use raw string literals for configuration section names or keys in runtime code.
- Put all changeable values in appsettings (numbers, limits, endpoints, headers, feature text, and similar runtime-tunable values).

## Key Naming Rules

- If a constant represents a section key, use `Section` suffix.
`public const string GqlQueriesSection = "GqlQueries";`
- If a constant represents a value key under a section, use `TitleCase` without `Section` suffix.
`public const string ResourceIdsHeader = "resourceIdsHeader";`
- Keep constant names readable and stable; keep constant values exactly aligned with JSON key names.

## Retrieval Pattern

Prefer:

```csharp
var resourceDuplicationParallelismDegree = config.GetOrThrow<int>(
    ConfigurationKeys.HadrachaPageDuplicationServiceSection,
    ConfigurationKeys.ResourceDuplicationParallelismDegree
);
```

Do not use:

```csharp
var value = configuration["HadrachaPageDuplicationService:resourceDuplicationParallelismDegree"];
```

## Placement Rules

- Keep configuration key constants in `Coacher.Shared` so all layers reuse the same contract.
- Keep runtime configuration values in `Coacher.WebApi/appsettings*.json` unless a different host explicitly owns them.
- Keep defaults and fallback behavior explicit in code, but keep the actual tunable values in config files.

## Review Checklist

- Reject hard-coded configuration keys in services, providers, controllers, and startup.
- Reject missing `ConfigurationKeys` constants for new appsettings keys.
- Reject runtime service code that bypasses `IAugustusConfiguration` for configuration reads.
- Reject `config.Get<T>(...)` or `config.GetOrDefault<T>(...)` for required settings where `config.GetOrThrow<T>(...)` should be used.
- Reject runtime-tunable values hard-coded in code when they should be in appsettings.
- Reject section constants without `Section` suffix.
- Reject value constants that do not follow readable TitleCase naming.
