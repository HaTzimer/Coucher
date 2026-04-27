using Augustus.Infra.Core.Shared.Interfaces;
using Coucher.Shared;
using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Models.Enums;
using Coucher.Shared.Models.WebApi.Requests.Exercises;

namespace Coucher.Lib.Services;

public sealed class ExerciseService : IExerciseService
{
    private readonly IExerciseRepository _repository;
    private readonly IClosedListItemRepository _closedListItemRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly Guid _defaultExerciseStatusId;

    public ExerciseService(
        IExerciseRepository repository,
        IClosedListItemRepository closedListItemRepository,
        ICurrentUserService currentUserService,
        IAugustusConfiguration config
    )
    {
        _repository = repository;
        _closedListItemRepository = closedListItemRepository;
        _currentUserService = currentUserService;
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
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
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
            CreationTime = now,
            LastUpdateTime = now,
            CompletionTime = null,
            ArchiveTime = null,
            ArchivedByUserId = null,
            Participants = BuildParticipants(exerciseId, request, now),
            UnitContacts = BuildUnitContacts(exerciseId, request, now),
            Influencers = BuildInfluencers(exerciseId, request.InfluencerIds),
            Sections = BuildSections(exerciseId, request.SectionIds),
            Tasks = new List<Coucher.Shared.Models.DAL.Tasks.ExerciseTask>()
        };
        var createdEntity = await _repository.CreateExerciseAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<Exercise> UpdateExerciseAsync(
        Guid exerciseId,
        UpdateExerciseRequest request,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(exerciseId, cancellationToken);
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.EndDate = request.EndDate;
        entity.StatusId = request.StatusId;
        entity.TraineeUnitId = request.TraineeUnitId;
        entity.TrainerUnitId = request.TrainerUnitId;
        entity.LastUpdateTime = DateTime.UtcNow;
        var updatedEntity = await _repository.UpdateExerciseAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<Exercise> UpdateExerciseDetailsAsync(
        Guid exerciseId,
        UpdateExerciseDetailsRequest request,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(exerciseId, cancellationToken);
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.LastUpdateTime = DateTime.UtcNow;
        var updatedEntity = await _repository.UpdateExerciseAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<Exercise> UpdateExerciseEndDateAsync(
        Guid exerciseId,
        DateOnly endDate,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(exerciseId, cancellationToken);
        entity.EndDate = endDate;
        entity.LastUpdateTime = DateTime.UtcNow;
        var updatedEntity = await _repository.UpdateExerciseAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<Exercise> UpdateExerciseStatusAsync(
        Guid exerciseId,
        Guid statusId,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(exerciseId, cancellationToken);
        entity.StatusId = statusId;
        entity.LastUpdateTime = DateTime.UtcNow;
        var updatedEntity = await _repository.UpdateExerciseAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<Exercise> UpdateExerciseTraineeUnitAsync(
        Guid exerciseId,
        Guid traineeUnitId,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(exerciseId, cancellationToken);
        entity.TraineeUnitId = traineeUnitId;
        entity.LastUpdateTime = DateTime.UtcNow;
        var updatedEntity = await _repository.UpdateExerciseAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<Exercise> UpdateExerciseTrainerUnitAsync(
        Guid exerciseId,
        Guid trainerUnitId,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(exerciseId, cancellationToken);
        entity.TrainerUnitId = trainerUnitId;
        entity.LastUpdateTime = DateTime.UtcNow;
        var updatedEntity = await _repository.UpdateExerciseAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<ExerciseParticipant> AddExerciseParticipantAsync(
        Guid exerciseId,
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = new ExerciseParticipant
        {
            Id = Guid.NewGuid(),
            ExerciseId = exerciseId,
            UserId = ParseManagerUserId(userId),
            Role = ExerciseRole.ExerciseParticipant,
            CreationTime = DateTime.UtcNow
        };
        var createdEntity = await _repository.CreateExerciseParticipantAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<ExerciseParticipant> UpdateExerciseParticipantRoleAsync(
        Guid participantId,
        ExerciseRole role,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        if (role == ExerciseRole.ExerciseManager)
        {
            throw new InvalidOperationException("Use the exercise manager endpoint to assign the manager role.");
        }

        var participant = await _repository.GetRequiredExerciseParticipantByIdAsync(participantId, cancellationToken);
        if (participant.Role == ExerciseRole.ExerciseManager)
        {
            throw new InvalidOperationException("Use the exercise manager endpoint to change the current manager.");
        }

        participant.Role = role;
        var updatedEntity = await _repository.UpdateExerciseParticipantAsync(participant, cancellationToken);

        return updatedEntity;
    }

    public async Task<ExerciseParticipant> ReassignExerciseManagerAsync(
        Guid exerciseId,
        string managerUserId,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
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

        return createdParticipant;
    }

    public async Task<ExerciseSection> AddExerciseSectionAsync(
        Guid exerciseId,
        Guid sectionId,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = new ExerciseSection
        {
            Id = Guid.NewGuid(),
            ExerciseId = exerciseId,
            SectionId = sectionId
        };
        var createdEntity = await _repository.CreateExerciseSectionAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<ExerciseInfluencer> AddExerciseInfluencerAsync(
        Guid exerciseId,
        Guid influencerId,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = new ExerciseInfluencer
        {
            Id = Guid.NewGuid(),
            ExerciseId = exerciseId,
            InfluencerId = influencerId
        };
        var createdEntity = await _repository.CreateExerciseInfluencerAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<ExerciseUnitContact> AddExerciseUnitContactAsync(
        Guid exerciseId,
        AddExerciseUnitContactRequest request,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
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

        return createdEntity;
    }

    public async Task<ExerciseUnitContact> UpdateExerciseUnitContactAsync(
        Guid contactId,
        UpdateExerciseUnitContactRequest request,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredExerciseUnitContactByIdAsync(contactId, cancellationToken);
        entity.ContactType = request.ContactType;
        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.PhoneNumber = request.PhoneNumber;
        entity.ProfileImageUrl = request.ProfileImageUrl;
        var updatedEntity = await _repository.UpdateExerciseUnitContactAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<Exercise> ArchiveExerciseAsync(Guid exerciseId, CancellationToken cancellationToken = default)
    {
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(exerciseId, cancellationToken);
        var archivedStatusId = await _closedListItemRepository.GetHighestDisplayOrderItemIdByKeyAsync(
            ConstantValues.ExerciseStatusClosedListKey,
            cancellationToken: cancellationToken
        );
        var now = DateTime.UtcNow;
        entity.ArchiveTime = now;
        entity.ArchivedByUserId = currentUserId;
        entity.LastUpdateTime = now;
        if (archivedStatusId.HasValue)
        {
            entity.StatusId = archivedStatusId.Value;
        }

        var archivedEntity = await _repository.ArchiveExerciseAsync(entity, cancellationToken);

        return archivedEntity;
    }

    public async Task<Exercise> UnarchiveExerciseAsync(Guid exerciseId, CancellationToken cancellationToken = default)
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
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
        entity.ArchiveTime = null;
        entity.ArchivedByUserId = null;
        entity.LastUpdateTime = DateTime.UtcNow;
        if (archivedStatusId.HasValue && entity.StatusId == archivedStatusId.Value && fallbackStatusId.HasValue)
        {
            entity.StatusId = fallbackStatusId.Value;
        }

        var unarchivedEntity = await _repository.UnarchiveExerciseAsync(entity, cancellationToken);

        return unarchivedEntity;
    }

    public async Task RemoveExerciseParticipantAsync(Guid participantId, CancellationToken cancellationToken = default)
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var participant = await _repository.GetRequiredExerciseParticipantByIdAsync(participantId, cancellationToken);
        if (participant.ExerciseId is null)
        {
            throw new InvalidOperationException("Exercise participant is not linked to an exercise.");
        }

        if (participant.Role == ExerciseRole.ExerciseManager)
        {
            var replacementParticipant = (await _repository
                    .ListExerciseParticipantsByExerciseIdAsync(participant.ExerciseId.Value, cancellationToken))
                .Where(item => item.Id != participantId)
                .OrderBy(item => item.CreationTime)
                .ThenBy(item => item.Id)
                .FirstOrDefault();
            if (replacementParticipant is null)
            {
                throw new InvalidOperationException("Cannot remove the exercise manager when no replacement participant exists.");
            }

            replacementParticipant.Role = ExerciseRole.ExerciseManager;
            _ = await _repository.UpdateExerciseParticipantAsync(replacementParticipant, cancellationToken);
        }

        await _repository.DeleteExerciseParticipantAsync(participantId, cancellationToken);
    }

    public async Task RemoveExerciseSectionAsync(Guid sectionLinkId, CancellationToken cancellationToken = default)
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        await _repository.DeleteExerciseSectionAsync(sectionLinkId, cancellationToken);
    }

    public async Task RemoveExerciseInfluencerAsync(Guid influencerLinkId, CancellationToken cancellationToken = default)
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        await _repository.DeleteExerciseInfluencerAsync(influencerLinkId, cancellationToken);
    }

    public async Task RemoveExerciseUnitContactAsync(Guid contactId, CancellationToken cancellationToken = default)
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        await _repository.DeleteExerciseUnitContactAsync(contactId, cancellationToken);
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
        throw new NotSupportedException("Exercises use archive and unarchive operations instead of physical delete.");
    }

    private static List<ExerciseParticipant> BuildParticipants(
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

    private static Guid ParseManagerUserId(string managerUserId)
    {
        if (!Guid.TryParse(managerUserId, out var parsedManagerUserId))
        {
            throw new InvalidOperationException("ManagerUserId must be a valid GUID string.");
        }

        return parsedManagerUserId;
    }
}
