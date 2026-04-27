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

    [HttpPut("{id:guid}/series")]
    public async Task<ActionResult<TaskTemplate>> UpdateSeriesAsync(
        Guid id,
        [FromBody] UpdateTaskTemplateSeriesRequest request,
        CancellationToken cancellationToken
    )
    {
        var taskTemplate = await _taskTemplateService.UpdateTaskTemplateSeriesAsync(id, request, cancellationToken);

        return Ok(taskTemplate);
    }

    [HttpPut("{id:guid}/category")]
    public async Task<ActionResult<TaskTemplate>> UpdateCategoryAsync(
        Guid id,
        [FromBody] UpdateTaskTemplateCategoryRequest request,
        CancellationToken cancellationToken
    )
    {
        var taskTemplate = await _taskTemplateService.UpdateTaskTemplateCategoryAsync(id, request, cancellationToken);

        return Ok(taskTemplate);
    }

    [HttpPut("{id:guid}/details")]
    public async Task<ActionResult<TaskTemplate>> UpdateDetailsAsync(
        Guid id,
        [FromBody] UpdateTaskTemplateDetailsRequest request,
        CancellationToken cancellationToken
    )
    {
        var taskTemplate = await _taskTemplateService.UpdateTaskTemplateDetailsAsync(id, request, cancellationToken);

        return Ok(taskTemplate);
    }

    [HttpPost("{id:guid}/children")]
    public async Task<ActionResult<TaskTemplate>> AddChildAsync(
        Guid id,
        [FromBody] CreateTaskTemplateRequest request,
        CancellationToken cancellationToken
    )
    {
        var taskTemplate = await _taskTemplateService.AddTaskTemplateChildAsync(id, request, cancellationToken);

        return Ok(taskTemplate);
    }

    [HttpPost("{id:guid}/dependencies")]
    public async Task<ActionResult<TaskTemplateDependency>> AddDependencyAsync(
        Guid id,
        [FromBody] AddTaskTemplateDependencyRequest request,
        CancellationToken cancellationToken
    )
    {
        var dependency = await _taskTemplateService.AddTaskTemplateDependencyAsync(id, request, cancellationToken);

        return Ok(dependency);
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
