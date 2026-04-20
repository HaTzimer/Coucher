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
    public UnitEchelon TraineeUnitEchelon { get; set; }
    public UnitEchelon TrainerUnitEchelon { get; set; }
    public int AssignedTaskCountForCurrentUser { get; set; }
    public double CompletionPercentage { get; set; }
    public int WeeksUntilStart { get; set; }
    public required string ManagerName { get; set; }
    public string? ManagerPosition { get; set; }
    public string? ManagerRank { get; set; }
    public ExerciseStatus Status { get; set; }
}
