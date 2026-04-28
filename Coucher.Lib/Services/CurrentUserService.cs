using System.Security.Authentication;
using Augustus.Infra.Core.Shared.Interfaces;
using Coucher.Shared;
using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.Enums;
using Microsoft.AspNetCore.Http;

namespace Coucher.Lib.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuthenticationService _authenticationService;
    private readonly string _sessionIdHeader;
    private readonly string _itemsUserIdKey;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        IAuthenticationService authenticationService,
        IAugustusConfiguration config
    )
    {
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
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new AuthenticationException("Missing HTTP context.");

        if (httpContext.Items.TryGetValue(_itemsUserIdKey, out var cachedUserId))
        {
            if (cachedUserId is Guid userId)
                return userId;

            if (cachedUserId is string userIdString && Guid.TryParse(userIdString, out userId))
                return userId;
        }

        httpContext.Request.Headers.TryGetValue(_sessionIdHeader, out var sessionId);
        var authResult = await _authenticationService.AuthenticateSessionAsync(sessionId.ToString(), cancellationToken);
        if (authResult.HeaderAuthentication != SessionAuthenticationResult.Valid || authResult.UserId is null)
            throw new AuthenticationException(authResult.ErrorMessage ?? "Unauthorized request.");

        httpContext.Items[_itemsUserIdKey] = authResult.UserId.Value;

        return authResult.UserId.Value;
    }
}
