using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Models.DAL.Admin;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Lib.DAL.Providers;

public sealed class FixedTaskTemplateProvider : IFixedTaskTemplateProvider
{
    private readonly CoucherDbContext _dbContext;
    private readonly DbSet<FixedTaskTemplate> _entities;

    public FixedTaskTemplateProvider(CoucherDbContext dbContext)
    {
        _dbContext = dbContext;
        _entities = dbContext.Set<FixedTaskTemplate>();
    }

    public async Task<List<FixedTaskTemplate>> ListAsync(CancellationToken cancellationToken = default)
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

    public async Task<FixedTaskTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = BuildQuery();
        var entity = await query.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return entity;
    }

    public async Task<FixedTaskTemplate> AddAsync(FixedTaskTemplate entity, CancellationToken cancellationToken = default)
    {
        await _entities.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<FixedTaskTemplate> UpdateAsync(FixedTaskTemplate entity, CancellationToken cancellationToken = default)
    {
        _entities.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task DeleteAsync(FixedTaskTemplate entity, CancellationToken cancellationToken = default)
    {
        _entities.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<FixedTaskTemplate> BuildQuery()
    {
        var query = _entities
        .Include(item => item.Influencers)
        .Include(item => item.Dependencies);

        return query;
    }
}
