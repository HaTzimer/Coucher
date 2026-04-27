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

    public async Task<Guid?> GetHighestDisplayOrderItemIdByKeyAsync(
        string key,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ClosedListItem>().Where(item => item.Key == key);
        if (excludedId.HasValue)
        {
            entities = entities.Where(item => item.Id != excludedId.Value);
        }

        var entityId = await entities
            .OrderByDescending(item => item.DisplayOrder ?? int.MinValue)
            .ThenByDescending(item => item.Id)
            .Select(item => (Guid?)item.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return entityId;
    }

    public async Task<bool> IsHighestDisplayOrderItemForKeyAsync(
        Guid id,
        string key,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ClosedListItem>();
        var targetEntity = await entities.FirstOrDefaultAsync(
            item => item.Id == id && item.Key == key,
            cancellationToken
        );
        if (targetEntity is null)
        {
            return false;
        }

        var highestDisplayOrder = await entities
            .Where(item => item.Key == key)
            .MaxAsync(item => (int?)item.DisplayOrder, cancellationToken);
        var isHighestDisplayOrderItem = targetEntity.DisplayOrder == highestDisplayOrder;

        return isHighestDisplayOrderItem;
    }

    public async Task<ClosedListItem> CreateClosedListItemAsync(
        ClosedListItem entity,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ClosedListItem>();
        await entities.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<List<ClosedListItem>> CreateClosedListItemsAsync(
        List<ClosedListItem> entitiesToCreate,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ClosedListItem>();
        await entities.AddRangeAsync(entitiesToCreate, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entitiesToCreate;
    }

    public async Task<ClosedListItem> UpdateClosedListItemAsync(
        ClosedListItem entity,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ClosedListItem>();
        entities.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<List<ClosedListItem>> UpdateClosedListItemsAsync(
        List<ClosedListItem> entitiesToUpdate,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ClosedListItem>();
        entities.UpdateRange(entitiesToUpdate);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entitiesToUpdate;
    }

    public async Task<ClosedListItem> ArchiveClosedListItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ClosedListItem>();
        var entity = await entities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"{nameof(ClosedListItem)} '{id}' was not found.");
        entity.IsArchive = true;
        entity.LastUpdateTime = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<ClosedListItem> UnarchiveClosedListItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ClosedListItem>();
        var entity = await entities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"{nameof(ClosedListItem)} '{id}' was not found.");
        entity.IsArchive = false;
        entity.LastUpdateTime = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<ClosedListItem> AddAsync(ClosedListItem entity, CancellationToken cancellationToken = default)
    {
        var createdEntity = await CreateClosedListItemAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<ClosedListItem> UpdateAsync(ClosedListItem entity, CancellationToken cancellationToken = default)
    {
        var updatedEntity = await UpdateClosedListItemAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task DeleteAsync(ClosedListItem entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<ClosedListItem>();
        entities.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
