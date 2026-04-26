using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Models.DAL.Notifications;
using Coucher.Shared.Models.DAL.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Users;

public sealed class UserProfile
{
    public Guid Id { get; set; }
    public required string IdentityNumber { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? PersonalNumber { get; set; }
    public string? ExternalId { get; set; }
    public Guid? UnitId { get; set; }
    public string? Rank { get; set; }
    public string? Position { get; set; }
    public string? PhoneNumber { get; set; }
    public string? CivilianEmail { get; set; }
    public string? MilitaryEmail { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? PasswordHash { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(UnitId))]
    public Unit? Unit { get; set; }

    [InverseProperty("User")]
    public required List<UserRole> Roles { get; set; }

    [InverseProperty("User")]
    public required List<ExerciseParticipant> ExerciseParticipants { get; set; }

    [InverseProperty("ResponsibleUser")]
    public required List<ExerciseTask> ResponsibleTasks { get; set; }

    [InverseProperty("User")]
    public required List<UserNotification> Notifications { get; set; }
}
