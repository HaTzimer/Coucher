namespace Coucher.Shared.Interfaces.Services;

public interface IServiceBase<TEntity, in TId>
    where TEntity : class
    where TId : notnull
{
    Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<TEntity> GetRequiredByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TId id, CancellationToken cancellationToken = default);
}
