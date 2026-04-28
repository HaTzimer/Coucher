using Coacher.Shared.Interfaces.Repositories;
using Coacher.Shared.Interfaces.Services;
using Coacher.Shared.Models.DAL.Notifications;

namespace Coacher.Lib.Services;

public sealed class UserNotificationService : IUserNotificationService
{
    private readonly IUserNotificationRepository _repository;

    public UserNotificationService(IUserNotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<UserNotification>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.ListAsync(cancellationToken);

        return items;
    }

    public async Task<UserNotification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<UserNotification> GetRequiredByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetRequiredByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<UserNotification> AddAsync(UserNotification entity, CancellationToken cancellationToken = default)
    {
        var createdEntity = await _repository.AddAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<UserNotification> UpdateAsync(UserNotification entity, CancellationToken cancellationToken = default)
    {
        var updatedEntity = await _repository.UpdateAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
    }
}
