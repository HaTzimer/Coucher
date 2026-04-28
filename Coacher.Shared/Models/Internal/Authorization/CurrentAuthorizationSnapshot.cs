using Coacher.Shared.Models.Enums;

namespace Coacher.Shared.Models.Internal.Authorization;

public sealed class CurrentAuthorizationSnapshot
{
    public required Guid UserId { get; set; }

    public required List<GlobalRole> GlobalRoles { get; set; }

    public bool IsAdmin => GlobalRoles.Contains(GlobalRole.Admin);

    public bool IsAuditor => GlobalRoles.Contains(GlobalRole.Auditor);
}
