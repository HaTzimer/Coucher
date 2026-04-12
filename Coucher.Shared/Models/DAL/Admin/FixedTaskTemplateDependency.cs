namespace Coucher.Shared.Models.DAL.Admin;

public sealed class FixedTaskTemplateDependency
{
    public Guid Id { get; set; }
    public Guid FixedTaskTemplateId { get; set; }
    public Guid DependsOnTemplateId { get; set; }
    public required FixedTaskTemplate FixedTaskTemplate { get; set; }
}
