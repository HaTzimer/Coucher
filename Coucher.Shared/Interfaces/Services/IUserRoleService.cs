using Coucher.Shared.Models.DAL.Users;
using Coucher.Shared.Models.WebApi.Requests.Admin;

namespace Coucher.Shared.Interfaces.Services;

public interface IUserRoleService : IServiceBase<UserRole, Guid>
{
    Task<UserRole> CreateUserRoleAsync(
        CreateUserRoleRequestModel request,
        CancellationToken cancellationToken = default
    );
    Task<UserRole> UpdateUserRoleAsync(
        Guid userRoleId,
        UpdateUserRoleRequestModel request,
        CancellationToken cancellationToken = default
    );
}
