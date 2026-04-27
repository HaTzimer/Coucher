using Coucher.Shared.Models.Enums;
using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Models.DAL.Users;

namespace Coucher.Lib.Repositories;

public sealed class UserRoleRepository : IUserRoleRepository
{
    private readonly IUserRoleProvider _provider;

    public UserRoleRepository(IUserRoleProvider provider)
    {
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
            throw new KeyNotFoundException($"{nameof(UserRole)} '{id}' was not found.");
        }

        return entity;
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
