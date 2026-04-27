using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared.Models.WebApi.Requests.Admin;

namespace Coucher.Lib.Services;

public sealed class ClosedListItemService : IClosedListItemService
{
    private readonly IClosedListItemRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public ClosedListItemService(IClosedListItemRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
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
        CreateClosedListItemRequestModel request,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var entity = new ClosedListItem
        {
            Id = Guid.NewGuid(),
            Key = request.Key,
            Value = request.Value,
            Description = request.Description,
            DisplayOrder = request.DisplayOrder,
            CreationTime = now,
            LastUpdateTime = now
        };
        var createdEntity = await _repository.CreateClosedListItemAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<List<ClosedListItem>> CreateClosedListItemsAsync(
        List<CreateClosedListItemRequestModel> requests,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var entities = requests.Select(request => new ClosedListItem
        {
            Id = Guid.NewGuid(),
            Key = request.Key,
            Value = request.Value,
            Description = request.Description,
            DisplayOrder = request.DisplayOrder,
            CreationTime = now,
            LastUpdateTime = now
        }).ToList();
        var createdEntities = await _repository.CreateClosedListItemsAsync(entities, cancellationToken);

        return createdEntities;
    }

    public async Task<ClosedListItem> UpdateClosedListItemAsync(
        Guid closedListItemId,
        UpdateClosedListItemRequestModel request,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(closedListItemId, cancellationToken);
        entity.Key = request.Key;
        entity.Value = request.Value;
        entity.Description = request.Description;
        entity.DisplayOrder = request.DisplayOrder;
        entity.LastUpdateTime = DateTime.UtcNow;
        var updatedEntity = await _repository.UpdateClosedListItemAsync(entity, cancellationToken);

        return updatedEntity;
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

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
    }
}
