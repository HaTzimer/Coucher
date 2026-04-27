using Coucher.Shared.Models.WebApi.Requests.Exercises;
using Coucher.Shared.Models.DAL.Exercises;

namespace Coucher.Shared.Interfaces.Services;

public interface IExerciseService : IServiceBase<Exercise, Guid>
{
    Task<Exercise> CreateExerciseAsync(CreateExerciseRequest request, CancellationToken cancellationToken = default);
    Task<Exercise> UpdateExerciseDetailsAsync(
        Guid exerciseId,
        UpdateExerciseDetailsRequest request,
        CancellationToken cancellationToken = default
    );
    Task<Exercise> UpdateExerciseEndDateAsync(
        Guid exerciseId,
        UpdateExerciseEndDateRequest request,
        CancellationToken cancellationToken = default
    );
    Task<Exercise> UpdateExerciseStatusAsync(
        Guid exerciseId,
        UpdateExerciseStatusRequest request,
        CancellationToken cancellationToken = default
    );
    Task<Exercise> UpdateExerciseTraineeUnitAsync(
        Guid exerciseId,
        UpdateExerciseTraineeUnitRequest request,
        CancellationToken cancellationToken = default
    );
    Task<Exercise> UpdateExerciseTrainerUnitAsync(
        Guid exerciseId,
        UpdateExerciseTrainerUnitRequest request,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseParticipant> AddExerciseParticipantAsync(
        Guid exerciseId,
        AddExerciseParticipantRequest request,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseSection> AddExerciseSectionAsync(
        Guid exerciseId,
        AddExerciseSectionRequest request,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseInfluencer> AddExerciseInfluencerAsync(
        Guid exerciseId,
        AddExerciseInfluencerRequest request,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseUnitContact> AddExerciseUnitContactAsync(
        Guid exerciseId,
        AddExerciseUnitContactRequest request,
        CancellationToken cancellationToken = default
    );
    Task<Exercise> ArchiveExerciseAsync(Guid exerciseId, CancellationToken cancellationToken = default);
    Task<Exercise> UnarchiveExerciseAsync(Guid exerciseId, CancellationToken cancellationToken = default);
    Task RemoveExerciseParticipantAsync(Guid participantId, CancellationToken cancellationToken = default);
    Task RemoveExerciseSectionAsync(Guid sectionLinkId, CancellationToken cancellationToken = default);
    Task RemoveExerciseInfluencerAsync(Guid influencerLinkId, CancellationToken cancellationToken = default);
    Task RemoveExerciseUnitContactAsync(Guid contactId, CancellationToken cancellationToken = default);
}
