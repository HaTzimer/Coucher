using Coucher.Shared.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Admin;

public sealed class FixedTaskTemplateInfluencerLink
{
    [Key]
    public Guid Id { get; set; }
    public Guid FixedTaskTemplateId { get; set; }
    public ExerciseInfluencer Influencer { get; set; }

    [ForeignKey(nameof(FixedTaskTemplateId))]
    [InverseProperty("Influencers")]
    public required FixedTaskTemplate FixedTaskTemplate { get; set; }
}
