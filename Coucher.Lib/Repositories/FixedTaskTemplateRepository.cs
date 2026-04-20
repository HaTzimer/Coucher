using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Models.DAL.Admin;

namespace Coucher.Lib.Repositories;

public sealed class FixedTaskTemplateRepository : IFixedTaskTemplateRepository
{
    private readonly IFixedTaskTemplateProvider _provider;

    public FixedTaskTemplateRepository(IFixedTaskTemplateProvider provider)
    {
        _provider = provider;
    }

    public async Task<List<FixedTaskTemplate>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _provider.ListAsync(cancellationToken);

        return items;
    }

    public async Task<FixedTaskTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _provider.GetByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<FixedTaskTemplate> GetRequiredByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _provider.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            throw new KeyNotFoundException($"{nameof(FixedTaskTemplate)} '{id}' was not found.");
        }

        return entity;
    }

    public async Task<FixedTaskTemplate> AddAsync(FixedTaskTemplate entity, CancellationToken cancellationToken = default)
    {
        var createdEntity = await _provider.AddAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<FixedTaskTemplate> UpdateAsync(FixedTaskTemplate entity, CancellationToken cancellationToken = default)
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
