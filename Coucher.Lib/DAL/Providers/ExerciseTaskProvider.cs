using System.Net;
using System.Diagnostics.CodeAnalysis;
using Augustus.Infra.Core.Shared.Exceptions;
using Augustus.Infra.Core.Shared.Interfaces;
using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Models.DAL.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Lib.DAL.Providers;

public sealed class ExerciseTaskProvider : IExerciseTaskProvider
{
    private readonly IAugustusLogger _logger;
    private readonly IDbContextFactory<CoucherDbContext> _dbContextFactory;

    public ExerciseTaskProvider(
        IAugustusLogger logger,
        IDbContextFactory<CoucherDbContext> dbContextFactory
    )
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<ExerciseTask>> ListAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = dbContext.Set<ExerciseTask>();
        var items = await entities.ToListAsync(cancellationToken);

        return items;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = dbContext.Set<ExerciseTask>();
        var exists = await entities.AnyAsync(item => item.Id == id, cancellationToken);

        return exists;
    }

    public async Task<ExerciseTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = dbContext.Set<ExerciseTask>();
        var entity = await entities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return entity;
    }

    public async Task<int> GetNextSerialNumberAsync(Guid exerciseId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = dbContext.Set<ExerciseTask>();
        var nextSerialNumber = (await entities
            .Where(item => item.ExerciseId == exerciseId)
            .MaxAsync(item => (int?)item.SerialNumber, cancellationToken) ?? 0) + 1;

        return nextSerialNumber;
    }

    public async Task<ExerciseTask> CreateExerciseTaskAsync(
        ExerciseTask entity,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = dbContext.Set<ExerciseTask>();
        await entities.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<List<ExerciseTask>> CreateExerciseTasksAsync(
        List<ExerciseTask> entitiesToCreate,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = dbContext.Set<ExerciseTask>();
        await entities.AddRangeAsync(entitiesToCreate, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entitiesToCreate;
    }

    public async Task<ExerciseTask> UpdateExerciseTaskAsync(
        ExerciseTask entity,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = dbContext.Set<ExerciseTask>();
        entities.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task SetExerciseTaskHasChildrenAsync(
        Guid taskId,
        bool hasChildren,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        await dbContext.Set<ExerciseTask>()
            .Where(item => item.Id == taskId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(item => item.HasChildren, hasChildren),
                cancellationToken
            );
    }

    public async Task<TaskDependency> CreateTaskDependencyAsync(
        TaskDependency entity,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = dbContext.Set<TaskDependency>();
        await entities.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<TaskDependency?> GetTaskDependencyByIdAsync(
        Guid dependencyId,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = dbContext.Set<TaskDependency>();
        var entity = await entities.FirstOrDefaultAsync(item => item.Id == dependencyId, cancellationToken);

        return entity;
    }

    public async Task DeleteTaskDependencyAsync(Guid dependencyId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        await dbContext.Set<TaskDependency>()
            .Where(item => item.Id == dependencyId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<ExerciseTaskResponsibleUser> CreateExerciseTaskResponsibleUserAsync(
        ExerciseTaskResponsibleUser entity,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = dbContext.Set<ExerciseTaskResponsibleUser>();
        await entities.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<ExerciseTaskResponsibleUser?> GetExerciseTaskResponsibleUserByIdAsync(
        Guid responsibilityId,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = dbContext.Set<ExerciseTaskResponsibleUser>();
        var entity = await entities.FirstOrDefaultAsync(item => item.Id == responsibilityId, cancellationToken);

        return entity;
    }

    public async Task DeleteExerciseTaskResponsibleUserAsync(
        Guid responsibilityId,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        await dbContext.Set<ExerciseTaskResponsibleUser>()
            .Where(item => item.Id == responsibilityId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task DeleteExerciseTaskResponsibleUsersAsync(
        Guid taskId,
        List<Guid> responsibilityIds,
        CancellationToken cancellationToken = default
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        await dbContext.Set<ExerciseTaskResponsibleUser>()
            .Where(item => item.TaskId == taskId && responsibilityIds.Contains(item.Id))
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task DeleteExerciseTaskDeepAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var taskEntities = dbContext.Set<ExerciseTask>();
        var rootTask = await taskEntities
            .Where(item => item.Id == taskId)
            .Select(item => new { item.Id, item.ParentId })
            .FirstOrDefaultAsync(cancellationToken);
        if (rootTask is null)
            ThrowNotFound(nameof(ExerciseTask), taskId);

        var levels = new List<List<Guid>>();
        var frontier = new List<Guid> { rootTask.Id };

        while (frontier.Count > 0)
        {
            levels.Add(frontier);
            frontier = await taskEntities
                .Where(item => item.ParentId.HasValue && frontier.Contains(item.ParentId.Value))
                .Select(item => item.Id)
                .ToListAsync(cancellationToken);
        }

        var taskIds = levels.SelectMany(item => item).ToList();

        await dbContext.Set<TaskDependency>()
            .Where(item =>
                (item.TaskId.HasValue && taskIds.Contains(item.TaskId.Value))
                || (item.DependsOnId.HasValue && taskIds.Contains(item.DependsOnId.Value))
            )
            .ExecuteDeleteAsync(cancellationToken);

        await dbContext.Set<ExerciseTaskResponsibleUser>()
            .Where(item => item.TaskId.HasValue && taskIds.Contains(item.TaskId.Value))
            .ExecuteDeleteAsync(cancellationToken);

        foreach (var level in levels.AsEnumerable().Reverse())
        {
            await taskEntities
                .Where(item => level.Contains(item.Id))
                .ExecuteDeleteAsync(cancellationToken);
        }

        if (rootTask.ParentId.HasValue)
        {
            var parentId = rootTask.ParentId.Value;
            var hasChildren = await taskEntities.AnyAsync(item => item.ParentId == parentId, cancellationToken);
            await taskEntities
                .Where(item => item.Id == parentId)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(item => item.HasChildren, hasChildren),
                    cancellationToken
                );
        }

        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<ExerciseTask> AddAsync(ExerciseTask entity, CancellationToken cancellationToken = default)
    {
        var createdEntity = await CreateExerciseTaskAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<ExerciseTask> UpdateAsync(ExerciseTask entity, CancellationToken cancellationToken = default)
    {
        var updatedEntity = await UpdateExerciseTaskAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task DeleteAsync(ExerciseTask entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = dbContext.Set<ExerciseTask>();
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

