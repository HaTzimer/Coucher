using Coacher.Shared.Models.WebApi.Requests.Tasks;
using Coacher.Shared.Models.DAL.Tasks;

namespace Coacher.Shared.Interfaces.Services;

public interface IExerciseTaskService : IServiceBase<ExerciseTask, Guid>
{
    Task<ExerciseTask> CreateExerciseTaskAsync(
        CreateExerciseTaskRequest request,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseTask> CreateExerciseTaskChildAsync(
        Guid parentTaskId,
        CreateExerciseTaskChildRequest request,
        CancellationToken cancellationToken = default
    );
    Task<List<ExerciseTask>> CreateExerciseTasksAsync(
        List<CreateExerciseTaskRequest> requests,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseTask> UpdateExerciseTaskAsync(
        Guid taskId,
        UpdateExerciseTaskRequest request,
        CancellationToken cancellationToken = default
    );
    Task<List<TaskDependency>> AddExerciseTaskDependenciesAsync(
        Guid taskId,
        List<string> dependsOnIds,
        CancellationToken cancellationToken = default
    );
    Task DeleteExerciseTaskDependenciesAsync(
        Guid taskId,
        List<string> dependencyIds,
        CancellationToken cancellationToken = default
    );
    Task<List<ExerciseTaskResponsibleUser>> AddExerciseTaskResponsibleUsersAsync(
        Guid taskId,
        List<string> userIds,
        CancellationToken cancellationToken = default
    );
    Task DeleteExerciseTaskResponsibleUsersAsync(
        Guid taskId,
        List<string> responsibilityIds,
        CancellationToken cancellationToken = default
    );
    Task DeleteExerciseTaskDeepAsync(Guid taskId, CancellationToken cancellationToken = default);
}
