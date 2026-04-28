using Coacher.Shared.Interfaces.Services;
using Coacher.Shared.Models.DAL.Users;
using Coacher.Shared.Models.WebApi.Requests.Admin;
using Coacher.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Coacher.WebApi.Controllers;

[ApiController]
[Route("api/admin/user-role")]
[ServiceFilter(typeof(WebApiSessionAuthenticationFilter))]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[SwaggerTag("Administrative global user-role management endpoints.")]
public sealed class AdminUserRolesController : ControllerBase
{
    private readonly IUserRoleService _userRoleService;

    public AdminUserRolesController(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    [HttpPut("update/{id:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(UserRole), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Update one global user role.",
        Description = "Updates a single persisted global role assignment for a user."
    )]
    public async Task<ActionResult<UserRole>> UpdateAsync(
        Guid id,
        [FromBody] UpdateUserRoleRequest request,
        CancellationToken cancellationToken
    )
    {
        var userRole = await _userRoleService.UpdateUserRoleAsync(id, request, cancellationToken);

        return Ok(userRole);
    }
}
