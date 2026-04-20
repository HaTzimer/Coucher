using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Models.DAL.Admin;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Lib.DAL.Providers;

public sealed class ClosedListItemProvider : IClosedListItemProvider
{
    private readonly CoucherDbContext _dbContext;
    private readonly DbSet<ClosedListItem> _entities;

    public ClosedListItemProvider(CoucherDbContext dbContext)
    {
        _dbContext = dbContext;
        _entities = dbContext.Set<ClosedListItem>();
    }

    public async Task<List<ClosedListItem>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _entities.ToListAsync(cancellationToken);

        return items;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var exists = await _entities.AnyAsync(item => item.Id == id, cancellationToken);

        return exists;
    }

    public async Task<ClosedListItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _entities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return entity;
    }

    public async Task<ClosedListItem> AddAsync(ClosedListItem entity, CancellationToken cancellationToken = default)
    {
        await _entities.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<ClosedListItem> UpdateAsync(ClosedListItem entity, CancellationToken cancellationToken = default)
    {
        _entities.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task DeleteAsync(ClosedListItem entity, CancellationToken cancellationToken = default)
    {
        _entities.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
