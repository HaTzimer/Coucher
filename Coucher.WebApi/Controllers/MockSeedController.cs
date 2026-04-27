using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.Internal.Mocking;
using Coucher.Shared.Models.WebApi.Requests.Common;
using Microsoft.AspNetCore.Mvc;

namespace Coucher.WebApi.Controllers;

[ApiController]
[Route("api/mock")]
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
    public async Task<ActionResult<MockSeedSummary>> SeedAsync(
        [FromBody] SeedMocksRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!IsAllowedEnvironment(_environment))
        {
            return NotFound();
        }

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
        {
            return Conflict(summary);
        }

        return Ok(summary);
    }

    private static bool IsAllowedEnvironment(IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            return true;
        }

        var isLocal = environment.EnvironmentName.Equals("Local", StringComparison.OrdinalIgnoreCase);

        return isLocal;
    }
}
