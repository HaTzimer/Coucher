using Coucher.Shared.Models.DAL.Admin;

namespace Coucher.Shared.Interfaces.DAL.Providers;

public interface ITaskTemplateProvider : IProviderBase<TaskTemplate, Guid>
{
    Task<int> GetNextSerialNumberAsync(CancellationToken cancellationToken = default);
    Task<TaskTemplate> CreateTaskTemplateAsync(TaskTemplate entity, CancellationToken cancellationToken = default);
    Task<List<TaskTemplate>> CreateTaskTemplatesAsync(
        List<TaskTemplate> entities,
        CancellationToken cancellationToken = default
    );
    Task<TaskTemplate> UpdateTaskTemplateAsync(TaskTemplate entity, CancellationToken cancellationToken = default);
}
