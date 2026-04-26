using Coucher.Shared.Models.Enums;

namespace Coucher.WebApi.Models.WebApi.Requests.Exercises;

public sealed class ExerciseParticipantRequestModel
{
    public Guid? UserId { get; set; }
    public ExerciseRole Role { get; set; }
}
