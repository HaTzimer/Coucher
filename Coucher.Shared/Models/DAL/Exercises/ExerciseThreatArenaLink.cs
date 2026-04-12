using Coucher.Shared.Models.Enums;

namespace Coucher.Shared.Models.DAL.Exercises;

public sealed class ExerciseThreatArenaLink
{
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public ThreatArenaCode ThreatArenaCode { get; set; }
    public required Exercise Exercise { get; set; }
}
