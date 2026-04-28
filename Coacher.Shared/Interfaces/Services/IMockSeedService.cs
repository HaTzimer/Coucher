using Coacher.Shared.Models.Internal.Mocking;

namespace Coacher.Shared.Interfaces.Services;

public interface IMockSeedService
{
    Task<MockSeedSummary> SeedAsync(MockSeedOptions options, CancellationToken cancellationToken = default);
}

