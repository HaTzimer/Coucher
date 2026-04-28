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
    Task<ExerciseParticipant> AddExerciseParticipantAsync(
        Guid exerciseId,
        string userId,
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
    Task<ExerciseSection> AddExerciseSectionAsync(
        Guid exerciseId,
        Guid sectionId,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseInfluencer> AddExerciseInfluencerAsync(
        Guid exerciseId,
        Guid influencerId,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseUnitContact> AddExerciseUnitContactAsync(
        Guid exerciseId,
        AddExerciseUnitContactRequest request,
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
    Task RemoveExerciseParticipantAsync(Guid participantId, CancellationToken cancellationToken = default);
    Task RemoveExerciseSectionAsync(Guid sectionLinkId, CancellationToken cancellationToken = default);
    Task RemoveExerciseInfluencerAsync(Guid influencerLinkId, CancellationToken cancellationToken = default);
    Task RemoveExerciseUnitContactAsync(Guid contactId, CancellationToken cancellationToken = default);
}
