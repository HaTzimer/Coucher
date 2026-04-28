using Coacher.Shared.Models.Enums;

namespace Coacher.Shared.Models.WebApi.Requests.Exercises;

public sealed class UpdateExerciseUnitContactRequest
{
    public ExerciseUnitContactType ContactType { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfileImageUrl { get; set; }
}
