using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Models.DAL.Users;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Lib.DAL.Providers;

public sealed class UserProfileProvider : IUserProfileProvider
{
    private readonly IDbContextFactory<CoucherDbContext> _dbContextFactory;

    public UserProfileProvider(IDbContextFactory<CoucherDbContext> dbContextFactory)
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
