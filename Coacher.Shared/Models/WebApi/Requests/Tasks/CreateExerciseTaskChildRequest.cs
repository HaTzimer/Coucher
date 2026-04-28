namespace Coacher.Shared.Models.WebApi.Requests.Tasks;

public sealed class CreateExerciseTaskChildRequest
{
    public Guid SeriesId { get; set; }
    public Guid CategoryId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public DateTime DueDate { get; set; }
    public List<string>? ResponsibleUserIds { get; set; }
    public List<string>? DependsOnTaskIds { get; set; }
}
