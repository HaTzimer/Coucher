using Augustus.Infra.Core.Shared.Interfaces;
using Coucher.Shared;
using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.Internal.Authentication;
using Coucher.Shared.Models.WebApi.Requests.Auth;
using Coucher.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Coucher.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly string _sessionIdHeader;
    private readonly string _itemsUserIdKey;

    public AuthenticationController(IAuthenticationService authenticationService, IAugustusConfiguration config)
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

    [HttpPost("login")]
    public async Task<ActionResult<AuthenticatedSession>> LoginAsync(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var session = await _authenticationService.LoginAsync(
            request.IdentityNumber,
            request.PasswordOrPersonalNumber,
            cancellationToken
        );
        if (session is null)
            return Unauthorized("Invalid identity number or password/personal number.");

        return Ok(session);
    }

    [ServiceFilter(typeof(WebApiSessionAuthenticationFilter))]
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken)
    {
        if (!Request.Headers.TryGetValue(_sessionIdHeader, out var sessionId) || string.IsNullOrWhiteSpace(sessionId))
            return Unauthorized("No session id sent.");

        await _authenticationService.LogoutAsync(sessionId.ToString(), cancellationToken);

        return NoContent();
    }

    [ServiceFilter(typeof(WebApiSessionAuthenticationFilter))]
    [HttpGet("session")]
    public ActionResult<AuthenticatedSession> GetCurrentSession()
    {
        if (!Request.Headers.TryGetValue(_sessionIdHeader, out var sessionId) || string.IsNullOrWhiteSpace(sessionId))
            return Unauthorized("No session id sent.");

        if (!TryGetCurrentUserId(out var userId))
            return Unauthorized("No authenticated user found on the request.");

        var session = new AuthenticatedSession
        {
            SessionId = sessionId.ToString(),
            UserId = userId
        };

        return Ok(session);
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        userId = Guid.Empty;
        if (!HttpContext.Items.TryGetValue(_itemsUserIdKey, out var value))
            return false;

        if (value is Guid directUserId)
        {
            userId = directUserId;

            return true;
        }

        if (value is string userIdString && Guid.TryParse(userIdString, out var parsedUserId))
        {
            userId = parsedUserId;

            return true;
        }

        return false;
    }
}
