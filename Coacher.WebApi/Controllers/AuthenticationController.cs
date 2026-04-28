using Augustus.Infra.Core.Shared.Interfaces;
using Coacher.Shared;
using Coacher.Shared.Interfaces.Services;
using Coacher.Shared.Models.Internal.Authentication;
using Coacher.Shared.Models.WebApi.Requests.Auth;
using Coacher.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Coacher.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
[SwaggerTag("Authentication and session lifecycle endpoints.")]
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
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AuthenticatedSession), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [SwaggerOperation(
        Summary = "Create a user session.",
        Description = "Authenticates the user by identity number and password or personal number, then returns the active session payload."
    )]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [SwaggerOperation(
        Summary = "Close the current session.",
        Description = "Invalidates the current session id from the request header."
    )]
    public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken)
    {
        if (!Request.Headers.TryGetValue(_sessionIdHeader, out var sessionId) || string.IsNullOrWhiteSpace(sessionId))
            return Unauthorized("No session id sent.");

        await _authenticationService.LogoutAsync(sessionId.ToString(), cancellationToken);

        return NoContent();
    }

    [ServiceFilter(typeof(WebApiSessionAuthenticationFilter))]
    [HttpGet("session")]
    [ProducesResponseType(typeof(AuthenticatedSession), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [SwaggerOperation(
        Summary = "Get the current session.",
        Description = "Returns the authenticated user id together with the current session id header value."
    )]
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
