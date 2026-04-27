using Coucher.Shared.Models.Enums;

namespace Coucher.Shared.Models.WebApi.Requests.Admin;

public sealed class CreateUserRoleRequestModel
{
    public Guid UserId { get; set; }
    public GlobalRole Role { get; set; }
    public bool IsActive { get; set; }
}
