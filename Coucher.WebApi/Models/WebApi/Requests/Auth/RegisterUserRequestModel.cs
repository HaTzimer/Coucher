namespace Coucher.WebApi.Models.WebApi.Requests.Auth;

public sealed class RegisterUserRequestModel
{
    public required string IdentityNumber { get; set; }
    public string? PersonalNumber { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PhoneNumber { get; set; }
    public required string CivilianEmail { get; set; }
    public string? MilitaryEmail { get; set; }
}
