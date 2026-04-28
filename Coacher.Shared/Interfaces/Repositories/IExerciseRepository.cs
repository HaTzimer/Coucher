using Coacher.Shared.Models.DAL.Exercises;

namespace Coacher.Shared.Interfaces.Repositories;

public interface IExerciseRepository : IRepositoryBase<Exercise, Guid>
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
    Task<ExerciseParticipant> GetRequiredExerciseParticipantByIdAsync(
        Guid participantId,
        CancellationToken cancellationToken = default
    );
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
    Task<ExerciseSection> GetRequiredExerciseSectionByIdAsync(
        Guid sectionLinkId,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseInfluencer> GetRequiredExerciseInfluencerByIdAsync(
        Guid influencerLinkId,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseUnitContact> GetRequiredExerciseUnitContactByIdAsync(
        Guid contactId,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseUnitContact> UpdateExerciseUnitContactAsync(
        ExerciseUnitContact entity,
        CancellationToken cancellationToken = default
    );
    Task DeleteExerciseParticipantsAsync(
        Guid exerciseId,
        List<Guid> participantIds,
        CancellationToken cancellationToken = default
    );
    Task DeleteExerciseSectionsAsync(
        Guid exerciseId,
        List<Guid> sectionLinkIds,
        CancellationToken cancellationToken = default
    );
    Task DeleteExerciseInfluencersAsync(
        Guid exerciseId,
        List<Guid> influencerLinkIds,
        CancellationToken cancellationToken = default
    );
    Task DeleteExerciseUnitContactsAsync(
        Guid exerciseId,
        List<Guid> contactIds,
        CancellationToken cancellationToken = default
    );
}
