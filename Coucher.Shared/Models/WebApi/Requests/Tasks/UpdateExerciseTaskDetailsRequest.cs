namespace Coucher.Shared.Models.WebApi.Requests.Tasks;

public sealed class UpdateExerciseTaskDetailsRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
}
