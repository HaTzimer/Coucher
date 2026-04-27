namespace Coucher.Shared.Models.WebApi.Requests.Auth;

public sealed class ForgotPasswordRequest
{
    public required string IdentityNumber { get; set; }
}
