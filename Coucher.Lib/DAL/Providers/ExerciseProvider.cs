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

    public async Task<Exercise> AddAsync(Exercise entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<Exercise>();
        await entities.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<Exercise> UpdateAsync(Exercise entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<Exercise>();
        entities.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task DeleteAsync(Exercise entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<Exercise>();
        entities.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
