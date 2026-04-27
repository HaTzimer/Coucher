namespace Coucher.Shared.Interfaces.Services;

public interface IGraphQlCurrentUserService
{
    Task<Guid> GetRequiredCurrentUserIdAsync(CancellationToken cancellationToken = default);
}
