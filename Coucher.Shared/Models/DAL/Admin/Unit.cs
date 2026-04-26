using Coucher.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Admin;

[Table(ConstantValues.UnitTableName)]
[Index(nameof(Name))]
[Index(nameof(EchelonId))]
public sealed class Unit
{
    [Key]
    public Guid Id { get; set; }
    [MaxLength(200)]
    public required string Name { get; set; }
    public Guid? EchelonId { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime LastUpdateTime { get; set; }

    [ForeignKey(nameof(EchelonId))]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public ClosedListItem? Echelon { get; set; }
}
