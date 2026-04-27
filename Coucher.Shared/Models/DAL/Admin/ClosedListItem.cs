using Coucher.Shared;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Admin;

[Table(ConstantValues.ClosedListItemTableName)]
[Index(nameof(Key), nameof(DisplayOrder))]
[GraphQLDescription("A configurable lookup entry used for dropdowns, statuses, and other closed lists.")]
public sealed class ClosedListItem
{
    [GraphQLDescription("The unique identifier of the closed-list entry.")]
    public Guid Id { get; set; }
    [GraphQLDescription("The logical list key that groups related closed-list entries.")]
    [MaxLength(256)]
    public required string Key { get; set; }
    [GraphQLDescription("The display value shown to users for this entry.")]
    [MaxLength(256)]
    public required string Value { get; set; }
    [GraphQLDescription("An optional explanation or note for this entry.")]
    [MaxLength(1024)]
    public string? Description { get; set; }
    [GraphQLDescription("The display order within the same closed-list key.")]
    public int? DisplayOrder { get; set; }
    [GraphQLDescription("When the entry was created.")]
    public required DateTime CreationTime { get; set; }
    [GraphQLDescription("When the entry was last updated.")]
    public required DateTime LastUpdateTime { get; set; }
}
