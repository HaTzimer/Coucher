using Microsoft.EntityFrameworkCore;

namespace Coucher.Shared.Models.Internal.Projections.Exercises;

[Keyless]
public sealed class ExerciseSummary
{
    public Guid ExerciseId { get; set; }
    public required string Name { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public Guid TraineeUnitId { get; set; }
    public Guid TrainerUnitId { get; set; }
    public int AssignedTaskCountForCurrentUser { get; set; }
    public int OpenTaskCount { get; set; }
    public int OverdueTaskCount { get; set; }
    public int DueSoonTaskCount { get; set; }
    public double CompletionPercentage { get; set; }
    public int WeeksUntilStart { get; set; }
    public int DaysUntilStart { get; set; }
    public required string ManagerName { get; set; }
    public string? ManagerPosition { get; set; }
    public string? ManagerRank { get; set; }
    public string? CurrentUserRole { get; set; }
    public Guid StatusId { get; set; }
    public DateTime? ArchiveTime { get; set; }
}
