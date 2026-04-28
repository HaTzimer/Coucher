using Coacher.Shared.Models.DAL.Tasks;

namespace Coacher.Shared.Interfaces.Repositories;

public interface IExerciseTaskRepository : IRepositoryBase<ExerciseTask, Guid>
{
    Task<Guid> GetRequiredExerciseIdByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<Guid> GetRequiredExerciseIdByDependencyIdAsync(Guid dependencyId, CancellationToken cancellationToken = default);
    Task<Guid> GetRequiredExerciseIdByResponsibilityIdAsync(
        Guid responsibilityId,
        CancellationToken cancellationToken = default
    );
    Task<int> GetNextSerialNumberAsync(Guid exerciseId, CancellationToken cancellationToken = default);
    Task<ExerciseTask> CreateExerciseTaskAsync(ExerciseTask entity, CancellationToken cancellationToken = default);
    Task<List<ExerciseTask>> CreateExerciseTasksAsync(
        List<ExerciseTask> entities,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseTask> UpdateExerciseTaskAsync(ExerciseTask entity, CancellationToken cancellationToken = default);
    Task SetExerciseTaskHasChildrenAsync(
        Guid taskId,
        bool hasChildren,
        CancellationToken cancellationToken = default
    );
    Task<TaskDependency> CreateTaskDependencyAsync(TaskDependency entity, CancellationToken cancellationToken = default);
    Task DeleteTaskDependenciesAsync(
        Guid taskId,
        List<Guid> dependencyIds,
        CancellationToken cancellationToken = default
    );
    Task<List<ExerciseTaskResponsibleUser>> CreateExerciseTaskResponsibleUsersAsync(
        List<ExerciseTaskResponsibleUser> entities,
        CancellationToken cancellationToken = default
    );
    Task DeleteExerciseTaskResponsibleUsersAsync(
        Guid taskId,
        List<Guid> responsibilityIds,
        CancellationToken cancellationToken = default
    );
    Task DeleteExerciseTaskDeepAsync(Guid taskId, CancellationToken cancellationToken = default);
}
