using Augustus.Infra.Core.Shared.Interfaces;
using Coacher.Shared;
using Coacher.Shared.Interfaces.Services;
using Coacher.Shared.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Coacher.WebApi.Filters;

public sealed class WebApiSessionAuthenticationFilter : IAsyncAuthorizationFilter
{
    private readonly IAuthenticationService _authenticationService;
    private readonly string _sessionIdHeader;
    private readonly string _itemsUserIdKey;

    public WebApiSessionAuthenticationFilter(
        IAugustusConfiguration config,
        IAuthenticationService authenticationService
    )
    {
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

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        context.HttpContext.Request.Headers.TryGetValue(_sessionIdHeader, out var sessionId);
        var authenticationResult = await _authenticationService.AuthenticateSessionAsync(sessionId.ToString());
        if (authenticationResult.HeaderAuthentication != SessionAuthenticationResult.Valid || authenticationResult.UserId is null)
        {
            context.Result = new UnauthorizedObjectResult(
                authenticationResult.ErrorMessage ?? "Unauthorized request."
            );

            return;
        }

        context.HttpContext.Items[_itemsUserIdKey] = authenticationResult.UserId.Value;
    }
}
