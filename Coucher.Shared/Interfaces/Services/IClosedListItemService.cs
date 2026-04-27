using Coucher.Shared.Models.WebApi.Requests.Admin;
using Coucher.Shared.Models.DAL.Admin;

namespace Coucher.Shared.Interfaces.Services;

public interface IClosedListItemService : IServiceBase<ClosedListItem, Guid>
{
    Task<ClosedListItem> CreateClosedListItemAsync(
        CreateClosedListItemRequest request,
        CancellationToken cancellationToken = default
    );
    Task<List<ClosedListItem>> CreateClosedListItemsAsync(
        List<CreateClosedListItemRequest> requests,
        CancellationToken cancellationToken = default
    );
    Task<ClosedListItem> UpdateClosedListItemValueAsync(
        Guid closedListItemId,
        UpdateClosedListItemValueRequest request,
        CancellationToken cancellationToken = default
    );
    Task<ClosedListItem> UpdateClosedListItemDisplayOrderAsync(
        Guid closedListItemId,
        UpdateClosedListItemDisplayOrderRequest request,
        CancellationToken cancellationToken = default
    );
    Task<List<ClosedListItem>> BulkUpdateClosedListItemDisplayOrdersAsync(
        BulkUpdateClosedListItemDisplayOrdersRequest request,
        CancellationToken cancellationToken = default
    );
    Task<ClosedListItem> ArchiveClosedListItemAsync(Guid closedListItemId, CancellationToken cancellationToken = default);
    Task<ClosedListItem> UnarchiveClosedListItemAsync(
        Guid closedListItemId,
        CancellationToken cancellationToken = default
    );
}
