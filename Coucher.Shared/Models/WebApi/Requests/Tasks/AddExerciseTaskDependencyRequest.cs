namespace Coucher.Shared.Models.WebApi.Requests.Tasks;

public sealed class AddExerciseTaskDependencyRequest
{
    public Guid DependsOnId { get; set; }
}
