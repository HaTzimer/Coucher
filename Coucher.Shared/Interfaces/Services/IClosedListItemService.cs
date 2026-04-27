using Coucher.Shared.Models.WebApi.Requests.Admin;
using Coucher.Shared.Models.DAL.Admin;

namespace Coucher.Shared.Interfaces.Services;

public interface IClosedListItemService : IServiceBase<ClosedListItem, Guid>
{
    Task<ClosedListItem> CreateClosedListItemAsync(
        CreateClosedListItemRequestModel request,
        CancellationToken cancellationToken = default
    );
    Task<List<ClosedListItem>> CreateClosedListItemsAsync(
        List<CreateClosedListItemRequestModel> requests,
        CancellationToken cancellationToken = default
    );
    Task<ClosedListItem> UpdateClosedListItemAsync(
        Guid closedListItemId,
        UpdateClosedListItemRequestModel request,
        CancellationToken cancellationToken = default
    );
}
