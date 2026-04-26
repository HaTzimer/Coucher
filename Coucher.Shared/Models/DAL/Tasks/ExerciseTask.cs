using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Tasks;

[Table(ConstantValues.ExerciseTaskTableName)]
[Index(nameof(ExerciseId))]
[Index(nameof(ParentId))]
[Index(nameof(StatusId))]
public sealed class ExerciseTask
{
    public Guid Id { get; set; }
    public required Guid ExerciseId { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? SourceId { get; set; }
    public Guid SeriesId { get; set; }
    public Guid CategoryId { get; set; }
    public required Guid StatusId { get; set; }
    public required int SerialNumber { get; set; }
    [MaxLength(256)]
    public required string Name { get; set; }
    [MaxLength(1024)]
    public required string Description { get; set; }
    [MaxLength(1024)]
    public string? Notes { get; set; }
    public required DateTime DueDate { get; set; }
    public Guid? LastStatusUpdaterId { get; set; }
    public required DateTime CreationTime { get; set; }
    public required DateTime LastUpdateTime { get; set; }
    public DateTime? LastStatusUpdateTime { get; set; } 
    public DateTime? CompletionTime { get; set; } //last status update
    // add has children

    [ForeignKey(nameof(ExerciseId))]
    [InverseProperty("Tasks")]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public Exercise? Exercise { get; set; }

    [ForeignKey(nameof(ParentId))]
    [InverseProperty(nameof(Children))]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public ExerciseTask? Parent { get; set; }

    [ForeignKey(nameof(SourceId))]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public TaskTemplate? Source { get; set; }

    [ForeignKey(nameof(SeriesId))]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public ClosedListItem? Series { get; set; }

    [ForeignKey(nameof(CategoryId))]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public ClosedListItem? Category { get; set; }

    [ForeignKey(nameof(StatusId))]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public ClosedListItem? Status { get; set; }

    [InverseProperty(nameof(Parent))]
    public required List<ExerciseTask> Children { get; set; }

    [InverseProperty("Task")]
    public required List<ExerciseTaskResponsibleUser> ResponsibleUsers { get; set; }

    [InverseProperty("Task")]
    public required List<TaskDependency> Dependencies { get; set; }

    [InverseProperty("DependsOn")]
    public required List<TaskDependency> DependedOnBy { get; set; }
}
