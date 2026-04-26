using Coucher.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Admin;

[Table(ConstantValues.TaskTemplateTableName)]
[Index(nameof(ParentTaskTemplateId))]
[Index(nameof(SeriesId))]
[Index(nameof(CategoryId))]
public sealed class TaskTemplate
{
    public Guid Id { get; set; }
    public Guid? ParentTaskTemplateId { get; set; }
    public Guid? SeriesId { get; set; }
    public Guid? CategoryId { get; set; }
    public int SerialNumber { get; set; }
    [MaxLength(256)]
    public required string Name { get; set; }
    [MaxLength(1024)]
    public required string Description { get; set; }
    [MaxLength(1024)]
    public string? Notes { get; set; }//is needed
    public int DefaultWeeksBeforeExerciseStart { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime LastUpdateTime { get; set; }

    [ForeignKey(nameof(ParentTaskTemplateId))]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public TaskTemplate? ParentTaskTemplate { get; set; }

    [ForeignKey(nameof(SeriesId))]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public ClosedListItem? Series { get; set; }

    [ForeignKey(nameof(CategoryId))]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public ClosedListItem? Category { get; set; }

    [InverseProperty(nameof(ParentTaskTemplate))]
    public required List<TaskTemplate> SubTaskTemplates { get; set; }

    [InverseProperty(nameof(TaskTemplateDependency.TaskTemplate))]
    public required List<TaskTemplateDependency> Dependencies { get; set; }

    [InverseProperty(nameof(TaskTemplateDependency.DependsOn))]
    public required List<TaskTemplateDependency> DependedOnByTemplates { get; set; }

    [InverseProperty(nameof(TaskTemplateInfluencer.TaskTemplate))]
    public required List<TaskTemplateInfluencer> Influencers { get; set; }
}
