using Coacher.Shared.Models.Internal.Authentication;

namespace Coacher.Shared.Interfaces.Services;

public interface IAuthenticationService
{
    Task<AuthenticatedSession?> LoginAsync(
        string identityNumber,
        string passwordOrPersonalNumber,
        CancellationToken cancellationToken = default
    );

    Task LogoutAsync(string sessionId, CancellationToken cancellationToken = default);

    Task<HeaderAuthenticationResult> AuthenticateSessionAsync(
        string? sessionId,
        CancellationToken cancellationToken = default
    );
}
