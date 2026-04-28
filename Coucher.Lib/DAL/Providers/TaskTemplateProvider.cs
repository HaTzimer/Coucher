using System.Net;
using System.Diagnostics.CodeAnalysis;
using Augustus.Infra.Core.Shared.Exceptions;
using Augustus.Infra.Core.Shared.Interfaces;
using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Models.DAL.Admin;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Lib.DAL.Providers;

public sealed class TaskTemplateProvider : ITaskTemplateProvider
{
    private readonly IAugustusLogger _logger;
    private readonly IDbContextFactory<CoucherDbContext> _dbContextFactory;

    public TaskTemplateProvider(
        IAugustusLogger logger,
        IDbContextFactory<CoucherDbContext> dbContextFactory
    )
    {
        _logger = logger;
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

    public async Task<List<TaskTemplateInfluencer>> CreateTaskTemplateInfluencersAsync(
        Guid taskTemplateId,
        List<Guid> influencerIds,
        DateTime creationTime,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = dbContext.Set<TaskTemplateInfluencer>();
        var normalizedInfluencerIds = influencerIds.Distinct().ToList();
        if (normalizedInfluencerIds.Count == 0)
            return new List<TaskTemplateInfluencer>();

        var existingInfluencerIds = await entities
            .Where(item => item.TemplateId == taskTemplateId && item.InfluencerId.HasValue)
            .Select(item => item.InfluencerId!.Value)
            .ToListAsync(cancellationToken);
        var newEntities = normalizedInfluencerIds.Select(influencerId => new TaskTemplateInfluencer
        {
            Id = Guid.NewGuid(),
            TemplateId = taskTemplateId,
            InfluencerId = influencerId,
            CreationTime = creationTime
        })
            .Where(item => !existingInfluencerIds.Contains(item.InfluencerId!.Value))
            .ToList();
        if (newEntities.Count == 0)
            return new List<TaskTemplateInfluencer>();

        await entities.AddRangeAsync(newEntities, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return newEntities;
    }

    public async Task<TaskTemplateInfluencer?> GetTaskTemplateInfluencerByIdAsync(
        Guid influencerLinkId,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entity = await dbContext.Set<TaskTemplateInfluencer>()
            .FirstOrDefaultAsync(item => item.Id == influencerLinkId, cancellationToken);

        return entity;
    }

    public async Task DeleteTaskTemplateDependencyAsync(Guid dependencyId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entity = await dbContext.Set<TaskTemplateDependency>()
            .FirstOrDefaultAsync(item => item.Id == dependencyId, cancellationToken);
        if (entity is null)
            ThrowNotFound(nameof(TaskTemplateDependency), dependencyId);

        dbContext.Set<TaskTemplateDependency>().Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTaskTemplateInfluencerAsync(Guid influencerLinkId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entity = await dbContext.Set<TaskTemplateInfluencer>()
            .FirstOrDefaultAsync(item => item.Id == influencerLinkId, cancellationToken);
        if (entity is null)
            ThrowNotFound(nameof(TaskTemplateInfluencer), influencerLinkId);

        dbContext.Set<TaskTemplateInfluencer>().Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<TaskTemplate> ArchiveTaskTemplateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = dbContext.Set<TaskTemplate>();
        var entity = await entities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (entity is null)
            ThrowNotFound(nameof(TaskTemplate), id);

        entity.IsArchive = true;
        entity.LastUpdateTime = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<TaskTemplate> UnarchiveTaskTemplateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = dbContext.Set<TaskTemplate>();
        var entity = await entities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (entity is null)
            ThrowNotFound(nameof(TaskTemplate), id);

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

    [DoesNotReturn]
    private void ThrowNotFound(string resourceName, Guid resourceId)
    {
        var exception = new HttpStatusCodeException(
            $"{resourceName} '{resourceId}' was not found.",
            new Dictionary<string, object?>
            {
                { "resourceName", resourceName },
                { "resourceId", resourceId }
            },
            HttpStatusCode.NotFound
        );

        _logger.Error(exception);

        throw exception;
    }
}

