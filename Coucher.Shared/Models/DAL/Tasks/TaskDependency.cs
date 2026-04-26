using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Tasks;

public sealed class TaskDependency
{
    [Key]
    public Guid Id { get; set; }
    public Guid ExerciseTaskId { get; set; }
    public Guid DependsOnTaskId { get; set; }
    public bool IsBlocking { get; set; }
    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(ExerciseTaskId))]
    [InverseProperty("Dependencies")]
    public required ExerciseTask ExerciseTask { get; set; }

    [ForeignKey(nameof(DependsOnTaskId))]
    [InverseProperty("DependedOnByTasks")]
    public required ExerciseTask DependsOnTask { get; set; }
}