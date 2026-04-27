# DbContext and DAL Rules

## DbContext role

The `DbContext` should be a thin EF Core entry point in the Lib layer.

Keep it responsible for:

- Accepting `DbContextOptions<TContext>` in the constructor.
- Exposing `DbSet<T>` properties for persisted DAL entities.
- Do not add `DbSet<T>` for projection/read models (for example `Models/Internal/Projections/...`). These must never generate tables in migrations.
- Nothing more unless there is an exceptional case that cannot be expressed with attributes.

Preferred shape:

```csharp
public sealed class CoucherDbContext : DbContext
{
    public CoucherDbContext(DbContextOptions<CoucherDbContext> options)
        : base(options)
    {
    }

    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
}
```

## Mapping rules

Do not configure normal entity mapping in `OnModelCreating`.

That means:

- No `modelBuilder.Entity<T>(...)` blocks for standard entities.
- No `HasKey(...)`.
- No `HasOne(...)` or `HasMany(...)`.
- No `WithOne(...)` or `WithMany(...)`.
- No `HasForeignKey(...)`.
- No `OnDelete(...)`.
- No `ToTable(...)`, `ToView(...)`, or similar mapping calls for normal DAL entities.

Express mapping rules with attributes on the entity classes instead.

Use attributes for:

- Keys
- Foreign keys
- Inverse properties
- Required fields
- String length or column shape when needed
- Table or column naming if needed

## Data rules

Do not seed data in `OnModelCreating`.

Do not put any fixed data, lookup rows, or initialization content there.

If the system needs bootstrap or seed data, handle it through a dedicated initialization flow outside entity mapping.

## SQL fetch rules

When querying SQL, fetch only the data that is actually needed.

Do not:

- Load a full entity when only a few fields are required.
- Pull navigation graphs that the caller does not use.
- Waste database, network, and memory resources on unused columns.

Preferred approach:

1. Define a dedicated internal DAL model for the needed shape.
2. Put that model under `Coucher.Shared/Models/DAL/Internal`.
3. Use `Select(...)` to project directly into that model.
4. Return only that model or a list of that model from the provider.

Preferred example:

```csharp
var items = await DbContext.Exercises
    .Select(item => new ExerciseListItemInternalModel
    {
        Id = item.Id,
        Name = item.Name,
        Status = item.Status
    })
    .ToListAsync(cancellationToken);

return items;
```

Avoid:

```csharp
var items = await DbContext.Exercises.ToListAsync(cancellationToken);

return items;
```

if the caller only needs `Id`, `Name`, and `Status`.

## Provider DbContext access

In this repository, providers should access EF through `IDbContextFactory<CoucherDbContext>`.

Preferred provider pattern:

```csharp
private readonly IDbContextFactory<CoucherDbContext> _dbContextFactory;

public ExerciseProvider(IDbContextFactory<CoucherDbContext> dbContextFactory)
{
    _dbContextFactory = dbContextFactory;
}

public async Task<List<ExerciseListItemInternalModel>> ListAsync(CancellationToken cancellationToken = default)
{
    await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

    var entities = dbContext.Set<Exercise>();
    var items = await entities
        .Select(item => new ExerciseListItemInternalModel
        {
            Id = item.Id,
            Name = item.Name
        })
        .ToListAsync(cancellationToken);

    return items;
}
```

Avoid direct provider injection of `CoucherDbContext` when using pooled factory registration.
Prefer `CreateDbContextAsync(cancellationToken)` with `await using`.
After the `await using` line, leave an empty line.
Keep provider constructors minimal: only assign `_dbContextFactory`.
Resolve `DbSet<T>` once per method and reuse that local variable.

## Query shape in providers

Do not create generic `BuildQuery()` helper methods in providers by default.

Build each query inline inside the method so the required shape is explicit and easy to review.

Do not add broad `Include(...)` chains unless the caller explicitly needs that navigation graph.
If only a subset is needed, project directly to the required model and avoid eager-loading unrelated data.

## Internal DAL models

Use `Coucher.Shared/Models/DAL/Internal` for SQL-only projection shapes that are not public domain entities.

Use them for:

- List item shapes
- Lookup shapes
- Partial read models
- Projection-only results used by providers or repositories

Do not overload the main DAL entities with responsibilities meant only for partial SQL reads.

## Set-based writes

When deleting or updating rows in SQL, prefer set-based EF Core operations.

Preferred:

- `ExecuteDeleteAsync(...)`
- `ExecuteUpdateAsync(...)`

Use them when the operation is a direct SQL-side delete or update and does not require loading entities first.

Avoid loading entities just to delete or update them row by row when a set-based statement is enough.

## Entity ownership

- DAL entities live in `Coucher.Shared`.
- EF-ready shape belongs on the entity itself through attributes.
- The Lib layer owns the `DbContext` and persistence orchestration.
- The Web API layer must not own DAL mapping.

## Review heuristics

Treat these as violations:

- A `DbContext` full of fluent mapping blocks.
- Relationship rules expressed in `OnModelCreating` instead of on the entity classes.
- Seed data or default rows configured in `OnModelCreating`.
- DAL entities that cannot be understood without reading a large fluent mapping section.
- SQL reads that fetch more columns or entities than the caller needs.
- Missing internal projection models for repeated partial-read shapes.
- Row-by-row delete or update logic where `ExecuteDeleteAsync(...)` or `ExecuteUpdateAsync(...)` should be used.
- Providers that inject `CoucherDbContext` directly instead of using `IDbContextFactory<CoucherDbContext>`.
- Providers that use synchronous factory creation when async factory creation with cancellation can be used.
- Providers with constructors that do more than assigning `_dbContextFactory`.
- Missing empty lines after `await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);`.
- Repeated `dbContext.Set<T>()` calls in a method instead of one local `DbSet<T>` variable.
- Provider `BuildQuery()` patterns and broad eager-loading include chains without explicit need.

## Current direction

If existing code uses `OnModelCreating` fluent configuration, treat that as legacy to be reduced over time.

The target style for this repository is:

1. Attributes on entities.
2. Thin `DbContext`.
3. No mapping data in `OnModelCreating`.
4. SQL projections only fetch required data.
5. Partial SQL read shapes live in `DAL/Internal`.
6. Set-based deletes and updates use EF Core bulk operations.
