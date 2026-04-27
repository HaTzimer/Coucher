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
        [FromBody] CreateExerciseTaskRequestModel request,
        CancellationToken cancellationToken
    )
    {
        var exerciseTask = await _exerciseTaskService.CreateExerciseTaskAsync(request, cancellationToken);

        return Ok(exerciseTask);
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<List<ExerciseTask>>> BulkCreateAsync(
        [FromBody] List<CreateExerciseTaskRequestModel> requests,
        CancellationToken cancellationToken
    )
    {
        var exerciseTasks = await _exerciseTaskService.CreateExerciseTasksAsync(requests, cancellationToken);

        return Ok(exerciseTasks);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ExerciseTask>> UpdateAsync(
        Guid id,
        [FromBody] UpdateExerciseTaskRequestModel request,
        CancellationToken cancellationToken
    )
    {
        var exerciseTask = await _exerciseTaskService.UpdateExerciseTaskAsync(id, request, cancellationToken);

        return Ok(exerciseTask);
    }
}
