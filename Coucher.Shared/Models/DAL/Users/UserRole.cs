using Coucher.Shared;
using Coucher.Shared.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Users;

[Table(ConstantValues.UserRoleTableName)]
public sealed class UserRole
{
    [Key]
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public GlobalRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime AssignedTime { get; set; }
    public Guid? AssignedByUserId { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty("Roles")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    public UserProfile? User { get; set; }
}
