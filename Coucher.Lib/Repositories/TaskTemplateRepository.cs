using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Models.DAL.Admin;

namespace Coucher.Lib.Repositories;

public sealed class TaskTemplateRepository : ITaskTemplateRepository
{
    private readonly ITaskTemplateProvider _provider;

    public TaskTemplateRepository(ITaskTemplateProvider provider)
    {
        _provider = provider;
    }

    public async Task<List<TaskTemplate>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _provider.ListAsync(cancellationToken);

        return items;
    }

    public async Task<TaskTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _provider.GetByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<TaskTemplate> GetRequiredByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _provider.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            throw new KeyNotFoundException($"{nameof(TaskTemplate)} '{id}' was not found.");
        }

        return entity;
    }

    public async Task<TaskTemplate> AddAsync(TaskTemplate entity, CancellationToken cancellationToken = default)
    {
        var createdEntity = await _provider.AddAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<TaskTemplate> UpdateAsync(TaskTemplate entity, CancellationToken cancellationToken = default)
    {
        var updatedEntity = await _provider.UpdateAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetRequiredByIdAsync(id, cancellationToken);
        await _provider.DeleteAsync(entity, cancellationToken);
    }
}
