using System.Net;
using System.Diagnostics.CodeAnalysis;
using Augustus.Infra.Core.Shared.Exceptions;
using Augustus.Infra.Core.Shared.Interfaces;
using Coacher.Shared.Interfaces.DAL.Providers;
using Coacher.Shared.Interfaces.Repositories;
using Coacher.Shared.Models.DAL.Tasks;

namespace Coacher.Lib.Repositories;

public sealed class ExerciseTaskRepository : IExerciseTaskRepository
{
    private readonly IAugustusLogger _logger;
    private readonly IExerciseTaskProvider _provider;

    public ExerciseTaskRepository(IAugustusLogger logger, IExerciseTaskProvider provider)
    {
        _logger = logger;
        _provider = provider;
    }

    public async Task<List<ExerciseTask>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _provider.ListAsync(cancellationToken);

        return items;
    }

    public async Task<ExerciseTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _provider.GetByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<ExerciseTask> GetRequiredByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _provider.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            ThrowNotFound(nameof(ExerciseTask), id);

        return entity;
    }

    public async Task<Guid> GetRequiredExerciseIdByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await GetRequiredByIdAsync(taskId, cancellationToken);

        return task.ExerciseId;
    }

    public async Task<Guid> GetRequiredExerciseIdByDependencyIdAsync(
        Guid dependencyId,
        CancellationToken cancellationToken = default
    )
    {
        var dependency = await _provider.GetTaskDependencyByIdAsync(dependencyId, cancellationToken);
        if (dependency is null || !dependency.TaskId.HasValue)
            ThrowNotFound(nameof(TaskDependency), dependencyId);

        var exerciseId = await GetRequiredExerciseIdByTaskIdAsync(dependency.TaskId.Value, cancellationToken);

        return exerciseId;
    }

    public async Task<Guid> GetRequiredExerciseIdByResponsibilityIdAsync(
        Guid responsibilityId,
        CancellationToken cancellationToken = default
    )
    {
        var responsibility = await _provider.GetExerciseTaskResponsibleUserByIdAsync(
            responsibilityId,
            cancellationToken
        );
        if (responsibility is null || !responsibility.TaskId.HasValue)
            ThrowNotFound(nameof(ExerciseTaskResponsibleUser), responsibilityId);

        var exerciseId = await GetRequiredExerciseIdByTaskIdAsync(responsibility.TaskId.Value, cancellationToken);

        return exerciseId;
    }

    public async Task<int> GetNextSerialNumberAsync(Guid exerciseId, CancellationToken cancellationToken = default)
    {
        var nextSerialNumber = await _provider.GetNextSerialNumberAsync(exerciseId, cancellationToken);

        return nextSerialNumber;
    }

    public async Task<ExerciseTask> CreateExerciseTaskAsync(
        ExerciseTask entity,
        CancellationToken cancellationToken = default
    )
    {
        var createdEntity = await _provider.CreateExerciseTaskAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<List<ExerciseTask>> CreateExerciseTasksAsync(
        List<ExerciseTask> entities,
        CancellationToken cancellationToken = default
    )
    {
        var createdEntities = await _provider.CreateExerciseTasksAsync(entities, cancellationToken);

        return createdEntities;
    }

    public async Task<ExerciseTask> UpdateExerciseTaskAsync(
        ExerciseTask entity,
        CancellationToken cancellationToken = default
    )
    {
        var updatedEntity = await _provider.UpdateExerciseTaskAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task SetExerciseTaskHasChildrenAsync(
        Guid taskId,
        bool hasChildren,
        CancellationToken cancellationToken = default
    )
    {
        _ = await GetRequiredByIdAsync(taskId, cancellationToken);
        await _provider.SetExerciseTaskHasChildrenAsync(taskId, hasChildren, cancellationToken);
    }

    public async Task<TaskDependency> CreateTaskDependencyAsync(
        TaskDependency entity,
        CancellationToken cancellationToken = default
    )
    {
        var createdEntity = await _provider.CreateTaskDependencyAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task DeleteTaskDependenciesAsync(
        Guid taskId,
        List<Guid> dependencyIds,
        CancellationToken cancellationToken = default
    )
    {
        _ = await GetRequiredByIdAsync(taskId, cancellationToken);
        await _provider.DeleteTaskDependenciesAsync(taskId, dependencyIds, cancellationToken);
    }

    public async Task<List<ExerciseTaskResponsibleUser>> CreateExerciseTaskResponsibleUsersAsync(
        List<ExerciseTaskResponsibleUser> entities,
        CancellationToken cancellationToken = default
    )
    {
        var createdEntities = await _provider.CreateExerciseTaskResponsibleUsersAsync(
            entities,
            cancellationToken
        );

        return createdEntities;
    }

    public async Task DeleteExerciseTaskResponsibleUsersAsync(
        Guid taskId,
        List<Guid> responsibilityIds,
        CancellationToken cancellationToken = default
    )
    {
        _ = await GetRequiredByIdAsync(taskId, cancellationToken);
        await _provider.DeleteExerciseTaskResponsibleUsersAsync(taskId, responsibilityIds, cancellationToken);
    }

    public async Task DeleteExerciseTaskDeepAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        _ = await GetRequiredByIdAsync(taskId, cancellationToken);
        await _provider.DeleteExerciseTaskDeepAsync(taskId, cancellationToken);
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

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetRequiredByIdAsync(id, cancellationToken);
        await _provider.DeleteAsync(entity, cancellationToken);
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

