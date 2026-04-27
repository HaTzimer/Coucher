using Coucher.Shared.Models.DAL.Users;

namespace Coucher.Shared.Interfaces.DAL.Providers;

public interface IUserProfileProvider : IProviderBase<UserProfile, Guid>
{
    Task<UserProfile?> GetByIdentityNumberAsync(string identityNumber, CancellationToken cancellationToken = default);
    Task UpdateLastLoginTimeAsync(Guid userId, DateTime lastLoginTime, CancellationToken cancellationToken = default);
}
