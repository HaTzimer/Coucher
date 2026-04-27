namespace Coucher.Shared.Models.WebApi.Requests.Tasks;

public sealed class CreateExerciseTaskRequestModel
{
    public Guid ExerciseId { get; set; }
    public Guid SeriesId { get; set; }
    public Guid CategoryId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? Notes { get; set; }
    public DateOnly DueDate { get; set; }
    public List<Guid>? ResponsibleUserIds { get; set; }
    public required List<Guid> InfluencerIds { get; set; }
    public required List<Guid> DependencyTaskIds { get; set; }
    public bool SaveAsFixedTemplate { get; set; }
}

