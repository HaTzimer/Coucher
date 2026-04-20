using Coucher.Shared.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Users;

public sealed class UserRole
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public GlobalRole Role { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty("Roles")]
    public required UserProfile User { get; set; }
}
