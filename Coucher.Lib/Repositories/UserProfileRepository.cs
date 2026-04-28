using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Models.DAL.Users;

namespace Coucher.Lib.Repositories;

public sealed class UserProfileRepository : IUserProfileRepository
{
    private readonly IUserProfileProvider _provider;

    public UserProfileRepository(IUserProfileProvider provider)
    {
        _provider = provider;
    }

    public async Task<List<UserProfile>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _provider.ListAsync(cancellationToken);

        return items;
    }

    public async Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _provider.GetByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<UserProfile?> GetByIdentityNumberAsync(
        string identityNumber,
        CancellationToken cancellationToken = default
    )
    {
        var entity = await _provider.GetByIdentityNumberAsync(identityNumber, cancellationToken);

        return entity;
    }

    public async Task UpdateLastLoginTimeAsync(
        Guid userId,
        DateTime lastLoginTime,
        CancellationToken cancellationToken = default
    )
    {
        await _provider.UpdateLastLoginTimeAsync(userId, lastLoginTime, cancellationToken);
    }

    public async Task<UserProfile> GetRequiredByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _provider.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new KeyNotFoundException($"{nameof(UserProfile)} '{id}' was not found.");

        return entity;
    }

    public async Task<UserProfile> AddAsync(UserProfile entity, CancellationToken cancellationToken = default)
    {
        var createdEntity = await _provider.AddAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<UserProfile> UpdateAsync(UserProfile entity, CancellationToken cancellationToken = default)
    {
        var updatedEntity = await _provider.UpdateAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetRequiredByIdAsync(id, cancellationToken);
        await _provider.DeleteAsync(entity, cancellationToken);
    }
}
