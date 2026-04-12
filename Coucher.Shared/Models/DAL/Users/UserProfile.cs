using Coucher.Shared.Interfaces;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Models.DAL.Notifications;
using Coucher.Shared.Models.DAL.Tasks;
using Coucher.Shared.Models.Enums;

namespace Coucher.Shared.Models.DAL.Users;

public sealed class UserProfile : IHasId<Guid>
{
    public Guid Id { get; set; }
    public required string IdentityNumber { get; set; }
    public string? PersonalNumber { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Rank { get; set; }
    public string? Position { get; set; }
    public required string PhoneNumber { get; set; }
    public required string CivilianEmail { get; set; }
    public string? MilitaryEmail { get; set; }
    public string? PasswordHash { get; set; }
    public bool IsFirstLoginCompleted { get; set; }
    public bool IsForgotPasswordLocked { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public required List<UserRole> Roles { get; set; }
    public required List<UserSecurityQuestion> SecurityQuestions { get; set; }
    public required List<ExerciseParticipant> ExerciseParticipants { get; set; }
    public required List<ExerciseTask> ResponsibleTasks { get; set; }
    public required List<UserNotification> Notifications { get; set; }
}
