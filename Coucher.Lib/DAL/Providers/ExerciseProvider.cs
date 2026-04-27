using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Models.DAL.Exercises;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Lib.DAL.Providers;

public sealed class ExerciseProvider : IExerciseProvider
{
    private readonly IDbContextFactory<CoucherDbContext> _dbContextFactory;

    public ExerciseProvider(IDbContextFactory<CoucherDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<Exercise>> ListAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<Exercise>();
        var items = await entities.ToListAsync(cancellationToken);

        return items;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<Exercise>();
        var exists = await entities.AnyAsync(item => item.Id == id, cancellationToken);

        return exists;
    }

    public async Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<Exercise>();
        var entity = await entities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return entity;
    }

    public async Task<Exercise> CreateExerciseAsync(Exercise entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<Exercise>();
        await entities.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<Exercise> UpdateExerciseAsync(Exercise entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<Exercise>();
        entities.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<Exercise> ArchiveExerciseAsync(Exercise entity, CancellationToken cancellationToken = default)
    {
        var updatedEntity = await UpdateExerciseAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<Exercise> UnarchiveExerciseAsync(Exercise entity, CancellationToken cancellationToken = default)
    {
        var updatedEntity = await UpdateExerciseAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<ExerciseParticipant> CreateExerciseParticipantAsync(
        ExerciseParticipant entity,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseParticipant>();
        await entities.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<ExerciseSection> CreateExerciseSectionAsync(
        ExerciseSection entity,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseSection>();
        await entities.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<ExerciseInfluencer> CreateExerciseInfluencerAsync(
        ExerciseInfluencer entity,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseInfluencer>();
        await entities.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<ExerciseUnitContact> CreateExerciseUnitContactAsync(
        ExerciseUnitContact entity,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseUnitContact>();
        await entities.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<ExerciseParticipant?> GetExerciseParticipantByIdAsync(
        Guid participantId,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseParticipant>();
        var entity = await entities.FirstOrDefaultAsync(item => item.Id == participantId, cancellationToken);

        return entity;
    }

    public async Task<List<ExerciseParticipant>> ListExerciseParticipantsByExerciseIdAsync(
        Guid exerciseId,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseParticipant>();
        var items = await entities.Where(item => item.ExerciseId == exerciseId).ToListAsync(cancellationToken);

        return items;
    }

    public async Task<ExerciseParticipant> UpdateExerciseParticipantAsync(
        ExerciseParticipant entity,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseParticipant>();
        entities.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task DeleteExerciseParticipantAsync(Guid participantId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseParticipant>();
        await entities.Where(item => item.Id == participantId).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<bool> ExerciseSectionExistsAsync(Guid sectionLinkId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseSection>();
        var exists = await entities.AnyAsync(item => item.Id == sectionLinkId, cancellationToken);

        return exists;
    }

    public async Task DeleteExerciseSectionAsync(Guid sectionLinkId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseSection>();
        await entities.Where(item => item.Id == sectionLinkId).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<bool> ExerciseInfluencerExistsAsync(Guid influencerLinkId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseInfluencer>();
        var exists = await entities.AnyAsync(item => item.Id == influencerLinkId, cancellationToken);

        return exists;
    }

    public async Task DeleteExerciseInfluencerAsync(Guid influencerLinkId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseInfluencer>();
        await entities.Where(item => item.Id == influencerLinkId).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<bool> ExerciseUnitContactExistsAsync(Guid contactId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseUnitContact>();
        var exists = await entities.AnyAsync(item => item.Id == contactId, cancellationToken);

        return exists;
    }

    public async Task DeleteExerciseUnitContactAsync(Guid contactId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseUnitContact>();
        await entities.Where(item => item.Id == contactId).ExecuteDeleteAsync(cancellationToken);
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

    public async Task DeleteAsync(Exercise entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<Exercise>();
        entities.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
