using Coucher.Shared;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Admin;

[Table(ConstantValues.TaskTemplateDependencyTableName)]
[Index(nameof(TemplateId), nameof(DependsOnId), IsUnique = true)]
[Index(nameof(DependsOnId))]
[GraphQLDescription("A dependency link between two task templates.")]
public sealed class TaskTemplateDependency
{
    [Key]
    [GraphQLDescription("The unique identifier of the dependency link.")]
    public Guid Id { get; set; }

    [GraphQLDescription("The template that depends on another template.")]
    public Guid? TemplateId { get; set; }

    [GraphQLDescription("The template that should be completed first.")]
    public Guid? DependsOnId { get; set; }

    [GraphQLDescription("When the dependency link was created.")]
    public DateTime CreationTime { get; set; }

    [ForeignKey(nameof(TemplateId))]
    [InverseProperty("Dependencies")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The template that depends on another template.")]
    public TaskTemplate? Template { get; set; }

    [ForeignKey(nameof(DependsOnId))]
    [InverseProperty("DependedOnBy")]
    [DeleteBehavior(ConstantValues.DeleteBehaviorType)]
    [GraphQLDescription("The template that must be completed first.")]
    public TaskTemplate? DependsOn { get; set; }

}
