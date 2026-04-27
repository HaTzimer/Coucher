namespace Coucher.Shared.Models.WebApi.Requests.Auth;

public sealed class LoginRequestModel
{
    public required string IdentityNumber { get; set; }
    public required string PasswordOrPersonalNumber { get; set; }
}

