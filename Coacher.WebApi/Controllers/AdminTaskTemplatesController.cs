using Coacher.Shared.Interfaces.Services;
using Coacher.Shared.Models.DAL.Admin;
using Coacher.Shared.Models.WebApi.Requests.Admin;
using Coacher.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Coacher.WebApi.Controllers;

[ApiController]
[Route("api/admin/task-template")]
[ServiceFilter(typeof(WebApiSessionAuthenticationFilter))]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[SwaggerTag("Administrative task-template management endpoints.")]
public sealed class AdminTaskTemplatesController : ControllerBase
{
    private readonly ITaskTemplateService _taskTemplateService;

    public AdminTaskTemplatesController(ITaskTemplateService taskTemplateService)
    {
        _taskTemplateService = taskTemplateService;
    }

    [HttpPost("create/single")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(TaskTemplate), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Create one task template.",
        Description = "Creates a single reusable task template root record."
    )]
    public async Task<ActionResult<TaskTemplate>> CreateAsync(
        [FromBody] CreateTaskTemplateRequest request,
        CancellationToken cancellationToken
    )
    {
        var taskTemplate = await _taskTemplateService.CreateTaskTemplateAsync(request, cancellationToken);

        return Ok(taskTemplate);
    }

    [HttpPost("create/bulk")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(List<TaskTemplate>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Create many task templates.",
        Description = "Creates multiple reusable task templates in one request."
    )]
    public async Task<ActionResult<List<TaskTemplate>>> BulkCreateAsync(
        [FromBody] List<CreateTaskTemplateRequest> requests,
        CancellationToken cancellationToken
    )
    {
        var taskTemplates = await _taskTemplateService.CreateTaskTemplatesAsync(requests, cancellationToken);

        return Ok(taskTemplates);
    }

    [HttpPut("update/{id:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(TaskTemplate), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Update root task-template fields.",
        Description = "Updates only the non-null request fields of a single task template."
    )]
    public async Task<ActionResult<TaskTemplate>> UpdateAsync(
        Guid id,
        [FromBody] UpdateTaskTemplateRequest request,
        CancellationToken cancellationToken
    )
    {
        var taskTemplate = await _taskTemplateService.UpdateTaskTemplateAsync(id, request, cancellationToken);

        return Ok(taskTemplate);
    }

    [HttpPost("{id:guid}/add-child")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(TaskTemplate), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Create one child task template.",
        Description = "Creates a child task template under the given parent template id."
    )]
    public async Task<ActionResult<TaskTemplate>> AddChildAsync(
        Guid id,
        [FromBody] CreateTaskTemplateChildRequest request,
        CancellationToken cancellationToken
    )
    {
        var taskTemplate = await _taskTemplateService.AddTaskTemplateChildAsync(id, request, cancellationToken);

        return Ok(taskTemplate);
    }

    [HttpPost("{id:guid}/add-dependencies")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(List<TaskTemplateDependency>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Attach dependencies to a template.",
        Description = "Adds multiple dependency links to the target task template in one bulk operation."
    )]
    public async Task<ActionResult<List<TaskTemplateDependency>>> AddDependenciesAsync(
        Guid id,
        [FromBody] List<Guid> dependsOnIds,
        CancellationToken cancellationToken
    )
    {
        var dependencies = await _taskTemplateService.AddTaskTemplateDependenciesAsync(
            id,
            dependsOnIds,
            cancellationToken
        );

        return Ok(dependencies);
    }

    [HttpPost("{id:guid}/add-influencers")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(List<TaskTemplateInfluencer>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Attach influencers to a template.",
        Description = "Adds multiple influencer ids to the target task template in one bulk operation."
    )]
    public async Task<ActionResult<List<TaskTemplateInfluencer>>> AddInfluencersAsync(
        Guid id,
        [FromBody] List<Guid> influencerIds,
        CancellationToken cancellationToken
    )
    {
        var influencers = await _taskTemplateService.AddTaskTemplateInfluencersAsync(
            id,
            influencerIds,
            cancellationToken
        );

        return Ok(influencers);
    }

    [HttpDelete("{id:guid}/remove-dependencies")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [SwaggerOperation(
        Summary = "Detach dependencies from a template.",
        Description = "Removes multiple dependency link ids from the target task template in one bulk operation."
    )]
    public async Task<IActionResult> DeleteDependenciesAsync(
        Guid id,
        [FromBody] List<Guid> dependencyIds,
        CancellationToken cancellationToken
    )
    {
        await _taskTemplateService.DeleteTaskTemplateDependenciesAsync(id, dependencyIds, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}/remove-influencers")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [SwaggerOperation(
        Summary = "Detach influencers from a template.",
        Description = "Removes multiple influencer link ids from the target task template in one bulk operation."
    )]
    public async Task<IActionResult> DeleteInfluencersAsync(
        Guid id,
        [FromBody] List<Guid> influencerLinkIds,
        CancellationToken cancellationToken
    )
    {
        await _taskTemplateService.DeleteTaskTemplateInfluencersAsync(id, influencerLinkIds, cancellationToken);

        return NoContent();
    }

    [HttpPut("archive/{id:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(TaskTemplate), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Set task-template archive state.",
        Description = "Archives the template when the boolean body is true and restores it when the body is false."
    )]
    public async Task<ActionResult<TaskTemplate>> SetArchiveStateAsync(
        Guid id,
        [FromBody] bool isArchived,
        CancellationToken cancellationToken
    )
    {
        var taskTemplate = await _taskTemplateService.SetTaskTemplateArchiveStateAsync(
            id,
            isArchived,
            cancellationToken
        );

        return Ok(taskTemplate);
    }
}
