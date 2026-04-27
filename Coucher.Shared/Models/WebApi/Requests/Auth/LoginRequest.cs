namespace Coucher.Shared.Models.WebApi.Requests.Auth;

public sealed class LoginRequest
{
    public required string IdentityNumber { get; set; }
    public required string PasswordOrPersonalNumber { get; set; }
}
