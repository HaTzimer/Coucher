using Coacher.Shared.Models.DAL.Users;
using Coacher.Shared.Models.WebApi.Requests.Admin;

namespace Coacher.Shared.Interfaces.Services;

public interface IUserRoleService : IServiceBase<UserRole, Guid>
{
    Task<UserRole> UpdateUserRoleAsync(
        Guid userRoleId,
        UpdateUserRoleRequest request,
        CancellationToken cancellationToken = default
    );
}
