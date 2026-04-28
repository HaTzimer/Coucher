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

    [HttpPost("{id:guid}/children")]
    public async Task<ActionResult<ExerciseTask>> CreateChildAsync(
        Guid id,
        [FromBody] CreateExerciseTaskChildRequest request,
        CancellationToken cancellationToken
    )
    {
        var exerciseTask = await _exerciseTaskService.CreateExerciseTaskChildAsync(id, request, cancellationToken);

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

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ExerciseTask>> UpdateAsync(
        Guid id,
        [FromBody] UpdateExerciseTaskRequest request,
        CancellationToken cancellationToken
    )
    {
        var exerciseTask = await _exerciseTaskService.UpdateExerciseTaskAsync(id, request, cancellationToken);

        return Ok(exerciseTask);
    }

    [HttpPost("{id:guid}/dependencies")]
    public async Task<ActionResult<List<TaskDependency>>> AddDependencyAsync(
        Guid id,
        [FromBody] List<string> dependsOnIds,
        CancellationToken cancellationToken
    )
    {
        var dependencies = await _exerciseTaskService.AddExerciseTaskDependenciesAsync(
            id,
            dependsOnIds,
            cancellationToken
        );

        return Ok(dependencies);
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
        [FromBody] string userId,
        CancellationToken cancellationToken
    )
    {
        var responsibleUser = await _exerciseTaskService.AddExerciseTaskResponsibleUserAsync(
            id,
            userId,
            cancellationToken
        );

        return Ok(responsibleUser);
    }

    [HttpPut("{id:guid}/responsible-users")]
    public async Task<ActionResult<List<ExerciseTaskResponsibleUser>>> BulkUpdateResponsibleUsersAsync(
        Guid id,
        [FromBody] List<string> userIds,
        CancellationToken cancellationToken
    )
    {
        var responsibleUsers = await _exerciseTaskService.BulkUpdateExerciseTaskResponsibleUsersAsync(
            id,
            userIds,
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
