namespace Coucher.Shared.Models.WebApi.Requests.Admin;

public sealed class UpdateTaskTemplateDetailsRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
}
