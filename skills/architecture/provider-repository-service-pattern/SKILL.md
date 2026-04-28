---
name: provider-repository-service-pattern
description: Document and enforce project organization and provider/repository/service layering for the Coacher solution. Use when adding or reviewing code in `Coacher.WebApi`, `Coacher.Lib`, or `Coacher.Shared`, including DAL code, external integrations, repositories, services, DTO placement, DI registrations, and decisions about what belongs in each project.
---

# Project Organization

## Overview

Use this skill to place new code in the correct project and category and preserve the intended separation between transport, business logic, shared models, providers, repositories, services, and other organized class groups.

Read [references/architecture.md](references/architecture.md) before implementing new provider, repository, or service code.

## Project Boundaries

- Put API host and transport concerns in `Coacher.WebApi`.
- Put business logic, DAL orchestration, DbContext, providers, repositories, GraphQL setup, services, factories, and similar implementation categories in `Coacher.Lib`.
- Put canonical DAL/domain entities, enums, all DI-facing interfaces, and shared extensions in `Coacher.Shared`.
- Keep dependency direction one-way: `Coacher.WebApi` -> `Coacher.Lib` -> `Coacher.Shared`.
- Keep Web API request DTOs under `Coacher.Shared/Models/WebApi/Requests/...`.
- Do not put business orchestration into `Coacher.WebApi`.
- Do not put host-specific concerns into `Coacher.Shared`.

## Apply The Layer Boundaries

- Put low-level database and external-system access in providers.
- Put business-facing data access that wraps one or more providers in repositories.
- Put orchestration, use-case flow, and cross-repository coordination in service classes.
- Let services call providers directly only when there is a clear reason and the code is still orchestration rather than reusable access logic.
- Do not expose `IQueryable` from providers or repositories. Return materialized results such as entities, lists, pages, or dedicated result models.
- Do not return expressions directly. Assign the result to a local variable first, then return the variable.
- Do not create or reuse provider, repository, or service base classes unless the user explicitly asks for that pattern.

## Place New Code

1. Decide which project owns the responsibility: `WebApi`, `Lib`, or `Shared`.
2. Decide whether the behavior is provider, repository, service, factory, DTO, shared model, or host wiring.
3. Prefer explicit concrete classes over shared provider, repository, or service base classes.
4. Keep provider methods close to persistence semantics.
5. Keep repository methods close to application semantics.
6. Keep service methods focused on workflows, coordination, and side effects.
7. Register each implementation in DI at the layer where it belongs.

## Folder Structure Rules

- Organize implementation categories into their own folders.
- Put all interfaces in `Coacher.Shared/Interfaces`.
- Keep those interfaces organized by category under `Coacher.Shared/Interfaces`, using patterns such as `Interfaces/DAL/Providers`, `Interfaces/Repositories`, `Interfaces/Services`, and `Interfaces/Factories`.
- Keep implementation classes in `Coacher.Lib` beside their owning category folders.

## DI Rules

- Create an interface and a concrete class for providers, repositories, services, factories, and similar injectable class groups.
- Put all of those interfaces in `Coacher.Shared/Interfaces/...`.
- Put the implementations in the matching category folder in `Coacher.Lib`.
- Reuse `Augustus.Infra.Core` DI entry points before adding local infrastructure registration patterns.
- Use `services.AddGenericAugustusServices()` in `Startup.ConfigureServices(...)` for shared Augustus infrastructure such as `IAugustusLogger`, configuration, correlation, and related common services.
- `services.AddPooledDbContextFactory<CoacherDbContext>(...)` registers `IDbContextFactory<CoacherDbContext>`, not `CoacherDbContext` itself.
- Prefer injecting `IDbContextFactory<CoacherDbContext>` (or HotChocolate pooled DbContext registration) instead of adding a scoped bridge that resolves `CoacherDbContext` from the factory.
- Register Lib implementations explicitly in `Startup.ConfigureServices(...)`.
- Keep the composition root in `Startup` instead of introducing a Lib DI module class.

## DTO And Model Rules

- Put Web API request models under `Coacher.Shared/Models/WebApi/Requests/...`.
- Map Web API request DTOs to DAL/domain entities in `WebApi` or `Lib`.
- Keep EF-ready DAL/domain entities in `Coacher.Shared`.
- Keep shared enums in `Coacher.Shared/Models/Enums`.
- Keep extensions in an `Extensions` folder inside the owning project.

## Review Checklist

- Reject code placed in the wrong project for its responsibility.
- Reject dependency direction violations between `WebApi`, `Lib`, and `Shared`.
- Reject request DTOs added to `Coacher.Shared`.
- Reject business logic that stays in controllers or the Web API host when it belongs in `Lib`.
- Reject service classes without DI interfaces when the project pattern expects them to be injected.
- Reject DI-facing interfaces placed outside `Coacher.Shared/Interfaces`.
- Reject new Lib DI module classes when `Startup` should own the composition root directly.
- Reject redundant `AddScoped` DbContext bridge registrations when pooled factory usage can stay on `IDbContextFactory<CoacherDbContext>`.
- Reject changes that let repositories become thin pass-through wrappers with no application-facing contract.
- Reject changes that push persistence details into services when a repository or provider should own them.
- Reject changes that put EF Core or external client mechanics in repositories if that work belongs in providers.
- Reject providers or repositories that leak data-source semantics such as `IQueryable` to callers.
- Reject direct-expression returns when the local coding rule requires returning through a named variable.
- Reject new provider, repository, or service base classes unless the user explicitly requested them.
- Check DI registrations to ensure provider, repository, and service implementations are all wired consistently.
