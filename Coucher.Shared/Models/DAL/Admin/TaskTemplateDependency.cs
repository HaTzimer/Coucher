using Coucher.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Admin;

[Table(ConstantValues.TaskTemplateDependencyTableName)]
[Index(nameof(TaskTemplateId), nameof(DependsOnId), IsUnique = true)]
[Index(nameof(DependsOnId))]
public sealed class TaskTemplateDependency
{
    [Key]
    public Guid Id { get; set; }
    public Guid? TaskTemplateId { get; set; }
    public Guid? DependsOnId { get; set; }
    public DateTime CreationTime { get; set; }

    [ForeignKey(nameof(TaskTemplateId))]
    [InverseProperty("Dependencies")]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public TaskTemplate? TaskTemplate { get; set; }

    [ForeignKey(nameof(DependsOnId))]
    [InverseProperty("DependedOnByTemplates")]
    [DeleteBehavior(CommonConstantValues.DeleteBehaviorType)]
    public TaskTemplate? DependsOn { get; set; }
}
