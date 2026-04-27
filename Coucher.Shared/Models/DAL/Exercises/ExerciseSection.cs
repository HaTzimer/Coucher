using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared;
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
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    public Exercise? Exercise { get; set; }

    [ForeignKey(nameof(SectionId))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    public ClosedListItem? Section { get; set; }
}
