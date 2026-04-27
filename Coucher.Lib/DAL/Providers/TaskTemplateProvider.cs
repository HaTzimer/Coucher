using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Models.DAL.Admin;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Lib.DAL.Providers;

public sealed class TaskTemplateProvider : ITaskTemplateProvider
{
    private readonly IDbContextFactory<CoucherDbContext> _dbContextFactory;

    public TaskTemplateProvider(IDbContextFactory<CoucherDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<TaskTemplate>> ListAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<TaskTemplate>();
        var items = await entities.ToListAsync(cancellationToken);

        return items;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<TaskTemplate>();
        var exists = await entities.AnyAsync(item => item.Id == id, cancellationToken);

        return exists;
    }

    public async Task<TaskTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<TaskTemplate>();
        var entity = await entities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return entity;
    }

    public async Task<int> GetNextSerialNumberAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<TaskTemplate>();
        var nextSerialNumber = (await entities.MaxAsync(item => (int?)item.SerialNumber, cancellationToken) ?? 0) + 1;

        return nextSerialNumber;
    }

    public async Task<TaskTemplate> CreateTaskTemplateAsync(
        TaskTemplate entity,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<TaskTemplate>();
        await entities.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<List<TaskTemplate>> CreateTaskTemplatesAsync(
        List<TaskTemplate> entitiesToCreate,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<TaskTemplate>();
        await entities.AddRangeAsync(entitiesToCreate, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entitiesToCreate;
    }

    public async Task<TaskTemplate> UpdateTaskTemplateAsync(
        TaskTemplate entity,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<TaskTemplate>();
        entities.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<TaskTemplateDependency> CreateTaskTemplateDependencyAsync(
        TaskTemplateDependency entity,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await dbContext.Set<TaskTemplateDependency>().AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<TaskTemplate> ArchiveTaskTemplateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<TaskTemplate>();
        var entity = await entities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"{nameof(TaskTemplate)} '{id}' was not found.");
        entity.IsArchive = true;
        entity.LastUpdateTime = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<TaskTemplate> UnarchiveTaskTemplateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<TaskTemplate>();
        var entity = await entities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"{nameof(TaskTemplate)} '{id}' was not found.");
        entity.IsArchive = false;
        entity.LastUpdateTime = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<TaskTemplate> AddAsync(TaskTemplate entity, CancellationToken cancellationToken = default)
    {
        var createdEntity = await CreateTaskTemplateAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<TaskTemplate> UpdateAsync(TaskTemplate entity, CancellationToken cancellationToken = default)
    {
        var updatedEntity = await UpdateTaskTemplateAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task DeleteAsync(TaskTemplate entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = dbContext.Set<TaskTemplate>();
        entities.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
