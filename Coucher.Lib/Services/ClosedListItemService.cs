using Augustus.Infra.Core.Shared.Interfaces;
using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared.Models.WebApi.Requests.Admin;

namespace Coucher.Lib.Services;

public sealed class ClosedListItemService : IClosedListItemService
{
    private readonly IAugustusLogger _logger;
    private readonly IClosedListItemRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICoucherAuthorizationService _authorizationService;

    public ClosedListItemService(
        IAugustusLogger logger,
        IClosedListItemRepository repository,
        ICurrentUserService currentUserService,
        ICoucherAuthorizationService authorizationService
    )
    {
        _logger = logger;
        _repository = repository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<List<ClosedListItem>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.ListAsync(cancellationToken);

        return items;
    }

    public async Task<ClosedListItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<ClosedListItem> GetRequiredByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetRequiredByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<ClosedListItem> CreateClosedListItemAsync(
        CreateClosedListItemRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var entity = new ClosedListItem
        {
            Id = Guid.NewGuid(),
            Key = request.Key,
            Value = request.Value,
            Description = request.Description,
            DisplayOrder = request.DisplayOrder,
            IsArchive = false,
            CreationTime = now,
            LastUpdateTime = now
        };

        var createdEntity = await _repository.CreateClosedListItemAsync(entity, cancellationToken);

        _logger.Info("Closed-list item created", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "closedListItemId", createdEntity.Id },
            { "key", createdEntity.Key },
            { "result", "success" }
        });

        return createdEntity;
    }

    public async Task<List<ClosedListItem>> CreateClosedListItemsAsync(
        List<CreateClosedListItemRequest> requests,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var entities = requests.Select(request => new ClosedListItem
        {
            Id = Guid.NewGuid(),
            Key = request.Key,
            Value = request.Value,
            Description = request.Description,
            DisplayOrder = request.DisplayOrder,
            IsArchive = false,
            CreationTime = now,
            LastUpdateTime = now
        }).ToList();

        _logger.Info("Closed-list item bulk create started", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "count", entities.Count },
            { "result", "started" }
        });

        var createdEntities = await _repository.CreateClosedListItemsAsync(entities, cancellationToken);

        _logger.Info("Closed-list item bulk create completed", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "count", createdEntities.Count },
            { "result", "success" }
        });

        return createdEntities;
    }

    public async Task<ClosedListItem> UpdateClosedListItemAsync(
        Guid closedListItemId,
        UpdateClosedListItemRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(closedListItemId, cancellationToken);

        entity.Value = request.Value;
        entity.Description = request.Description;
        entity.DisplayOrder = request.DisplayOrder;
        entity.LastUpdateTime = DateTime.UtcNow;

        var updatedEntity = await _repository.UpdateClosedListItemAsync(entity, cancellationToken);

        _logger.Info("Closed-list item updated", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "closedListItemId", closedListItemId },
            { "key", entity.Key },
            { "changedFields", new[] { "Value", "Description", "DisplayOrder" } },
            { "result", "success" }
        });

        return updatedEntity;
    }

    public async Task<ClosedListItem> UpdateClosedListItemValueAsync(
        Guid closedListItemId,
        string value,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(closedListItemId, cancellationToken);

        entity.Value = value;
        entity.LastUpdateTime = DateTime.UtcNow;

        var updatedEntity = await _repository.UpdateClosedListItemAsync(entity, cancellationToken);

        _logger.Info("Closed-list item value updated", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "closedListItemId", closedListItemId },
            { "key", entity.Key },
            { "changedFields", new[] { "Value" } },
            { "result", "success" }
        });

        return updatedEntity;
    }

    public async Task<ClosedListItem> UpdateClosedListItemDescriptionAsync(
        Guid closedListItemId,
        string? description,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(closedListItemId, cancellationToken);

        entity.Description = description;
        entity.LastUpdateTime = DateTime.UtcNow;

        var updatedEntity = await _repository.UpdateClosedListItemAsync(entity, cancellationToken);

        _logger.Info("Closed-list item description updated", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "closedListItemId", closedListItemId },
            { "key", entity.Key },
            { "changedFields", new[] { "Description" } },
            { "result", "success" }
        });

        return updatedEntity;
    }

    public async Task<ClosedListItem> UpdateClosedListItemDisplayOrderAsync(
        Guid closedListItemId,
        int? displayOrder,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(closedListItemId, cancellationToken);

        entity.DisplayOrder = displayOrder;
        entity.LastUpdateTime = DateTime.UtcNow;

        var updatedEntity = await _repository.UpdateClosedListItemAsync(entity, cancellationToken);

        _logger.Info("Closed-list item display order updated", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "closedListItemId", closedListItemId },
            { "key", entity.Key },
            { "changedFields", new[] { "DisplayOrder" } },
            { "result", "success" }
        });

        return updatedEntity;
    }

    public async Task<List<ClosedListItem>> BulkUpdateClosedListItemDisplayOrdersAsync(
        BulkUpdateClosedListItemDisplayOrdersRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        if (request.Items.Count == 0)
            return new List<ClosedListItem>();

        var updatesById = request.Items
            .GroupBy(item => item.Id)
            .Select(group => group.Last())
            .ToList();
        var now = DateTime.UtcNow;
        var entities = new List<ClosedListItem>(updatesById.Count);

        _logger.Info("Closed-list item display-order bulk update started", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "count", updatesById.Count },
            { "result", "started" }
        });

        foreach (var item in updatesById)
        {
            var entity = await _repository.GetRequiredByIdAsync(item.Id, cancellationToken);

            entity.DisplayOrder = item.DisplayOrder;
            entity.LastUpdateTime = now;
            entities.Add(entity);
        }

        var updatedEntities = await _repository.UpdateClosedListItemsAsync(entities, cancellationToken);

        _logger.Info("Closed-list item display-order bulk update completed", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "count", updatedEntities.Count },
            { "result", "success" }
        });

        return updatedEntities;
    }

    public async Task<ClosedListItem> ArchiveClosedListItemAsync(
        Guid closedListItemId,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var archivedEntity = await _repository.ArchiveClosedListItemAsync(closedListItemId, cancellationToken);

        _logger.Info("Closed-list item archived", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "closedListItemId", closedListItemId },
            { "key", archivedEntity.Key },
            { "result", "success" }
        });

        return archivedEntity;
    }

    public async Task<ClosedListItem> UnarchiveClosedListItemAsync(
        Guid closedListItemId,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var unarchivedEntity = await _repository.UnarchiveClosedListItemAsync(closedListItemId, cancellationToken);

        _logger.Info("Closed-list item unarchived", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "closedListItemId", closedListItemId },
            { "key", unarchivedEntity.Key },
            { "result", "success" }
        });

        return unarchivedEntity;
    }

    public async Task<ClosedListItem> AddAsync(ClosedListItem entity, CancellationToken cancellationToken = default)
    {
        var createdEntity = await _repository.AddAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<ClosedListItem> UpdateAsync(ClosedListItem entity, CancellationToken cancellationToken = default)
    {
        var updatedEntity = await _repository.UpdateAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Closed-list items use archive and unarchive operations instead of physical delete.");
    }
}
