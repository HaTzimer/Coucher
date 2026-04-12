using Coucher.Shared.Models.Enums;

namespace Coucher.Shared.Models.DAL.Tasks;

public sealed class ExerciseTaskInfluencerLink
{
    public Guid Id { get; set; }
    public Guid ExerciseTaskId { get; set; }
    public ExerciseInfluencer Influencer { get; set; }
    public required ExerciseTask ExerciseTask { get; set; }
}
