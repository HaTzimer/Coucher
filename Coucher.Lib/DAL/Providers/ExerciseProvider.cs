using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Models.DAL.Exercises;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Lib.DAL.Providers;

public sealed class ExerciseProvider : IExerciseProvider
{
    private readonly CoucherDbContext _dbContext;
    private readonly DbSet<Exercise> _entities;

    public ExerciseProvider(CoucherDbContext dbContext)
    {
        _dbContext = dbContext;
        _entities = dbContext.Set<Exercise>();
    }

    public async Task<List<Exercise>> ListAsync(CancellationToken cancellationToken = default)
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

    public async Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = BuildQuery();
        var entity = await query.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return entity;
    }

    public async Task<Exercise> AddAsync(Exercise entity, CancellationToken cancellationToken = default)
    {
        await _entities.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<Exercise> UpdateAsync(Exercise entity, CancellationToken cancellationToken = default)
    {
        _entities.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task DeleteAsync(Exercise entity, CancellationToken cancellationToken = default)
    {
        _entities.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Exercise> BuildQuery()
    {
        var query = _entities
        .Include(item => item.ManagerParticipant)
        .Include(item => item.TraineeUnitContactParticipant)
        .Include(item => item.Participants)
        .ThenInclude(item => item.User)
        .Include(item => item.Influencers)
        .Include(item => item.ThreatArenas)
        .Include(item => item.Tasks)
        .ThenInclude(item => item.ResponsibleUser)
        .Include(item => item.Tasks)
        .ThenInclude(item => item.Influencers)
        .Include(item => item.Tasks)
        .ThenInclude(item => item.Dependencies)
        .Include(item => item.Tasks)
        .ThenInclude(item => item.DependedOnByTasks);

        return query;
    }
}
