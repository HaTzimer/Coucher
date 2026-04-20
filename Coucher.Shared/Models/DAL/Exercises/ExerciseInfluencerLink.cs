using Coucher.Shared.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Exercises;

public sealed class ExerciseInfluencerLink
{
    [Key]
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public ExerciseInfluencer Influencer { get; set; }

    [ForeignKey(nameof(ExerciseId))]
    [InverseProperty("Influencers")]
    public required Exercise Exercise { get; set; }
}
