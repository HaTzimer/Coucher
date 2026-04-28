using Coacher.Shared.Models.DAL.Exercises;

namespace Coacher.Shared.Interfaces.DAL.Providers;

public interface IExerciseProvider : IProviderBase<Exercise, Guid>
{
    Task<Exercise> CreateExerciseAsync(Exercise entity, CancellationToken cancellationToken = default);
    Task<Exercise> UpdateExerciseAsync(Exercise entity, CancellationToken cancellationToken = default);
    Task<Exercise> ArchiveExerciseAsync(Exercise entity, CancellationToken cancellationToken = default);
    Task<Exercise> UnarchiveExerciseAsync(Exercise entity, CancellationToken cancellationToken = default);
    Task<ExerciseParticipant> CreateExerciseParticipantAsync(
        ExerciseParticipant entity,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseSection> CreateExerciseSectionAsync(ExerciseSection entity, CancellationToken cancellationToken = default);
    Task<ExerciseInfluencer> CreateExerciseInfluencerAsync(
        ExerciseInfluencer entity,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseUnitContact> CreateExerciseUnitContactAsync(
        ExerciseUnitContact entity,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseParticipant?> GetExerciseParticipantByIdAsync(Guid participantId, CancellationToken cancellationToken = default);
    Task<List<ExerciseParticipant>> ListExerciseParticipantsByExerciseIdAsync(
        Guid exerciseId,
        CancellationToken cancellationToken = default
    );
    Task<bool> IsExerciseCreatedByUserAsync(Guid exerciseId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsExerciseParticipantAsync(Guid exerciseId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsExerciseManagerAsync(Guid exerciseId, Guid userId, CancellationToken cancellationToken = default);
    Task<ExerciseParticipant> UpdateExerciseParticipantAsync(
        ExerciseParticipant entity,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseSection?> GetExerciseSectionByIdAsync(Guid sectionLinkId, CancellationToken cancellationToken = default);
    Task<ExerciseInfluencer?> GetExerciseInfluencerByIdAsync(
        Guid influencerLinkId,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseUnitContact?> GetExerciseUnitContactByIdAsync(Guid contactId, CancellationToken cancellationToken = default);
    Task<ExerciseUnitContact> UpdateExerciseUnitContactAsync(
        ExerciseUnitContact entity,
        CancellationToken cancellationToken = default
    );
    Task DeleteExerciseParticipantAsync(Guid participantId, CancellationToken cancellationToken = default);
    Task<bool> ExerciseSectionExistsAsync(Guid sectionLinkId, CancellationToken cancellationToken = default);
    Task DeleteExerciseSectionAsync(Guid sectionLinkId, CancellationToken cancellationToken = default);
    Task<bool> ExerciseInfluencerExistsAsync(Guid influencerLinkId, CancellationToken cancellationToken = default);
    Task DeleteExerciseInfluencerAsync(Guid influencerLinkId, CancellationToken cancellationToken = default);
    Task<bool> ExerciseUnitContactExistsAsync(Guid contactId, CancellationToken cancellationToken = default);
    Task DeleteExerciseUnitContactAsync(Guid contactId, CancellationToken cancellationToken = default);
}
