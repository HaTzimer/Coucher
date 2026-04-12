namespace Coucher.Shared.Models.DAL.Users;

public sealed class UserSecurityQuestion
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string QuestionId { get; set; }
    public required string AnswerHash { get; set; }
    public required UserProfile User { get; set; }
}
