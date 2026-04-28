using Coacher.Shared.Models.DAL.Admin;
using Coacher.Shared;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coacher.Shared.Models.DAL.Exercises;

[Table(ConstantValues.ExerciseInfluencerTableName)]
[Index(nameof(ExerciseId), nameof(InfluencerId), IsUnique = true)]
[GraphQLDescription("A link between an exercise and a selected influencer.")]
public sealed class ExerciseInfluencer
{
    [Key]
    [GraphQLDescription("The unique identifier of the exercise-to-influencer link.")]
    public Guid Id { get; set; }

    [GraphQLDescription("The exercise id linked to the influencer.")]
    public Guid? ExerciseId { get; set; }

    [GraphQLDescription("The closed-list influencer id linked to the exercise.")]
    public Guid? InfluencerId { get; set; }

    [ForeignKey(nameof(ExerciseId))]
    [InverseProperty("Influencers")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The exercise linked to the influencer.")]
    public Exercise? Exercise { get; set; }

    [ForeignKey(nameof(InfluencerId))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The closed-list entry that represents the influencer.")]
    public ClosedListItem? Influencer { get; set; }

}
