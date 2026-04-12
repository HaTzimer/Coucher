namespace Coucher.WebApi.Models.WebApi.Requests.Auth;

public sealed class SecurityQuestionAnswerRequestModel
{
    public required string QuestionId { get; set; }
    public required string Answer { get; set; }
}
