using Coucher.Shared.Models.DAL.Tasks;
using Coucher.Shared.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Exercises;

public sealed class Exercise
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

    [ForeignKey(nameof(ManagerParticipantId))]
    public ExerciseParticipant? ManagerParticipant { get; set; }

    [ForeignKey(nameof(TraineeUnitContactParticipantId))]
    public ExerciseParticipant? TraineeUnitContactParticipant { get; set; }

    [InverseProperty("Exercise")]
    public required List<ExerciseParticipant> Participants { get; set; }

    [InverseProperty("Exercise")]
    public required List<ExerciseInfluencerLink> Influencers { get; set; }

    [InverseProperty("Exercise")]
    public required List<ExerciseThreatArenaLink> ThreatArenas { get; set; }

    [InverseProperty("Exercise")]
    public required List<ExerciseTask> Tasks { get; set; }
}
