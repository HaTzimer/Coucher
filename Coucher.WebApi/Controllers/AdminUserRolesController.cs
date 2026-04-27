using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.DAL.Users;
using Coucher.Shared.Models.WebApi.Requests.Admin;
using Coucher.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Coucher.WebApi.Controllers;

[ApiController]
[Route("api/admin/user-roles")]
[ServiceFilter(typeof(WebApiSessionAuthenticationFilter))]
public sealed class AdminUserRolesController : ControllerBase
{
    private readonly IUserRoleService _userRoleService;

    public AdminUserRolesController(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    [HttpPut("{id:guid}")]
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
