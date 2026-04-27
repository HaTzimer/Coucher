namespace Coucher.Shared.Models.WebApi.Requests.Auth;

public sealed class ResetPasswordWithSecurityAnswersRequestModel
{
    public required string IdentityNumber { get; set; }
    public required List<SecurityQuestionAnswerRequestModel> SecurityQuestionAnswers { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmPassword { get; set; }
}

