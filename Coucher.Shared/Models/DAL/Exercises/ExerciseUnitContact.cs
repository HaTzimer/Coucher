using Coucher.Shared.Constants;
using Coucher.Shared.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Exercises;

[Table(ConstantValues.ExerciseUnitContactTableName)]
[Index(nameof(ExerciseId))]
[Index(nameof(ExerciseId), nameof(ContactType))]
public sealed class ExerciseUnitContact
{
    [Key]
    public Guid Id { get; set; }

    public Guid? ExerciseId { get; set; }
    public ExerciseUnitContactType ContactType { get; set; }

    [MaxLength(256)]
    public required string FirstName { get; set; }

    [MaxLength(256)]
    public required string LastName { get; set; }

    [MaxLength(50)]
    public string? PhoneNumber { get; set; }

    [MaxLength(512)]
    public string? ProfileImageUrl { get; set; }

    public required DateTime CreationTime { get; set; }

    [ForeignKey(nameof(ExerciseId))]
    [InverseProperty("UnitContacts")]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public Exercise? Exercise { get; set; }
}
