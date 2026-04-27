using Coucher.Shared;
using Coucher.Shared.Models.Enums;
using Coucher.Shared.Models.DAL.Users;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Exercises;

[Table(ConstantValues.ExerciseParticipantTableName)]
public sealed class ExerciseParticipant
{
    [Key]
    public Guid Id { get; set; }
    public Guid? ExerciseId { get; set; }
    public Guid? UserId { get; set; }
    public ExerciseRole Role { get; set; }
    public DateTime CreationTime { get; set; }

    [ForeignKey(nameof(ExerciseId))]
    [InverseProperty("Participants")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    public Exercise? Exercise { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty("ExerciseParticipants")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    public UserProfile? User { get; set; }
}
