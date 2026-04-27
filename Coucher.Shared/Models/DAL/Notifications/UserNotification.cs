using Coucher.Shared.Constants;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Models.DAL.Users;
using Coucher.Shared.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Notifications;

[Table(ConstantValues.UserNotificationTableName)]
[Index(nameof(UserId), nameof(IsRead))]
[Index(nameof(UserId), nameof(CreationTime))]
[Index(nameof(ExerciseId))]
[Index(nameof(TaskId))]
public sealed class UserNotification
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public NotificationType Type { get; set; }
    public NotificationSeverity Severity { get; set; }
    [MaxLength(256)]
    public required string Title { get; set; }
    [MaxLength(1024)]
    public required string Message { get; set; }
    public DateTime CreationTime { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadTime { get; set; }
    public Guid? ExerciseId { get; set; }
    public Guid? TaskId { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty("Notifications")]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public UserProfile? User { get; set; }

    [ForeignKey(nameof(ExerciseId))]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public Exercise? Exercise { get; set; }
}
