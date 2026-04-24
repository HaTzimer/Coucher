using Coucher.Shared.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Shared.Models.DAL.Exercises;

[Keyless]
public sealed class ExerciseSummary
{
    public Guid ExerciseId { get; set; }
    public required string Name { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public required string TraineeUnitName { get; set; }
    public required string TrainerUnitName { get; set; }
    public UnitEchelon TraineeUnitEchelon { get; set; }
    public UnitEchelon TrainerUnitEchelon { get; set; }
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
    public ExerciseStatus Status { get; set; }
    public DateTime? ArchivedAt { get; set; }
}
