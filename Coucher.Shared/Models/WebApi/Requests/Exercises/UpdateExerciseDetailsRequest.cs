namespace Coucher.Shared.Models.WebApi.Requests.Exercises;

public sealed class UpdateExerciseDetailsRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
