using Coucher.Shared;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Admin;

[Table(ConstantValues.TaskTemplateTableName)]
[Index(nameof(ParentId))]
[Index(nameof(SeriesId))]
[Index(nameof(CategoryId))]
[GraphQLDescription("A reusable task blueprint that can be imported into exercises.")]
public sealed class TaskTemplate
{
    [GraphQLDescription("The unique identifier of the task template.")]
    public Guid Id { get; set; }

    [GraphQLDescription("The optional parent template id when this template is nested under another template.")]
    public Guid? ParentId { get; set; }

    [GraphQLDescription("The closed-list id that groups the template into a series.")]
    public Guid? SeriesId { get; set; }

    [GraphQLDescription("The closed-list id that categorizes the template.")]
    public Guid? CategoryId { get; set; }

    [GraphQLDescription("The serial number used to order the template.")]
    public int SerialNumber { get; set; }

    [GraphQLDescription("The template name.")]
    [MaxLength(256)]
    public required string Name { get; set; }

    [GraphQLDescription("An optional detailed description of the template.")]
    [MaxLength(1024)]
    public string? Description { get; set; }

    [GraphQLDescription("Optional internal notes for the template.")]
    [MaxLength(1024)]
    public string? Notes { get; set; }

    [GraphQLDescription("The default lead time, in weeks before exercise start, for planning this template.")]
    public int DefaultWeeksBeforeExerciseStart { get; set; }

    [GraphQLDescription("Whether the template is archived and hidden from normal active management flows.")]
    public bool IsArchive { get; set; }

    [GraphQLDescription("When the template was created.")]
    public DateTime CreationTime { get; set; }

    [GraphQLDescription("When the template was last updated.")]
    public DateTime LastUpdateTime { get; set; }

    [ForeignKey(nameof(ParentId))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The parent template when this template is part of a hierarchy.")]
    public TaskTemplate? Parent { get; set; }

    [ForeignKey(nameof(SeriesId))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The closed-list entry that represents the template series.")]
    public ClosedListItem? Series { get; set; }

    [ForeignKey(nameof(CategoryId))]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The closed-list entry that represents the template category.")]
    public ClosedListItem? Category { get; set; }

    [InverseProperty(nameof(Parent))]
    [GraphQLDescription("Child templates nested under this template.")]
    public required List<TaskTemplate> Children { get; set; }

    [InverseProperty(nameof(TaskTemplateDependency.Template))]
    [GraphQLDescription("Dependency links where this template depends on other templates.")]
    public required List<TaskTemplateDependency> Dependencies { get; set; }

    [InverseProperty(nameof(TaskTemplateDependency.DependsOn))]
    [GraphQLDescription("Dependency links where other templates depend on this template.")]
    public required List<TaskTemplateDependency> DependedOnBy { get; set; }

    [InverseProperty(nameof(TaskTemplateInfluencer.Template))]
    [GraphQLDescription("Influencer links attached to this template.")]
    public required List<TaskTemplateInfluencer> Influencers { get; set; }

}
