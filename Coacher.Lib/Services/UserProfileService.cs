using Augustus.Infra.Core.Shared.Interfaces;
using Coacher.Shared;
using Coacher.Shared.Models.Enums;
using Coacher.Shared.Interfaces.Repositories;
using Coacher.Shared.Interfaces.Services;
using Coacher.Shared.Models.DAL.Users;

namespace Coacher.Lib.Services;

public sealed class UserProfileService : IUserProfileService
{
    private readonly IAugustusLogger _logger;
    private readonly IUserProfileRepository _repository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly GlobalRole _starterGlobalRole;

    public UserProfileService(
        IAugustusLogger logger,
        IUserProfileRepository repository,
        IUserRoleRepository userRoleRepository,
        IAugustusConfiguration config
    )
    {
        _logger = logger;
        _repository = repository;
        _userRoleRepository = userRoleRepository;
        _starterGlobalRole = config.GetOrThrow<GlobalRole>(
            ConfigurationKeys.UserDefaultsSection,
            ConfigurationKeys.StarterGlobalRole
        );
    }

    public async Task<List<UserProfile>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.ListAsync(cancellationToken);

        return items;
    }

    public async Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<UserProfile> GetRequiredByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetRequiredByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<UserProfile> AddAsync(UserProfile entity, CancellationToken cancellationToken = default)
    {
        var createdEntity = await _repository.AddAsync(entity, cancellationToken);
        var existingRole = await _userRoleRepository.GetByUserIdAndRoleAsync(
            createdEntity.Id,
            _starterGlobalRole,
            cancellationToken
        );
        if (existingRole is null)
        {
            await _userRoleRepository.CreateUserRoleAsync(
                new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = createdEntity.Id,
                    Role = _starterGlobalRole,
                    AssignedTime = DateTime.UtcNow,
                    AssignedByUserId = createdEntity.Id
                },
                cancellationToken
            );

            _logger.Info("User profile starter role created", new Dictionary<string, object>
            {
                { "userId", createdEntity.Id },
                { "role", _starterGlobalRole.ToString() }
            });
        }

        return createdEntity;
    }

    public async Task<UserProfile> UpdateAsync(UserProfile entity, CancellationToken cancellationToken = default)
    {
        var updatedEntity = await _repository.UpdateAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
    }
}
