using Coacher.Shared.Models.DAL.Admin;
using Coacher.Shared.Models.DAL.Exercises;
using Coacher.Shared.Models.DAL.Notifications;
using Coacher.Shared.Models.DAL.Tasks;
using Coacher.Shared;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coacher.Shared.Models.DAL.Users;

[Table(ConstantValues.UserProfileTableName)]
[Index(nameof(IdentityNumber), IsUnique = true)]
[Index(nameof(UnitId))]
[GraphQLDescription("A user profile stored by the system.")]
public sealed class UserProfile
{
    [GraphQLDescription("The unique identifier of the user profile.")]
    public Guid Id { get; set; }

    [GraphQLDescription("The identity number used to identify the user.")]
    [MaxLength(128)]
    public required string IdentityNumber { get; set; }

    [GraphQLDescription("The user's first name.")]
    [MaxLength(256)]
    public required string FirstName { get; set; }

    [GraphQLDescription("The user's last name.")]
    [MaxLength(256)]
    public required string LastName { get; set; }

    [GraphQLDescription("An optional personal number for the user.")]
    [MaxLength(128)]
    public string? PersonalNumber { get; set; }

    [GraphQLDescription("The unit id assigned to the user.")]
    public Guid? UnitId { get; set; }

    [GraphQLDescription("The user's rank.")]
    [MaxLength(128)]
    public string? Rank { get; set; }

    [GraphQLDescription("The user's position or role title.")]
    [MaxLength(128)]
    public string? Position { get; set; }

    [GraphQLDescription("The user's phone number.")]
    [MaxLength(128)]
    public string? PhoneNumber { get; set; }

    [GraphQLDescription("The user's civilian email address.")]
    [MaxLength(256)]
    public string? CivilianEmail { get; set; }

    [GraphQLDescription("The user's military email address.")]
    [MaxLength(256)]
    public string? MilitaryEmail { get; set; }

    [GraphQLDescription("An optional profile image URL for the user.")]
    [MaxLength(512)]
    public string? ProfileImageUrl { get; set; }

    [GraphQLDescription("The stored password hash, when authentication data exists.")]
    [MaxLength(512)]
    public string? PasswordHash { get; set; }

    [GraphQLDescription("When the user last logged in.")]
    public DateTime? LastLoginTime { get; set; }

    [GraphQLDescription("When the user profile was created.")]
    public DateTime CreationTime { get; set; }

    [GraphQLDescription("When the user profile was last updated.")]
    public DateTime LastUpdateTime { get; set; }

    [ForeignKey(nameof(UnitId))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The unit assigned to the user.")]
    public Unit? Unit { get; set; }

    [InverseProperty("User")]
    [GraphQLDescription("Global role assignments attached to the user.")]
    public required List<UserRole> Roles { get; set; }

    [InverseProperty("User")]
    [GraphQLDescription("Exercise participation records linked to the user.")]
    public required List<ExerciseParticipant> ExerciseParticipants { get; set; }

    [InverseProperty("User")]
    [GraphQLDescription("Task responsibility links assigned to the user.")]
    public required List<ExerciseTaskResponsibleUser> ResponsibleTaskLinks { get; set; }

    [InverseProperty("User")]
    [GraphQLDescription("Notifications shown to the user.")]
    public required List<UserNotification> Notifications { get; set; }

    [InverseProperty(nameof(ExternalId.User))]
    [GraphQLDescription("External identifiers attached to the user from source systems.")]
    public required List<ExternalId> ExternalIds { get; set; }

}
