using Coucher.Shared.Models.DAL.Exercises;

namespace Coucher.Shared.Interfaces.Repositories;

public interface IExerciseRepository : IRepositoryBase<Exercise, Guid>
{
    Task<Exercise> CreateExerciseAsync(Exercise entity, CancellationToken cancellationToken = default);
    Task<Exercise> UpdateExerciseAsync(Exercise entity, CancellationToken cancellationToken = default);
}
