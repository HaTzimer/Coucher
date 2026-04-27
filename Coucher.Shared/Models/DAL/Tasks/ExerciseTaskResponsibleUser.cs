using Coucher.Shared;
using Coucher.Shared.Models.DAL.Users;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Tasks;

[Table(ConstantValues.ExerciseTaskResponsibleUserTableName)]
[Index(nameof(TaskId), nameof(UserId), IsUnique = true)]
[Index(nameof(UserId))]
[GraphQLDescription("A link between an exercise task and a responsible user.")]
public sealed class ExerciseTaskResponsibleUser
{
    [Key]
    [GraphQLDescription("The unique identifier of the responsibility link.")]
    public Guid Id { get; set; }
    [GraphQLDescription("The task id linked to the responsible user.")]
    public Guid? TaskId { get; set; }
    [GraphQLDescription("The user id linked to the task.")]
    public Guid? UserId { get; set; }
    [GraphQLDescription("When the responsibility link was created.")]
    public DateTime CreationTime { get; set; }

    [ForeignKey(nameof(TaskId))]
    [InverseProperty("ResponsibleUsers")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The task linked to the responsible user.")]
    public ExerciseTask? Task { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty("ResponsibleTaskLinks")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The user linked to the task.")]
    public UserProfile? User { get; set; }
}
