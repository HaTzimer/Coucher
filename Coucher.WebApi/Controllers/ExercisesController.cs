using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Models.Enums;
using Coucher.Shared.Models.WebApi.Requests.Exercises;
using Coucher.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Coucher.WebApi.Controllers;

[ApiController]
[Route("api/exercise")]
[ServiceFilter(typeof(WebApiSessionAuthenticationFilter))]
public sealed class ExercisesController : ControllerBase
{
    private readonly IExerciseService _exerciseService;

    public ExercisesController(IExerciseService exerciseService)
    {
        _exerciseService = exerciseService;
    }

    [HttpPost("create/single")]
    public async Task<ActionResult<Exercise>> CreateAsync(
        [FromBody] CreateExerciseRequest request,
        CancellationToken cancellationToken
    )
    {
        var exercise = await _exerciseService.CreateExerciseAsync(request, cancellationToken);

        return Ok(exercise);
    }

    [HttpPut("update/{id:guid}")]
    public async Task<ActionResult<Exercise>> UpdateAsync(
        Guid id,
        [FromBody] UpdateExerciseRequest request,
        CancellationToken cancellationToken
    )
    {
        var exercise = await _exerciseService.UpdateExerciseAsync(id, request, cancellationToken);

        return Ok(exercise);
    }

    [HttpPost("{id:guid}/add-participant")]
    public async Task<ActionResult<ExerciseParticipant>> AddParticipantAsync(
        Guid id,
        [FromBody] string userId,
        CancellationToken cancellationToken
    )
    {
        var participant = await _exerciseService.AddExerciseParticipantAsync(id, userId, cancellationToken);

        return Ok(participant);
    }

    [HttpPut("update-participant-role/{participantId:guid}")]
    public async Task<ActionResult<ExerciseParticipant>> UpdateParticipantRoleAsync(
        Guid participantId,
        [FromBody] ExerciseRole role,
        CancellationToken cancellationToken
    )
    {
        var participant = await _exerciseService.UpdateExerciseParticipantRoleAsync(
            participantId,
            role,
            cancellationToken
        );

        return Ok(participant);
    }

    [HttpPut("{id:guid}/set-manager")]
    public async Task<ActionResult<ExerciseParticipant>> UpdateManagerAsync(
        Guid id,
        [FromBody] string managerUserId,
        CancellationToken cancellationToken
    )
    {
        var participant = await _exerciseService.ReassignExerciseManagerAsync(id, managerUserId, cancellationToken);

        return Ok(participant);
    }

    [HttpPost("{id:guid}/add-section")]
    public async Task<ActionResult<ExerciseSection>> AddSectionAsync(
        Guid id,
        [FromBody] Guid sectionId,
        CancellationToken cancellationToken
    )
    {
        var section = await _exerciseService.AddExerciseSectionAsync(id, sectionId, cancellationToken);

        return Ok(section);
    }

    [HttpPost("{id:guid}/add-influencer")]
    public async Task<ActionResult<ExerciseInfluencer>> AddInfluencerAsync(
        Guid id,
        [FromBody] Guid influencerId,
        CancellationToken cancellationToken
    )
    {
        var influencer = await _exerciseService.AddExerciseInfluencerAsync(id, influencerId, cancellationToken);

        return Ok(influencer);
    }

    [HttpPost("{id:guid}/add-contact")]
    public async Task<ActionResult<ExerciseUnitContact>> AddContactAsync(
        Guid id,
        [FromBody] AddExerciseUnitContactRequest request,
        CancellationToken cancellationToken
    )
    {
        var contact = await _exerciseService.AddExerciseUnitContactAsync(id, request, cancellationToken);

        return Ok(contact);
    }

    [HttpPut("update-contact/{contactId:guid}")]
    public async Task<ActionResult<ExerciseUnitContact>> UpdateContactAsync(
        Guid contactId,
        [FromBody] UpdateExerciseUnitContactRequest request,
        CancellationToken cancellationToken
    )
    {
        var contact = await _exerciseService.UpdateExerciseUnitContactAsync(contactId, request, cancellationToken);

        return Ok(contact);
    }

    [HttpPut("archive/{id:guid}")]
    public async Task<ActionResult<Exercise>> SetArchiveStateAsync(
        Guid id,
        [FromBody] bool isArchived,
        CancellationToken cancellationToken
    )
    {
        var exercise = await _exerciseService.SetExerciseArchiveStateAsync(id, isArchived, cancellationToken);

        return Ok(exercise);
    }

    [HttpDelete("remove-participant/{participantId:guid}")]
    public async Task<IActionResult> RemoveParticipantAsync(Guid participantId, CancellationToken cancellationToken)
    {
        await _exerciseService.RemoveExerciseParticipantAsync(participantId, cancellationToken);

        return NoContent();
    }

    [HttpDelete("remove-section/{sectionLinkId:guid}")]
    public async Task<IActionResult> RemoveSectionAsync(Guid sectionLinkId, CancellationToken cancellationToken)
    {
        await _exerciseService.RemoveExerciseSectionAsync(sectionLinkId, cancellationToken);

        return NoContent();
    }

    [HttpDelete("remove-influencer/{influencerLinkId:guid}")]
    public async Task<IActionResult> RemoveInfluencerAsync(Guid influencerLinkId, CancellationToken cancellationToken)
    {
        await _exerciseService.RemoveExerciseInfluencerAsync(influencerLinkId, cancellationToken);

        return NoContent();
    }

    [HttpDelete("remove-contact/{contactId:guid}")]
    public async Task<IActionResult> RemoveContactAsync(Guid contactId, CancellationToken cancellationToken)
    {
        await _exerciseService.RemoveExerciseUnitContactAsync(contactId, cancellationToken);

        return NoContent();
    }
}
