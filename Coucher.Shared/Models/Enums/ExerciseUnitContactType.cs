using HotChocolate;

namespace Coucher.Shared.Models.Enums;

[GraphQLDescription("Defines which unit side a contact belongs to inside an exercise.")]
public enum ExerciseUnitContactType
{
    [GraphQLDescription("A contact for the trainee unit.")]
    TraineeUnitContact = 0,
    [GraphQLDescription("A contact for the trainer unit.")]
    TrainerUnitContact = 1
}
