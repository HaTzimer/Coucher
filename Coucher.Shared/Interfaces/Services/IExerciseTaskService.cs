using Coucher.Shared.Models.WebApi.Requests.Tasks;
using Coucher.Shared.Models.DAL.Tasks;

namespace Coucher.Shared.Interfaces.Services;

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
    Task<ExerciseTask> UpdateExerciseTaskSeriesAsync(
        Guid taskId,
        Guid seriesId,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseTask> UpdateExerciseTaskCategoryAsync(
        Guid taskId,
        Guid categoryId,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseTask> UpdateExerciseTaskStatusAsync(
        Guid taskId,
        Guid statusId,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseTask> UpdateExerciseTaskDueDateAsync(
        Guid taskId,
        DateTime dueDate,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseTask> UpdateExerciseTaskDetailsAsync(
        Guid taskId,
        UpdateExerciseTaskDetailsRequest request,
        CancellationToken cancellationToken = default
    );
    Task<List<TaskDependency>> AddExerciseTaskDependenciesAsync(
        Guid taskId,
        List<string> dependsOnIds,
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
        List<string> userIds,
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
