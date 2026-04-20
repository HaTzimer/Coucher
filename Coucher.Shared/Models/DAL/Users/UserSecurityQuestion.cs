using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Users;

public sealed class UserSecurityQuestion
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string QuestionId { get; set; }
    public required string AnswerHash { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty("SecurityQuestions")]
    public required UserProfile User { get; set; }
}
