using Coucher.Shared.Models.Internal.Authentication;

namespace Coucher.Shared.Interfaces.DAL.Providers;

public interface IAuthorizationCacheProvider
{
    Task<string> CreateUserSessionAsync(Guid userId);
    Task RemoveUserSessionByUserIdAsync(Guid userId);
    Task<UserAuthenticationResult> GetUserAuthenticationResultBySessionAsync(string sessionId);
}
