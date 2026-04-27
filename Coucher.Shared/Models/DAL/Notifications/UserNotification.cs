using Coucher.Shared;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Models.DAL.Users;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Notifications;

[Table(ConstantValues.UserNotificationTableName)]
[Index(nameof(UserId), nameof(IsRead))]
[Index(nameof(UserId), nameof(CreationTime))]
[Index(nameof(ExerciseId))]
[Index(nameof(TaskId))]
[GraphQLDescription("A notification shown to a user.")]
public sealed class UserNotification
{
    [GraphQLDescription("The unique identifier of the notification.")]
    public Guid Id { get; set; }
    [GraphQLDescription("The user id that receives the notification.")]
    public Guid? UserId { get; set; }
    [GraphQLDescription("The notification title shown to the user.")]
    [MaxLength(256)]
    public required string Title { get; set; }
    [GraphQLDescription("The detailed notification message shown to the user.")]
    [MaxLength(1024)]
    public required string Message { get; set; }
    [GraphQLDescription("When the notification was created.")]
    public required DateTime CreationTime { get; set; }
    [GraphQLDescription("Whether the user has marked the notification as read.")]
    public required bool IsRead { get; set; }
    [GraphQLDescription("The related exercise id, when the notification points to an exercise.")]
    public Guid? ExerciseId { get; set; }
    [GraphQLDescription("The related task id, when the notification points to a task.")]
    public Guid? TaskId { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty("Notifications")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The user who receives the notification.")]
    public UserProfile? User { get; set; }

    [ForeignKey(nameof(ExerciseId))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The exercise related to the notification.")]
    public Exercise? Exercise { get; set; }
}
