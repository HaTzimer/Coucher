using Coucher.Shared;
using Coucher.Shared.Models.Enums;
using Coucher.Shared.Models.DAL.Users;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Exercises;

[Table(ConstantValues.ExerciseParticipantTableName)]
[GraphQLDescription("A link between an exercise and a participating user.")]
public sealed class ExerciseParticipant
{
    [Key]
    [GraphQLDescription("The unique identifier of the participation record.")]
    public Guid Id { get; set; }
    [GraphQLDescription("The exercise id linked to the participant.")]
    public Guid? ExerciseId { get; set; }
    [GraphQLDescription("The user id of the participant.")]
    public Guid? UserId { get; set; }
    [GraphQLDescription("The user's role inside the exercise.")]
    public ExerciseRole Role { get; set; }
    [GraphQLDescription("When the participation record was created.")]
    public DateTime CreationTime { get; set; }

    [ForeignKey(nameof(ExerciseId))]
    [InverseProperty("Participants")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The exercise linked to the participant.")]
    public Exercise? Exercise { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty("ExerciseParticipants")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The user linked to the exercise.")]
    public UserProfile? User { get; set; }
}
