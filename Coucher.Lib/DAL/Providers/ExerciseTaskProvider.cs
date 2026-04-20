using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Models.DAL.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Lib.DAL.Providers;

public sealed class ExerciseTaskProvider : IExerciseTaskProvider
{
    private readonly CoucherDbContext _dbContext;
    private readonly DbSet<ExerciseTask> _entities;

    public ExerciseTaskProvider(CoucherDbContext dbContext)
    {
        _dbContext = dbContext;
        _entities = dbContext.Set<ExerciseTask>();
    }

    public async Task<List<ExerciseTask>> ListAsync(CancellationToken cancellationToken = default)
    {
        var query = BuildQuery();
        var items = await query.ToListAsync(cancellationToken);

        return items;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = BuildQuery();
        var exists = await query.AnyAsync(item => item.Id == id, cancellationToken);

        return exists;
    }

    public async Task<ExerciseTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = BuildQuery();
        var entity = await query.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return entity;
    }

    public async Task<ExerciseTask> AddAsync(ExerciseTask entity, CancellationToken cancellationToken = default)
    {
        await _entities.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<ExerciseTask> UpdateAsync(ExerciseTask entity, CancellationToken cancellationToken = default)
    {
        _entities.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task DeleteAsync(ExerciseTask entity, CancellationToken cancellationToken = default)
    {
        _entities.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<ExerciseTask> BuildQuery()
    {
        var query = _entities
        .Include(item => item.Exercise)
        .Include(item => item.ResponsibleUser)
        .Include(item => item.Influencers)
        .Include(item => item.Dependencies)
        .ThenInclude(item => item.DependsOnTask)
        .Include(item => item.DependedOnByTasks)
        .ThenInclude(item => item.ExerciseTask);

        return query;
    }
}
