namespace Coucher.Shared.Models.WebApi.Requests.Admin;

public sealed class CreateTaskTemplateRequest : TaskTemplateNodeRequestBase
{
    public List<CreateTaskTemplateChildRequest>? Children { get; set; }
}
