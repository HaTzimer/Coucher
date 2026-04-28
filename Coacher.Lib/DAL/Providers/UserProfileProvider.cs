using Coacher.Shared.Interfaces.DAL.Providers;
using Coacher.Shared.Models.DAL.Users;
using Microsoft.EntityFrameworkCore;

namespace Coacher.Lib.DAL.Providers;

public sealed class UserProfileProvider : IUserProfileProvider
{
    private readonly IDbContextFactory<CoacherDbContext> _dbContextFactory;

    public UserProfileProvider(IDbContextFactory<CoacherDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<UserProfile>> ListAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserProfile>();
        var items = await entities.ToListAsync(cancellationToken);

        return items;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserProfile>();
        var exists = await entities.AnyAsync(item => item.Id == id, cancellationToken);

        return exists;
    }

    public async Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserProfile>();
        var entity = await entities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return entity;
    }

    public async Task<UserProfile?> GetByIdentityNumberAsync(string identityNumber, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserProfile>();
        var entity = await entities.FirstOrDefaultAsync(
            item => item.IdentityNumber == identityNumber,
            cancellationToken
        );

        return entity;
    }

    public async Task UpdateLastLoginTimeAsync(
        Guid userId,
        DateTime lastLoginTime,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserProfile>();
        await entities
            .Where(item => item.Id == userId)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(item => item.LastLoginTime, lastLoginTime)
                    .SetProperty(item => item.LastUpdateTime, lastLoginTime),
                cancellationToken
            );
    }

    public async Task<UserProfile> AddAsync(UserProfile entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserProfile>();
        await entities.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<UserProfile> UpdateAsync(UserProfile entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserProfile>();
        entities.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task DeleteAsync(UserProfile entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserProfile>();
        entities.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
