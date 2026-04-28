using Coacher.Shared.Interfaces.Services;
using Coacher.Shared.Models.DAL.Users;
using Coacher.Shared.Models.WebApi.Requests.Admin;
using Coacher.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Coacher.WebApi.Controllers;

[ApiController]
[Route("api/admin/user-role")]
[ServiceFilter(typeof(WebApiSessionAuthenticationFilter))]
public sealed class AdminUserRolesController : ControllerBase
{
    private readonly IUserRoleService _userRoleService;

    public AdminUserRolesController(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    [HttpPut("update/{id:guid}")]
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
