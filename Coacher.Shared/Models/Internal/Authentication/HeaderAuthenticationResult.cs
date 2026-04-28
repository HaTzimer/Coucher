using Coacher.Shared.Models.Enums;

namespace Coacher.Shared.Models.Internal.Authentication;

public sealed class HeaderAuthenticationResult
{
    public SessionAuthenticationResult HeaderAuthentication { get; set; }
    public string? SessionId { get; set; }
    public Guid? UserId { get; set; }
    public string? ErrorMessage { get; set; }
}
