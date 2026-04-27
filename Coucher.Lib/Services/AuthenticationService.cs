using Augustus.Infra.Core.Shared.Interfaces;
using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.Internal.Authentication;
using Coucher.Shared.Models.Enums;

namespace Coucher.Lib.Services;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly ICacheProvider _cacheProvider;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IHashGenerator _hashGenerator;

    public AuthenticationService(
        ICacheProvider cacheProvider,
        IUserProfileRepository userProfileRepository,
        IHashGenerator hashGenerator
    )
    {
        _cacheProvider = cacheProvider;
        _userProfileRepository = userProfileRepository;
        _hashGenerator = hashGenerator;
    }

    public async Task<AuthenticatedSession?> LoginAsync(
        string identityNumber,
        string passwordOrPersonalNumber,
        CancellationToken cancellationToken = default
    )
    {
        var user = await _userProfileRepository.GetByIdentityNumberAsync(identityNumber, cancellationToken);
        if (user is null || !IsValidCredential(user, passwordOrPersonalNumber))
        {
            return null;
        }

        await _cacheProvider.RemoveUserSessionByUserIdAsync(user.Id);
        await _userProfileRepository.UpdateLastLoginTimeAsync(user.Id, DateTime.UtcNow, cancellationToken);

        var sessionId = await _cacheProvider.CreateUserSessionAsync(user.Id);

        return new AuthenticatedSession
        {
            SessionId = sessionId,
            UserId = user.Id
        };
    }

    public async Task LogoutAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        var authenticationResult = await _cacheProvider.GetUserAuthenticationResultBySessionAsync(sessionId);
        if (!authenticationResult.IsValid || authenticationResult.UserId is null)
        {
            return;
        }

        await _cacheProvider.RemoveUserSessionByUserIdAsync(authenticationResult.UserId.Value);
    }

    public async Task<HeaderAuthenticationResult> AuthenticateSessionAsync(
        string? sessionId,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return new HeaderAuthenticationResult
            {
                HeaderAuthentication = SessionAuthenticationResult.Missing,
                ErrorMessage = "No session id sent."
            };
        }

        var authenticationResult = await _cacheProvider.GetUserAuthenticationResultBySessionAsync(sessionId);
        if (!authenticationResult.IsValid || authenticationResult.UserId is null)
        {
            return new HeaderAuthenticationResult
            {
                HeaderAuthentication = SessionAuthenticationResult.Invalid,
                SessionId = sessionId,
                ErrorMessage = "Invalid session id sent."
            };
        }

        return new HeaderAuthenticationResult
        {
            HeaderAuthentication = SessionAuthenticationResult.Valid,
            SessionId = sessionId,
            UserId = authenticationResult.UserId.Value
        };
    }

    private bool IsValidCredential(Coucher.Shared.Models.DAL.Users.UserProfile user, string passwordOrPersonalNumber)
    {
        if (!string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            var passwordHash = _hashGenerator.GetHashString(passwordOrPersonalNumber);

            return string.Equals(user.PasswordHash, passwordHash, StringComparison.Ordinal);
        }

        return !string.IsNullOrWhiteSpace(user.PersonalNumber)
            && string.Equals(user.PersonalNumber, passwordOrPersonalNumber, StringComparison.Ordinal);
    }
}
