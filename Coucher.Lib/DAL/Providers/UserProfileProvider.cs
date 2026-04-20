using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Models.DAL.Users;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Lib.DAL.Providers;

public sealed class UserProfileProvider : IUserProfileProvider
{
    private readonly CoucherDbContext _dbContext;
    private readonly DbSet<UserProfile> _entities;

    public UserProfileProvider(CoucherDbContext dbContext)
    {
        _dbContext = dbContext;
        _entities = dbContext.Set<UserProfile>();
    }

    public async Task<List<UserProfile>> ListAsync(CancellationToken cancellationToken = default)
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

    public async Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = BuildQuery();
        var entity = await query.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return entity;
    }

    public async Task<UserProfile> AddAsync(UserProfile entity, CancellationToken cancellationToken = default)
    {
        await _entities.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<UserProfile> UpdateAsync(UserProfile entity, CancellationToken cancellationToken = default)
    {
        _entities.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task DeleteAsync(UserProfile entity, CancellationToken cancellationToken = default)
    {
        _entities.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<UserProfile> BuildQuery()
    {
        var query = _entities
        .Include(item => item.Roles)
        .Include(item => item.SecurityQuestions)
        .Include(item => item.ExerciseParticipants)
        .Include(item => item.ResponsibleTasks)
        .Include(item => item.Notifications);

        return query;
    }
}
