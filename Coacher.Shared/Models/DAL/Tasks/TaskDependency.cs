using Coacher.Shared;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coacher.Shared.Models.DAL.Tasks;

[Table(ConstantValues.TaskDependencyTableName)]
[Index(nameof(TaskId), nameof(DependsOnId), IsUnique = true)]
[Index(nameof(DependsOnId))]
[GraphQLDescription("A dependency link between two exercise tasks.")]
public sealed class TaskDependency
{
    [Key]
    [GraphQLDescription("The unique identifier of the task dependency link.")]
    public Guid Id { get; set; }

    [GraphQLDescription("The task that depends on another task.")]
    public Guid? TaskId { get; set; }

    [GraphQLDescription("The task that should be completed first.")]
    public Guid? DependsOnId { get; set; }

    [GraphQLDescription("When the dependency link was created.")]
    public DateTime CreationTime { get; set; }

    [ForeignKey(nameof(TaskId))]
    [InverseProperty("Dependencies")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The task that depends on another task.")]
    public ExerciseTask? Task { get; set; }

    [ForeignKey(nameof(DependsOnId))]
    [InverseProperty("DependedOnBy")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The task that must be completed first.")]
    public ExerciseTask? DependsOn { get; set; }

}
