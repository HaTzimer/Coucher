using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Models.DAL.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Lib.DAL.Providers;

public sealed class UserNotificationProvider : IUserNotificationProvider
{
    private readonly CoucherDbContext _dbContext;
    private readonly DbSet<UserNotification> _entities;

    public UserNotificationProvider(CoucherDbContext dbContext)
    {
        _dbContext = dbContext;
        _entities = dbContext.Set<UserNotification>();
    }

    public async Task<List<UserNotification>> ListAsync(CancellationToken cancellationToken = default)
    {
        var query = BuildQuery();
        var items = await query.ToListAsync(cancellationToken);

        return items;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = BuildQuery();
        var exists = await query.AnyAsync(item => item.Id == id, cancellationToken);

        return exists;
    }

    public async Task<UserNotification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = BuildQuery();
        var entity = await query.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return entity;
    }

    public async Task<UserNotification> AddAsync(UserNotification entity, CancellationToken cancellationToken = default)
    {
        await _entities.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<UserNotification> UpdateAsync(UserNotification entity, CancellationToken cancellationToken = default)
    {
        _entities.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task DeleteAsync(UserNotification entity, CancellationToken cancellationToken = default)
    {
        _entities.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<UserNotification> BuildQuery()
    {
        var query = _entities
        .Include(item => item.User);

        return query;
    }
}
