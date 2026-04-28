namespace Coacher.Shared.Models.WebApi.Requests.Auth;

public sealed class ResetPasswordWithSecurityAnswersRequest
{
    public required string IdentityNumber { get; set; }
    public required List<SecurityQuestionAnswerRequest> SecurityQuestionAnswers { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmPassword { get; set; }
}
