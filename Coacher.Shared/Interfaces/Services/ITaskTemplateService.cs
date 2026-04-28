using Coacher.Shared.Models.WebApi.Requests.Admin;
using Coacher.Shared.Models.DAL.Admin;

namespace Coacher.Shared.Interfaces.Services;

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
    Task<TaskTemplate> UpdateTaskTemplateAsync(
        Guid taskTemplateId,
        UpdateTaskTemplateRequest request,
        CancellationToken cancellationToken = default
    );
    Task<TaskTemplate> AddTaskTemplateChildAsync(
        Guid taskTemplateId,
        CreateTaskTemplateChildRequest request,
        CancellationToken cancellationToken = default
    );
    Task<TaskTemplateDependency> AddTaskTemplateDependencyAsync(
        Guid taskTemplateId,
        Guid dependsOnId,
        CancellationToken cancellationToken = default
    );
    Task<List<TaskTemplateInfluencer>> AddTaskTemplateInfluencersAsync(
        Guid taskTemplateId,
        List<Guid> influencerIds,
        CancellationToken cancellationToken = default
    );
    Task DeleteTaskTemplateDependencyAsync(Guid dependencyId, CancellationToken cancellationToken = default);
    Task DeleteTaskTemplateInfluencerAsync(Guid influencerLinkId, CancellationToken cancellationToken = default);
    Task<TaskTemplate> SetTaskTemplateArchiveStateAsync(
        Guid taskTemplateId,
        bool isArchived,
        CancellationToken cancellationToken = default
    );
}
