using Coucher.Shared.Interfaces;
using Coucher.Shared.Models.DAL.Users;
using Coucher.Shared.Models.Enums;

namespace Coucher.Shared.Models.DAL.Notifications;

public sealed class UserNotification : IHasId<Guid>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public required string Title { get; set; }
    public required string Message { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public bool IsRead { get; set; }
    public Guid? ExerciseId { get; set; }
    public Guid? TaskId { get; set; }
    public required UserProfile User { get; set; }
}
