using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Models.DAL.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Tasks;

public sealed class ExerciseTask
{
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public Guid? ParentTaskId { get; set; }
    public Guid? SourceFixedTaskTemplateId { get; set; }
    public Guid SeriesClosedListItemId { get; set; }
    public Guid CategoryClosedListItemId { get; set; }
    public Guid StatusClosedListItemId { get; set; }
    public int SerialNumber { get; set; }
    public int? SubTaskSerialNumber { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? Notes { get; set; }
    public DateOnly DueDate { get; set; }
    public Guid? ResponsibleUserId { get; set; }
    public Guid? LastStatusChangedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsFixedTemplateTask { get; set; }

    [ForeignKey(nameof(ExerciseId))]
    [InverseProperty("Tasks")]
    public required Exercise Exercise { get; set; }

    [ForeignKey(nameof(ParentTaskId))]
    [InverseProperty(nameof(SubTasks))]
    public ExerciseTask? ParentTask { get; set; }

    [ForeignKey(nameof(SourceFixedTaskTemplateId))]
    public FixedTaskTemplate? SourceFixedTaskTemplate { get; set; }

    [ForeignKey(nameof(SeriesClosedListItemId))]
    public required ClosedListItem SeriesClosedListItem { get; set; }

    [ForeignKey(nameof(CategoryClosedListItemId))]
    public required ClosedListItem CategoryClosedListItem { get; set; }

    [ForeignKey(nameof(StatusClosedListItemId))]
    public required ClosedListItem StatusClosedListItem { get; set; }

    [ForeignKey(nameof(ResponsibleUserId))]
    [InverseProperty("ResponsibleTasks")]
    public UserProfile? ResponsibleUser { get; set; }

    [InverseProperty(nameof(ParentTask))]
    public required List<ExerciseTask> SubTasks { get; set; }

    [InverseProperty("ExerciseTask")]
    public required List<ExerciseTaskInfluencerLink> Influencers { get; set; }

    [InverseProperty("ExerciseTask")]
    public required List<TaskDependency> Dependencies { get; set; }

    [InverseProperty("DependsOnTask")]
    public required List<TaskDependency> DependedOnByTasks { get; set; }
}
