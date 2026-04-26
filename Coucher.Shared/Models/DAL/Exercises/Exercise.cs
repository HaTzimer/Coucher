using Coucher.Shared.Models.DAL.Tasks;
using Coucher.Shared.Models.DAL.Admin;
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
    public Guid TraineeUnitId { get; set; }
    public Guid TrainerUnitId { get; set; }
    public bool IsOperationalTerrainExercise { get; set; }
    public Guid StatusClosedListItemId { get; set; }
    public double? DueCompressionFactor { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
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

    [ForeignKey(nameof(TraineeUnitId))]
    public required Unit TraineeUnit { get; set; }

    [ForeignKey(nameof(TrainerUnitId))]
    public required Unit TrainerUnit { get; set; }

    [ForeignKey(nameof(StatusClosedListItemId))]
    public required ClosedListItem StatusClosedListItem { get; set; }

    [InverseProperty("Exercise")]
    public required List<ExerciseParticipant> Participants { get; set; }

    [InverseProperty("Exercise")]
    public required List<ExerciseInfluencerLink> Influencers { get; set; }

    [InverseProperty("Exercise")]
    public required List<ExerciseThreatArenaLink> ThreatArenas { get; set; }

    [InverseProperty("Exercise")]
    public required List<ExerciseTask> Tasks { get; set; }
}
