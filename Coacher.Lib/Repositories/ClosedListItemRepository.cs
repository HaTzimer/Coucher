using System.Net;
using Augustus.Infra.Core.Shared.Exceptions;
using Augustus.Infra.Core.Shared.Interfaces;
using Coacher.Shared.Interfaces.DAL.Providers;
using Coacher.Shared.Interfaces.Repositories;
using Coacher.Shared.Models.DAL.Admin;

namespace Coacher.Lib.Repositories;

public sealed class ClosedListItemRepository : IClosedListItemRepository
{
    private readonly IAugustusLogger _logger;
    private readonly IClosedListItemProvider _provider;

    public ClosedListItemRepository(IAugustusLogger logger, IClosedListItemProvider provider)
    {
        _logger = logger;
        _provider = provider;
    }

    public async Task<List<ClosedListItem>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _provider.ListAsync(cancellationToken);

        return items;
    }

    public async Task<ClosedListItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _provider.GetByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<Guid?> GetHighestDisplayOrderItemIdByKeyAsync(
        string key,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default
    )
    {
        var entityId = await _provider.GetHighestDisplayOrderItemIdByKeyAsync(key, excludedId, cancellationToken);

        return entityId;
    }

    public async Task<ClosedListItem> GetRequiredByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _provider.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            var exception = new HttpStatusCodeException(
                $"{nameof(ClosedListItem)} '{id}' was not found.",
                new Dictionary<string, object?>
                {
                    { "resourceName", nameof(ClosedListItem) },
                    { "resourceId", id }
                },
                HttpStatusCode.NotFound
            );

            _logger.Error(exception);

            throw exception;
        }

        return entity;
    }

    public async Task<bool> IsHighestDisplayOrderItemForKeyAsync(
        Guid id,
        string key,
        CancellationToken cancellationToken = default
    )
    {
        var isHighestDisplayOrderItem = await _provider.IsHighestDisplayOrderItemForKeyAsync(
            id,
            key,
            cancellationToken
        );

        return isHighestDisplayOrderItem;
    }

    public async Task<ClosedListItem> CreateClosedListItemAsync(
        ClosedListItem entity,
        CancellationToken cancellationToken = default
    )
    {
        var createdEntity = await _provider.CreateClosedListItemAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<List<ClosedListItem>> CreateClosedListItemsAsync(
        List<ClosedListItem> entities,
        CancellationToken cancellationToken = default
    )
    {
        var createdEntities = await _provider.CreateClosedListItemsAsync(entities, cancellationToken);

        return createdEntities;
    }

    public async Task<ClosedListItem> UpdateClosedListItemAsync(
        ClosedListItem entity,
        CancellationToken cancellationToken = default
    )
    {
        var updatedEntity = await _provider.UpdateClosedListItemAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<List<ClosedListItem>> UpdateClosedListItemsAsync(
        List<ClosedListItem> entities,
        CancellationToken cancellationToken = default
    )
    {
        var updatedEntities = await _provider.UpdateClosedListItemsAsync(entities, cancellationToken);

        return updatedEntities;
    }

    public async Task<ClosedListItem> ArchiveClosedListItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var archivedEntity = await _provider.ArchiveClosedListItemAsync(id, cancellationToken);

        return archivedEntity;
    }

    public async Task<ClosedListItem> UnarchiveClosedListItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var unarchivedEntity = await _provider.UnarchiveClosedListItemAsync(id, cancellationToken);

        return unarchivedEntity;
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

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetRequiredByIdAsync(id, cancellationToken);
        await _provider.DeleteAsync(entity, cancellationToken);
    }
}

