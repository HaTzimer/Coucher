using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Admin;

public sealed class Unit
{
    [Key]
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public Guid EchelonClosedListItemId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(EchelonClosedListItemId))]
    public required ClosedListItem EchelonClosedListItem { get; set; }
}

