using Coucher.Shared.Models.DAL.Users;

namespace Coucher.Shared.Interfaces.Repositories;

public interface IUserProfileRepository : IRepositoryBase<UserProfile, Guid>
{
    Task<UserProfile?> GetByIdentityNumberAsync(string identityNumber, CancellationToken cancellationToken = default);
    Task UpdateLastLoginTimeAsync(Guid userId, DateTime lastLoginTime, CancellationToken cancellationToken = default);
}
