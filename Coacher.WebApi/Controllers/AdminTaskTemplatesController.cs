using Coacher.Shared.Interfaces.Services;
using Coacher.Shared.Models.DAL.Admin;
using Coacher.Shared.Models.WebApi.Requests.Admin;
using Coacher.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Coacher.WebApi.Controllers;

[ApiController]
[Route("api/admin/task-template")]
[ServiceFilter(typeof(WebApiSessionAuthenticationFilter))]
public sealed class AdminTaskTemplatesController : ControllerBase
{
    private readonly ITaskTemplateService _taskTemplateService;

    public AdminTaskTemplatesController(ITaskTemplateService taskTemplateService)
    {
        _taskTemplateService = taskTemplateService;
    }

    [HttpPost("create/single")]
    public async Task<ActionResult<TaskTemplate>> CreateAsync(
        [FromBody] CreateTaskTemplateRequest request,
        CancellationToken cancellationToken
    )
    {
        var taskTemplate = await _taskTemplateService.CreateTaskTemplateAsync(request, cancellationToken);

        return Ok(taskTemplate);
    }

    [HttpPost("create/bulk")]
    public async Task<ActionResult<List<TaskTemplate>>> BulkCreateAsync(
        [FromBody] List<CreateTaskTemplateRequest> requests,
        CancellationToken cancellationToken
    )
    {
        var taskTemplates = await _taskTemplateService.CreateTaskTemplatesAsync(requests, cancellationToken);

        return Ok(taskTemplates);
    }

    [HttpPut("update/{id:guid}")]
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
