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
    Task<TaskTemplate> UpdateTaskTemplateAsync(
        Guid taskTemplateId,
        UpdateTaskTemplateRequest request,
        CancellationToken cancellationToken = default
    );
    Task<TaskTemplate> UpdateTaskTemplateSeriesAsync(
        Guid taskTemplateId,
        Guid? seriesId,
        CancellationToken cancellationToken = default
    );
    Task<TaskTemplate> UpdateTaskTemplateCategoryAsync(
        Guid taskTemplateId,
        Guid? categoryId,
        CancellationToken cancellationToken = default
    );
    Task<TaskTemplate> UpdateTaskTemplateDefaultWeeksBeforeExerciseStartAsync(
        Guid taskTemplateId,
        int defaultWeeksBeforeExerciseStart,
        CancellationToken cancellationToken = default
    );
    Task<TaskTemplate> UpdateTaskTemplateDetailsAsync(
        Guid taskTemplateId,
        UpdateTaskTemplateDetailsRequest request,
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
    Task<TaskTemplate> ArchiveTaskTemplateAsync(Guid taskTemplateId, CancellationToken cancellationToken = default);
    Task<TaskTemplate> UnarchiveTaskTemplateAsync(
        Guid taskTemplateId,
        CancellationToken cancellationToken = default
    );
}
