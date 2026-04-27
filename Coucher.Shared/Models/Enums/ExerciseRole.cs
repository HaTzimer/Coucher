using HotChocolate;

namespace Coucher.Shared.Models.Enums;

[GraphQLDescription("Defines how a user participates in an exercise.")]
public enum ExerciseRole
{
    [GraphQLDescription("The user manages the exercise.")]
    ExerciseManager = 0,
    [GraphQLDescription("The user participates in the exercise without being its manager.")]
    ExerciseParticipant = 1
}
