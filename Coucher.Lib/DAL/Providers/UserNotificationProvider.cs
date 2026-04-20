using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Models.DAL.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Lib.DAL.Providers;

public sealed class UserNotificationProvider : IUserNotificationProvider
{
    private readonly IDbContextFactory<CoucherDbContext> _dbContextFactory;

    public UserNotificationProvider(IDbContextFactory<CoucherDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<UserNotification>> ListAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserNotification>();
        var items = await entities.ToListAsync(cancellationToken);

        return items;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserNotification>();
        var exists = await entities.AnyAsync(item => item.Id == id, cancellationToken);

        return exists;
    }

    public async Task<UserNotification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserNotification>();
        var entity = await entities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return entity;
    }

    public async Task<UserNotification> AddAsync(UserNotification entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserNotification>();
        await entities.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<UserNotification> UpdateAsync(UserNotification entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserNotification>();
        entities.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task DeleteAsync(UserNotification entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<UserNotification>();
        entities.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
