using System.Net;
using System.Diagnostics.CodeAnalysis;
using Augustus.Infra.Core.Shared.Exceptions;
using Augustus.Infra.Core.Shared.Interfaces;
using Coacher.Shared;
using Coacher.Shared.Interfaces.Repositories;
using Coacher.Shared.Interfaces.Services;
using Coacher.Shared.Models.DAL.Exercises;
using Coacher.Shared.Models.Enums;
using Coacher.Shared.Models.WebApi.Requests.Exercises;

namespace Coacher.Lib.Services;

public sealed class ExerciseService : IExerciseService
{
    private readonly IAugustusLogger _logger;
    private readonly IExerciseRepository _repository;
    private readonly IClosedListItemRepository _closedListItemRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICoacherAuthorizationService _authorizationService;
    private readonly Guid _defaultExerciseStatusId;

    public ExerciseService(
        IAugustusLogger logger,
        IExerciseRepository repository,
        IClosedListItemRepository closedListItemRepository,
        ICurrentUserService currentUserService,
        ICoacherAuthorizationService authorizationService,
        IAugustusConfiguration config
    )
    {
        _logger = logger;
        _repository = repository;
        _closedListItemRepository = closedListItemRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
        _defaultExerciseStatusId = config.GetOrThrow<Guid>(
            ConfigurationKeys.ExerciseDefaultsSection,
            ConfigurationKeys.DefaultExerciseStatusId
        );
    }

    public async Task<List<Exercise>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.ListAsync(cancellationToken);

        return items;
    }

    public async Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<Exercise> GetRequiredByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetRequiredByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<Exercise> CreateExerciseAsync(
        CreateExerciseRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanCreateExerciseAsync(cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var exerciseId = Guid.NewGuid();
        var entity = new Exercise
        {
            Id = exerciseId,
            Name = request.Name,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TraineeUnitId = request.TraineeUnitId,
            TrainerUnitId = request.TrainerUnitId,
            StatusId = _defaultExerciseStatusId,
            CompressionFactor = request.CompressionFactor,
            CreatedByUserId = currentUserId,
            CreationTime = now,
            LastUpdateTime = now,
            CompletionTime = null,
            ArchiveTime = null,
            ArchivedByUserId = null,
            Participants = BuildParticipants(exerciseId, request, now),
            UnitContacts = BuildUnitContacts(exerciseId, request, now),
            Influencers = BuildInfluencers(exerciseId, request.InfluencerIds),
            Sections = BuildSections(exerciseId, request.SectionIds),
            Tasks = new List<Coacher.Shared.Models.DAL.Tasks.ExerciseTask>()
        };

        var createdEntity = await _repository.CreateExerciseAsync(entity, cancellationToken);

        _logger.Info("Exercise created", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", createdEntity.Id },
            { "participantCount", createdEntity.Participants.Count },
            { "influencerCount", createdEntity.Influencers.Count },
            { "sectionCount", createdEntity.Sections.Count },
            { "result", "success" }
        });

        return createdEntity;
    }

    public async Task<Exercise> UpdateExerciseAsync(
        Guid exerciseId,
        UpdateExerciseRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanManageExerciseAsync(exerciseId, cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(exerciseId, cancellationToken);

        ValidateUpdateExerciseRequest(request);

        var changedFields = ApplyExerciseFieldUpdates(entity, request);
        if (changedFields.Count == 0)
            return entity;

        entity.LastUpdateTime = DateTime.UtcNow;

        var updatedEntity = await _repository.UpdateExerciseAsync(entity, cancellationToken);

        _logger.Info("Exercise updated", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", exerciseId },
            { "changedFields", changedFields.ToArray() },
            { "result", "success" }
        });

        return updatedEntity;
    }

    public async Task<ExerciseParticipant> AddExerciseParticipantAsync(
        Guid exerciseId,
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanManageExerciseAsync(exerciseId, cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var parsedUserId = ParseManagerUserId(userId);
        var entity = new ExerciseParticipant
        {
            Id = Guid.NewGuid(),
            ExerciseId = exerciseId,
            UserId = parsedUserId,
            Role = ExerciseRole.ExerciseParticipant,
            CreationTime = DateTime.UtcNow
        };
        var createdEntity = await _repository.CreateExerciseParticipantAsync(entity, cancellationToken);

        _logger.Info("Exercise participant added", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", exerciseId },
            { "targetUserId", parsedUserId },
            { "role", ExerciseRole.ExerciseParticipant.ToString() },
            { "result", "success" }
        });

        return createdEntity;
    }

    public async Task<ExerciseParticipant> UpdateExerciseParticipantRoleAsync(
        Guid participantId,
        ExerciseRole role,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanManageExerciseByParticipantIdAsync(participantId, cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        if (role == ExerciseRole.ExerciseManager)
            ThrowBadRequest(
                "Use the exercise manager endpoint to assign the manager role.",
                ("participantId", participantId),
                ("role", role.ToString())
            );

        var participant = await _repository.GetRequiredExerciseParticipantByIdAsync(participantId, cancellationToken);
        if (participant.Role == ExerciseRole.ExerciseManager)
            ThrowBadRequest(
                "Use the exercise manager endpoint to change the current manager.",
                ("participantId", participantId),
                ("role", participant.Role.ToString())
            );

        participant.Role = role;
        var updatedEntity = await _repository.UpdateExerciseParticipantAsync(participant, cancellationToken);

        _logger.Info("Exercise participant role updated", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", participant.ExerciseId ?? Guid.Empty },
            { "participantId", participantId },
            { "targetUserId", participant.UserId ?? Guid.Empty },
            { "role", role.ToString() },
            { "result", "success" }
        });

        return updatedEntity;
    }

    public async Task<ExerciseParticipant> ReassignExerciseManagerAsync(
        Guid exerciseId,
        string managerUserId,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanManageExerciseAsync(exerciseId, cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        _ = await _repository.GetRequiredByIdAsync(exerciseId, cancellationToken);
        var parsedManagerUserId = ParseManagerUserId(managerUserId);
        var participants = await _repository.ListExerciseParticipantsByExerciseIdAsync(exerciseId, cancellationToken);
        var currentManager = participants.FirstOrDefault(item => item.Role == ExerciseRole.ExerciseManager);
        var targetParticipant = participants.FirstOrDefault(item => item.UserId == parsedManagerUserId);

        if (currentManager is not null && currentManager.Id != targetParticipant?.Id)
        {
            currentManager.Role = ExerciseRole.ExerciseParticipant;
            _ = await _repository.UpdateExerciseParticipantAsync(currentManager, cancellationToken);
        }

        if (targetParticipant is not null)
        {
            targetParticipant.Role = ExerciseRole.ExerciseManager;
            var updatedParticipant = await _repository.UpdateExerciseParticipantAsync(targetParticipant, cancellationToken);

            _logger.Info("Exercise manager reassigned", new Dictionary<string, object>
            {
                { "userId", currentUserId },
                { "exerciseId", exerciseId },
                { "previousManagerUserId", currentManager?.UserId ?? Guid.Empty },
                { "targetUserId", parsedManagerUserId },
                { "result", "success" }
            });

            return updatedParticipant;
        }

        var entity = new ExerciseParticipant
        {
            Id = Guid.NewGuid(),
            ExerciseId = exerciseId,
            UserId = parsedManagerUserId,
            Role = ExerciseRole.ExerciseManager,
            CreationTime = DateTime.UtcNow
        };
        var createdParticipant = await _repository.CreateExerciseParticipantAsync(entity, cancellationToken);

        _logger.Info("Exercise manager reassigned", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", exerciseId },
            { "previousManagerUserId", currentManager?.UserId ?? Guid.Empty },
            { "targetUserId", parsedManagerUserId },
            { "result", "success" }
        });

        return createdParticipant;
    }

    public async Task<ExerciseSection> AddExerciseSectionAsync(
        Guid exerciseId,
        Guid sectionId,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanManageExerciseAsync(exerciseId, cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = new ExerciseSection
        {
            Id = Guid.NewGuid(),
            ExerciseId = exerciseId,
            SectionId = sectionId
        };
        var createdEntity = await _repository.CreateExerciseSectionAsync(entity, cancellationToken);

        _logger.Info("Exercise section added", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", exerciseId },
            { "sectionId", sectionId },
            { "result", "success" }
        });

        return createdEntity;
    }

    public async Task<ExerciseInfluencer> AddExerciseInfluencerAsync(
        Guid exerciseId,
        Guid influencerId,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanManageExerciseAsync(exerciseId, cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = new ExerciseInfluencer
        {
            Id = Guid.NewGuid(),
            ExerciseId = exerciseId,
            InfluencerId = influencerId
        };
        var createdEntity = await _repository.CreateExerciseInfluencerAsync(entity, cancellationToken);

        _logger.Info("Exercise influencer added", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", exerciseId },
            { "influencerId", influencerId },
            { "result", "success" }
        });

        return createdEntity;
    }

    public async Task<ExerciseUnitContact> AddExerciseUnitContactAsync(
        Guid exerciseId,
        AddExerciseUnitContactRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanManageExerciseAsync(exerciseId, cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = new ExerciseUnitContact
        {
            Id = Guid.NewGuid(),
            ExerciseId = exerciseId,
            ContactType = request.ContactType,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            ProfileImageUrl = request.ProfileImageUrl,
            CreationTime = DateTime.UtcNow
        };
        var createdEntity = await _repository.CreateExerciseUnitContactAsync(entity, cancellationToken);

        _logger.Info("Exercise contact added", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", exerciseId },
            { "contactId", createdEntity.Id },
            { "contactType", request.ContactType.ToString() },
            { "result", "success" }
        });

        return createdEntity;
    }

    public async Task<ExerciseUnitContact> UpdateExerciseUnitContactAsync(
        Guid contactId,
        UpdateExerciseUnitContactRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanManageExerciseByContactIdAsync(contactId, cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredExerciseUnitContactByIdAsync(contactId, cancellationToken);
        entity.ContactType = request.ContactType;
        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.PhoneNumber = request.PhoneNumber;
        entity.ProfileImageUrl = request.ProfileImageUrl;
        var updatedEntity = await _repository.UpdateExerciseUnitContactAsync(entity, cancellationToken);

        _logger.Info("Exercise contact updated", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", entity.ExerciseId ?? Guid.Empty },
            { "contactId", contactId },
            { "changedFields", new[] { "ContactType", "FirstName", "LastName", "PhoneNumber", "ProfileImageUrl" } },
            { "result", "success" }
        });

        return updatedEntity;
    }

    public async Task<Exercise> SetExerciseArchiveStateAsync(
        Guid exerciseId,
        bool isArchived,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanManageExerciseAsync(exerciseId, cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(exerciseId, cancellationToken);
        var archivedStatusId = await _closedListItemRepository.GetHighestDisplayOrderItemIdByKeyAsync(
            ConstantValues.ExerciseStatusClosedListKey,
            cancellationToken: cancellationToken
        );
        var fallbackStatusId = await _closedListItemRepository.GetHighestDisplayOrderItemIdByKeyAsync(
            ConstantValues.ExerciseStatusClosedListKey,
            archivedStatusId,
            cancellationToken
        );
        var now = DateTime.UtcNow;

        if (isArchived)
        {
            entity.ArchiveTime = now;
            entity.ArchivedByUserId = currentUserId;
            entity.LastUpdateTime = now;
            if (archivedStatusId.HasValue)
                entity.StatusId = archivedStatusId.Value;

            var archivedEntity = await _repository.ArchiveExerciseAsync(entity, cancellationToken);

            _logger.Info("Exercise archived", new Dictionary<string, object>
            {
                { "userId", currentUserId },
                { "exerciseId", exerciseId },
                { "statusId", entity.StatusId ?? Guid.Empty },
                { "result", "success" }
            });

            return archivedEntity;
        }

        entity.ArchiveTime = null;
        entity.ArchivedByUserId = null;
        entity.LastUpdateTime = now;
        if (archivedStatusId.HasValue && entity.StatusId == archivedStatusId.Value && fallbackStatusId.HasValue)
            entity.StatusId = fallbackStatusId.Value;

        var unarchivedEntity = await _repository.UnarchiveExerciseAsync(entity, cancellationToken);

        _logger.Info("Exercise unarchived", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", exerciseId },
            { "statusId", entity.StatusId ?? Guid.Empty },
            { "result", "success" }
        });

        return unarchivedEntity;
    }

    public async Task RemoveExerciseParticipantAsync(Guid participantId, CancellationToken cancellationToken = default)
    {
        await _authorizationService.EnsureCanManageExerciseByParticipantIdAsync(participantId, cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var participant = await _repository.GetRequiredExerciseParticipantByIdAsync(participantId, cancellationToken);
        if (participant.ExerciseId is null)
            ThrowConflict("Exercise participant is not linked to an exercise.", ("participantId", participantId));

        if (participant.Role == ExerciseRole.ExerciseManager)
        {
            var replacementParticipant = (await _repository
                    .ListExerciseParticipantsByExerciseIdAsync(participant.ExerciseId.Value, cancellationToken))
                .Where(item => item.Id != participantId)
                .OrderBy(item => item.CreationTime)
                .ThenBy(item => item.Id)
                .FirstOrDefault();
            if (replacementParticipant is null)
                ThrowConflict(
                    "Cannot remove the exercise manager when no replacement participant exists.",
                    ("participantId", participantId),
                    ("exerciseId", participant.ExerciseId.Value)
                );

            replacementParticipant.Role = ExerciseRole.ExerciseManager;
            _ = await _repository.UpdateExerciseParticipantAsync(replacementParticipant, cancellationToken);

            _logger.Info("Exercise manager reassigned during participant removal", new Dictionary<string, object>
            {
                { "userId", currentUserId },
                { "exerciseId", participant.ExerciseId.Value },
                { "removedParticipantId", participantId },
                { "replacementUserId", replacementParticipant.UserId ?? Guid.Empty },
                { "result", "success" }
            });
        }

        await _repository.DeleteExerciseParticipantAsync(participantId, cancellationToken);

        _logger.Info("Exercise participant removed", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", participant.ExerciseId ?? Guid.Empty },
            { "participantId", participantId },
            { "targetUserId", participant.UserId ?? Guid.Empty },
            { "role", participant.Role.ToString() },
            { "result", "success" }
        });
    }

    public async Task RemoveExerciseSectionAsync(Guid sectionLinkId, CancellationToken cancellationToken = default)
    {
        await _authorizationService.EnsureCanManageExerciseBySectionLinkIdAsync(sectionLinkId, cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var section = await _repository.GetRequiredExerciseSectionByIdAsync(sectionLinkId, cancellationToken);
        await _repository.DeleteExerciseSectionAsync(sectionLinkId, cancellationToken);

        _logger.Info("Exercise section removed", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", section.ExerciseId ?? Guid.Empty },
            { "sectionLinkId", sectionLinkId },
            { "sectionId", section.SectionId ?? Guid.Empty },
            { "result", "success" }
        });
    }

    public async Task RemoveExerciseInfluencerAsync(Guid influencerLinkId, CancellationToken cancellationToken = default)
    {
        await _authorizationService.EnsureCanManageExerciseByInfluencerLinkIdAsync(
            influencerLinkId,
            cancellationToken
        );
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var influencer = await _repository.GetRequiredExerciseInfluencerByIdAsync(influencerLinkId, cancellationToken);
        await _repository.DeleteExerciseInfluencerAsync(influencerLinkId, cancellationToken);

        _logger.Info("Exercise influencer removed", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", influencer.ExerciseId ?? Guid.Empty },
            { "influencerLinkId", influencerLinkId },
            { "influencerId", influencer.InfluencerId ?? Guid.Empty },
            { "result", "success" }
        });
    }

    public async Task RemoveExerciseUnitContactAsync(Guid contactId, CancellationToken cancellationToken = default)
    {
        await _authorizationService.EnsureCanManageExerciseByContactIdAsync(contactId, cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var contact = await _repository.GetRequiredExerciseUnitContactByIdAsync(contactId, cancellationToken);
        await _repository.DeleteExerciseUnitContactAsync(contactId, cancellationToken);

        _logger.Info("Exercise contact removed", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", contact.ExerciseId ?? Guid.Empty },
            { "contactId", contactId },
            { "contactType", contact.ContactType.ToString() },
            { "result", "success" }
        });
    }

    public async Task<Exercise> AddAsync(Exercise entity, CancellationToken cancellationToken = default)
    {
        var createdEntity = await _repository.AddAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<Exercise> UpdateAsync(Exercise entity, CancellationToken cancellationToken = default)
    {
        var updatedEntity = await _repository.UpdateAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var exception = new HttpStatusCodeException(
            "Exercises use archive and unarchive operations instead of physical delete.",
            new Dictionary<string, object?>
            {
                { "exerciseId", id }
            },
            HttpStatusCode.BadRequest
        );

        _logger.Error(exception);

        throw exception;
    }

    private List<ExerciseParticipant> BuildParticipants(
        Guid exerciseId,
        CreateExerciseRequest request,
        DateTime creationTime
    )
    {
        var participants = new List<ExerciseParticipant>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ExerciseId = exerciseId,
                UserId = ParseManagerUserId(request.ManagerUserId),
                Role = ExerciseRole.ExerciseManager,
                CreationTime = creationTime
            }
        };

        participants.AddRange(request.AdditionalParticipants.Select(participant => new ExerciseParticipant
        {
            Id = Guid.NewGuid(),
            ExerciseId = exerciseId,
            UserId = participant.UserId,
            Role = participant.Role,
            CreationTime = creationTime
        }));

        return participants;
    }

    private static List<ExerciseUnitContact> BuildUnitContacts(
        Guid exerciseId,
        CreateExerciseRequest request,
        DateTime creationTime
    )
    {
        var traineeContacts = request.TraineeUnitContacts.Select(contact => new ExerciseUnitContact
        {
            Id = Guid.NewGuid(),
            ExerciseId = exerciseId,
            ContactType = ExerciseUnitContactType.TraineeUnitContact,
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            PhoneNumber = contact.PhoneNumber,
            ProfileImageUrl = contact.ProfileImageUrl,
            CreationTime = creationTime
        });
        var trainerContacts = request.TrainerUnitContacts.Select(contact => new ExerciseUnitContact
        {
            Id = Guid.NewGuid(),
            ExerciseId = exerciseId,
            ContactType = ExerciseUnitContactType.TrainerUnitContact,
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            PhoneNumber = contact.PhoneNumber,
            ProfileImageUrl = contact.ProfileImageUrl,
            CreationTime = creationTime
        });
        var unitContacts = traineeContacts.Concat(trainerContacts).ToList();

        return unitContacts;
    }

    private static List<ExerciseInfluencer> BuildInfluencers(Guid exerciseId, List<Guid> influencerIds)
    {
        var influencerLinks = influencerIds.Select(influencerId => new ExerciseInfluencer
        {
            Id = Guid.NewGuid(),
            ExerciseId = exerciseId,
            InfluencerId = influencerId
        }).ToList();

        return influencerLinks;
    }

    private static List<ExerciseSection> BuildSections(Guid exerciseId, List<Guid> sectionIds)
    {
        var sectionLinks = sectionIds.Select(sectionId => new ExerciseSection
        {
            Id = Guid.NewGuid(),
            ExerciseId = exerciseId,
            SectionId = sectionId
        }).ToList();

        return sectionLinks;
    }

    private void ValidateUpdateExerciseRequest(UpdateExerciseRequest request)
    {
        if (request.Description is not null && request.ClearDescription)
            ThrowBadRequest(
                "Description cannot be updated and cleared in the same request.",
                ("clearDescription", request.ClearDescription)
            );
    }

    private static List<string> ApplyExerciseFieldUpdates(Exercise entity, UpdateExerciseRequest request)
    {
        var changedFields = new List<string>();

        if (request.Name is not null && entity.Name != request.Name)
        {
            entity.Name = request.Name;
            changedFields.Add("Name");
        }

        if (request.ClearDescription)
        {
            if (entity.Description is not null)
            {
                entity.Description = null;
                changedFields.Add("Description");
            }
        }

        if (request.Description is not null && entity.Description != request.Description)
        {
            entity.Description = request.Description;
            changedFields.Add("Description");
        }

        if (request.EndDate.HasValue && entity.EndDate != request.EndDate.Value)
        {
            entity.EndDate = request.EndDate.Value;
            changedFields.Add("EndDate");
        }

        if (request.StatusId.HasValue && entity.StatusId != request.StatusId.Value)
        {
            entity.StatusId = request.StatusId.Value;
            changedFields.Add("StatusId");
        }

        if (request.TraineeUnitId.HasValue && entity.TraineeUnitId != request.TraineeUnitId.Value)
        {
            entity.TraineeUnitId = request.TraineeUnitId.Value;
            changedFields.Add("TraineeUnitId");
        }

        if (request.TrainerUnitId.HasValue && entity.TrainerUnitId != request.TrainerUnitId.Value)
        {
            entity.TrainerUnitId = request.TrainerUnitId.Value;
            changedFields.Add("TrainerUnitId");
        }

        return changedFields;
    }

    private Guid ParseManagerUserId(string managerUserId)
    {
        if (!Guid.TryParse(managerUserId, out var parsedManagerUserId))
            ThrowBadRequest("ManagerUserId must be a valid GUID string.", ("managerUserId", managerUserId));

        return parsedManagerUserId;
    }

    [DoesNotReturn]
    private void ThrowBadRequest(string message, params (string Key, object? Value)[] entries)
    {
        var exception = new HttpStatusCodeException(
            message,
            entries.ToDictionary(item => item.Key, item => item.Value),
            HttpStatusCode.BadRequest
        );

        _logger.Error(exception);

        throw exception;
    }

    [DoesNotReturn]
    private void ThrowConflict(string message, params (string Key, object? Value)[] entries)
    {
        var exception = new DataConflictException(
            message,
            parameters: entries.ToDictionary(item => item.Key, item => item.Value)
        );

        _logger.Error(exception);

        throw exception;
    }
}

