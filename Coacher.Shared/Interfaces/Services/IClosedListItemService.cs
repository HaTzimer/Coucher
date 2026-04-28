using Coacher.Shared.Models.WebApi.Requests.Admin;
using Coacher.Shared.Models.DAL.Admin;

namespace Coacher.Shared.Interfaces.Services;

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
    Task<ClosedListItem> UpdateClosedListItemAsync(
        Guid closedListItemId,
        UpdateClosedListItemRequest request,
        CancellationToken cancellationToken = default
    );
    Task<List<ClosedListItem>> BulkUpdateClosedListItemDisplayOrdersAsync(
        BulkUpdateClosedListItemDisplayOrdersRequest request,
        CancellationToken cancellationToken = default
    );
    Task<ClosedListItem> SetClosedListItemArchiveStateAsync(
        Guid closedListItemId,
        bool isArchived,
        CancellationToken cancellationToken = default
    );
}
