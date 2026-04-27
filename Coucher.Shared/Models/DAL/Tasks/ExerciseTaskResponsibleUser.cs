using Coucher.Shared;
using Coucher.Shared.Models.DAL.Users;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Tasks;

[Table(ConstantValues.ExerciseTaskResponsibleUserTableName)]
[Index(nameof(TaskId), nameof(UserId), IsUnique = true)]
[Index(nameof(UserId))]
public sealed class ExerciseTaskResponsibleUser
{
    [Key]
    public Guid Id { get; set; }
    public Guid? TaskId { get; set; }
    public Guid? UserId { get; set; }
    public DateTime CreationTime { get; set; }

    [ForeignKey(nameof(TaskId))]
    [InverseProperty("ResponsibleUsers")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    public ExerciseTask? Task { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty("ResponsibleTaskLinks")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    public UserProfile? User { get; set; }
}
