using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Models.DAL.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Lib.DAL.Providers;

public sealed class ExerciseTaskProvider : IExerciseTaskProvider
{
    private readonly IDbContextFactory<CoucherDbContext> _dbContextFactory;

    public ExerciseTaskProvider(IDbContextFactory<CoucherDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<ExerciseTask>> ListAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseTask>();
        var items = await entities.ToListAsync(cancellationToken);

        return items;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseTask>();
        var exists = await entities.AnyAsync(item => item.Id == id, cancellationToken);

        return exists;
    }

    public async Task<ExerciseTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseTask>();
        var entity = await entities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return entity;
    }

    public async Task<int> GetNextSerialNumberAsync(Guid exerciseId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseTask>();
        var nextSerialNumber = (await entities
            .Where(item => item.ExerciseId == exerciseId)
            .MaxAsync(item => (int?)item.SerialNumber, cancellationToken) ?? 0) + 1;

        return nextSerialNumber;
    }

    public async Task<ExerciseTask> CreateExerciseTaskAsync(
        ExerciseTask entity,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseTask>();
        await entities.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<List<ExerciseTask>> CreateExerciseTasksAsync(
        List<ExerciseTask> entitiesToCreate,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseTask>();
        await entities.AddRangeAsync(entitiesToCreate, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entitiesToCreate;
    }

    public async Task<ExerciseTask> UpdateExerciseTaskAsync(
        ExerciseTask entity,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseTask>();
        entities.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<ExerciseTask> AddAsync(ExerciseTask entity, CancellationToken cancellationToken = default)
    {
        var createdEntity = await CreateExerciseTaskAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<ExerciseTask> UpdateAsync(ExerciseTask entity, CancellationToken cancellationToken = default)
    {
        var updatedEntity = await UpdateExerciseTaskAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task DeleteAsync(ExerciseTask entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ExerciseTask>();
        entities.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
