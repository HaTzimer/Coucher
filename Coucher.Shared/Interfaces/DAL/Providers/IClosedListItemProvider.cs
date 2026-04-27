using Coucher.Shared.Models.DAL.Admin;

namespace Coucher.Shared.Interfaces.DAL.Providers;

public interface IClosedListItemProvider : IProviderBase<ClosedListItem, Guid>
{
    Task<Guid?> GetHighestDisplayOrderItemIdByKeyAsync(
        string key,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default
    );
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
    Task<List<ClosedListItem>> UpdateClosedListItemsAsync(
        List<ClosedListItem> entities,
        CancellationToken cancellationToken = default
    );
    Task<ClosedListItem> ArchiveClosedListItemAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ClosedListItem> UnarchiveClosedListItemAsync(Guid id, CancellationToken cancellationToken = default);
}
