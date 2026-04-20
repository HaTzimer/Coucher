using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coucher.Shared.Models.DAL.Admin;

public sealed class FixedTaskTemplateDependency
{
    [Key]
    public Guid Id { get; set; }
    public Guid FixedTaskTemplateId { get; set; }
    public Guid DependsOnTemplateId { get; set; }

    [ForeignKey(nameof(FixedTaskTemplateId))]
    [InverseProperty("Dependencies")]
    public required FixedTaskTemplate FixedTaskTemplate { get; set; }
}
