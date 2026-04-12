using Coucher.Shared.Models.Enums;

namespace Coucher.Shared.Models.DAL.Exercises;

public sealed class ExerciseInfluencerLink
{
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public ExerciseInfluencer Influencer { get; set; }
    public required Exercise Exercise { get; set; }
}
