using Coucher.Shared.Models.DAL.Admin;

namespace Coucher.Shared.Interfaces.Repositories;

public interface ITaskTemplateRepository : IRepositoryBase<TaskTemplate, Guid>
{
    Task<int> GetNextSerialNumberAsync(CancellationToken cancellationToken = default);
    Task<TaskTemplate> CreateTaskTemplateAsync(TaskTemplate entity, CancellationToken cancellationToken = default);
    Task<List<TaskTemplate>> CreateTaskTemplatesAsync(
        List<TaskTemplate> entities,
        CancellationToken cancellationToken = default
    );
    Task<TaskTemplate> UpdateTaskTemplateAsync(TaskTemplate entity, CancellationToken cancellationToken = default);
    Task<TaskTemplateDependency> CreateTaskTemplateDependencyAsync(
        TaskTemplateDependency entity,
        CancellationToken cancellationToken = default
    );
    Task<List<TaskTemplateInfluencer>> CreateTaskTemplateInfluencersAsync(
        Guid taskTemplateId,
        List<Guid> influencerIds,
        DateTime creationTime,
        CancellationToken cancellationToken = default
    );
    Task<List<TaskTemplateInfluencer>> ReplaceTaskTemplateInfluencersAsync(
        Guid taskTemplateId,
        List<Guid> influencerIds,
        DateTime creationTime,
        CancellationToken cancellationToken = default
    );
    Task DeleteTaskTemplateDependencyAsync(Guid dependencyId, CancellationToken cancellationToken = default);
    Task DeleteTaskTemplateInfluencerAsync(Guid influencerLinkId, CancellationToken cancellationToken = default);
    Task<TaskTemplate> ArchiveTaskTemplateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskTemplate> UnarchiveTaskTemplateAsync(Guid id, CancellationToken cancellationToken = default);
}
