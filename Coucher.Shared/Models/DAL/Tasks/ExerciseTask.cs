using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Models.DAL.Users;
using Coucher.Shared.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using TaskProgressStatus = Coucher.Shared.Models.Enums.TaskStatus;

namespace Coucher.Shared.Models.DAL.Tasks;

public sealed class ExerciseTask
{
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public int SerialNumber { get; set; }
    public ExerciseSeries Series { get; set; }
    public required string Category { get; set; }
    public required string SubCategory { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? Notes { get; set; }
    public TaskProgressStatus Status { get; set; }
    public DateOnly DueDate { get; set; }
    public int WeeksBeforeExerciseStart { get; set; }
    public Guid? ResponsibleUserId { get; set; }
    public bool IsFixedTemplateTask { get; set; }
    public bool IsOverdue { get; set; }
    public bool IsDueWithinAWeek { get; set; }
    public bool HasOpenDependencies { get; set; }

    [ForeignKey(nameof(ExerciseId))]
    [InverseProperty("Tasks")]
    public required Exercise Exercise { get; set; }

    [ForeignKey(nameof(ResponsibleUserId))]
    [InverseProperty("ResponsibleTasks")]
    public UserProfile? ResponsibleUser { get; set; }

    [InverseProperty("ExerciseTask")]
    public required List<ExerciseTaskInfluencerLink> Influencers { get; set; }

    [InverseProperty("ExerciseTask")]
    public required List<TaskDependency> Dependencies { get; set; }

    [InverseProperty("DependsOnTask")]
    public required List<TaskDependency> DependedOnByTasks { get; set; }
}
