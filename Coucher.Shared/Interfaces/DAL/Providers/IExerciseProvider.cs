using Coucher.Shared.Models.DAL.Exercises;

namespace Coucher.Shared.Interfaces.DAL.Providers;

public interface IExerciseProvider : IProviderBase<Exercise, Guid>
{
    Task<Exercise> CreateExerciseAsync(Exercise entity, CancellationToken cancellationToken = default);
    Task<Exercise> UpdateExerciseAsync(Exercise entity, CancellationToken cancellationToken = default);
}
