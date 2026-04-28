using System.Net;
using Augustus.Infra.Core.Shared.Exceptions;
using Augustus.Infra.Core.Shared.Interfaces;
using Coacher.Shared.Interfaces.DAL.Providers;
using Coacher.Shared.Interfaces.Repositories;
using Coacher.Shared.Models.DAL.Users;

namespace Coacher.Lib.Repositories;

public sealed class UserProfileRepository : IUserProfileRepository
{
    private readonly IAugustusLogger _logger;
    private readonly IUserProfileProvider _provider;

    public UserProfileRepository(IAugustusLogger logger, IUserProfileProvider provider)
    {
        _logger = logger;
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
        {
            var exception = new HttpStatusCodeException(
                $"{nameof(UserProfile)} '{id}' was not found.",
                new Dictionary<string, object?>
                {
                    { "resourceName", nameof(UserProfile) },
                    { "resourceId", id }
                },
                HttpStatusCode.NotFound
            );

            _logger.Error(exception);

            throw exception;
        }

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

