using Coacher.Shared.Models.Enums;
using Coacher.Shared.Models.DAL.Users;

namespace Coacher.Shared.Interfaces.DAL.Providers;

public interface IUserRoleProvider : IProviderBase<UserRole, Guid>
{
    Task<List<UserRole>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserRole?> GetByUserIdAndRoleAsync(Guid userId, GlobalRole role, CancellationToken cancellationToken = default);
    Task<UserRole> CreateUserRoleAsync(UserRole entity, CancellationToken cancellationToken = default);
    Task<UserRole> UpdateUserRoleAsync(UserRole entity, CancellationToken cancellationToken = default);
}
