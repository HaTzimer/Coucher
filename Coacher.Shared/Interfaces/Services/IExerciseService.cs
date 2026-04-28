using Coacher.Shared.Models.WebApi.Requests.Exercises;
using Coacher.Shared.Models.DAL.Exercises;
using Coacher.Shared.Models.Enums;

namespace Coacher.Shared.Interfaces.Services;

public interface IExerciseService : IServiceBase<Exercise, Guid>
{
    Task<Exercise> CreateExerciseAsync(CreateExerciseRequest request, CancellationToken cancellationToken = default);
    Task<Exercise> UpdateExerciseAsync(
        Guid exerciseId,
        UpdateExerciseRequest request,
        CancellationToken cancellationToken = default
    );
    Task<List<ExerciseParticipant>> AddExerciseParticipantsAsync(
        Guid exerciseId,
        List<string> userIds,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseParticipant> UpdateExerciseParticipantRoleAsync(
        Guid participantId,
        ExerciseRole role,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseParticipant> ReassignExerciseManagerAsync(
        Guid exerciseId,
        string managerUserId,
        CancellationToken cancellationToken = default
    );
    Task<List<ExerciseSection>> AddExerciseSectionsAsync(
        Guid exerciseId,
        List<Guid> sectionIds,
        CancellationToken cancellationToken = default
    );
    Task<List<ExerciseInfluencer>> AddExerciseInfluencersAsync(
        Guid exerciseId,
        List<Guid> influencerIds,
        CancellationToken cancellationToken = default
    );
    Task<List<ExerciseUnitContact>> AddExerciseUnitContactsAsync(
        Guid exerciseId,
        List<AddExerciseUnitContactRequest> requests,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseUnitContact> UpdateExerciseUnitContactAsync(
        Guid contactId,
        UpdateExerciseUnitContactRequest request,
        CancellationToken cancellationToken = default
    );
    Task<Exercise> SetExerciseArchiveStateAsync(
        Guid exerciseId,
        bool isArchived,
        CancellationToken cancellationToken = default
    );
    Task RemoveExerciseParticipantsAsync(
        Guid exerciseId,
        List<Guid> participantIds,
        CancellationToken cancellationToken = default
    );
    Task RemoveExerciseSectionsAsync(
        Guid exerciseId,
        List<Guid> sectionLinkIds,
        CancellationToken cancellationToken = default
    );
    Task RemoveExerciseInfluencersAsync(
        Guid exerciseId,
        List<Guid> influencerLinkIds,
        CancellationToken cancellationToken = default
    );
    Task RemoveExerciseUnitContactsAsync(
        Guid exerciseId,
        List<Guid> contactIds,
        CancellationToken cancellationToken = default
    );
}
