using Coucher.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Admin;

[Table(ConstantValues.TaskTemplateInfluencerTableName)]
[Index(nameof(TemplateId), nameof(InfluencerId), IsUnique = true)]
[Index(nameof(InfluencerId))]
public sealed class TaskTemplateInfluencer
{
    [Key]
    public Guid Id { get; set; }
    public Guid? TemplateId { get; set; }
    public Guid? InfluencerId { get; set; }
    public DateTime CreationTime { get; set; }

    [ForeignKey(nameof(TemplateId))]
    [InverseProperty("Influencers")]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public TaskTemplate? Template { get; set; }

    [ForeignKey(nameof(InfluencerId))]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public ClosedListItem? Influencer { get; set; }
}
