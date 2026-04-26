using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Exercises;

[Table(ConstantValues.ExerciseSectionTableName)]
[Index(nameof(ExerciseId), nameof(SectionId), IsUnique = true)]
public sealed class ExerciseSection
{
    [Key]
    public Guid Id { get; set; }
    public Guid? ExerciseId { get; set; }
    public Guid? SectionId { get; set; }

    [ForeignKey(nameof(ExerciseId))]
    [InverseProperty("Sections")]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public Exercise? Exercise { get; set; }

    [ForeignKey(nameof(SectionId))]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public ClosedListItem? Section { get; set; }
}
