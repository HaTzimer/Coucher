using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Models.DAL.Admin;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Lib.DAL.Providers;

public sealed class ClosedListItemProvider : IClosedListItemProvider
{
    private readonly IDbContextFactory<CoucherDbContext> _dbContextFactory;

    public ClosedListItemProvider(IDbContextFactory<CoucherDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<ClosedListItem>> ListAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ClosedListItem>();
        var items = await entities.ToListAsync(cancellationToken);

        return items;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ClosedListItem>();
        var exists = await entities.AnyAsync(item => item.Id == id, cancellationToken);

        return exists;
    }

    public async Task<ClosedListItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ClosedListItem>();
        var entity = await entities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return entity;
    }

    public async Task<ClosedListItem> AddAsync(ClosedListItem entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ClosedListItem>();
        await entities.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<ClosedListItem> UpdateAsync(ClosedListItem entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ClosedListItem>();
        entities.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task DeleteAsync(ClosedListItem entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ClosedListItem>();
        entities.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
