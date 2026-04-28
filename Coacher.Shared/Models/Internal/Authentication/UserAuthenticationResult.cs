namespace Coacher.Shared.Models.Internal.Authentication;

public sealed class UserAuthenticationResult
{
    public Guid? UserId { get; set; }
    public bool IsValid { get; set; }
}
