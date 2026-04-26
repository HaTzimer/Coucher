using Coucher.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Tasks;

[Table(ConstantValues.TaskDependencyTableName)]
[Index(nameof(TaskId), nameof(DependsOnId), IsUnique = true)]
[Index(nameof(DependsOnId))]
public sealed class TaskDependency
{
    [Key]
    public Guid Id { get; set; }
    public Guid? TaskId { get; set; }
    public Guid? DependsOnId { get; set; }
    public DateTime CreationTime { get; set; }

    [ForeignKey(nameof(TaskId))]
    [InverseProperty("Dependencies")]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public ExerciseTask? Task { get; set; }

    [ForeignKey(nameof(DependsOnId))]
    [InverseProperty("DependedOnBy")]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public ExerciseTask? DependsOn { get; set; }
}
