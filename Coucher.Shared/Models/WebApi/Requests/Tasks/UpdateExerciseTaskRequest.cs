namespace Coucher.Shared.Models.WebApi.Requests.Tasks;

public sealed class UpdateExerciseTaskRequest
{
    public Guid SeriesId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid StatusId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public DateTime DueDate { get; set; }
}
