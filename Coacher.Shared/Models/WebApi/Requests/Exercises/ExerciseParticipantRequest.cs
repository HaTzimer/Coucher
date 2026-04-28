using Coacher.Shared.Models.Enums;

namespace Coacher.Shared.Models.WebApi.Requests.Exercises;

public sealed class ExerciseParticipantRequest
{
    public Guid? UserId { get; set; }
    public ExerciseRole Role { get; set; }
}
