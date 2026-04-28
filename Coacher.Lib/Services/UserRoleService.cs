using Augustus.Infra.Core.Shared.Interfaces;
using Coacher.Shared.Interfaces.Repositories;
using Coacher.Shared.Interfaces.Services;
using Coacher.Shared.Models.DAL.Users;
using Coacher.Shared.Models.WebApi.Requests.Admin;

namespace Coacher.Lib.Services;

public sealed class UserRoleService : IUserRoleService
{
    private readonly IAugustusLogger _logger;
    private readonly IUserRoleRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICoacherAuthorizationService _authorizationService;

    public UserRoleService(
        IAugustusLogger logger,
        IUserRoleRepository repository,
        ICurrentUserService currentUserService,
        ICoacherAuthorizationService authorizationService
    )
    {
        _logger = logger;
        _repository = repository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<List<UserRole>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.ListAsync(cancellationToken);

        return items;
    }

    public async Task<UserRole?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<UserRole> GetRequiredByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetRequiredByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<UserRole> UpdateUserRoleAsync(
        Guid userRoleId,
        UpdateUserRoleRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(userRoleId, cancellationToken);
        entity.UserId = request.UserId;
        entity.Role = request.Role;
        entity.AssignedTime = DateTime.UtcNow;
        entity.AssignedByUserId = currentUserId;
        var updatedEntity = await _repository.UpdateUserRoleAsync(entity, cancellationToken);

        _logger.Info("User role updated", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "userRoleId", userRoleId },
            { "targetUserId", request.UserId },
            { "role", request.Role.ToString() },
            { "result", "success" }
        });

        return updatedEntity;
    }

    public async Task<UserRole> AddAsync(UserRole entity, CancellationToken cancellationToken = default)
    {
        var createdEntity = await _repository.AddAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<UserRole> UpdateAsync(UserRole entity, CancellationToken cancellationToken = default)
    {
        var updatedEntity = await _repository.UpdateAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
    }
}
