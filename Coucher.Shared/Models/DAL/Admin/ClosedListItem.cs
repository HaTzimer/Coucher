using Coucher.Shared;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Admin;

[Table(ConstantValues.ClosedListItemTableName)]
[Index(nameof(Key), nameof(DisplayOrder))]
public sealed class ClosedListItem
{
    public Guid Id { get; set; }
    [MaxLength(256)]
    public required string Key { get; set; }
    [MaxLength(256)]
    public required string Value { get; set; }
    [MaxLength(1024)]
    public string? Description { get; set; }
    public int? DisplayOrder { get; set; }
    public required DateTime CreationTime { get; set; }
    public required DateTime LastUpdateTime { get; set; }
}
