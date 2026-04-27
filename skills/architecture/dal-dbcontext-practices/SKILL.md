---
name: dal-dbcontext-practices
description: Define how DAL entities and DbContext classes should be built in this repository. Use when creating or editing EF Core entities, DbContext classes, SQL-backed DAL code, entity relationships, keys, delete behaviors, table mapping, or persistence-layer conventions.
---

# DAL DbContext Practices

## Overview

Use this skill when working on EF Core DAL code so the `DbContext`, entity classes, and mapping rules stay consistent across the repository.

Read [references/dbcontext-rules.md](references/dbcontext-rules.md) before changing DAL entities or the `DbContext`.

## Core Rules

- Keep the `DbContext` in the Lib layer.
- Keep DAL entities in the Shared layer.
- Expose one `DbSet<T>` per persisted DAL entity.
- Do not add `DbSet<T>` for projection/read-model classes (for example `Models/Internal/Projections/...`). Projection models must never generate tables in migrations.
- Do not configure entity mapping, relationships, keys, delete behavior, or seed data in `OnModelCreating`.
- Rely on entity attributes for EF mapping instead of fluent configuration.
- Treat `OnModelCreating` as disallowed for normal entity mapping in this repository.
- When using SQL, fetch only the data actually needed by the caller.
- Do not load full entities when only a subset of fields is required.
- Create dedicated internal DAL projection models for partial SQL reads under `Coucher.Shared/Models/DAL/Internal`.
- Use `Select(...)` into those internal DAL models so SQL returns only the required fields.
- For deletes and set-based updates, prefer `ExecuteDeleteAsync(...)` and `ExecuteUpdateAsync(...)`.
- In providers, use `IDbContextFactory<CoucherDbContext>` as the DbContext access pattern.
- Create provider DbContext instances with:
  `await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);`
- After async DbContext creation, add an empty line before further statements.
- Keep provider constructors minimal and assign only `_dbContextFactory`, for example:
  `private readonly IDbContextFactory<CoucherDbContext> _dbContextFactory;`
  `public UserNotificationProvider(IDbContextFactory<CoucherDbContext> dbContextFactory) { _dbContextFactory = dbContextFactory; }`
- After creating the DbContext in a method, resolve the target `DbSet<T>` once and reuse that local variable in the method body.
- Do not use `BuildQuery` helper methods in providers by default.
- Do not add broad `Include(...)` chains unless the caller explicitly needs those navigation graphs.

## Review Checklist

- Reject `modelBuilder.Entity<T>(...)` mapping blocks for normal DAL entities.
- Reject `HasKey`, `HasOne`, `HasMany`, `WithOne`, `WithMany`, `HasForeignKey`, or `OnDelete` in `OnModelCreating` for regular entity configuration.
- Reject data seeding in `OnModelCreating`.
- Reject DAL entities that require fluent configuration when the same rule can be expressed with attributes.
- Reject SQL reads that fetch extra columns or full entities when only part of the data is needed.
- Reject ad hoc anonymous projections when the same shape should live as a reusable DAL internal model.
- Reject row-by-row delete or update flows when a set-based `ExecuteDeleteAsync(...)` or `ExecuteUpdateAsync(...)` should be used.
- Check that the `DbContext` remains a thin registry of `DbSet<T>` properties and constructor wiring.
- Reject provider code that injects `CoucherDbContext` directly instead of using `IDbContextFactory<CoucherDbContext>`.
- Reject synchronous factory usage when `CreateDbContextAsync(cancellationToken)` can be used.
- Reject provider constructors that do more than assigning `_dbContextFactory`.
- Reject missing empty lines after async DbContext factory creation statements.
- Reject repeated `dbContext.Set<T>()` calls in a method when one local `DbSet<T>` variable can be reused.
- Reject `BuildQuery` provider patterns and unnecessary eager-loading include graphs.
