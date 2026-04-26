using Coucher.Shared.Models.DAL.Admin;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Exercises;

public sealed class ExerciseThreatArenaLink
{
    [Key]
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public Guid ThreatArenaClosedListItemId { get; set; }

    [ForeignKey(nameof(ExerciseId))]
    [InverseProperty("ThreatArenas")]
    public required Exercise Exercise { get; set; }

    [ForeignKey(nameof(ThreatArenaClosedListItemId))]
    public required ClosedListItem ThreatArenaClosedListItem { get; set; }
}
