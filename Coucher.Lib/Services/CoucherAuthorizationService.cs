using System.Net;
using System.Diagnostics.CodeAnalysis;
using Augustus.Infra.Core.Shared.Exceptions;
using Augustus.Infra.Core.Shared.Interfaces;
using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.Internal.Authorization;

namespace Coucher.Lib.Services;

public sealed class CoucherAuthorizationService : ICoucherAuthorizationService
{
    private readonly IAugustusLogger _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IExerciseTaskRepository _exerciseTaskRepository;

    public CoucherAuthorizationService(
        IAugustusLogger logger,
        ICurrentUserService currentUserService,
        IUserRoleRepository userRoleRepository,
        IExerciseRepository exerciseRepository,
        IExerciseTaskRepository exerciseTaskRepository
    )
    {
        _logger = logger;
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
            return;

        ThrowAuthorizationException(
            "Only admins can access this resource.",
            snapshot.UserId,
            ("scope", "admin")
        );
    }

    public async Task EnsureCanCreateExerciseAsync(CancellationToken cancellationToken = default)
    {
        var snapshot = await GetCurrentAuthorizationSnapshotAsync(cancellationToken);
        if (snapshot.IsAdmin)
            return;

        if (snapshot.IsAuditor)
        {
            ThrowAuthorizationException(
                "Auditors cannot create exercises.",
                snapshot.UserId,
                ("scope", "exercise"),
                ("action", "create")
            );
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
            var exception = new DataConflictException(
                "Exercise participant is not linked to an exercise.",
                parameters: new Dictionary<string, object?>
                {
                    { "participantId", participantId }
                }
            );

            _logger.Error(exception);

            throw exception;
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
            var exception = new DataConflictException(
                "Exercise section is not linked to an exercise.",
                parameters: new Dictionary<string, object?>
                {
                    { "sectionLinkId", sectionLinkId }
                }
            );

            _logger.Error(exception);

            throw exception;
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
            var exception = new DataConflictException(
                "Exercise influencer is not linked to an exercise.",
                parameters: new Dictionary<string, object?>
                {
                    { "influencerLinkId", influencerLinkId }
                }
            );

            _logger.Error(exception);

            throw exception;
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
            var exception = new DataConflictException(
                "Exercise contact is not linked to an exercise.",
                parameters: new Dictionary<string, object?>
                {
                    { "contactId", contactId }
                }
            );

            _logger.Error(exception);

            throw exception;
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
            return;

        if (snapshot.IsAuditor)
        {
            ThrowAuthorizationException(
                "Auditors have read-only access.",
                snapshot.UserId,
                ("scope", "exercise"),
                ("exerciseId", exerciseId),
                ("action", "manage")
            );
        }

        var isManager = await _exerciseRepository.IsExerciseManagerAsync(exerciseId, snapshot.UserId, cancellationToken);
        if (isManager)
            return;

        ThrowAuthorizationException(
            "Only the exercise manager or an admin can modify this exercise.",
            snapshot.UserId,
            ("scope", "exercise"),
            ("exerciseId", exerciseId),
            ("action", "manage")
        );
    }

    private async Task EnsureCanPartiallyEditExerciseAsync(
        Guid exerciseId,
        CurrentAuthorizationSnapshot snapshot,
        CancellationToken cancellationToken
    )
    {
        if (snapshot.IsAdmin)
            return;

        if (snapshot.IsAuditor)
        {
            ThrowAuthorizationException(
                "Auditors have read-only access.",
                snapshot.UserId,
                ("scope", "task"),
                ("exerciseId", exerciseId),
                ("action", "partialEdit")
            );
        }

        var isParticipant = await _exerciseRepository.IsExerciseParticipantAsync(
            exerciseId,
            snapshot.UserId,
            cancellationToken
        );
        if (isParticipant)
            return;

        ThrowAuthorizationException(
            "Only exercise participants or admins can perform this task update.",
            snapshot.UserId,
            ("scope", "task"),
            ("exerciseId", exerciseId),
            ("action", "partialEdit")
        );
    }

    [DoesNotReturn]
    private void ThrowAuthorizationException(
        string message,
        Guid userId,
        params (string Key, object? Value)[] entries
    )
    {
        var entryParameters = entries
        .Where(item => item.Value is not null)
        .Select(item => new KeyValuePair<string, object>(item.Key, item.Value!));

        var parameters = new[]
        {
            new KeyValuePair<string, object>("result", "denied"),
            new KeyValuePair<string, object>("userId", userId)
        }
        .Concat(entryParameters)
        .GroupBy(item => item.Key)
        .ToDictionary(item => item.Key, item => item.Last().Value);

        _logger.Warn("Authorization denied", parameters);

        var exception = new HttpStatusCodeException(
            message,
            parameters.ToDictionary(item => item.Key, item => (object?)item.Value),
            HttpStatusCode.Forbidden
        );

        _logger.Error(exception);

        throw exception;
    }
}

