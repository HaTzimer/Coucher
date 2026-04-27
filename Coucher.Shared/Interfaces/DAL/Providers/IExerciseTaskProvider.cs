using Coucher.Shared.Models.DAL.Tasks;

namespace Coucher.Shared.Interfaces.DAL.Providers;

public interface IExerciseTaskProvider : IProviderBase<ExerciseTask, Guid>
{
    Task<int> GetNextSerialNumberAsync(Guid exerciseId, CancellationToken cancellationToken = default);
    Task<ExerciseTask> CreateExerciseTaskAsync(ExerciseTask entity, CancellationToken cancellationToken = default);
    Task<List<ExerciseTask>> CreateExerciseTasksAsync(
        List<ExerciseTask> entities,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseTask> UpdateExerciseTaskAsync(ExerciseTask entity, CancellationToken cancellationToken = default);
    Task<TaskDependency> CreateTaskDependencyAsync(TaskDependency entity, CancellationToken cancellationToken = default);
    Task<TaskDependency?> GetTaskDependencyByIdAsync(Guid dependencyId, CancellationToken cancellationToken = default);
    Task DeleteTaskDependencyAsync(Guid dependencyId, CancellationToken cancellationToken = default);
    Task<ExerciseTaskResponsibleUser> CreateExerciseTaskResponsibleUserAsync(
        ExerciseTaskResponsibleUser entity,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseTaskResponsibleUser?> GetExerciseTaskResponsibleUserByIdAsync(
        Guid responsibilityId,
        CancellationToken cancellationToken = default
    );
    Task<List<ExerciseTaskResponsibleUser>> ReplaceExerciseTaskResponsibleUsersAsync(
        Guid taskId,
        List<Guid> userIds,
        DateTime creationTime,
        CancellationToken cancellationToken = default
    );
    Task DeleteExerciseTaskResponsibleUserAsync(Guid responsibilityId, CancellationToken cancellationToken = default);
    Task DeleteExerciseTaskResponsibleUsersAsync(
        Guid taskId,
        List<Guid> responsibilityIds,
        CancellationToken cancellationToken = default
    );
    Task DeleteExerciseTaskDeepAsync(Guid taskId, CancellationToken cancellationToken = default);
}
