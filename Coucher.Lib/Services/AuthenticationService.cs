using Augustus.Infra.Core.Shared.Interfaces;
using Coucher.Shared;
using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.Internal.Authentication;
using Coucher.Shared.Models.Enums;

namespace Coucher.Lib.Services;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly IAugustusLogger _logger;
    private readonly ICacheProvider _cacheProvider;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IHashGenerator _hashGenerator;
    private readonly GlobalRole _starterGlobalRole;

    public AuthenticationService(
        IAugustusLogger logger,
        ICacheProvider cacheProvider,
        IUserProfileRepository userProfileRepository,
        IUserRoleRepository userRoleRepository,
        IHashGenerator hashGenerator,
        IAugustusConfiguration config
    )
    {
        _logger = logger;
        _cacheProvider = cacheProvider;
        _userProfileRepository = userProfileRepository;
        _userRoleRepository = userRoleRepository;
        _hashGenerator = hashGenerator;
        _starterGlobalRole = config.GetOrThrow<GlobalRole>(
            ConfigurationKeys.UserDefaultsSection,
            ConfigurationKeys.StarterGlobalRole
        );
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
            _logger.Warn("Authentication login failed", new Dictionary<string, object>
            {
                { "result", "failed" },
                { "identityNumberSuffix", GetIdentityNumberSuffix(identityNumber) }
            });

            return null;
        }

        await _cacheProvider.RemoveUserSessionByUserIdAsync(user.Id);
        await EnsureStarterRoleAsync(user.Id, cancellationToken);
        await _userProfileRepository.UpdateLastLoginTimeAsync(user.Id, DateTime.UtcNow, cancellationToken);

        var sessionId = await _cacheProvider.CreateUserSessionAsync(user.Id);

        _logger.Info("Authentication login succeeded", new Dictionary<string, object>
        {
            { "result", "success" },
            { "userId", user.Id }
        });

        var authenticatedSession = new AuthenticatedSession
        {
            SessionId = sessionId,
            UserId = user.Id
        };

        return authenticatedSession;
    }

    public async Task LogoutAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        var authenticationResult = await _cacheProvider.GetUserAuthenticationResultBySessionAsync(sessionId);
        if (!authenticationResult.IsValid || authenticationResult.UserId is null)
        {
            _logger.Warn("Authentication logout ignored for invalid session", new Dictionary<string, object>
            {
                { "result", "ignored" }
            });

            return;
        }

        await _cacheProvider.RemoveUserSessionByUserIdAsync(authenticationResult.UserId.Value);

        _logger.Info("Authentication logout succeeded", new Dictionary<string, object>
        {
            { "result", "success" },
            { "userId", authenticationResult.UserId.Value }
        });
    }

    public async Task<HeaderAuthenticationResult> AuthenticateSessionAsync(
        string? sessionId,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            _logger.Warn("Authentication session missing", new Dictionary<string, object>
            {
                { "result", "missing" }
            });

            var missingAuthenticationResult = new HeaderAuthenticationResult
            {
                HeaderAuthentication = SessionAuthenticationResult.Missing,
                ErrorMessage = "No session id sent."
            };

            return missingAuthenticationResult;
        }

        var authenticationResult = await _cacheProvider.GetUserAuthenticationResultBySessionAsync(sessionId);
        if (!authenticationResult.IsValid || authenticationResult.UserId is null)
        {
            _logger.Warn("Authentication session invalid", new Dictionary<string, object>
            {
                { "result", "invalid" }
            });

            var invalidAuthenticationResult = new HeaderAuthenticationResult
            {
                HeaderAuthentication = SessionAuthenticationResult.Invalid,
                SessionId = sessionId,
                ErrorMessage = "Invalid session id sent."
            };

            return invalidAuthenticationResult;
        }

        var validAuthenticationResult = new HeaderAuthenticationResult
        {
            HeaderAuthentication = SessionAuthenticationResult.Valid,
            SessionId = sessionId,
            UserId = authenticationResult.UserId.Value
        };

        return validAuthenticationResult;
    }

    private async Task EnsureStarterRoleAsync(Guid userId, CancellationToken cancellationToken)
    {
        var existingRole = await _userRoleRepository.GetByUserIdAndRoleAsync(
            userId,
            _starterGlobalRole,
            cancellationToken
        );
        if (existingRole is not null)
            return;

        await _userRoleRepository.CreateUserRoleAsync(
            new Shared.Models.DAL.Users.UserRole
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Role = _starterGlobalRole,
                AssignedTime = DateTime.UtcNow,
                AssignedByUserId = userId
            },
            cancellationToken
        );

        _logger.Info("Authentication starter role created", new Dictionary<string, object>
        {
            { "userId", userId },
            { "role", _starterGlobalRole.ToString() }
        });
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

    private static string GetIdentityNumberSuffix(string identityNumber)
    {
        if (string.IsNullOrWhiteSpace(identityNumber))
            return string.Empty;

        return identityNumber.Length <= 4
            ? identityNumber
            : identityNumber[^4..];
    }
}
