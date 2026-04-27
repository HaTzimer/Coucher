using Coucher.Shared.Models.DAL.Admin;

namespace Coucher.Shared.Interfaces.Repositories;

public interface IClosedListItemRepository : IRepositoryBase<ClosedListItem, Guid>
{
    Task<bool> IsHighestDisplayOrderItemForKeyAsync(
        Guid id,
        string key,
        CancellationToken cancellationToken = default
    );
    Task<ClosedListItem> CreateClosedListItemAsync(ClosedListItem entity, CancellationToken cancellationToken = default);
    Task<List<ClosedListItem>> CreateClosedListItemsAsync(
        List<ClosedListItem> entities,
        CancellationToken cancellationToken = default
    );
    Task<ClosedListItem> UpdateClosedListItemAsync(
        ClosedListItem entity,
        CancellationToken cancellationToken = default
    );
}
