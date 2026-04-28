using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared.Models.WebApi.Requests.Admin;
using Coucher.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Coucher.WebApi.Controllers;

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

    [HttpPost("{id:guid}/add-dependency")]
    public async Task<ActionResult<TaskTemplateDependency>> AddDependencyAsync(
        Guid id,
        [FromBody] Guid dependsOnId,
        CancellationToken cancellationToken
    )
    {
        var dependency = await _taskTemplateService.AddTaskTemplateDependencyAsync(id, dependsOnId, cancellationToken);

        return Ok(dependency);
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

    [HttpDelete("remove-dependency/{dependencyId:guid}")]
    public async Task<IActionResult> DeleteDependencyAsync(Guid dependencyId, CancellationToken cancellationToken)
    {
        await _taskTemplateService.DeleteTaskTemplateDependencyAsync(dependencyId, cancellationToken);

        return NoContent();
    }

    [HttpDelete("remove-influencer/{influencerLinkId:guid}")]
    public async Task<IActionResult> DeleteInfluencerAsync(Guid influencerLinkId, CancellationToken cancellationToken)
    {
        await _taskTemplateService.DeleteTaskTemplateInfluencerAsync(influencerLinkId, cancellationToken);

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
