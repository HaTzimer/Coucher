namespace Coucher.Shared.Models.WebApi.Requests.Admin;

public sealed class AddTaskTemplateDependencyRequest
{
    public Guid DependsOnId { get; set; }
}
