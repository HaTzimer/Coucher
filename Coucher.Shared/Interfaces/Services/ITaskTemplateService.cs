using Coucher.Shared.Models.WebApi.Requests.Admin;
using Coucher.Shared.Models.DAL.Admin;

namespace Coucher.Shared.Interfaces.Services;

public interface ITaskTemplateService : IServiceBase<TaskTemplate, Guid>
{
    Task<TaskTemplate> CreateTaskTemplateAsync(
        CreateTaskTemplateRequestModel request,
        CancellationToken cancellationToken = default
    );
    Task<List<TaskTemplate>> CreateTaskTemplatesAsync(
        List<CreateTaskTemplateRequestModel> requests,
        CancellationToken cancellationToken = default
    );
    Task<TaskTemplate> UpdateTaskTemplateAsync(
        Guid taskTemplateId,
        UpdateTaskTemplateRequestModel request,
        CancellationToken cancellationToken = default
    );
}
