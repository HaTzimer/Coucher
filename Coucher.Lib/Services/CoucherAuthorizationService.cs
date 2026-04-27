using Coucher.Shared.Exceptions;
using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.Internal.Authorization;

namespace Coucher.Lib.Services;

public sealed class CoucherAuthorizationService : ICoucherAuthorizationService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IExerciseTaskRepository _exerciseTaskRepository;

    public CoucherAuthorizationService(
        ICurrentUserService currentUserService,
        IUserRoleRepository userRoleRepository,
        IExerciseRepository exerciseRepository,
        IExerciseTaskRepository exerciseTaskRepository
    )
    {
        _currentUserService = currentUserService;
        _userRoleRepository = userRoleRepository;
        _exerciseRepository = exerciseRepository;
        _exerciseTaskRepository = exerciseTaskRepository;
    }

    public async Task<CurrentAuthorizationSnapshot> GetCurrentAuthorizationSnapshotAsync(
        CancellationToken cancellationToken = default
    )
    {
        var userId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var roles = await _userRoleRepository.ListByUserIdAsync(userId, cancellationToken);
        var snapshot = new CurrentAuthorizationSnapshot
        {
            UserId = userId,
            GlobalRoles = roles.Select(item => item.Role).Distinct().ToList()
        };

        return snapshot;
    }

    public async Task EnsureCanAccessAdminSurfaceAsync(CancellationToken cancellationToken = default)
    {
        var snapshot = await GetCurrentAuthorizationSnapshotAsync(cancellationToken);
        if (snapshot.IsAdmin)
        {
            return;
        }

        throw new CoucherAuthorizationException("Only admins can access this resource.");
    }

    public async Task EnsureCanCreateExerciseAsync(CancellationToken cancellationToken = default)
    {
        var snapshot = await GetCurrentAuthorizationSnapshotAsync(cancellationToken);
        if (snapshot.IsAdmin)
        {
            return;
        }

        if (snapshot.IsAuditor)
        {
            throw new CoucherAuthorizationException("Auditors cannot create exercises.");
        }
    }

    public async Task EnsureCanManageExerciseAsync(Guid exerciseId, CancellationToken cancellationToken = default)
    {
        var snapshot = await GetCurrentAuthorizationSnapshotAsync(cancellationToken);

        await EnsureCanManageExerciseAsync(exerciseId, snapshot, cancellationToken);
    }

    public async Task EnsureCanManageExerciseByParticipantIdAsync(
        Guid participantId,
        CancellationToken cancellationToken = default
    )
    {
        var participant = await _exerciseRepository.GetRequiredExerciseParticipantByIdAsync(participantId, cancellationToken);
        if (!participant.ExerciseId.HasValue)
        {
            throw new KeyNotFoundException("Exercise participant is not linked to an exercise.");
        }

        await EnsureCanManageExerciseAsync(participant.ExerciseId.Value, cancellationToken);
    }

    public async Task EnsureCanManageExerciseBySectionLinkIdAsync(
        Guid sectionLinkId,
        CancellationToken cancellationToken = default
    )
    {
        var section = await _exerciseRepository.GetRequiredExerciseSectionByIdAsync(sectionLinkId, cancellationToken);
        if (!section.ExerciseId.HasValue)
        {
            throw new KeyNotFoundException("Exercise section is not linked to an exercise.");
        }

        await EnsureCanManageExerciseAsync(section.ExerciseId.Value, cancellationToken);
    }

    public async Task EnsureCanManageExerciseByInfluencerLinkIdAsync(
        Guid influencerLinkId,
        CancellationToken cancellationToken = default
    )
    {
        var influencer = await _exerciseRepository.GetRequiredExerciseInfluencerByIdAsync(
            influencerLinkId,
            cancellationToken
        );
        if (!influencer.ExerciseId.HasValue)
        {
            throw new KeyNotFoundException("Exercise influencer is not linked to an exercise.");
        }

        await EnsureCanManageExerciseAsync(influencer.ExerciseId.Value, cancellationToken);
    }

    public async Task EnsureCanManageExerciseByContactIdAsync(
        Guid contactId,
        CancellationToken cancellationToken = default
    )
    {
        var contact = await _exerciseRepository.GetRequiredExerciseUnitContactByIdAsync(contactId, cancellationToken);
        if (!contact.ExerciseId.HasValue)
        {
            throw new KeyNotFoundException("Exercise contact is not linked to an exercise.");
        }

        await EnsureCanManageExerciseAsync(contact.ExerciseId.Value, cancellationToken);
    }

    public async Task EnsureCanCreateExerciseTaskAsync(
        Guid exerciseId,
        CancellationToken cancellationToken = default
    )
    {
        await EnsureCanManageExerciseAsync(exerciseId, cancellationToken);
    }

    public async Task EnsureCanFullyEditExerciseTaskAsync(
        Guid taskId,
        CancellationToken cancellationToken = default
    )
    {
        var exerciseId = await _exerciseTaskRepository.GetRequiredExerciseIdByTaskIdAsync(taskId, cancellationToken);

        await EnsureCanManageExerciseAsync(exerciseId, cancellationToken);
    }

    public async Task EnsureCanFullyEditExerciseTaskDependencyAsync(
        Guid dependencyId,
        CancellationToken cancellationToken = default
    )
    {
        var exerciseId = await _exerciseTaskRepository.GetRequiredExerciseIdByDependencyIdAsync(
            dependencyId,
            cancellationToken
        );

        await EnsureCanManageExerciseAsync(exerciseId, cancellationToken);
    }

    public async Task EnsureCanPartiallyEditExerciseTaskAsync(
        Guid taskId,
        CancellationToken cancellationToken = default
    )
    {
        var snapshot = await GetCurrentAuthorizationSnapshotAsync(cancellationToken);
        var exerciseId = await _exerciseTaskRepository.GetRequiredExerciseIdByTaskIdAsync(taskId, cancellationToken);

        await EnsureCanPartiallyEditExerciseAsync(exerciseId, snapshot, cancellationToken);
    }

    public async Task EnsureCanPartiallyEditExerciseTaskResponsibleUserAsync(
        Guid responsibilityId,
        CancellationToken cancellationToken = default
    )
    {
        var snapshot = await GetCurrentAuthorizationSnapshotAsync(cancellationToken);
        var exerciseId = await _exerciseTaskRepository.GetRequiredExerciseIdByResponsibilityIdAsync(
            responsibilityId,
            cancellationToken
        );

        await EnsureCanPartiallyEditExerciseAsync(exerciseId, snapshot, cancellationToken);
    }

    private async Task EnsureCanManageExerciseAsync(
        Guid exerciseId,
        CurrentAuthorizationSnapshot snapshot,
        CancellationToken cancellationToken
    )
    {
        if (snapshot.IsAdmin)
        {
            return;
        }

        if (snapshot.IsAuditor)
        {
            throw new CoucherAuthorizationException("Auditors have read-only access.");
        }

        var isManager = await _exerciseRepository.IsExerciseManagerAsync(exerciseId, snapshot.UserId, cancellationToken);
        if (isManager)
        {
            return;
        }

        throw new CoucherAuthorizationException("Only the exercise manager or an admin can modify this exercise.");
    }

    private async Task EnsureCanPartiallyEditExerciseAsync(
        Guid exerciseId,
        CurrentAuthorizationSnapshot snapshot,
        CancellationToken cancellationToken
    )
    {
        if (snapshot.IsAdmin)
        {
            return;
        }

        if (snapshot.IsAuditor)
        {
            throw new CoucherAuthorizationException("Auditors have read-only access.");
        }

        var isParticipant = await _exerciseRepository.IsExerciseParticipantAsync(
            exerciseId,
            snapshot.UserId,
            cancellationToken
        );
        if (isParticipant)
        {
            return;
        }

        throw new CoucherAuthorizationException(
            "Only exercise participants or admins can perform this task update."
        );
    }
}
