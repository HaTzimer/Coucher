using Coacher.Shared.Models.DAL.Tasks;
using Coacher.Shared.Models.DAL.Admin;
using Coacher.Shared.Models.DAL.Users;
using Coacher.Shared;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coacher.Shared.Models.DAL.Exercises;

[Table(ConstantValues.ExerciseTableName)]
[Index(nameof(StatusId))]
[Index(nameof(TraineeUnitId))]
[Index(nameof(TrainerUnitId))]
[Index(nameof(CreatedByUserId))]
[Index(nameof(StartDate))]
[Index(nameof(EndDate))]
[GraphQLDescription("An exercise that groups participants, contacts, sections, influencers, and tasks.")]
public sealed class Exercise
{
    [GraphQLDescription("The unique identifier of the exercise.")]
    public Guid Id { get; set; }

    [GraphQLDescription("The display name of the exercise.")]
    [MaxLength(256)]
    public required string Name { get; set; }

    [GraphQLDescription("An optional detailed description of the exercise.")]
    [MaxLength(1024)]
    public string? Description { get; set; }

    [GraphQLDescription("The scheduled start date of the exercise.")]
    public required DateOnly StartDate { get; set; }

    [GraphQLDescription("The scheduled end date of the exercise.")]
    public required DateOnly EndDate { get; set; }

    [GraphQLDescription("The trainee unit id assigned to the exercise.")]
    public Guid? TraineeUnitId { get; set; }

    [GraphQLDescription("The trainer unit id assigned to the exercise.")]
    public Guid? TrainerUnitId { get; set; }

    [GraphQLDescription("The closed-list status id of the exercise.")]
    public Guid? StatusId { get; set; }

    [GraphQLDescription("An optional factor used to compress or stretch the exercise timeline.")]
    public double? CompressionFactor { get; set; }

    [GraphQLDescription("The id of the user who created the exercise.")]
    public Guid? CreatedByUserId { get; set; }

    [GraphQLDescription("When the exercise record was created.")]
    public required DateTime CreationTime { get; set; }

    [GraphQLDescription("When the exercise record was last updated.")]
    public required DateTime LastUpdateTime { get; set; }

    [GraphQLDescription("When the exercise was completed, if it has been completed.")]
    public DateTime? CompletionTime { get; set; }

    [GraphQLDescription("When the exercise was archived, if it has been archived.")]
    public DateTime? ArchiveTime { get; set; }

    [GraphQLDescription("The id of the user who archived the exercise, if available.")]
    public Guid? ArchivedByUserId { get; set; }

    [ForeignKey(nameof(TraineeUnitId))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The trainee unit assigned to the exercise.")]
    public Unit? TraineeUnit { get; set; }

    [ForeignKey(nameof(TrainerUnitId))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The trainer unit assigned to the exercise.")]
    public Unit? TrainerUnit { get; set; }

    [ForeignKey(nameof(StatusId))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The closed-list entry that represents the exercise status.")]
    public ClosedListItem? Status { get; set; }

    [ForeignKey(nameof(CreatedByUserId))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The user who created the exercise.")]
    public UserProfile? CreatedByUser { get; set; }

    [InverseProperty("Exercise")]
    [GraphQLDescription("Users participating in the exercise.")]
    public required List<ExerciseParticipant> Participants { get; set; }

    [InverseProperty("Exercise")]
    [GraphQLDescription("Unit contacts linked to the exercise outside the participant list.")]
    public required List<ExerciseUnitContact> UnitContacts { get; set; }

    [InverseProperty("Exercise")]
    [GraphQLDescription("Influencer selections linked to the exercise.")]
    public required List<ExerciseInfluencer> Influencers { get; set; }

    [InverseProperty("Exercise")]
    [GraphQLDescription("Section selections linked to the exercise.")]
    public required List<ExerciseSection> Sections { get; set; }

    [InverseProperty("Exercise")]
    [GraphQLDescription("Tasks that belong to the exercise.")]
    public required List<ExerciseTask> Tasks { get; set; }

}
