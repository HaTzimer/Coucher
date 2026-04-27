namespace Coucher.Shared.Models.WebApi.Requests.Admin;

public sealed class ClosedListItemDisplayOrderUpdateRequest
{
    public Guid Id { get; set; }
    public int? DisplayOrder { get; set; }
}
