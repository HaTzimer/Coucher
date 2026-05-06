using Coacher.Shared;
using Coacher.Shared.Models.DAL.Admin;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coacher.Shared.Models.DAL.Users;

[Table(ConstantValues.ExternalIdTableName)]
[Index(nameof(UserId))]
[Index(nameof(ExternalSourceId))]
[Index(nameof(ExternalIdValue))]
[GraphQLDescription("An external identifier attached to a user profile from a specific source system.")]
public sealed class ExternalId
{
    [Key]
    [GraphQLDescription("The unique identifier of the external id record.")]
    public Guid Id { get; set; }

    [GraphQLDescription("The user id that owns this external id.")]
    public Guid? UserId { get; set; }

    [GraphQLDescription("The raw external id value from the source system.")]
    [MaxLength(512)]
    public required string ExternalIdValue { get; set; }

    [GraphQLDescription("The closed-list id that identifies the source system for this external id.")]
    public Guid? ExternalSourceId { get; set; }

    [GraphQLDescription("When the external id record was created.")]
    public DateTime CreationTime { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(UserProfile.ExternalIds))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The user profile that owns this external id.")]
    public UserProfile? User { get; set; }

    [ForeignKey(nameof(ExternalSourceId))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The closed-list entry that represents the external source system.")]
    public ClosedListItem? ExternalSource { get; set; }
}
