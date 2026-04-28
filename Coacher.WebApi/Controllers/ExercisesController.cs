using Coacher.Shared.Interfaces.Services;
using Coacher.Shared.Models.DAL.Exercises;
using Coacher.Shared.Models.Enums;
using Coacher.Shared.Models.WebApi.Requests.Exercises;
using Coacher.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Coacher.WebApi.Controllers;

[ApiController]
[Route("api/exercise")]
[ServiceFilter(typeof(WebApiSessionAuthenticationFilter))]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[SwaggerTag("Exercise management endpoints for root exercise fields and linked collections.")]
public sealed class ExercisesController : ControllerBase
{
    private readonly IExerciseService _exerciseService;

    public ExercisesController(IExerciseService exerciseService)
    {
        _exerciseService = exerciseService;
    }

    [HttpPost("create/single")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Exercise), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Create one exercise.",
        Description = "Creates a single exercise root record with its initial core planning fields."
    )]
    public async Task<ActionResult<Exercise>> CreateAsync(
        [FromBody] CreateExerciseRequest request,
        CancellationToken cancellationToken
    )
    {
        var exercise = await _exerciseService.CreateExerciseAsync(request, cancellationToken);

        return Ok(exercise);
    }

    [HttpPut("update/{id:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Exercise), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Update root exercise fields.",
        Description = "Updates only the non-null fields in the request body. Clear flags are used for clearable nullable fields."
    )]
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
    [Consumes("application/json")]
    [ProducesResponseType(typeof(List<ExerciseParticipant>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Attach participants to an exercise.",
        Description = "Adds multiple user ids to the exercise participant set in one request."
    )]
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
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ExerciseParticipant), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Update one participant role.",
        Description = "Changes the exercise-scoped role of an existing participant link."
    )]
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
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ExerciseParticipant), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Set the exercise manager.",
        Description = "Assigns the given user id as the exercise manager for the target exercise."
    )]
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
    [Consumes("application/json")]
    [ProducesResponseType(typeof(List<ExerciseSection>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Attach sections to an exercise.",
        Description = "Adds multiple section ids to the exercise in one bulk operation."
    )]
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
    [Consumes("application/json")]
    [ProducesResponseType(typeof(List<ExerciseInfluencer>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Attach influencers to an exercise.",
        Description = "Adds multiple influencer ids to the exercise in one bulk operation."
    )]
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
    [Consumes("application/json")]
    [ProducesResponseType(typeof(List<ExerciseUnitContact>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Attach contacts to an exercise.",
        Description = "Creates multiple unit contact rows under the target exercise."
    )]
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
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ExerciseUnitContact), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Update one exercise contact.",
        Description = "Updates the editable fields of one existing exercise unit contact."
    )]
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
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Exercise), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Set exercise archive state.",
        Description = "Archives the exercise when the boolean body is true and restores it when the body is false."
    )]
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
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [SwaggerOperation(
        Summary = "Detach participants from an exercise.",
        Description = "Removes multiple participant link ids from the exercise in one bulk operation."
    )]
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
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [SwaggerOperation(
        Summary = "Detach sections from an exercise.",
        Description = "Removes multiple section link ids from the exercise in one bulk operation."
    )]
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
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [SwaggerOperation(
        Summary = "Detach influencers from an exercise.",
        Description = "Removes multiple influencer link ids from the exercise in one bulk operation."
    )]
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
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [SwaggerOperation(
        Summary = "Detach contacts from an exercise.",
        Description = "Removes multiple contact ids from the exercise in one bulk operation."
    )]
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
