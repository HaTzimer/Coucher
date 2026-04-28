using Coacher.Shared.Interfaces.Services;
using Coacher.Shared.Models.Internal.Mocking;
using Coacher.Shared.Models.WebApi.Requests.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Coacher.WebApi.Controllers;

[ApiController]
[Route("api/mock")]
[Produces("application/json")]
[SwaggerTag("Development and local-only mock data seeding endpoints.")]
public sealed class MockSeedController : ControllerBase
{
    private readonly IMockSeedService _mockSeedService;
    private readonly IWebHostEnvironment _environment;

    public MockSeedController(IMockSeedService mockSeedService, IWebHostEnvironment environment)
    {
        _mockSeedService = mockSeedService;
        _environment = environment;
    }

    [HttpPost("seed")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(MockSeedSummary), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MockSeedSummary), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Seed local mock data.",
        Description = "Seeds local or development environments with mock users, exercises, tasks, templates, and notifications."
    )]
    public async Task<ActionResult<MockSeedSummary>> SeedAsync(
        [FromBody] SeedMocksRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!IsAllowedEnvironment(_environment))
            return NotFound();

        var options = new MockSeedOptions
        {
            ResetExisting = request.ResetExisting,
            UserCount = request.UserCount,
            ExerciseCount = request.ExerciseCount,
            AdditionalParticipantsPerExercise = request.AdditionalParticipantsPerExercise,
            TaskTemplateCount = request.TaskTemplateCount,
            TasksPerExercise = request.TasksPerExercise,
            NotificationsPerUser = request.NotificationsPerUser
        };

        var summary = await _mockSeedService.SeedAsync(options, cancellationToken);
        if (!string.IsNullOrWhiteSpace(summary.Note))
            return Conflict(summary);

        return Ok(summary);
    }

    private static bool IsAllowedEnvironment(IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
            return true;

        var isLocal = environment.EnvironmentName.Equals("Local", StringComparison.OrdinalIgnoreCase);

        return isLocal;
    }
}
