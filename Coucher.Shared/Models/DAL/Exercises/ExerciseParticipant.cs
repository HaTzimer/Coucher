using Coucher.Shared.Models.Enums;
using Coucher.Shared.Models.DAL.Users;

namespace Coucher.Shared.Models.DAL.Exercises;

public sealed class ExerciseParticipant
{
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public Guid? UserId { get; set; }
    public string? PersonalNumber { get; set; }
    public required string FullName { get; set; }
    public string? Rank { get; set; }
    public string? Position { get; set; }
    public string? PhoneNumber { get; set; }
    public ExerciseRole Role { get; set; }
    public required Exercise Exercise { get; set; }
    public UserProfile? User { get; set; }
}
