using Coucher.Shared.Models.Enums;

namespace Coucher.WebApi.Models.WebApi.Requests.Tasks;

public sealed class CreateExerciseTaskRequestModel
{
    public Guid ExerciseId { get; set; }
    public Guid SeriesClosedListItemId { get; set; }
    public Guid CategoryClosedListItemId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? Notes { get; set; }
    public DateOnly DueDate { get; set; }
    public Guid? ResponsibleUserId { get; set; }
    public required List<ExerciseInfluencer> Influencers { get; set; }
    public required List<Guid> DependencyTaskIds { get; set; }
    public bool SaveAsFixedTemplate { get; set; }
}
