using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Models.WebApi.Requests.Exercises;
using Coucher.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Coucher.WebApi.Controllers;

[ApiController]
[Route("api/exercises")]
[ServiceFilter(typeof(WebApiSessionAuthenticationFilter))]
public sealed class ExercisesController : ControllerBase
{
    private readonly IExerciseService _exerciseService;

    public ExercisesController(IExerciseService exerciseService)
    {
        _exerciseService = exerciseService;
    }

    [HttpPost]
    public async Task<ActionResult<Exercise>> CreateAsync(
        [FromBody] CreateExerciseRequestModel request,
        CancellationToken cancellationToken
    )
    {
        var exercise = await _exerciseService.CreateExerciseAsync(request, cancellationToken);

        return Ok(exercise);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Exercise>> UpdateAsync(
        Guid id,
        [FromBody] UpdateExerciseRequestModel request,
        CancellationToken cancellationToken
    )
    {
        var exercise = await _exerciseService.UpdateExerciseAsync(id, request, cancellationToken);

        return Ok(exercise);
    }
}
