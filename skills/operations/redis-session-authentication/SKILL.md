---
name: redis-session-authentication
description: Implement and maintain Redis usage in this repository, especially Redis-backed session authentication. Use when adding login or logout flows, creating server-side sessions, validating a `session-id` request header, securing REST controllers with an authorization filter, securing HotChocolate GraphQL queries with current-user resolution, or wiring related Redis/auth configuration, cache providers, and DI.
---

# Redis Session Authentication

Read [references/session-authentication.md](references/session-authentication.md) before changing session auth.

## Core Rules

- Mirror the Herum/Hadracha pattern locally; do not add a project reference to `Herum.Infra.Core`.
- Reuse Redis primitives from `..\Infra.Core\Augustus.Infra.Core`, especially `CommonRedisProvider`, `RedisCommunicationFactory`, `IAugustusConfiguration`, and `IAugustusLogger`.
- Keep WebApi transport concerns in `Coacher.WebApi`, reusable auth/session logic in `Coacher.Lib`, and shared models/interfaces/configuration keys in `Coacher.Shared`.
- Put cache provider implementations under `Coacher.Lib/DAL/Cache`.
- Put cache provider contracts under `Coacher.Shared/Interfaces/DAL/Providers`.
- Use a server-generated session id stored in Redis. Do not replace this flow with JWT or ASP.NET cookie auth unless the user explicitly requests an auth-model change.
- Validate the session header once per REST request in `WebApiSessionAuthenticationFilter`.
- Do not assume MVC filters protect GraphQL. Every protected GraphQL request must resolve the current user through the authentication service before returning data.
- Centralize GraphQL current-user resolution in one helper, interceptor, or service. Do not duplicate raw header parsing across resolvers.
- Define auth and Redis configuration keys only in `Coacher.Shared/ConfigurationKeys.cs`.
- Put stable non-config constants only in `Coacher.Shared/ConstantValues.cs`.

## Implementation Flow

1. Add shared auth contracts and configuration keys.
2. Add a Redis-backed cache provider in `Coacher.Lib/DAL/Cache` with its interface in `Coacher.Shared/Interfaces/DAL/Providers`.
3. Add an authentication service in `Coacher.Lib`.
4. Add `WebApiSessionAuthenticationFilter` in `Coacher.WebApi`.
5. Add GraphQL current-user resolution and make protected queries use it before data access.
6. Register the provider, services, filter, and HTTP context access in `Startup`.
7. Add login/logout endpoints and tests when the authentication feature is implemented.

## Coacher Defaults

- Prefer `session-id` as the request header name unless the product owner specifies a different contract.
- Store the authenticated user id in `HttpContext.Items` under a configured key, not a hard-coded string.
- Prefer `ICacheProvider` for the local Redis contract and `CacheRedisProvider` for the `CommonRedisProvider` implementation when the storage concern is session cache behavior.
- Keep both Redis mappings so the service can validate sessions and optionally enforce one active session per user:
  - `servicePrefix:sessionPrefix:{sessionId} -> userId`
  - `servicePrefix:userPrefix:{userId} -> sessionId`
- Expand session TTL on successful lookup when the remaining TTL is below the configured expansion threshold.
