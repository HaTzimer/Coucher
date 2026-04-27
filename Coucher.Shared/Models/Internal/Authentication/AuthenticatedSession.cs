namespace Coucher.Shared.Models.Internal.Authentication;

public sealed class AuthenticatedSession
{
    public required string SessionId { get; set; }
    public Guid UserId { get; set; }
}
