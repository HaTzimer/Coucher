using Coucher.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Tasks;

[Table(ConstantValues.TaskDependencyTableName)]
[Index(nameof(ExerciseTaskId), nameof(DependsOnId), IsUnique = true)]
[Index(nameof(DependsOnId))]
public sealed class TaskDependency
{
    [Key]
    public Guid Id { get; set; }
    public Guid? ExerciseTaskId { get; set; }
    public Guid? DependsOnId { get; set; }
    public DateTime CreationTime { get; set; }

    [ForeignKey(nameof(ExerciseTaskId))]
    [InverseProperty("Dependencies")]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public ExerciseTask? ExerciseTask { get; set; }

    [ForeignKey(nameof(DependsOnId))]
    [InverseProperty("DependedOnByTasks")]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public ExerciseTask? DependsOn { get; set; }
}
