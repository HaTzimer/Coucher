using Coucher.Shared;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Admin;

[Table(ConstantValues.TaskTemplateInfluencerTableName)]
[Index(nameof(TemplateId), nameof(InfluencerId), IsUnique = true)]
[Index(nameof(InfluencerId))]
[GraphQLDescription("A link between a task template and an influencer.")]
public sealed class TaskTemplateInfluencer
{
    [Key]
    [GraphQLDescription("The unique identifier of the template-to-influencer link.")]
    public Guid Id { get; set; }
    [GraphQLDescription("The task template linked to the influencer.")]
    public Guid? TemplateId { get; set; }
    [GraphQLDescription("The closed-list influencer id linked to the template.")]
    public Guid? InfluencerId { get; set; }
    [GraphQLDescription("When the link was created.")]
    public DateTime CreationTime { get; set; }

    [ForeignKey(nameof(TemplateId))]
    [InverseProperty("Influencers")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The task template linked to the influencer.")]
    public TaskTemplate? Template { get; set; }

    [ForeignKey(nameof(InfluencerId))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The closed-list entry that represents the influencer.")]
    public ClosedListItem? Influencer { get; set; }
}
