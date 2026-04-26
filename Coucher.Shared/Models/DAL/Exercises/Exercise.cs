using Coucher.Shared.Models.DAL.Tasks;
using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared.Constants;
using Coucher.Shared.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Exercises;

[Table(ConstantValues.ExerciseTableName)]
[Index(nameof(StatusId))]
[Index(nameof(TraineeUnitId))]
[Index(nameof(TrainerUnitId))]
[Index(nameof(StartDate))]
[Index(nameof(EndDate))]
public sealed class Exercise
{
    public Guid Id { get; set; }
    [MaxLength(200)]
    public required string Name { get; set; }
    [MaxLength(4000)]
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public Guid? TraineeUnitId { get; set; }
    public Guid? TrainerUnitId { get; set; }
    public bool IsOperationalTerrainExercise { get; set; }//mashpiim
    public Guid? StatusId { get; set; }//look again
    public double? DueCompressionFactor { get; set; }//rename
    public DateTime CreationTime { get; set; }
    public DateTime LastUpdateTime { get; set; }
    public DateTime? CompletionTime { get; set; }
    public DateTime? ArchiveTime { get; set; }
    public Guid? ArchivedByUserId { get; set; }
    public Guid? PrimaryManagerParticipantId { get; set; }//haran
    public Guid? PrimaryTraineeUnitContactParticipantId { get; set; }//haran not user necesarly

    [ForeignKey(nameof(PrimaryManagerParticipantId))]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public ExerciseParticipant? PrimaryManagerParticipant { get; set; }

    [ForeignKey(nameof(PrimaryTraineeUnitContactParticipantId))]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public ExerciseParticipant? PrimaryTraineeUnitContactParticipant { get; set; }

    [ForeignKey(nameof(TraineeUnitId))]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public Unit? TraineeUnit { get; set; }

    [ForeignKey(nameof(TrainerUnitId))]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public Unit? TrainerUnit { get; set; }

    [ForeignKey(nameof(StatusId))]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public ClosedListItem? Status { get; set; }

    [InverseProperty("Exercise")]
    public required List<ExerciseParticipant> Participants { get; set; }

    [InverseProperty("Exercise")]
    public required List<ExerciseInfluencer> Influencers { get; set; }

    [InverseProperty("Exercise")]
    public required List<ExerciseSection> Sections { get; set; }

    [InverseProperty("Exercise")]
    public required List<ExerciseTask> Tasks { get; set; }
}
