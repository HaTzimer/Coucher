using Coucher.Shared.Models.WebApi.Requests.Admin;
using Coucher.Shared.Models.DAL.Admin;

namespace Coucher.Shared.Interfaces.Services;

public interface ITaskTemplateService : IServiceBase<TaskTemplate, Guid>
{
    Task<TaskTemplate> CreateTaskTemplateAsync(
        CreateTaskTemplateRequest request,
        CancellationToken cancellationToken = default
    );
    Task<List<TaskTemplate>> CreateTaskTemplatesAsync(
        List<CreateTaskTemplateRequest> requests,
        CancellationToken cancellationToken = default
    );
    Task<TaskTemplate> UpdateTaskTemplateSeriesAsync(
        Guid taskTemplateId,
        UpdateTaskTemplateSeriesRequest request,
        CancellationToken cancellationToken = default
    );
    Task<TaskTemplate> UpdateTaskTemplateCategoryAsync(
        Guid taskTemplateId,
        UpdateTaskTemplateCategoryRequest request,
        CancellationToken cancellationToken = default
    );
    Task<TaskTemplate> UpdateTaskTemplateDetailsAsync(
        Guid taskTemplateId,
        UpdateTaskTemplateDetailsRequest request,
        CancellationToken cancellationToken = default
    );
    Task<TaskTemplate> AddTaskTemplateChildAsync(
        Guid taskTemplateId,
        CreateTaskTemplateRequest request,
        CancellationToken cancellationToken = default
    );
    Task<TaskTemplateDependency> AddTaskTemplateDependencyAsync(
        Guid taskTemplateId,
        AddTaskTemplateDependencyRequest request,
        CancellationToken cancellationToken = default
    );
    Task<TaskTemplate> ArchiveTaskTemplateAsync(Guid taskTemplateId, CancellationToken cancellationToken = default);
    Task<TaskTemplate> UnarchiveTaskTemplateAsync(
        Guid taskTemplateId,
        CancellationToken cancellationToken = default
    );
}
