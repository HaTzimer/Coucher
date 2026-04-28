namespace Coacher.Shared.Interfaces.Services;

public interface ICurrentUserService
{
    Task<Guid?> TryGetCurrentUserIdAsync(CancellationToken cancellationToken = default);

    Task<Guid> GetRequiredCurrentUserIdAsync(CancellationToken cancellationToken = default);
}
