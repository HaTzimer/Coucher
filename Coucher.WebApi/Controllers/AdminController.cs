using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared.Models.DAL.Users;
using Coucher.Shared.Models.WebApi.Requests.Admin;
using Coucher.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Coucher.WebApi.Controllers;

[ApiController]
[Route("api/admin")]
[ServiceFilter(typeof(WebApiSessionAuthenticationFilter))]
public sealed class AdminController : ControllerBase
{
    private readonly IClosedListItemService _closedListItemService;
    private readonly ITaskTemplateService _taskTemplateService;
    private readonly IUserRoleService _userRoleService;

    public AdminController(
        IClosedListItemService closedListItemService,
        ITaskTemplateService taskTemplateService,
        IUserRoleService userRoleService
    )
    {
        _closedListItemService = closedListItemService;
        _taskTemplateService = taskTemplateService;
        _userRoleService = userRoleService;
    }

    [HttpPost("closed-list-items")]
    public async Task<ActionResult<ClosedListItem>> CreateClosedListItemAsync(
        [FromBody] CreateClosedListItemRequestModel request,
        CancellationToken cancellationToken
    )
    {
        var closedListItem = await _closedListItemService.CreateClosedListItemAsync(request, cancellationToken);

        return Ok(closedListItem);
    }

    [HttpPost("closed-list-items/bulk")]
    public async Task<ActionResult<List<ClosedListItem>>> CreateClosedListItemsAsync(
        [FromBody] List<CreateClosedListItemRequestModel> requests,
        CancellationToken cancellationToken
    )
    {
        var closedListItems = await _closedListItemService.CreateClosedListItemsAsync(requests, cancellationToken);

        return Ok(closedListItems);
    }

    [HttpPut("closed-list-items/{id:guid}")]
    public async Task<ActionResult<ClosedListItem>> UpdateClosedListItemAsync(
        Guid id,
        [FromBody] UpdateClosedListItemRequestModel request,
        CancellationToken cancellationToken
    )
    {
        var closedListItem = await _closedListItemService.UpdateClosedListItemAsync(id, request, cancellationToken);

        return Ok(closedListItem);
    }

    [HttpPost("task-templates")]
    public async Task<ActionResult<TaskTemplate>> CreateTaskTemplateAsync(
        [FromBody] CreateTaskTemplateRequestModel request,
        CancellationToken cancellationToken
    )
    {
        var taskTemplate = await _taskTemplateService.CreateTaskTemplateAsync(request, cancellationToken);

        return Ok(taskTemplate);
    }

    [HttpPost("task-templates/bulk")]
    public async Task<ActionResult<List<TaskTemplate>>> CreateTaskTemplatesAsync(
        [FromBody] List<CreateTaskTemplateRequestModel> requests,
        CancellationToken cancellationToken
    )
    {
        var taskTemplates = await _taskTemplateService.CreateTaskTemplatesAsync(requests, cancellationToken);

        return Ok(taskTemplates);
    }

    [HttpPut("task-templates/{id:guid}")]
    public async Task<ActionResult<TaskTemplate>> UpdateTaskTemplateAsync(
        Guid id,
        [FromBody] UpdateTaskTemplateRequestModel request,
        CancellationToken cancellationToken
    )
    {
        var taskTemplate = await _taskTemplateService.UpdateTaskTemplateAsync(id, request, cancellationToken);

        return Ok(taskTemplate);
    }

    [HttpPost("user-roles")]
    public async Task<ActionResult<UserRole>> CreateUserRoleAsync(
        [FromBody] CreateUserRoleRequestModel request,
        CancellationToken cancellationToken
    )
    {
        var userRole = await _userRoleService.CreateUserRoleAsync(request, cancellationToken);

        return Ok(userRole);
    }

    [HttpPut("user-roles/{id:guid}")]
    public async Task<ActionResult<UserRole>> UpdateUserRoleAsync(
        Guid id,
        [FromBody] UpdateUserRoleRequestModel request,
        CancellationToken cancellationToken
    )
    {
        var userRole = await _userRoleService.UpdateUserRoleAsync(id, request, cancellationToken);

        return Ok(userRole);
    }
}
