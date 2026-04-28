using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Models.DAL.Exercises;

namespace Coucher.Lib.Repositories;

public sealed class ExerciseRepository : IExerciseRepository
{
    private readonly IExerciseProvider _provider;

    public ExerciseRepository(IExerciseProvider provider)
    {
        _provider = provider;
    }

    public async Task<List<Exercise>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _provider.ListAsync(cancellationToken);

        return items;
    }

    public async Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _provider.GetByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<Exercise> GetRequiredByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _provider.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new KeyNotFoundException($"{nameof(Exercise)} '{id}' was not found.");

        return entity;
    }

    public async Task<Exercise> CreateExerciseAsync(Exercise entity, CancellationToken cancellationToken = default)
    {
        var createdEntity = await _provider.CreateExerciseAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<Exercise> UpdateExerciseAsync(Exercise entity, CancellationToken cancellationToken = default)
    {
        var updatedEntity = await _provider.UpdateExerciseAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<Exercise> ArchiveExerciseAsync(Exercise entity, CancellationToken cancellationToken = default)
    {
        var archivedEntity = await _provider.ArchiveExerciseAsync(entity, cancellationToken);

        return archivedEntity;
    }

    public async Task<Exercise> UnarchiveExerciseAsync(Exercise entity, CancellationToken cancellationToken = default)
    {
        var unarchivedEntity = await _provider.UnarchiveExerciseAsync(entity, cancellationToken);

        return unarchivedEntity;
    }

    public async Task<ExerciseParticipant> CreateExerciseParticipantAsync(
        ExerciseParticipant entity,
        CancellationToken cancellationToken = default
    )
    {
        _ = await GetRequiredByIdAsync(entity.ExerciseId ?? Guid.Empty, cancellationToken);
        var createdEntity = await _provider.CreateExerciseParticipantAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<ExerciseSection> CreateExerciseSectionAsync(
        ExerciseSection entity,
        CancellationToken cancellationToken = default
    )
    {
        _ = await GetRequiredByIdAsync(entity.ExerciseId ?? Guid.Empty, cancellationToken);
        var createdEntity = await _provider.CreateExerciseSectionAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<ExerciseInfluencer> CreateExerciseInfluencerAsync(
        ExerciseInfluencer entity,
        CancellationToken cancellationToken = default
    )
    {
        _ = await GetRequiredByIdAsync(entity.ExerciseId ?? Guid.Empty, cancellationToken);
        var createdEntity = await _provider.CreateExerciseInfluencerAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<ExerciseUnitContact> CreateExerciseUnitContactAsync(
        ExerciseUnitContact entity,
        CancellationToken cancellationToken = default
    )
    {
        _ = await GetRequiredByIdAsync(entity.ExerciseId ?? Guid.Empty, cancellationToken);
        var createdEntity = await _provider.CreateExerciseUnitContactAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<ExerciseParticipant> GetRequiredExerciseParticipantByIdAsync(
        Guid participantId,
        CancellationToken cancellationToken = default
    )
    {
        var entity = await _provider.GetExerciseParticipantByIdAsync(participantId, cancellationToken);
        if (entity is null)
            throw new KeyNotFoundException($"{nameof(ExerciseParticipant)} '{participantId}' was not found.");

        return entity;
    }

    public async Task<List<ExerciseParticipant>> ListExerciseParticipantsByExerciseIdAsync(
        Guid exerciseId,
        CancellationToken cancellationToken = default
    )
    {
        var items = await _provider.ListExerciseParticipantsByExerciseIdAsync(exerciseId, cancellationToken);

        return items;
    }

    public async Task<bool> IsExerciseCreatedByUserAsync(
        Guid exerciseId,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var exists = await _provider.IsExerciseCreatedByUserAsync(exerciseId, userId, cancellationToken);

        return exists;
    }

    public async Task<bool> IsExerciseParticipantAsync(
        Guid exerciseId,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var exists = await _provider.IsExerciseParticipantAsync(exerciseId, userId, cancellationToken);

        return exists;
    }

    public async Task<bool> IsExerciseManagerAsync(
        Guid exerciseId,
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var exists = await _provider.IsExerciseManagerAsync(exerciseId, userId, cancellationToken);

        return exists;
    }

    public async Task<ExerciseParticipant> UpdateExerciseParticipantAsync(
        ExerciseParticipant entity,
        CancellationToken cancellationToken = default
    )
    {
        var updatedEntity = await _provider.UpdateExerciseParticipantAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<ExerciseSection> GetRequiredExerciseSectionByIdAsync(
        Guid sectionLinkId,
        CancellationToken cancellationToken = default
    )
    {
        var entity = await _provider.GetExerciseSectionByIdAsync(sectionLinkId, cancellationToken);
        if (entity is null)
            throw new KeyNotFoundException($"{nameof(ExerciseSection)} '{sectionLinkId}' was not found.");

        return entity;
    }

    public async Task<ExerciseInfluencer> GetRequiredExerciseInfluencerByIdAsync(
        Guid influencerLinkId,
        CancellationToken cancellationToken = default
    )
    {
        var entity = await _provider.GetExerciseInfluencerByIdAsync(influencerLinkId, cancellationToken);
        if (entity is null)
            throw new KeyNotFoundException($"{nameof(ExerciseInfluencer)} '{influencerLinkId}' was not found.");

        return entity;
    }

    public async Task<ExerciseUnitContact> GetRequiredExerciseUnitContactByIdAsync(
        Guid contactId,
        CancellationToken cancellationToken = default
    )
    {
        var entity = await _provider.GetExerciseUnitContactByIdAsync(contactId, cancellationToken);
        if (entity is null)
            throw new KeyNotFoundException($"{nameof(ExerciseUnitContact)} '{contactId}' was not found.");

        return entity;
    }

    public async Task<ExerciseUnitContact> UpdateExerciseUnitContactAsync(
        ExerciseUnitContact entity,
        CancellationToken cancellationToken = default
    )
    {
        var updatedEntity = await _provider.UpdateExerciseUnitContactAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task DeleteExerciseParticipantAsync(Guid participantId, CancellationToken cancellationToken = default)
    {
        _ = await GetRequiredExerciseParticipantByIdAsync(participantId, cancellationToken);
        await _provider.DeleteExerciseParticipantAsync(participantId, cancellationToken);
    }

    public async Task DeleteExerciseSectionAsync(Guid sectionLinkId, CancellationToken cancellationToken = default)
    {
        var exists = await _provider.ExerciseSectionExistsAsync(sectionLinkId, cancellationToken);
        if (!exists)
            throw new KeyNotFoundException($"{nameof(ExerciseSection)} '{sectionLinkId}' was not found.");

        await _provider.DeleteExerciseSectionAsync(sectionLinkId, cancellationToken);
    }

    public async Task DeleteExerciseInfluencerAsync(Guid influencerLinkId, CancellationToken cancellationToken = default)
    {
        var exists = await _provider.ExerciseInfluencerExistsAsync(influencerLinkId, cancellationToken);
        if (!exists)
            throw new KeyNotFoundException($"{nameof(ExerciseInfluencer)} '{influencerLinkId}' was not found.");

        await _provider.DeleteExerciseInfluencerAsync(influencerLinkId, cancellationToken);
    }

    public async Task DeleteExerciseUnitContactAsync(Guid contactId, CancellationToken cancellationToken = default)
    {
        var exists = await _provider.ExerciseUnitContactExistsAsync(contactId, cancellationToken);
        if (!exists)
            throw new KeyNotFoundException($"{nameof(ExerciseUnitContact)} '{contactId}' was not found.");

        await _provider.DeleteExerciseUnitContactAsync(contactId, cancellationToken);
    }

    public async Task<Exercise> AddAsync(Exercise entity, CancellationToken cancellationToken = default)
    {
        var createdEntity = await CreateExerciseAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<Exercise> UpdateAsync(Exercise entity, CancellationToken cancellationToken = default)
    {
        var updatedEntity = await UpdateExerciseAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetRequiredByIdAsync(id, cancellationToken);
        await _provider.DeleteAsync(entity, cancellationToken);
    }
}
