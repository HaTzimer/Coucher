using Coucher.Shared.Models.DAL.Users;
using Coucher.Shared.Models.WebApi.Requests.Admin;

namespace Coucher.Shared.Interfaces.Services;

public interface IUserRoleService : IServiceBase<UserRole, Guid>
{
    Task<UserRole> UpdateUserRoleAsync(
        Guid userRoleId,
        UpdateUserRoleRequest request,
        CancellationToken cancellationToken = default
    );
}
