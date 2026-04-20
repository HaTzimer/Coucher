using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Models.DAL.Admin;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Lib.DAL.Providers;

public sealed class FixedTaskTemplateProvider : IFixedTaskTemplateProvider
{
    private readonly IDbContextFactory<CoucherDbContext> _dbContextFactory;

    public FixedTaskTemplateProvider(IDbContextFactory<CoucherDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<FixedTaskTemplate>> ListAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<FixedTaskTemplate>();
        var items = await entities.ToListAsync(cancellationToken);

        return items;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<FixedTaskTemplate>();
        var exists = await entities.AnyAsync(item => item.Id == id, cancellationToken);

        return exists;
    }

    public async Task<FixedTaskTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<FixedTaskTemplate>();
        var entity = await entities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return entity;
    }

    public async Task<FixedTaskTemplate> AddAsync(FixedTaskTemplate entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<FixedTaskTemplate>();
        await entities.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<FixedTaskTemplate> UpdateAsync(FixedTaskTemplate entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<FixedTaskTemplate>();
        entities.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task DeleteAsync(FixedTaskTemplate entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<FixedTaskTemplate>();
        entities.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
