using Coucher.Shared.Models.Enums;
using Coucher.Shared.Models.DAL.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Exercises;

public sealed class ExerciseParticipant
{
    [Key]
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public Guid? UserId { get; set; }
    public string? PersonalNumber { get; set; }
    public required string FullName { get; set; }
    public string? Rank { get; set; }
    public string? Position { get; set; }
    public string? PhoneNumber { get; set; }
    public ExerciseRole Role { get; set; }
    public bool IsExerciseCreator { get; set; }
    public bool IsActive { get; set; }
    public DateTime AddedAt { get; set; }
    public DateTime? RemovedAt { get; set; }
    public string? Notes { get; set; }

    [ForeignKey(nameof(ExerciseId))]
    [InverseProperty("Participants")]
    public required Exercise Exercise { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty("ExerciseParticipants")]
    public UserProfile? User { get; set; }
}
