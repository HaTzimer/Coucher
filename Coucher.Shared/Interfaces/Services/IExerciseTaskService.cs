using Coucher.Shared.Models.WebApi.Requests.Tasks;
using Coucher.Shared.Models.DAL.Tasks;

namespace Coucher.Shared.Interfaces.Services;

public interface IExerciseTaskService : IServiceBase<ExerciseTask, Guid>
{
    Task<ExerciseTask> CreateExerciseTaskAsync(
        CreateExerciseTaskRequest request,
        CancellationToken cancellationToken = default
    );
    Task<List<ExerciseTask>> CreateExerciseTasksAsync(
        List<CreateExerciseTaskRequest> requests,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseTask> UpdateExerciseTaskSeriesAsync(
        Guid taskId,
        UpdateExerciseTaskSeriesRequest request,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseTask> UpdateExerciseTaskCategoryAsync(
        Guid taskId,
        UpdateExerciseTaskCategoryRequest request,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseTask> UpdateExerciseTaskStatusAsync(
        Guid taskId,
        UpdateExerciseTaskStatusRequest request,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseTask> UpdateExerciseTaskDetailsAsync(
        Guid taskId,
        UpdateExerciseTaskDetailsRequest request,
        CancellationToken cancellationToken = default
    );
    Task<TaskDependency> AddExerciseTaskDependencyAsync(
        Guid taskId,
        AddExerciseTaskDependencyRequest request,
        CancellationToken cancellationToken = default
    );
    Task DeleteExerciseTaskDependencyAsync(Guid dependencyId, CancellationToken cancellationToken = default);
    Task<ExerciseTaskResponsibleUser> AddExerciseTaskResponsibleUserAsync(
        Guid taskId,
        AddExerciseTaskResponsibleUserRequest request,
        CancellationToken cancellationToken = default
    );
    Task<List<ExerciseTaskResponsibleUser>> BulkUpdateExerciseTaskResponsibleUsersAsync(
        Guid taskId,
        BulkUpdateExerciseTaskResponsibleUsersRequest request,
        CancellationToken cancellationToken = default
    );
    Task DeleteExerciseTaskResponsibleUserAsync(Guid responsibilityId, CancellationToken cancellationToken = default);
    Task BulkDeleteExerciseTaskResponsibleUsersAsync(
        Guid taskId,
        BulkDeleteExerciseTaskResponsibleUsersRequest request,
        CancellationToken cancellationToken = default
    );
    Task DeleteExerciseTaskDeepAsync(Guid taskId, CancellationToken cancellationToken = default);
}
