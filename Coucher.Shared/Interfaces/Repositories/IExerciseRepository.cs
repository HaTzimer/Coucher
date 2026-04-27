using Coucher.Shared.Models.DAL.Exercises;

namespace Coucher.Shared.Interfaces.Repositories;

public interface IExerciseRepository : IRepositoryBase<Exercise, Guid>
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
    Task<ExerciseParticipant> GetRequiredExerciseParticipantByIdAsync(
        Guid participantId,
        CancellationToken cancellationToken = default
    );
    Task<List<ExerciseParticipant>> ListExerciseParticipantsByExerciseIdAsync(
        Guid exerciseId,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseParticipant> UpdateExerciseParticipantAsync(
        ExerciseParticipant entity,
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
    Task DeleteExerciseParticipantAsync(Guid participantId, CancellationToken cancellationToken = default);
    Task DeleteExerciseSectionAsync(Guid sectionLinkId, CancellationToken cancellationToken = default);
    Task DeleteExerciseInfluencerAsync(Guid influencerLinkId, CancellationToken cancellationToken = default);
    Task DeleteExerciseUnitContactAsync(Guid contactId, CancellationToken cancellationToken = default);
}
