using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Models.DAL.Notifications;
using Coucher.Shared.Models.DAL.Tasks;
using Coucher.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Users;

[Table(ConstantValues.UserProfileTableName)]
[Index(nameof(IdentityNumber), IsUnique = true)]
[Index(nameof(ExternalId))]
[Index(nameof(UnitId))]
public sealed class UserProfile
{
    public Guid Id { get; set; }
    [MaxLength(50)]
    public required string IdentityNumber { get; set; }
    [MaxLength(100)]
    public required string FirstName { get; set; }
    [MaxLength(100)]
    public required string LastName { get; set; }
    [MaxLength(50)]
    public string? PersonalNumber { get; set; }
    [MaxLength(128)]
    public string? ExternalId { get; set; }
    public Guid? UnitId { get; set; }
    [MaxLength(100)]
    public string? Rank { get; set; }
    [MaxLength(150)]
    public string? Position { get; set; }
    [MaxLength(32)]
    public string? PhoneNumber { get; set; }
    [MaxLength(256)]
    public string? CivilianEmail { get; set; }
    [MaxLength(256)]
    public string? MilitaryEmail { get; set; }
    [MaxLength(2048)]
    public string? ProfileImageUrl { get; set; }
    [MaxLength(512)]
    public string? PasswordHash { get; set; }
    public DateTime? LastLoginTime { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime LastUpdateTime { get; set; }

    [ForeignKey(nameof(UnitId))]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public Unit? Unit { get; set; }

    [InverseProperty("User")]
    public required List<UserRole> Roles { get; set; }

    [InverseProperty("User")]
    public required List<ExerciseParticipant> ExerciseParticipants { get; set; }

    [InverseProperty("User")]
    public required List<ExerciseTaskResponsibleUser> ResponsibleTaskLinks { get; set; }

    [InverseProperty("User")]
    public required List<UserNotification> Notifications { get; set; }
}
