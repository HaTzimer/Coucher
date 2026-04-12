using Coucher.Shared.Models.Enums;

namespace Coucher.WebApi.Models.WebApi.Requests.Exercises;

public sealed class ExerciseParticipantRequestModel
{
    public Guid? UserId { get; set; }
    public string? PersonalNumber { get; set; }
    public required string FullName { get; set; }
    public string? Rank { get; set; }
    public string? Position { get; set; }
    public string? PhoneNumber { get; set; }
    public ExerciseRole Role { get; set; }
}
