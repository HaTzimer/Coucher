namespace Coucher.Shared.Models.WebApi.Requests.Tasks;

public sealed class CreateExerciseTaskRequest
{
    public Guid ExerciseId { get; set; }
    public Guid SeriesId { get; set; }
    public Guid CategoryId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public DateTime DueDate { get; set; }
}
