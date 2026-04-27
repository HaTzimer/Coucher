using Coucher.Shared.Models.Internal.Mocking;

namespace Coucher.Shared.Interfaces.Services;

public interface IMockSeedService
{
    Task<MockSeedSummary> SeedAsync(MockSeedOptions options, CancellationToken cancellationToken = default);
}

