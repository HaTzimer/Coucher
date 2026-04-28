using System.Net;
using Augustus.Infra.Core.Shared.Exceptions;
using Augustus.Infra.Core.Shared.Interfaces;
using Coucher.Shared.Models.Enums;
using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Models.DAL.Users;

namespace Coucher.Lib.Repositories;

public sealed class UserRoleRepository : IUserRoleRepository
{
    private readonly IAugustusLogger _logger;
    private readonly IUserRoleProvider _provider;

    public UserRoleRepository(IAugustusLogger logger, IUserRoleProvider provider)
    {
        _logger = logger;
        _provider = provider;
    }

    public async Task<List<UserRole>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _provider.ListAsync(cancellationToken);

        return items;
    }

    public async Task<UserRole?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _provider.GetByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<UserRole> GetRequiredByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _provider.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            var exception = new HttpStatusCodeException(
                $"{nameof(UserRole)} '{id}' was not found.",
                new Dictionary<string, object?>
                {
                    { "resourceName", nameof(UserRole) },
                    { "resourceId", id }
                },
                HttpStatusCode.NotFound
            );

            _logger.Error(exception);

            throw exception;
        }

        return entity;
    }

    public async Task<List<UserRole>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await _provider.ListByUserIdAsync(userId, cancellationToken);

        return items;
    }

    public async Task<UserRole?> GetByUserIdAndRoleAsync(
        Guid userId,
        GlobalRole role,
        CancellationToken cancellationToken = default
    )
    {
        var entity = await _provider.GetByUserIdAndRoleAsync(userId, role, cancellationToken);

        return entity;
    }

    public async Task<UserRole> CreateUserRoleAsync(UserRole entity, CancellationToken cancellationToken = default)
    {
        var createdEntity = await _provider.CreateUserRoleAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<UserRole> UpdateUserRoleAsync(UserRole entity, CancellationToken cancellationToken = default)
    {
        var updatedEntity = await _provider.UpdateUserRoleAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<UserRole> AddAsync(UserRole entity, CancellationToken cancellationToken = default)
    {
        var createdEntity = await CreateUserRoleAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<UserRole> UpdateAsync(UserRole entity, CancellationToken cancellationToken = default)
    {
        var updatedEntity = await UpdateUserRoleAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetRequiredByIdAsync(id, cancellationToken);
        await _provider.DeleteAsync(entity, cancellationToken);
    }
}

