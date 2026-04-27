namespace Coucher.Shared.Interfaces.Services;

public interface ICurrentUserService
{
    Task<Guid> GetRequiredCurrentUserIdAsync(CancellationToken cancellationToken = default);
}
