using Coacher.Shared.Interfaces.Services;
using Coacher.Shared.Models.DAL.Tasks;
using Coacher.Shared.Models.WebApi.Requests.Tasks;
using Coacher.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Coacher.WebApi.Controllers;

[ApiController]
[Route("api/exercise-task")]
[ServiceFilter(typeof(WebApiSessionAuthenticationFilter))]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[SwaggerTag("Exercise task management endpoints for root task fields and linked collections.")]
public sealed class ExerciseTasksController : ControllerBase
{
    private readonly IExerciseTaskService _exerciseTaskService;

    public ExerciseTasksController(IExerciseTaskService exerciseTaskService)
    {
        _exerciseTaskService = exerciseTaskService;
    }

    [HttpPost("create/single")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ExerciseTask), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Create one exercise task.",
        Description = "Creates a single exercise task under its target exercise."
    )]
    public async Task<ActionResult<ExerciseTask>> CreateAsync(
        [FromBody] CreateExerciseTaskRequest request,
        CancellationToken cancellationToken
    )
    {
        var exerciseTask = await _exerciseTaskService.CreateExerciseTaskAsync(request, cancellationToken);

        return Ok(exerciseTask);
    }

    [HttpPost("{id:guid}/add-child")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ExerciseTask), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Create one child task.",
        Description = "Creates a child exercise task under the given parent task id."
    )]
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
    [Consumes("application/json")]
    [ProducesResponseType(typeof(List<ExerciseTask>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Create many exercise tasks.",
        Description = "Creates multiple exercise tasks in one request, including smart intra-batch dependency wiring where supported."
    )]
    public async Task<ActionResult<List<ExerciseTask>>> BulkCreateAsync(
        [FromBody] List<CreateExerciseTaskRequest> requests,
        CancellationToken cancellationToken
    )
    {
        var exerciseTasks = await _exerciseTaskService.CreateExerciseTasksAsync(requests, cancellationToken);

        return Ok(exerciseTasks);
    }

    [HttpPut("update/{id:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ExerciseTask), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Update root exercise task fields.",
        Description = "Updates only the non-null request fields and uses clear flags for clearable nullable fields such as description and notes."
    )]
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
    [Consumes("application/json")]
    [ProducesResponseType(typeof(List<TaskDependency>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Attach dependencies to a task.",
        Description = "Adds multiple dependency links to the target task from a bulk list of task ids."
    )]
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
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [SwaggerOperation(
        Summary = "Detach dependencies from a task.",
        Description = "Removes multiple dependency link ids from the target task in one bulk operation."
    )]
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
    [Consumes("application/json")]
    [ProducesResponseType(typeof(List<ExerciseTaskResponsibleUser>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Attach responsible users to a task.",
        Description = "Adds multiple responsible user ids to the target task in one bulk operation."
    )]
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
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [SwaggerOperation(
        Summary = "Detach responsible users from a task.",
        Description = "Removes multiple responsibility link ids from the target task in one bulk operation."
    )]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [SwaggerOperation(
        Summary = "Delete a task tree.",
        Description = "Deletes the target task together with its nested child task rows."
    )]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _exerciseTaskService.DeleteExerciseTaskDeepAsync(id, cancellationToken);

        return NoContent();
    }
}
