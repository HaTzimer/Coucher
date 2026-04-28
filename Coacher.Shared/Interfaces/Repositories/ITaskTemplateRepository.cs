using Coacher.Shared.Models.DAL.Admin;

namespace Coacher.Shared.Interfaces.Repositories;

public interface ITaskTemplateRepository : IRepositoryBase<TaskTemplate, Guid>
{
    Task<int> GetNextSerialNumberAsync(CancellationToken cancellationToken = default);
    Task<TaskTemplate> CreateTaskTemplateAsync(TaskTemplate entity, CancellationToken cancellationToken = default);
    Task<List<TaskTemplate>> CreateTaskTemplatesAsync(
        List<TaskTemplate> entities,
        CancellationToken cancellationToken = default
    );
    Task<TaskTemplate> UpdateTaskTemplateAsync(TaskTemplate entity, CancellationToken cancellationToken = default);
    Task<List<TaskTemplateDependency>> CreateTaskTemplateDependenciesAsync(
        List<TaskTemplateDependency> entities,
        CancellationToken cancellationToken = default
    );
    Task<List<TaskTemplateInfluencer>> CreateTaskTemplateInfluencersAsync(
        Guid taskTemplateId,
        List<Guid> influencerIds,
        DateTime creationTime,
        CancellationToken cancellationToken = default
    );
    Task DeleteTaskTemplateDependenciesAsync(
        Guid taskTemplateId,
        List<Guid> dependencyIds,
        CancellationToken cancellationToken = default
    );
    Task DeleteTaskTemplateInfluencersAsync(
        Guid taskTemplateId,
        List<Guid> influencerLinkIds,
        CancellationToken cancellationToken = default
    );
    Task<TaskTemplate> ArchiveTaskTemplateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskTemplate> UnarchiveTaskTemplateAsync(Guid id, CancellationToken cancellationToken = default);
}
