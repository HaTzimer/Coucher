using Coucher.Shared.Models.Enums;

namespace Coucher.Shared.Models.DAL.Users;

public sealed class UserRole
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public GlobalRole Role { get; set; }
    public required UserProfile User { get; set; }
}
