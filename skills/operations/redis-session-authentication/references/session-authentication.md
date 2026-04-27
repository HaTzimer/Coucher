# Session Authentication Pattern

## Source Pattern

This skill is based on the existing pattern in:

- `HadrachaSiteServer.WebApi/Gql/GqlQueries.cs`
- `HadrachaSiteServer.WebApi/Startup.cs`
- `Herum.Infra.Core/Services/WebApiSessionAuthenticationFilter.cs`
- `Herum.Infra.Core/Services/AuthenticationService.cs`
- `Herum.Infra.Core/Redis/AuthorizationCacheRedisProvider.cs`

The important split is:

- REST requests are authenticated by an MVC authorization filter.
- GraphQL requests are authenticated inside the GraphQL flow, not by the MVC filter.
- Session state is stored in Redis and resolved back to `UserId`.

## What The Redis Provider Does

`CacheRedisProvider` is the session store in Coucher. Its behavior is the part to preserve when changing Redis-backed session auth.

It keeps two Redis keys:

- `servicePrefix:sessionPrefix:{sessionId}` stores the `UserId` for a session.
- `servicePrefix:userPrefix:{userId}` stores the active `sessionId` for a user.

This gives the service three useful operations:

1. Create a new session quickly.
2. Remove the prior session when only one active session per user is allowed.
3. Resolve a `session-id` header back to `UserId` on every request.

The Hadracha/Herum implementation also expands the session TTL when a valid session is read and the remaining TTL is getting short. That keeps active sessions alive without recreating them on every call.

## Request Flow

1. Login validates credentials and gets a canonical `UserId`.
2. The authentication service asks the Redis provider to create a new server-side session id.
3. The client sends that session id in a request header.
4. REST requests hit `WebApiSessionAuthenticationFilter`, which:
   - reads the session header through the authentication service
   - rejects the request when the session is missing or invalid
   - writes the authenticated `UserId` into `HttpContext.Items`
5. GraphQL requests do not go through that MVC filter. They must call the authentication service from the GraphQL request path and resolve the current user before data access.
6. Business authorization then uses that authenticated `UserId`.

## Why GraphQL Is Different

HotChocolate queries are executed by the GraphQL middleware pipeline. MVC controller filters do not automatically guard those resolvers.

That is why Hadracha authenticates GraphQL queries directly in `GqlQueries` with a helper that calls `AuthenticateHeadersAsync(...)`.

For Coucher, keep the same auth contract but reduce duplication:

- Preferred: authenticate once in a GraphQL request interceptor or a shared current-user service and expose the resolved `UserId` to resolvers.
- Acceptable parity with Hadracha: keep a single private helper or injected helper service that every protected query calls before reading data.

Do not let individual resolvers parse header names themselves.

## Coucher Implementation Plan

### Shared

Add shared contracts and keys in `Coucher.Shared`.

Suggested configuration keys in `ConfigurationKeys.cs`:

- `AuthenticationSection`
- `RedisSection` if Coucher does not already share the Augustus Redis section contract
- `SessionIdHeader`
- `ItemsUserIdKey`
- `ServicePrefix`
- `SessionIdPrefix`
- `UserIdPrefix`
- `SessionDurationInHours`
- `SessionDurationExpansionInSeconds`
- `AuthorizationCacheSection`
- `CacheDurationInSeconds`

Suggested shared models:

- `Models/Internal/Authentication/HeaderAuthenticationResult.cs`
- `Models/Internal/Authentication/UserAuthenticationResult.cs`
- `Models/Enums/SessionAuthenticationResult.cs`

Suggested shared interfaces:

- `Interfaces/DAL/Providers/ICacheProvider.cs`
- `Interfaces/Services/IAuthenticationService.cs`

If login request DTOs are added, keep them in `Coucher.Shared/Models/WebApi/Requests/Auth`.

### Lib

Implement the reusable logic in `Coucher.Lib`.

Suggested files:

- `Coucher.Shared/Interfaces/DAL/Providers/ICacheProvider.cs`
- `DAL/Cache/CacheRedisProvider.cs`
- `Services/AuthenticationService.cs`
- `Services/GraphQlCurrentUserService.cs` or similar shared helper for GraphQL

Provider rules:

- Inherit from `CommonRedisProvider`.
- Use `RedisCommunicationFactory` from `Augustus.Infra.Core`.
- Keep Redis key construction internal to the provider.
- Expose methods equivalent to:
  - `CreateUserSessionAsync`
  - `RemoveUserSessionByUserIdAsync`
  - `GetUserAuthenticationResultBySessionAsync`

Service rules:

- Read the session header name from configuration.
- Validate headers by asking the Redis provider for the session lookup result.
- Return a structured result containing auth status, user id, session id, and error message.
- Keep login/logout orchestration here, not in providers.

### WebApi

Keep transport-specific pieces in `Coucher.WebApi`.

Suggested files:

- `Filters/WebApiSessionAuthenticationFilter.cs`
- `Controllers/AuthenticationController.cs`

Filter rules:

- Inject `IAuthenticationService`.
- Read the `ItemsUserIdKey` from configuration.
- Reject missing or invalid sessions with `UnauthorizedObjectResult`.
- Save the authenticated `UserId` into `HttpContext.Items`.

Startup rules:

- Register `AddHttpContextAccessor()`.
- Register `ICacheProvider`.
- Register `IAuthenticationService`.
- Register `WebApiSessionAuthenticationFilter`.
- Keep Redis primitives coming from `Augustus.Infra.Core`; do not reimplement the base Redis connection machinery.

### GraphQL

`Coucher.Lib/Gql/CoucherQuery.cs` is the current query entry point.

Protected GraphQL queries must not read data first and authenticate later. The resolver path must authenticate first.

Recommended approach for Coucher:

1. Add a shared GraphQL current-user resolver service that uses `IHttpContextAccessor` plus `IAuthenticationService`.
2. Inject that service into protected query methods.
3. Resolve `UserId` before touching the DbContext query.
4. Use that `UserId` for data filtering and future authorization logic.

If an interceptor is added later, keep the same authentication service and shared models. Only move the point where the authenticated `UserId` is attached to the GraphQL request context.

## Design Decisions For Coucher

- Do not add a direct dependency on `Herum.Infra.Core`.
  Recreate the small auth/session layer locally because Coucher already depends on `Augustus.Infra.Core`, which provides the Redis base classes needed for this pattern.
- Do not put ASP.NET filter classes in `Coucher.Shared`.
  They are transport concerns and belong to `Coucher.WebApi`.
- Do not let GraphQL rely on controller filters.
  The authentication service must be reachable from the GraphQL execution path itself.
- Do not hard-code header names, Redis prefixes, or item keys.
  All of those must come from configuration constants in `Coucher.Shared`.

## Review Checklist

- Reject direct references to `Herum.Infra.Core` from the Coucher solution.
- Reject raw string literals for auth config keys in services, filters, or GraphQL code.
- Reject GraphQL resolvers that return data without authenticating the session first.
- Reject duplicated header parsing logic across multiple resolvers.
- Reject session storage implementations that trust a user id sent directly by the client.
- Reject logout flows that remove only one of the two Redis mappings.
- Reject session refresh logic that extends TTL for invalid or missing sessions.
