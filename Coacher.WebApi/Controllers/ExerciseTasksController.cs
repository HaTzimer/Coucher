using Coacher.Shared.Interfaces.Services;
using Coacher.Shared.Models.DAL.Tasks;
using Coacher.Shared.Models.WebApi.Requests.Tasks;
using Coacher.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Coacher.WebApi.Controllers;

[ApiController]
[Route("api/exercise-task")]
[ServiceFilter(typeof(WebApiSessionAuthenticationFilter))]
public sealed class ExerciseTasksController : ControllerBase
{
    private readonly IExerciseTaskService _exerciseTaskService;

    public ExerciseTasksController(IExerciseTaskService exerciseTaskService)
    {
        _exerciseTaskService = exerciseTaskService;
    }

    [HttpPost("create/single")]
    public async Task<ActionResult<ExerciseTask>> CreateAsync(
        [FromBody] CreateExerciseTaskRequest request,
        CancellationToken cancellationToken
    )
    {
        var exerciseTask = await _exerciseTaskService.CreateExerciseTaskAsync(request, cancellationToken);

        return Ok(exerciseTask);
    }

    [HttpPost("{id:guid}/add-child")]
    public async Task<ActionResult<ExerciseTask>> CreateChildAsync(
        Guid id,
        [FromBody] CreateExerciseTaskChildRequest request,
        CancellationToken cancellationToken
    )
    {
        var exerciseTask = await _exerciseTaskService.CreateExerciseTaskChildAsync(id, request, cancellationToken);

        return Ok(exerciseTask);
    }

    [HttpPost("create/bulk")]
    public async Task<ActionResult<List<ExerciseTask>>> BulkCreateAsync(
        [FromBody] List<CreateExerciseTaskRequest> requests,
        CancellationToken cancellationToken
    )
    {
        var exerciseTasks = await _exerciseTaskService.CreateExerciseTasksAsync(requests, cancellationToken);

        return Ok(exerciseTasks);
    }

    [HttpPut("update/{id:guid}")]
    public async Task<ActionResult<ExerciseTask>> UpdateAsync(
        Guid id,
        [FromBody] UpdateExerciseTaskRequest request,
        CancellationToken cancellationToken
    )
    {
        var exerciseTask = await _exerciseTaskService.UpdateExerciseTaskAsync(id, request, cancellationToken);

        return Ok(exerciseTask);
    }

    [HttpPost("{id:guid}/add-dependencies")]
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

    [HttpDelete("{id:guid}/remove-dependencies")]
    public async Task<IActionResult> DeleteDependenciesAsync(
        Guid id,
        [FromBody] List<string> dependencyIds,
        CancellationToken cancellationToken
    )
    {
        await _exerciseTaskService.DeleteExerciseTaskDependenciesAsync(id, dependencyIds, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/add-responsible-users")]
    public async Task<ActionResult<List<ExerciseTaskResponsibleUser>>> AddResponsibleUsersAsync(
        Guid id,
        [FromBody] List<string> userIds,
        CancellationToken cancellationToken
    )
    {
        var responsibleUsers = await _exerciseTaskService.AddExerciseTaskResponsibleUsersAsync(
            id,
            userIds,
            cancellationToken
        );

        return Ok(responsibleUsers);
    }

    [HttpDelete("{id:guid}/remove-responsible-users")]
    public async Task<IActionResult> DeleteResponsibleUsersAsync(
        Guid id,
        [FromBody] List<string> responsibilityIds,
        CancellationToken cancellationToken
    )
    {
        await _exerciseTaskService.DeleteExerciseTaskResponsibleUsersAsync(
            id,
            responsibilityIds,
            cancellationToken
        );

        return NoContent();
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _exerciseTaskService.DeleteExerciseTaskDeepAsync(id, cancellationToken);

        return NoContent();
    }
}
