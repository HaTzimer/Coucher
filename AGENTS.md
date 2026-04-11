# AGENTS Guide for `Coucher`

## Scope
This repository (`Coucher`) is a .NET 7 Web API solution that references a second project located in a sibling repository:

- `Coucher.WebApi` (local project in this repo)
- `Augustus.Infra.Core` (project path: `..\Infra.Core\Augustus.Infra.Core\Augustus.Infra.Core.csproj`)

Important: `Coucher.WebApi` currently does **not** include a direct `ProjectReference` to `Augustus.Infra.Core` in its `.csproj`, and no code in `Coucher.WebApi` currently uses Infra.Core types.

## What `Augustus.Infra.Core` Provides
`Augustus.Infra.Core` is an infrastructure utility library with:

- Logging and correlation context propagation (`AugustusLogger`, `ContextCorrelationIdProvider`, resource filters)
- Standard exception filters for REST and GraphQL (`ExceptionFilter`, `GqlErrorFilter`)
- Configuration wrappers and helpers (`IAugustusConfiguration`, `BasicConfiguration`, `ConfigurationExtensions`)
- Data/integration helpers (Redis, Kafka, Mongo, S3, SQL DbContext factory)
- Pipeline and rate-limiter utilities

Primary DI entry points:

- `AddGenericAugustusServices(this IServiceCollection services)`
- `AddGenericPipelineServices(this IServiceCollection services)`
- `AddPooledDbContextFactory<T>(this IServiceCollection services, IConfiguration configuration)`

## Required Configuration (When Enabling Infra.Core)
At minimum, logging expects:

- `Logger:serviceName`

Optional but used for UDP log shipping:

- `Logger:remoteAddress`
- `Logger:remotePort`

Useful correlation/config sections used by filters:

- `ResourceFilter:logDebug`
- Correlation headers:
  - `AUGUSTUS_CORRELATION_ID`
  - `MAMRAZ_CORRELATION_ID`
  - `AUGUSTUS_CORRELATION_CREATION_TIMESTAMP`

If using SQL pooled factory:

- `SqlDb:uri`
- `SqlDb:initialCatalog`
- `SqlDb:userId`
- `SqlDb:password`

## Integration Checklist for `Coucher.WebApi`
1. Add a project reference from `Coucher.WebApi` to `..\Infra.Core\Augustus.Infra.Core\Augustus.Infra.Core.csproj`.
2. In `Program.cs`, register Infra.Core services:
   - `builder.Services.AddGenericAugustusServices();`
   - Optional pipeline services: `builder.Services.AddGenericPipelineServices();`
3. Register MVC filters if desired:
   - `options.Filters.AddService<CorrelationIdResourceFilter>();`
   - `options.Filters.AddService<ExceptionFilter>();`
4. Add Swagger correlation header support:
   - `builder.Services.AddSwaggerGen(c => c.OperationFilter<CorrelationHeaderFilter>());`
5. Ensure `appsettings*.json` includes the required keys above.

## Working Rules for Agents
- Treat `..\Infra.Core` as an external sibling codebase; avoid editing it unless explicitly requested.
- Prefer changes inside `Coucher.WebApi` that wire Infra.Core through DI and configuration.
- When adding Infra.Core usage, keep startup fail-fast for missing required config (`GetBySectionOrThrow` behavior).
- Verify with:
  - `dotnet restore Coucher.sln`
  - `dotnet build Coucher.sln`
