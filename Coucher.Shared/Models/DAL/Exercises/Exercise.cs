using Coucher.Shared.Interfaces;
using Coucher.Shared.Models.DAL.Tasks;
using Coucher.Shared.Models.Enums;

namespace Coucher.Shared.Models.DAL.Exercises;

public sealed class Exercise : IHasId<Guid>
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public UnitEchelon TraineeUnitEchelon { get; set; }
    public UnitEchelon TrainerUnitEchelon { get; set; }
    public bool IsOperationalTerrainExercise { get; set; }
    public ExerciseStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public Guid ManagerParticipantId { get; set; }
    public Guid? TraineeUnitContactParticipantId { get; set; }
    public ExerciseParticipant? ManagerParticipant { get; set; }
    public ExerciseParticipant? TraineeUnitContactParticipant { get; set; }
    public required List<ExerciseParticipant> Participants { get; set; }
    public required List<ExerciseInfluencerLink> Influencers { get; set; }
    public required List<ExerciseThreatArenaLink> ThreatArenas { get; set; }
    public required List<ExerciseTask> Tasks { get; set; }
}
