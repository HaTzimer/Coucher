using Coucher.Shared.Models.WebApi.Requests.Exercises;
using Coucher.Shared.Models.DAL.Exercises;

namespace Coucher.Shared.Interfaces.Services;

public interface IExerciseService : IServiceBase<Exercise, Guid>
{
    Task<Exercise> CreateExerciseAsync(CreateExerciseRequestModel request, CancellationToken cancellationToken = default);
    Task<Exercise> UpdateExerciseAsync(
        Guid exerciseId,
        UpdateExerciseRequestModel request,
        CancellationToken cancellationToken = default
    );
}
