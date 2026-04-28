using Coacher.Shared.Interfaces.Services;
using Coacher.Shared.Models.DAL.Exercises;
using Coacher.Shared.Models.Enums;
using Coacher.Shared.Models.WebApi.Requests.Exercises;
using Coacher.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Coacher.WebApi.Controllers;

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

    [HttpPost("{id:guid}/add-participants")]
    public async Task<ActionResult<List<ExerciseParticipant>>> AddParticipantsAsync(
        Guid id,
        [FromBody] List<string> userIds,
        CancellationToken cancellationToken
    )
    {
        var participants = await _exerciseService.AddExerciseParticipantsAsync(id, userIds, cancellationToken);

        return Ok(participants);
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

    [HttpPost("{id:guid}/add-sections")]
    public async Task<ActionResult<List<ExerciseSection>>> AddSectionsAsync(
        Guid id,
        [FromBody] List<Guid> sectionIds,
        CancellationToken cancellationToken
    )
    {
        var sections = await _exerciseService.AddExerciseSectionsAsync(id, sectionIds, cancellationToken);

        return Ok(sections);
    }

    [HttpPost("{id:guid}/add-influencers")]
    public async Task<ActionResult<List<ExerciseInfluencer>>> AddInfluencersAsync(
        Guid id,
        [FromBody] List<Guid> influencerIds,
        CancellationToken cancellationToken
    )
    {
        var influencers = await _exerciseService.AddExerciseInfluencersAsync(
            id,
            influencerIds,
            cancellationToken
        );

        return Ok(influencers);
    }

    [HttpPost("{id:guid}/add-contacts")]
    public async Task<ActionResult<List<ExerciseUnitContact>>> AddContactsAsync(
        Guid id,
        [FromBody] List<AddExerciseUnitContactRequest> requests,
        CancellationToken cancellationToken
    )
    {
        var contacts = await _exerciseService.AddExerciseUnitContactsAsync(id, requests, cancellationToken);

        return Ok(contacts);
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

    [HttpDelete("{id:guid}/remove-participants")]
    public async Task<IActionResult> RemoveParticipantsAsync(
        Guid id,
        [FromBody] List<Guid> participantIds,
        CancellationToken cancellationToken
    )
    {
        await _exerciseService.RemoveExerciseParticipantsAsync(id, participantIds, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}/remove-sections")]
    public async Task<IActionResult> RemoveSectionsAsync(
        Guid id,
        [FromBody] List<Guid> sectionLinkIds,
        CancellationToken cancellationToken
    )
    {
        await _exerciseService.RemoveExerciseSectionsAsync(id, sectionLinkIds, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}/remove-influencers")]
    public async Task<IActionResult> RemoveInfluencersAsync(
        Guid id,
        [FromBody] List<Guid> influencerLinkIds,
        CancellationToken cancellationToken
    )
    {
        await _exerciseService.RemoveExerciseInfluencersAsync(id, influencerLinkIds, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}/remove-contacts")]
    public async Task<IActionResult> RemoveContactsAsync(
        Guid id,
        [FromBody] List<Guid> contactIds,
        CancellationToken cancellationToken
    )
    {
        await _exerciseService.RemoveExerciseUnitContactsAsync(id, contactIds, cancellationToken);

        return NoContent();
    }
}
