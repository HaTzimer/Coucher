# Project Organization And Layering

## Solution structure

The repository is organized into these projects:

- `Coacher.WebApi` = API host and transport layer.
- `Coacher.Lib` = business logic and DAL orchestration.
- `Coacher.Shared` = canonical domain and DAL entities, enums, all DI-facing interfaces, and shared extensions.

Dependency direction:

- `Coacher.WebApi` depends on `Coacher.Lib`.
- `Coacher.Lib` depends on `Coacher.Shared`.

Treat that direction as a hard boundary.

## What belongs in each project

### `Coacher.WebApi`

Put this in `Coacher.WebApi`:

- Program and startup wiring.
- DI composition root.
- Controller endpoints.
- Request and transport DTOs.
- Host-only middleware wiring.
- Thin endpoint-layer mapping into Lib or shared models.

Do not put core business logic or reusable DAL logic here.

### `Coacher.Lib`

Put this in `Coacher.Lib`:

- DbContext.
- Providers.
- Repositories.
- Services.
- Factories and other Lib-owned implementation categories.
- GraphQL types and registration.
- Business orchestration and use-case logic.

GraphQL rule:
Keep `Coacher.Lib/Gql/CoacherQuery` in sync with `CoacherDbContext` and expose query fields for every table-backed `DbSet<>` (projection/filter/sort enabled by default). See `AGENTS.md` for the canonical requirement.

### `Coacher.Shared`

Put this in `Coacher.Shared`:

- EF-ready DAL/domain entities.
- Shared enums.
- All DI-facing interfaces and other shared contracts.
- Shared extensions in `Extensions`.

Keep API request DTOs under `Coacher.Shared/Models/WebApi/Requests/...`.

## DTO placement

Web API request models belong in `Coacher.Shared` so they can be reused and kept stable.

Use the request folders already established in the repository:

- `Models/WebApi/Requests/Auth`
- `Models/WebApi/Requests/Exercises`
- `Models/WebApi/Requests/Tasks`
- `Models/WebApi/Requests/Common`

Map those DTOs to DAL/domain models in `WebApi` or `Lib`.

## Core split

- Provider = low-level data access or external integration.
- Repository = business-facing access layer that wraps one or more providers.
- Service = orchestration or use-case layer that calls repositories and sometimes providers directly.

Treat the split as behavioral, not just naming.
Do not default to provider, repository, or service base classes in this repository. Use explicit concrete classes unless the user explicitly asks for shared base abstractions.

## Provider responsibilities

Put this in a provider:

- Raw persistence operations.
- Existence checks.
- Fetch by id or predicate.
- Materialized list and lookup operations.
- Delete or update mechanics.
- EF Core details.
- External API, cache, queue, or database client integration.

Keep providers close to infrastructure semantics. A provider may know how to fetch, save, attach, or delete, but it should not be the primary place for application-facing invariants.
Do not let providers expose `IQueryable` or any other abstraction that allows upper layers to reflect the underlying data source or shape provider execution externally.

Providers may share conventions, but in this repository they should not be forced through a common base class unless the user explicitly requests that design.
SQL-provider implementations may use EF Core queryables internally, but that detail must stay inside the provider.

## Repository responsibilities

Put this in a repository:

- Business-facing access methods.
- Composition over one or more providers.
- Domain-oriented fetch and delete expectations.
- Translation of low-level missing-data outcomes into application/domain exceptions.
- Reusable access behavior that callers should depend on instead of raw persistence APIs.

Repositories should raise the abstraction level above providers. They should not merely rename provider methods unless that wrapper is imposing application semantics.
Repositories also must not expose `IQueryable`. They should return concrete entities, lists, pages, counts, or dedicated response models.

Repositories may share conventions, but in this repository they should not be forced through a common base class unless the user explicitly requests that design.

## Service responsibilities

Put this in a service class:

- Use-case orchestration.
- Calls across multiple repositories.
- Coordination of side effects.
- Workflow-specific branching and sequencing.
- Cases where a provider is called directly because the code is orchestration rather than reusable business-facing access.

Do not move reusable access logic into services just because a service is already touching the data.

## Organized folder pattern

When adding categorized classes, keep the folder structure consistent:

- Provider implementations go in `Coacher.Lib/DAL/Providers`.
- Repository implementations go in `Coacher.Lib/Repositories`.
- Service implementations go in `Coacher.Lib/Services`.
- Factory implementations go in `Coacher.Lib/Factories` when that category exists.
- All DI-facing interfaces go in `Coacher.Shared/Interfaces`.
- Keep shared interfaces organized by category, for example `Interfaces/DAL/Providers`, `Interfaces/Repositories`, `Interfaces/Services`, and `Interfaces/Factories`.
- Apply the same structure to similar categories instead of mixing unrelated types into one folder.

## DI pattern

When adding injectable types in `Coacher.Lib`, use the DI pattern consistently:

- Create the interface in `Coacher.Shared/Interfaces/...`.
- Create the implementation in the matching category folder in `Coacher.Lib`.
- Inject repositories and other services through the constructor.
- Reuse `Augustus.Infra.Core` DI entry points before writing local replacements.
- Use `services.AddGenericAugustusServices()` in `Startup.ConfigureServices(...)` to register shared Augustus infrastructure such as `IAugustusLogger`, `IAugustusConfiguration`, correlation services, and common helpers.
- `services.AddPooledDbContextFactory<CoacherDbContext>(...)` registers `IDbContextFactory<CoacherDbContext>`, not a scoped `CoacherDbContext`.
- Prefer consuming `IDbContextFactory<CoacherDbContext>` (or HotChocolate pooled DbContext wiring) instead of adding a scoped bridge from the factory.
- Register the interface and implementation in `Startup.ConfigureServices(...)`.

Preferred pattern:

```csharp
public interface IExerciseService
{
    Task<List<Exercise>> ListAsync(CancellationToken cancellationToken = default);
}

public sealed class ExerciseService : IExerciseService
{
    private readonly IExerciseRepository _repository;

    public ExerciseService(IExerciseRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Exercise>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.ListAsync(cancellationToken);

        return items;
    }
}
```

Register it in `Startup.ConfigureServices(...)`:

```csharp
services.AddGenericAugustusServices();
services.AddScoped<IExerciseService, ExerciseService>();
```

## Placement rules

Use this decision rule when adding code:

1. If the code is transport or host-only, put it in `WebApi`.
2. If the code is a shared DAL/domain entity or shared enum, put it in `Shared`.
3. If the code talks directly to EF Core, SQL, Mongo, Redis, Elastic, HTTP, or another external system, start in a provider in `Lib`.
4. If the code exposes a business-facing access contract or combines providers into a reusable application-facing operation, put it in a repository in `Lib`.
5. If the code coordinates a use case, workflow, multiple repositories, or multiple side effects, put it in a service class in `Lib`.

## Review heuristics

Treat these as architecture violations:

- A request DTO placed outside `Coacher.Shared/Models/WebApi/Requests/...`.
- A shared entity placed outside `Coacher.Shared`.
- Business logic placed in controllers or startup code instead of `Lib`.
- Services placed outside `Coacher.Lib/Services`.
- Missing DI interfaces for injectable classes that are intended to be resolved through DI.
- DI-facing interfaces placed in `Coacher.Lib` instead of `Coacher.Shared/Interfaces`.
- Shared Augustus infrastructure re-registered manually when `AddGenericAugustusServices()` should have been reused.
- A Lib DI module class introduced even though `Startup` should hold the composition root.
- A redundant scoped DbContext bridge added on top of `AddPooledDbContextFactory` when factory-based consumption would be sufficient.
- Host-specific concerns placed in `Shared`.
- A repository that just mirrors provider method names and signatures with no added semantics.
- A service that accumulates low-level query or persistence logic that should be reusable elsewhere.
- A provider that starts making business-policy decisions.
- A provider or repository that exposes `IQueryable` to callers.
- A new provider, repository, or service base class introduced without an explicit user request.

## Coding style rule

Use an explicit local variable before returning a value.

Prefer:

```csharp
var entity = await provider.GetByIdAsync(id, cancellationToken);
return entity;
```

Do not use:

```csharp
return await provider.GetByIdAsync(id, cancellationToken);
```

## DI guidance

Register each concrete implementation in the layer where it belongs:

- Provider interfaces to provider implementations.
- Repository interfaces to repository implementations.
- Service interfaces to service implementations.

Keep the registrations explicit enough that the layering is visible during startup review.
