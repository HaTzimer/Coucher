using Coacher.Shared.Models.Internal.Authentication;

namespace Coacher.Shared.Interfaces.DAL.Providers;

public interface ICacheProvider
{
    Task<string> CreateUserSessionAsync(Guid userId);
    Task RemoveUserSessionByUserIdAsync(Guid userId);
    Task<UserAuthenticationResult> GetUserAuthenticationResultBySessionAsync(string sessionId);
}
