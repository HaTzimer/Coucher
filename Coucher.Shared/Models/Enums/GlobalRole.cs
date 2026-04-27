using HotChocolate;

namespace Coucher.Shared.Models.Enums;

[GraphQLDescription("Defines a user's global system role.")]
public enum GlobalRole
{
    [GraphQLDescription("A regular user of the system.")]
    User = 0,
    [GraphQLDescription("A user with read-focused auditing access.")]
    Auditor = 1,
    [GraphQLDescription("A user with administrative access across the system.")]
    Admin = 2
}
