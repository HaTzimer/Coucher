using Coucher.Shared.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Exercises;

public sealed class ExerciseThreatArenaLink
{
    [Key]
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public ThreatArenaCode ThreatArenaCode { get; set; }

    [ForeignKey(nameof(ExerciseId))]
    [InverseProperty("ThreatArenas")]
    public required Exercise Exercise { get; set; }
}
