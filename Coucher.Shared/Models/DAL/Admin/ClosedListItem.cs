namespace Coucher.Shared.Models.DAL.Admin;

public sealed class ClosedListItem
{
    public Guid Id { get; set; }
    public required string ListKey { get; set; }
    public required string Value { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}
