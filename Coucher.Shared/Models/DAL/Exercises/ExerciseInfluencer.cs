using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Exercises;

[Table(ConstantValues.ExerciseInfluencerTableName)]
[Index(nameof(ExerciseId), nameof(InfluencerId), IsUnique = true)]
public sealed class ExerciseInfluencer
{
    [Key]
    public Guid Id { get; set; }
    public Guid? ExerciseId { get; set; }
    public Guid? InfluencerId { get; set; }

    [ForeignKey(nameof(ExerciseId))]
    [InverseProperty("Influencers")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    public Exercise? Exercise { get; set; }

    [ForeignKey(nameof(InfluencerId))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    public ClosedListItem? Influencer { get; set; }
}
