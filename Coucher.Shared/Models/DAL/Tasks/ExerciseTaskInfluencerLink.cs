using Coucher.Shared.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Tasks;

public sealed class ExerciseTaskInfluencerLink
{
    [Key]
    public Guid Id { get; set; }
    public Guid ExerciseTaskId { get; set; }
    public ExerciseInfluencer Influencer { get; set; }

    [ForeignKey(nameof(ExerciseTaskId))]
    [InverseProperty("Influencers")]
    public required ExerciseTask ExerciseTask { get; set; }
}
