using Coucher.Shared.Models.Enums;

namespace Coucher.Shared.Models.DAL.Admin;

public sealed class FixedTaskTemplate
{
    public Guid Id { get; set; }
    public ExerciseSeries Series { get; set; }
    public required string Category { get; set; }
    public required string SubCategory { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int DefaultWeeksBeforeExerciseStart { get; set; }
    public required List<FixedTaskTemplateInfluencerLink> Influencers { get; set; }
    public required List<FixedTaskTemplateDependency> Dependencies { get; set; }
}
