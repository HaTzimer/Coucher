using Coacher.Shared.Models.DAL.Exercises;

namespace Coacher.Shared.Interfaces.DAL.Providers;

public interface IExerciseProvider : IProviderBase<Exercise, Guid>
{
    Task<Exercise> CreateExerciseAsync(Exercise entity, CancellationToken cancellationToken = default);
    Task<Exercise> UpdateExerciseAsync(Exercise entity, CancellationToken cancellationToken = default);
    Task<Exercise> ArchiveExerciseAsync(Exercise entity, CancellationToken cancellationToken = default);
    Task<Exercise> UnarchiveExerciseAsync(Exercise entity, CancellationToken cancellationToken = default);
    Task<List<ExerciseParticipant>> CreateExerciseParticipantsAsync(
        List<ExerciseParticipant> entities,
        CancellationToken cancellationToken = default
    );
    Task<List<ExerciseSection>> CreateExerciseSectionsAsync(
        List<ExerciseSection> entities,
        CancellationToken cancellationToken = default
    );
    Task<List<ExerciseInfluencer>> CreateExerciseInfluencersAsync(
        List<ExerciseInfluencer> entities,
        CancellationToken cancellationToken = default
    );
    Task<List<ExerciseUnitContact>> CreateExerciseUnitContactsAsync(
        List<ExerciseUnitContact> entities,
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
    Task DeleteExerciseParticipantsAsync(
        Guid exerciseId,
        List<Guid> participantIds,
        CancellationToken cancellationToken = default
    );
    Task<bool> ExerciseSectionExistsAsync(Guid sectionLinkId, CancellationToken cancellationToken = default);
    Task DeleteExerciseSectionsAsync(
        Guid exerciseId,
        List<Guid> sectionLinkIds,
        CancellationToken cancellationToken = default
    );
    Task<bool> ExerciseInfluencerExistsAsync(Guid influencerLinkId, CancellationToken cancellationToken = default);
    Task DeleteExerciseInfluencersAsync(
        Guid exerciseId,
        List<Guid> influencerLinkIds,
        CancellationToken cancellationToken = default
    );
    Task<bool> ExerciseUnitContactExistsAsync(Guid contactId, CancellationToken cancellationToken = default);
    Task DeleteExerciseUnitContactsAsync(
        Guid exerciseId,
        List<Guid> contactIds,
        CancellationToken cancellationToken = default
    );
}
