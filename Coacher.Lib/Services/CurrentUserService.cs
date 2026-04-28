using System.Net;
using Augustus.Infra.Core.Shared.Exceptions;
using Augustus.Infra.Core.Shared.Interfaces;
using Coacher.Shared;
using Coacher.Shared.Interfaces.Services;
using Coacher.Shared.Models.Enums;
using Microsoft.AspNetCore.Http;

namespace Coacher.Lib.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IAugustusLogger _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuthenticationService _authenticationService;
    private readonly string _sessionIdHeader;
    private readonly string _itemsUserIdKey;

    public CurrentUserService(
        IAugustusLogger logger,
        IHttpContextAccessor httpContextAccessor,
        IAuthenticationService authenticationService,
        IAugustusConfiguration config
    )
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _authenticationService = authenticationService;
        _sessionIdHeader = config.GetOrThrow<string>(
            ConfigurationKeys.AuthenticationSection,
            ConfigurationKeys.SessionIdHeader
        );
        _itemsUserIdKey = config.GetOrThrow<string>(
            ConfigurationKeys.AuthenticationSection,
            ConfigurationKeys.ItemsUserIdKey
        );
    }

    public async Task<Guid> GetRequiredCurrentUserIdAsync(CancellationToken cancellationToken = default)
    {
        var userId = await TryGetCurrentUserIdAsync(cancellationToken);
        if (userId.HasValue)
            return userId.Value;

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            var exception = new HttpStatusCodeException(
                "Unauthorized request.",
                new Dictionary<string, object?>(),
                HttpStatusCode.Unauthorized
            );

            _logger.Error(exception);

            throw exception;
        }

        httpContext.Request.Headers.TryGetValue(_sessionIdHeader, out var sessionId);
        var authResult = await _authenticationService.AuthenticateSessionAsync(sessionId.ToString(), cancellationToken);
        var unauthorizedException = new HttpStatusCodeException(
            authResult.ErrorMessage ?? "Unauthorized request.",
            new Dictionary<string, object?>
            {
                { "headerAuthentication", authResult.HeaderAuthentication.ToString() }
            },
            HttpStatusCode.Unauthorized
        );

        _logger.Error(unauthorizedException);

        throw unauthorizedException;
    }

    public async Task<Guid?> TryGetCurrentUserIdAsync(CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
            return null;

        if (httpContext.Items.TryGetValue(_itemsUserIdKey, out var cachedUserId))
        {
            if (cachedUserId is Guid userId)
                return userId;

            if (cachedUserId is string userIdString && Guid.TryParse(userIdString, out userId))
                return userId;
        }

        httpContext.Request.Headers.TryGetValue(_sessionIdHeader, out var sessionId);
        if (string.IsNullOrWhiteSpace(sessionId))
            return null;

        var authResult = await _authenticationService.AuthenticateSessionAsync(sessionId.ToString(), cancellationToken);
        if (authResult.HeaderAuthentication != SessionAuthenticationResult.Valid || authResult.UserId is null)
            return null;

        httpContext.Items[_itemsUserIdKey] = authResult.UserId.Value;

        return authResult.UserId.Value;
    }
}

