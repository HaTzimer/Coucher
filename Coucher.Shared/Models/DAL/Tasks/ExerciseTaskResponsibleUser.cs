using Coucher.Shared.Constants;
using Coucher.Shared.Models.DAL.Users;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Tasks;

[Table(ConstantValues.ExerciseTaskResponsibleUserTableName)]
[Index(nameof(ExerciseTaskId), nameof(UserId), IsUnique = true)]
[Index(nameof(UserId))]
public sealed class ExerciseTaskResponsibleUser
{
    [Key]
    public Guid Id { get; set; }
    public Guid? ExerciseTaskId { get; set; }
    public Guid? UserId { get; set; }
    public DateTime CreationTime { get; set; }

    [ForeignKey(nameof(ExerciseTaskId))]
    [InverseProperty("ResponsibleUsers")]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public ExerciseTask? ExerciseTask { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty("ResponsibleTaskLinks")]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public UserProfile? User { get; set; }
}
