using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Models.Enums;
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
        [FromBody] CreateExerciseRequest request,
        CancellationToken cancellationToken
    )
    {
        var exercise = await _exerciseService.CreateExerciseAsync(request, cancellationToken);

        return Ok(exercise);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Exercise>> UpdateAsync(
        Guid id,
        [FromBody] UpdateExerciseRequest request,
        CancellationToken cancellationToken
    )
    {
        var exercise = await _exerciseService.UpdateExerciseAsync(id, request, cancellationToken);

        return Ok(exercise);
    }

    [HttpPut("{id:guid}/details")]
    public async Task<ActionResult<Exercise>> UpdateDetailsAsync(
        Guid id,
        [FromBody] UpdateExerciseDetailsRequest request,
        CancellationToken cancellationToken
    )
    {
        var exercise = await _exerciseService.UpdateExerciseDetailsAsync(id, request, cancellationToken);

        return Ok(exercise);
    }

    [HttpPut("{id:guid}/end-date")]
    public async Task<ActionResult<Exercise>> UpdateEndDateAsync(
        Guid id,
        [FromBody] DateOnly endDate,
        CancellationToken cancellationToken
    )
    {
        var exercise = await _exerciseService.UpdateExerciseEndDateAsync(id, endDate, cancellationToken);

        return Ok(exercise);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<Exercise>> UpdateStatusAsync(
        Guid id,
        [FromBody] Guid statusId,
        CancellationToken cancellationToken
    )
    {
        var exercise = await _exerciseService.UpdateExerciseStatusAsync(id, statusId, cancellationToken);

        return Ok(exercise);
    }

    [HttpPut("{id:guid}/trainee-unit")]
    public async Task<ActionResult<Exercise>> UpdateTraineeUnitAsync(
        Guid id,
        [FromBody] Guid traineeUnitId,
        CancellationToken cancellationToken
    )
    {
        var exercise = await _exerciseService.UpdateExerciseTraineeUnitAsync(
            id,
            traineeUnitId,
            cancellationToken
        );

        return Ok(exercise);
    }

    [HttpPut("{id:guid}/trainer-unit")]
    public async Task<ActionResult<Exercise>> UpdateTrainerUnitAsync(
        Guid id,
        [FromBody] Guid trainerUnitId,
        CancellationToken cancellationToken
    )
    {
        var exercise = await _exerciseService.UpdateExerciseTrainerUnitAsync(
            id,
            trainerUnitId,
            cancellationToken
        );

        return Ok(exercise);
    }

    [HttpPost("{id:guid}/participants")]
    public async Task<ActionResult<ExerciseParticipant>> AddParticipantAsync(
        Guid id,
        [FromBody] AddExerciseParticipantRequest request,
        CancellationToken cancellationToken
    )
    {
        var participant = await _exerciseService.AddExerciseParticipantAsync(id, request, cancellationToken);

        return Ok(participant);
    }

    [HttpPut("participants/{participantId:guid}/role")]
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

    [HttpPut("{id:guid}/manager")]
    public async Task<ActionResult<ExerciseParticipant>> UpdateManagerAsync(
        Guid id,
        [FromBody] string managerUserId,
        CancellationToken cancellationToken
    )
    {
        var participant = await _exerciseService.ReassignExerciseManagerAsync(id, managerUserId, cancellationToken);

        return Ok(participant);
    }

    [HttpPost("{id:guid}/sections")]
    public async Task<ActionResult<ExerciseSection>> AddSectionAsync(
        Guid id,
        [FromBody] AddExerciseSectionRequest request,
        CancellationToken cancellationToken
    )
    {
        var section = await _exerciseService.AddExerciseSectionAsync(id, request, cancellationToken);

        return Ok(section);
    }

    [HttpPost("{id:guid}/influencers")]
    public async Task<ActionResult<ExerciseInfluencer>> AddInfluencerAsync(
        Guid id,
        [FromBody] AddExerciseInfluencerRequest request,
        CancellationToken cancellationToken
    )
    {
        var influencer = await _exerciseService.AddExerciseInfluencerAsync(id, request, cancellationToken);

        return Ok(influencer);
    }

    [HttpPost("{id:guid}/contacts")]
    public async Task<ActionResult<ExerciseUnitContact>> AddContactAsync(
        Guid id,
        [FromBody] AddExerciseUnitContactRequest request,
        CancellationToken cancellationToken
    )
    {
        var contact = await _exerciseService.AddExerciseUnitContactAsync(id, request, cancellationToken);

        return Ok(contact);
    }

    [HttpPut("contacts/{contactId:guid}")]
    public async Task<ActionResult<ExerciseUnitContact>> UpdateContactAsync(
        Guid contactId,
        [FromBody] UpdateExerciseUnitContactRequest request,
        CancellationToken cancellationToken
    )
    {
        var contact = await _exerciseService.UpdateExerciseUnitContactAsync(contactId, request, cancellationToken);

        return Ok(contact);
    }

    [HttpPost("{id:guid}/archive")]
    public async Task<ActionResult<Exercise>> ArchiveAsync(Guid id, CancellationToken cancellationToken)
    {
        var exercise = await _exerciseService.ArchiveExerciseAsync(id, cancellationToken);

        return Ok(exercise);
    }

    [HttpPost("{id:guid}/unarchive")]
    public async Task<ActionResult<Exercise>> UnarchiveAsync(Guid id, CancellationToken cancellationToken)
    {
        var exercise = await _exerciseService.UnarchiveExerciseAsync(id, cancellationToken);

        return Ok(exercise);
    }

    [HttpDelete("participants/{participantId:guid}")]
    public async Task<IActionResult> RemoveParticipantAsync(Guid participantId, CancellationToken cancellationToken)
    {
        await _exerciseService.RemoveExerciseParticipantAsync(participantId, cancellationToken);

        return NoContent();
    }

    [HttpDelete("sections/{sectionLinkId:guid}")]
    public async Task<IActionResult> RemoveSectionAsync(Guid sectionLinkId, CancellationToken cancellationToken)
    {
        await _exerciseService.RemoveExerciseSectionAsync(sectionLinkId, cancellationToken);

        return NoContent();
    }

    [HttpDelete("influencers/{influencerLinkId:guid}")]
    public async Task<IActionResult> RemoveInfluencerAsync(Guid influencerLinkId, CancellationToken cancellationToken)
    {
        await _exerciseService.RemoveExerciseInfluencerAsync(influencerLinkId, cancellationToken);

        return NoContent();
    }

    [HttpDelete("contacts/{contactId:guid}")]
    public async Task<IActionResult> RemoveContactAsync(Guid contactId, CancellationToken cancellationToken)
    {
        await _exerciseService.RemoveExerciseUnitContactAsync(contactId, cancellationToken);

        return NoContent();
    }
}
