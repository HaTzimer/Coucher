using Coacher.Shared.Models.Enums;
using Coacher.Shared.Interfaces.DAL.Providers;
using Coacher.Shared.Models.DAL.Users;
using Microsoft.EntityFrameworkCore;

namespace Coacher.Lib.DAL.Providers;

public sealed class UserRoleProvider : IUserRoleProvider
{
    private readonly IDbContextFactory<CoacherDbContext> _dbContextFactory;

    public UserRoleProvider(IDbContextFactory<CoacherDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<UserRole>> ListAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserRole>();
        var items = await entities.ToListAsync(cancellationToken);

        return items;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserRole>();
        var exists = await entities.AnyAsync(item => item.Id == id, cancellationToken);

        return exists;
    }

    public async Task<UserRole?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserRole>();
        var entity = await entities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return entity;
    }

    public async Task<List<UserRole>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserRole>();
        var items = await entities.Where(item => item.UserId == userId).ToListAsync(cancellationToken);

        return items;
    }

    public async Task<UserRole?> GetByUserIdAndRoleAsync(
        Guid userId,
        GlobalRole role,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserRole>();
        var entity = await entities.FirstOrDefaultAsync(
            item => item.UserId == userId && item.Role == role,
            cancellationToken
        );

        return entity;
    }

    public async Task<UserRole> CreateUserRoleAsync(UserRole entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserRole>();
        await entities.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<UserRole> UpdateUserRoleAsync(UserRole entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserRole>();
        entities.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
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

    public async Task DeleteAsync(UserRole entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserRole>();
        entities.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
