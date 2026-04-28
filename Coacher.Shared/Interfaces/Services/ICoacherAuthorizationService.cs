using Coacher.Shared.Models.Internal.Authorization;

namespace Coacher.Shared.Interfaces.Services;

public interface ICoacherAuthorizationService
{
    Task<CurrentAuthorizationSnapshot> GetCurrentAuthorizationSnapshotAsync(
        CancellationToken cancellationToken = default
    );

    Task EnsureCanAccessAdminSurfaceAsync(CancellationToken cancellationToken = default);

    Task EnsureCanCreateExerciseAsync(CancellationToken cancellationToken = default);

    Task EnsureCanManageExerciseAsync(Guid exerciseId, CancellationToken cancellationToken = default);

    Task EnsureCanManageExerciseByParticipantIdAsync(Guid participantId, CancellationToken cancellationToken = default);

    Task EnsureCanManageExerciseBySectionLinkIdAsync(Guid sectionLinkId, CancellationToken cancellationToken = default);

    Task EnsureCanManageExerciseByInfluencerLinkIdAsync(
        Guid influencerLinkId,
        CancellationToken cancellationToken = default
    );

    Task EnsureCanManageExerciseByContactIdAsync(Guid contactId, CancellationToken cancellationToken = default);

    Task EnsureCanCreateExerciseTaskAsync(Guid exerciseId, CancellationToken cancellationToken = default);

    Task EnsureCanFullyEditExerciseTaskAsync(Guid taskId, CancellationToken cancellationToken = default);

    Task EnsureCanFullyEditExerciseTaskDependencyAsync(
        Guid dependencyId,
        CancellationToken cancellationToken = default
    );

    Task EnsureCanPartiallyEditExerciseTaskAsync(Guid taskId, CancellationToken cancellationToken = default);

    Task EnsureCanPartiallyEditExerciseTaskResponsibleUserAsync(
        Guid responsibilityId,
        CancellationToken cancellationToken = default
    );
}
