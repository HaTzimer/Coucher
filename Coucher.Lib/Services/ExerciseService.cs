using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Models.Enums;
using Coucher.Shared.Models.WebApi.Requests.Exercises;

namespace Coucher.Lib.Services;

public sealed class ExerciseService : IExerciseService
{
    private readonly IExerciseRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public ExerciseService(IExerciseRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
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
        CreateExerciseRequestModel request,
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
            StatusId = request.StatusId,
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
        UpdateExerciseRequestModel request,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(exerciseId, cancellationToken);
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
        entity.StatusId = request.StatusId;
        entity.TraineeUnitId = request.TraineeUnitId;
        entity.TrainerUnitId = request.TrainerUnitId;
        entity.CompressionFactor = request.CompressionFactor;
        entity.LastUpdateTime = DateTime.UtcNow;
        var updatedEntity = await _repository.UpdateExerciseAsync(entity, cancellationToken);

        return updatedEntity;
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

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
    }

    private static List<ExerciseParticipant> BuildParticipants(
        Guid exerciseId,
        CreateExerciseRequestModel request,
        DateTime creationTime
    )
    {
        var participants = new List<ExerciseParticipant>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ExerciseId = exerciseId,
                UserId = request.Manager.UserId,
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
        CreateExerciseRequestModel request,
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
}
