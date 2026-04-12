using Coucher.Shared.Models.Enums;

namespace Coucher.Shared.Models.DAL.Admin;

public sealed class FixedTaskTemplateInfluencerLink
{
    public Guid Id { get; set; }
    public Guid FixedTaskTemplateId { get; set; }
    public ExerciseInfluencer Influencer { get; set; }
    public required FixedTaskTemplate FixedTaskTemplate { get; set; }
}
