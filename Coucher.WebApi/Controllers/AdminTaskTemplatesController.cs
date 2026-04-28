using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared.Models.WebApi.Requests.Admin;
using Coucher.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Coucher.WebApi.Controllers;

[ApiController]
[Route("api/admin/task-templates")]
[ServiceFilter(typeof(WebApiSessionAuthenticationFilter))]
public sealed class AdminTaskTemplatesController : ControllerBase
{
    private readonly ITaskTemplateService _taskTemplateService;

    public AdminTaskTemplatesController(ITaskTemplateService taskTemplateService)
    {
        _taskTemplateService = taskTemplateService;
    }

    [HttpPost]
    public async Task<ActionResult<TaskTemplate>> CreateAsync(
        [FromBody] CreateTaskTemplateRequest request,
        CancellationToken cancellationToken
    )
    {
        var taskTemplate = await _taskTemplateService.CreateTaskTemplateAsync(request, cancellationToken);

        return Ok(taskTemplate);
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<List<TaskTemplate>>> BulkCreateAsync(
        [FromBody] List<CreateTaskTemplateRequest> requests,
        CancellationToken cancellationToken
    )
    {
        var taskTemplates = await _taskTemplateService.CreateTaskTemplatesAsync(requests, cancellationToken);

        return Ok(taskTemplates);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskTemplate>> UpdateAsync(
        Guid id,
        [FromBody] UpdateTaskTemplateRequest request,
        CancellationToken cancellationToken
    )
    {
        var taskTemplate = await _taskTemplateService.UpdateTaskTemplateAsync(id, request, cancellationToken);

        return Ok(taskTemplate);
    }

    [HttpPost("{id:guid}/children")]
    public async Task<ActionResult<TaskTemplate>> AddChildAsync(
        Guid id,
        [FromBody] CreateTaskTemplateChildRequest request,
        CancellationToken cancellationToken
    )
    {
        var taskTemplate = await _taskTemplateService.AddTaskTemplateChildAsync(id, request, cancellationToken);

        return Ok(taskTemplate);
    }

    [HttpPost("{id:guid}/dependencies")]
    public async Task<ActionResult<TaskTemplateDependency>> AddDependencyAsync(
        Guid id,
        [FromBody] Guid dependsOnId,
        CancellationToken cancellationToken
    )
    {
        var dependency = await _taskTemplateService.AddTaskTemplateDependencyAsync(id, dependsOnId, cancellationToken);

        return Ok(dependency);
    }

    [HttpPost("{id:guid}/influencers")]
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

    [HttpDelete("dependencies/{dependencyId:guid}")]
    public async Task<IActionResult> DeleteDependencyAsync(Guid dependencyId, CancellationToken cancellationToken)
    {
        await _taskTemplateService.DeleteTaskTemplateDependencyAsync(dependencyId, cancellationToken);

        return NoContent();
    }

    [HttpDelete("influencers/{influencerLinkId:guid}")]
    public async Task<IActionResult> DeleteInfluencerAsync(Guid influencerLinkId, CancellationToken cancellationToken)
    {
        await _taskTemplateService.DeleteTaskTemplateInfluencerAsync(influencerLinkId, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/archive")]
    public async Task<ActionResult<TaskTemplate>> ArchiveAsync(Guid id, CancellationToken cancellationToken)
    {
        var taskTemplate = await _taskTemplateService.ArchiveTaskTemplateAsync(id, cancellationToken);

        return Ok(taskTemplate);
    }

    [HttpPost("{id:guid}/unarchive")]
    public async Task<ActionResult<TaskTemplate>> UnarchiveAsync(Guid id, CancellationToken cancellationToken)
    {
        var taskTemplate = await _taskTemplateService.UnarchiveTaskTemplateAsync(id, cancellationToken);

        return Ok(taskTemplate);
    }
}
