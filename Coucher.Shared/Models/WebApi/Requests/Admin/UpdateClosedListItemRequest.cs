namespace Coucher.Shared.Models.WebApi.Requests.Admin;

public sealed class UpdateClosedListItemRequest
{
    public required string Value { get; set; }
    public string? Description { get; set; }
    public int? DisplayOrder { get; set; }
}
