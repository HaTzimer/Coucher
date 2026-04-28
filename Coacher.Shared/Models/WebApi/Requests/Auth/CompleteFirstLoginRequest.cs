namespace Coacher.Shared.Models.WebApi.Requests.Auth;

public sealed class CompleteFirstLoginRequest
{
    public Guid UserId { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    public required List<SecurityQuestionAnswerRequest> SecurityQuestionAnswers { get; set; }
}
