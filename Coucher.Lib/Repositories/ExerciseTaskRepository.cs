using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Models.DAL.Tasks;

namespace Coucher.Lib.Repositories;

public sealed class ExerciseTaskRepository : IExerciseTaskRepository
{
    private readonly IExerciseTaskProvider _provider;

    public ExerciseTaskRepository(IExerciseTaskProvider provider)
    {
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
        {
            throw new KeyNotFoundException($"{nameof(ExerciseTask)} '{id}' was not found.");
        }

        return entity;
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

    public async Task DeleteTaskDependencyAsync(Guid dependencyId, CancellationToken cancellationToken = default)
    {
        var entity = await _provider.GetTaskDependencyByIdAsync(dependencyId, cancellationToken);
        if (entity is null)
        {
            throw new KeyNotFoundException($"{nameof(TaskDependency)} '{dependencyId}' was not found.");
        }

        await _provider.DeleteTaskDependencyAsync(dependencyId, cancellationToken);
    }

    public async Task<ExerciseTaskResponsibleUser> CreateExerciseTaskResponsibleUserAsync(
        ExerciseTaskResponsibleUser entity,
        CancellationToken cancellationToken = default
    )
    {
        var createdEntity = await _provider.CreateExerciseTaskResponsibleUserAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<List<ExerciseTaskResponsibleUser>> ReplaceExerciseTaskResponsibleUsersAsync(
        Guid taskId,
        List<Guid> userIds,
        DateTime creationTime,
        CancellationToken cancellationToken = default
    )
    {
        _ = await GetRequiredByIdAsync(taskId, cancellationToken);
        var updatedEntities = await _provider.ReplaceExerciseTaskResponsibleUsersAsync(
            taskId,
            userIds,
            creationTime,
            cancellationToken
        );

        return updatedEntities;
    }

    public async Task DeleteExerciseTaskResponsibleUserAsync(
        Guid responsibilityId,
        CancellationToken cancellationToken = default
    )
    {
        var entity = await _provider.GetExerciseTaskResponsibleUserByIdAsync(responsibilityId, cancellationToken);
        if (entity is null)
        {
            throw new KeyNotFoundException($"{nameof(ExerciseTaskResponsibleUser)} '{responsibilityId}' was not found.");
        }

        await _provider.DeleteExerciseTaskResponsibleUserAsync(responsibilityId, cancellationToken);
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
}
