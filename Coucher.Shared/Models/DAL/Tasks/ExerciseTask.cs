using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Tasks;

[Table(ConstantValues.ExerciseTaskTableName)]
[Index(nameof(ExerciseId))]
[Index(nameof(ParentId))]
[Index(nameof(StatusId))]
[GraphQLDescription("A task that belongs to an exercise.")]
public sealed class ExerciseTask
{
    [GraphQLDescription("The unique identifier of the task.")]
    public Guid Id { get; set; }
    [GraphQLDescription("The exercise id that owns the task.")]
    public required Guid ExerciseId { get; set; }
    [GraphQLDescription("The optional parent task id when the task is a child task.")]
    public Guid? ParentId { get; set; }
    [GraphQLDescription("The optional source template id from which the task was created.")]
    public Guid? SourceId { get; set; }
    [GraphQLDescription("The closed-list series id of the task.")]
    public Guid SeriesId { get; set; }
    [GraphQLDescription("The closed-list category id of the task.")]
    public Guid CategoryId { get; set; }
    [GraphQLDescription("The closed-list status id of the task.")]
    public required Guid StatusId { get; set; }
    [GraphQLDescription("The serial number used to order the task within the exercise.")]
    public required int SerialNumber { get; set; }
    [GraphQLDescription("The task name.")]
    [MaxLength(256)]
    public required string Name { get; set; }
    [GraphQLDescription("An optional detailed description of the task.")]
    [MaxLength(1024)]
    public string? Description { get; set; }
    [GraphQLDescription("Optional internal notes for the task.")]
    [MaxLength(1024)]
    public string? Notes { get; set; }
    [GraphQLDescription("The due date and time of the task.")]
    public required DateTime DueDate { get; set; }
    [GraphQLDescription("The id of the user who last changed the task status.")]
    public Guid? LastStatusUpdaterId { get; set; }
    [GraphQLDescription("When the task record was created.")]
    public required DateTime CreationTime { get; set; }
    [GraphQLDescription("When the task record was last updated.")]
    public required DateTime LastUpdateTime { get; set; }
    [GraphQLDescription("When the task status was last updated.")]
    public DateTime? LastStatusUpdateTime { get; set; } 
    [GraphQLDescription("When the task was completed, if it has been completed.")]
    public DateTime? CompletionTime { get; set; }
    [GraphQLDescription("Whether the task currently has child tasks.")]
    public bool HasChildren { get; set; }

    [ForeignKey(nameof(ExerciseId))]
    [InverseProperty("Tasks")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The exercise that owns the task.")]
    public Exercise? Exercise { get; set; }

    [ForeignKey(nameof(ParentId))]
    [InverseProperty(nameof(Children))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The parent task when this task is nested under another task.")]
    public ExerciseTask? Parent { get; set; }

    [ForeignKey(nameof(SourceId))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The source task template from which the task was created.")]
    public TaskTemplate? Source { get; set; }

    [ForeignKey(nameof(SeriesId))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The closed-list entry that represents the task series.")]
    public ClosedListItem? Series { get; set; }

    [ForeignKey(nameof(CategoryId))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The closed-list entry that represents the task category.")]
    public ClosedListItem? Category { get; set; }

    [ForeignKey(nameof(StatusId))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The closed-list entry that represents the task status.")]
    public ClosedListItem? Status { get; set; }

    [InverseProperty(nameof(Parent))]
    [GraphQLDescription("Child tasks nested under this task.")]
    public required List<ExerciseTask> Children { get; set; }

    [InverseProperty("Task")]
    [GraphQLDescription("User assignment links attached to this task.")]
    public required List<ExerciseTaskResponsibleUser> ResponsibleUsers { get; set; }

    [InverseProperty("Task")]
    [GraphQLDescription("Dependency links where this task depends on other tasks.")]
    public required List<TaskDependency> Dependencies { get; set; }

    [InverseProperty("DependsOn")]
    [GraphQLDescription("Dependency links where other tasks depend on this task.")]
    public required List<TaskDependency> DependedOnBy { get; set; }
}
