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
}
