namespace Coacher.Shared.Models.WebApi.Requests.Admin;

public sealed class BulkUpdateClosedListItemDisplayOrdersRequest
{
    public required List<ClosedListItemDisplayOrderUpdateRequest> Items { get; set; }
}
