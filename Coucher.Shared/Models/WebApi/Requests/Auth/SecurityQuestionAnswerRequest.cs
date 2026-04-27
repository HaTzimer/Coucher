namespace Coucher.Shared.Models.WebApi.Requests.Auth;

public sealed class SecurityQuestionAnswerRequest
{
    public required string QuestionId { get; set; }
    public required string Answer { get; set; }
}
