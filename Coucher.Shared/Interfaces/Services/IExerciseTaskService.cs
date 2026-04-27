using Coucher.Shared.Models.WebApi.Requests.Tasks;
using Coucher.Shared.Models.DAL.Tasks;

namespace Coucher.Shared.Interfaces.Services;

public interface IExerciseTaskService : IServiceBase<ExerciseTask, Guid>
{
    Task<ExerciseTask> CreateExerciseTaskAsync(
        CreateExerciseTaskRequestModel request,
        CancellationToken cancellationToken = default
    );
    Task<List<ExerciseTask>> CreateExerciseTasksAsync(
        List<CreateExerciseTaskRequestModel> requests,
        CancellationToken cancellationToken = default
    );
    Task<ExerciseTask> UpdateExerciseTaskAsync(
        Guid taskId,
        UpdateExerciseTaskRequestModel request,
        CancellationToken cancellationToken = default
    );
}
