using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Exercises;

[Table(ConstantValues.ExerciseSectionTableName)]
[Index(nameof(ExerciseId), nameof(SectionId), IsUnique = true)]
[GraphQLDescription("A link between an exercise and a selected section.")]
public sealed class ExerciseSection
{
    [Key]
    [GraphQLDescription("The unique identifier of the exercise-to-section link.")]
    public Guid Id { get; set; }

    [GraphQLDescription("The exercise id linked to the section.")]
    public Guid? ExerciseId { get; set; }

    [GraphQLDescription("The closed-list section id linked to the exercise.")]
    public Guid? SectionId { get; set; }

    [ForeignKey(nameof(ExerciseId))]
    [InverseProperty("Sections")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The exercise linked to the section.")]
    public Exercise? Exercise { get; set; }

    [ForeignKey(nameof(SectionId))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The closed-list entry that represents the section.")]
    public ClosedListItem? Section { get; set; }

}
