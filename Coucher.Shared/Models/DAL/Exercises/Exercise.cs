using Coucher.Shared.Models.DAL.Tasks;
using Coucher.Shared.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Exercises;

public sealed class Exercise
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public required string TraineeUnitName { get; set; }
    public required string TrainerUnitName { get; set; }
    public UnitEchelon TraineeUnitEchelon { get; set; }
    public UnitEchelon TrainerUnitEchelon { get; set; }
    public bool IsOperationalTerrainExercise { get; set; }
    public ExerciseStatus Status { get; set; }
    public bool UsesRelativeDueCompression { get; set; }
    public double? DueCompressionFactor { get; set; }
    public int? RecommendedPreparationDays { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ActivatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? ArchiveDueAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public Guid? ArchivedByUserId { get; set; }
    public bool IsArchivedManually { get; set; }
    public Guid? PrimaryManagerParticipantId { get; set; }
    public Guid? PrimaryTraineeUnitContactParticipantId { get; set; }

    [ForeignKey(nameof(PrimaryManagerParticipantId))]
    public ExerciseParticipant? PrimaryManagerParticipant { get; set; }

    [ForeignKey(nameof(PrimaryTraineeUnitContactParticipantId))]
    public ExerciseParticipant? PrimaryTraineeUnitContactParticipant { get; set; }

    [InverseProperty("Exercise")]
    public required List<ExerciseParticipant> Participants { get; set; }

    [InverseProperty("Exercise")]
    public required List<ExerciseInfluencerLink> Influencers { get; set; }

    [InverseProperty("Exercise")]
    public required List<ExerciseThreatArenaLink> ThreatArenas { get; set; }

    [InverseProperty("Exercise")]
    public required List<ExerciseTask> Tasks { get; set; }
}
