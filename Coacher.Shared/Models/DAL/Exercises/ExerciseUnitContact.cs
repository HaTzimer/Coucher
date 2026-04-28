using Coacher.Shared;
using Coacher.Shared.Models.Enums;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coacher.Shared.Models.DAL.Exercises;

[Table(ConstantValues.ExerciseUnitContactTableName)]
[Index(nameof(ExerciseId))]
[Index(nameof(ExerciseId), nameof(ContactType))]
[GraphQLDescription("A non-participant contact linked to an exercise for either the trainee or trainer unit.")]
public sealed class ExerciseUnitContact
{
    [Key]
    [GraphQLDescription("The unique identifier of the unit contact record.")]
    public Guid Id { get; set; }

    [GraphQLDescription("The exercise id linked to this contact.")]
    public Guid? ExerciseId { get; set; }

    [GraphQLDescription("Which unit side this contact belongs to in the exercise.")]
    public ExerciseUnitContactType ContactType { get; set; }

    [GraphQLDescription("The contact's first name.")]
    [MaxLength(256)]
    public required string FirstName { get; set; }

    [GraphQLDescription("The contact's last name.")]
    [MaxLength(256)]
    public required string LastName { get; set; }

    [GraphQLDescription("The contact's phone number.")]
    [MaxLength(50)]
    public string? PhoneNumber { get; set; }

    [GraphQLDescription("An optional profile image URL for the contact.")]
    [MaxLength(512)]
    public string? ProfileImageUrl { get; set; }

    [GraphQLDescription("When the contact record was created.")]
    public required DateTime CreationTime { get; set; }

    [ForeignKey(nameof(ExerciseId))]
    [InverseProperty("UnitContacts")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The exercise linked to this contact.")]
    public Exercise? Exercise { get; set; }

}
