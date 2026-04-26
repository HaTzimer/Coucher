namespace Coucher.Shared.Models.DAL.Admin;

public sealed class ClosedListItem
{
    public Guid Id { get; set; }
    public required string ListKey { get; set; }
    public required string Code { get; set; }
    public required string Value { get; set; }
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}