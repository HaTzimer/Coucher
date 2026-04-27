using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.DAL.Tasks;
using Coucher.Shared.Models.WebApi.Requests.Tasks;
using Coucher.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Coucher.WebApi.Controllers;

[ApiController]
[Route("api/exercise-tasks")]
[ServiceFilter(typeof(WebApiSessionAuthenticationFilter))]
public sealed class ExerciseTasksController : ControllerBase
{
    private readonly IExerciseTaskService _exerciseTaskService;

    public ExerciseTasksController(IExerciseTaskService exerciseTaskService)
    {
        _exerciseTaskService = exerciseTaskService;
    }

    [HttpPost]
    public async Task<ActionResult<ExerciseTask>> CreateAsync(
        [FromBody] CreateExerciseTaskRequest request,
        CancellationToken cancellationToken
    )
    {
        var exerciseTask = await _exerciseTaskService.CreateExerciseTaskAsync(request, cancellationToken);

        return Ok(exerciseTask);
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<List<ExerciseTask>>> BulkCreateAsync(
        [FromBody] List<CreateExerciseTaskRequest> requests,
        CancellationToken cancellationToken
    )
    {
        var exerciseTasks = await _exerciseTaskService.CreateExerciseTasksAsync(requests, cancellationToken);

        return Ok(exerciseTasks);
    }

    [HttpPut("{id:guid}/series")]
    public async Task<ActionResult<ExerciseTask>> UpdateSeriesAsync(
        Guid id,
        [FromBody] UpdateExerciseTaskSeriesRequest request,
        CancellationToken cancellationToken
    )
    {
        var exerciseTask = await _exerciseTaskService.UpdateExerciseTaskSeriesAsync(id, request, cancellationToken);

        return Ok(exerciseTask);
    }

    [HttpPut("{id:guid}/category")]
    public async Task<ActionResult<ExerciseTask>> UpdateCategoryAsync(
        Guid id,
        [FromBody] UpdateExerciseTaskCategoryRequest request,
        CancellationToken cancellationToken
    )
    {
        var exerciseTask = await _exerciseTaskService.UpdateExerciseTaskCategoryAsync(id, request, cancellationToken);

        return Ok(exerciseTask);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<ExerciseTask>> UpdateStatusAsync(
        Guid id,
        [FromBody] UpdateExerciseTaskStatusRequest request,
        CancellationToken cancellationToken
    )
    {
        var exerciseTask = await _exerciseTaskService.UpdateExerciseTaskStatusAsync(id, request, cancellationToken);

        return Ok(exerciseTask);
    }

    [HttpPut("{id:guid}/details")]
    public async Task<ActionResult<ExerciseTask>> UpdateDetailsAsync(
        Guid id,
        [FromBody] UpdateExerciseTaskDetailsRequest request,
        CancellationToken cancellationToken
    )
    {
        var exerciseTask = await _exerciseTaskService.UpdateExerciseTaskDetailsAsync(id, request, cancellationToken);

        return Ok(exerciseTask);
    }

    [HttpPost("{id:guid}/dependencies")]
    public async Task<ActionResult<TaskDependency>> AddDependencyAsync(
        Guid id,
        [FromBody] AddExerciseTaskDependencyRequest request,
        CancellationToken cancellationToken
    )
    {
        var dependency = await _exerciseTaskService.AddExerciseTaskDependencyAsync(id, request, cancellationToken);

        return Ok(dependency);
    }

    [HttpDelete("dependencies/{dependencyId:guid}")]
    public async Task<IActionResult> DeleteDependencyAsync(Guid dependencyId, CancellationToken cancellationToken)
    {
        await _exerciseTaskService.DeleteExerciseTaskDependencyAsync(dependencyId, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/responsible-users")]
    public async Task<ActionResult<ExerciseTaskResponsibleUser>> AddResponsibleUserAsync(
        Guid id,
        [FromBody] AddExerciseTaskResponsibleUserRequest request,
        CancellationToken cancellationToken
    )
    {
        var responsibleUser = await _exerciseTaskService.AddExerciseTaskResponsibleUserAsync(
            id,
            request,
            cancellationToken
        );

        return Ok(responsibleUser);
    }

    [HttpPut("{id:guid}/responsible-users")]
    public async Task<ActionResult<List<ExerciseTaskResponsibleUser>>> BulkUpdateResponsibleUsersAsync(
        Guid id,
        [FromBody] BulkUpdateExerciseTaskResponsibleUsersRequest request,
        CancellationToken cancellationToken
    )
    {
        var responsibleUsers = await _exerciseTaskService.BulkUpdateExerciseTaskResponsibleUsersAsync(
            id,
            request,
            cancellationToken
        );

        return Ok(responsibleUsers);
    }

    [HttpDelete("responsible-users/{responsibilityId:guid}")]
    public async Task<IActionResult> DeleteResponsibleUserAsync(Guid responsibilityId, CancellationToken cancellationToken)
    {
        await _exerciseTaskService.DeleteExerciseTaskResponsibleUserAsync(responsibilityId, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/responsible-users/bulk-delete")]
    public async Task<IActionResult> BulkDeleteResponsibleUsersAsync(
        Guid id,
        [FromBody] BulkDeleteExerciseTaskResponsibleUsersRequest request,
        CancellationToken cancellationToken
    )
    {
        await _exerciseTaskService.BulkDeleteExerciseTaskResponsibleUsersAsync(id, request, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _exerciseTaskService.DeleteExerciseTaskDeepAsync(id, cancellationToken);

        return NoContent();
    }
}
