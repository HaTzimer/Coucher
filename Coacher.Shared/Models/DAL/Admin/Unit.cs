using Coacher.Shared;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coacher.Shared.Models.DAL.Admin;

[Table(ConstantValues.UnitTableName)]
[Index(nameof(Name))]
[Index(nameof(EchelonId))]
[GraphQLDescription("An organizational unit that can be assigned to users and exercises.")]
public sealed class Unit
{
    [Key]
    [GraphQLDescription("The unique identifier of the unit.")]
    public Guid Id { get; set; }

    [GraphQLDescription("The display name of the unit.")]
    [MaxLength(256)]
    public required string Name { get; set; }

    [GraphQLDescription("The closed-list id of the unit echelon.")]
    public Guid? EchelonId { get; set; }

    [GraphQLDescription("When the unit was created.")]
    public DateTime CreationTime { get; set; }

    [GraphQLDescription("When the unit was last updated.")]
    public DateTime LastUpdateTime { get; set; }

    [ForeignKey(nameof(EchelonId))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The closed-list entry that represents the unit echelon.")]
    public ClosedListItem? Echelon { get; set; }

}
