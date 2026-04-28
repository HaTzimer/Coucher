namespace Coacher.Shared.Models.WebApi.Requests.Admin;

public sealed class UpdateClosedListItemRequest
{
    public string? Value { get; set; }

    public string? Description { get; set; }

    public bool ClearDescription { get; set; }

    public int? DisplayOrder { get; set; }
}
