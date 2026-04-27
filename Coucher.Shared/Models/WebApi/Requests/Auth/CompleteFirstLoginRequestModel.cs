namespace Coucher.Shared.Models.WebApi.Requests.Auth;

public sealed class CompleteFirstLoginRequestModel
{
    public Guid UserId { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    public required List<SecurityQuestionAnswerRequestModel> SecurityQuestionAnswers { get; set; }
}

