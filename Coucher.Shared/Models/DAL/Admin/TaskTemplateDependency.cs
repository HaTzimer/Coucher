using Coucher.Shared;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Admin;

[Table(ConstantValues.TaskTemplateDependencyTableName)]
[Index(nameof(TemplateId), nameof(DependsOnId), IsUnique = true)]
[Index(nameof(DependsOnId))]
public sealed class TaskTemplateDependency
{
    [Key]
    public Guid Id { get; set; }
    public Guid? TemplateId { get; set; }
    public Guid? DependsOnId { get; set; }
    public DateTime CreationTime { get; set; }

    [ForeignKey(nameof(TemplateId))]
    [InverseProperty("Dependencies")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    public TaskTemplate? Template { get; set; }

    [ForeignKey(nameof(DependsOnId))]
    [InverseProperty("DependedOnBy")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    public TaskTemplate? DependsOn { get; set; }
}
