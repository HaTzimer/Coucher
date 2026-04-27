using Coucher.Shared;
using Coucher.Shared.Models.Enums;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Users;

[Table(ConstantValues.UserRoleTableName)]
[GraphQLDescription("A global role assignment attached to a user.")]
public sealed class UserRole
{
    [Key]
    [GraphQLDescription("The unique identifier of the user role assignment.")]
    public Guid Id { get; set; }
    [GraphQLDescription("The user id that owns the role assignment.")]
    public Guid? UserId { get; set; }
    [GraphQLDescription("The global role assigned to the user.")]
    public GlobalRole Role { get; set; }
    [GraphQLDescription("When the role was assigned.")]
    public DateTime AssignedTime { get; set; }
    [GraphQLDescription("The id of the user who assigned the role, if tracked.")]
    public Guid? AssignedByUserId { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty("Roles")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The user who owns the role assignment.")]
    public UserProfile? User { get; set; }
}
