using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Admin;

public sealed class FixedTaskTemplateDependency
{
    [Key]
    public Guid Id { get; set; }
    public Guid FixedTaskTemplateId { get; set; }
    public Guid DependsOnTemplateId { get; set; }
    public bool IsBlocking { get; set; }
    public string? Notes { get; set; }

    [ForeignKey(nameof(FixedTaskTemplateId))]
    [InverseProperty("Dependencies")]
    public required FixedTaskTemplate FixedTaskTemplate { get; set; }

    [ForeignKey(nameof(DependsOnTemplateId))]
    [InverseProperty("DependedOnByTemplates")]
    public required FixedTaskTemplate DependsOnTemplate { get; set; }
}
