using System.Net;
using Augustus.Infra.Core.Shared.Exceptions;
using Augustus.Infra.Core.Shared.Interfaces;
using Coucher.Shared.Interfaces.DAL.Providers;
using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Models.DAL.Notifications;

namespace Coucher.Lib.Repositories;

public sealed class UserNotificationRepository : IUserNotificationRepository
{
    private readonly IAugustusLogger _logger;
    private readonly IUserNotificationProvider _provider;

    public UserNotificationRepository(IAugustusLogger logger, IUserNotificationProvider provider)
    {
        _logger = logger;
        _provider = provider;
    }

    public async Task<List<UserNotification>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _provider.ListAsync(cancellationToken);

        return items;
    }

    public async Task<UserNotification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _provider.GetByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<UserNotification> GetRequiredByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _provider.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            var exception = new HttpStatusCodeException(
                $"{nameof(UserNotification)} '{id}' was not found.",
                new Dictionary<string, object?>
                {
                    { "resourceName", nameof(UserNotification) },
                    { "resourceId", id }
                },
                HttpStatusCode.NotFound
            );

            _logger.Error(exception);

            throw exception;
        }

        return entity;
    }

    public async Task<UserNotification> AddAsync(UserNotification entity, CancellationToken cancellationToken = default)
    {
        var createdEntity = await _provider.AddAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<UserNotification> UpdateAsync(UserNotification entity, CancellationToken cancellationToken = default)
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

