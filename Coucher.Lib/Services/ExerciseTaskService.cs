using Augustus.Infra.Core.Shared.Interfaces;
using Coucher.Shared;
using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.WebApi.Requests.Tasks;
using Coucher.Shared.Models.DAL.Tasks;

namespace Coucher.Lib.Services;

public sealed class ExerciseTaskService : IExerciseTaskService
{
    private readonly IAugustusLogger _logger;
    private readonly IExerciseTaskRepository _repository;
    private readonly IClosedListItemRepository _closedListItemRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICoucherAuthorizationService _authorizationService;
    private readonly Guid _defaultExerciseTaskStatusId;

    public ExerciseTaskService(
        IAugustusLogger logger,
        IExerciseTaskRepository repository,
        IClosedListItemRepository closedListItemRepository,
        ICurrentUserService currentUserService,
        ICoucherAuthorizationService authorizationService,
        IAugustusConfiguration config
    )
    {
        _logger = logger;
        _repository = repository;
        _closedListItemRepository = closedListItemRepository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
        _defaultExerciseTaskStatusId = config.GetOrThrow<Guid>(
            ConfigurationKeys.TaskDefaultsSection,
            ConfigurationKeys.DefaultExerciseTaskStatusId
        );
    }

    public async Task<List<ExerciseTask>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.ListAsync(cancellationToken);

        return items;
    }

    public async Task<ExerciseTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<ExerciseTask> GetRequiredByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetRequiredByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<ExerciseTask> CreateExerciseTaskAsync(
        CreateExerciseTaskRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanCreateExerciseTaskAsync(request.ExerciseId, cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var nextSerialNumber = await _repository.GetNextSerialNumberAsync(request.ExerciseId, cancellationToken);
        var isCompletedStatus = await IsCompletedStatusAsync(_defaultExerciseTaskStatusId, cancellationToken);
        var entity = CreateExerciseTaskEntity(request, nextSerialNumber, now, isCompletedStatus);

        AttachResponsibleUsers(entity, request.ResponsibleUserIds, now);
        await AttachExistingDependenciesAsync(entity, request.DependsOnTaskIds, now, cancellationToken);

        if (request.DependsOnTaskKeys is { Count: > 0 })
            throw new InvalidOperationException("DependsOnTaskKeys are only supported in bulk create.");

        var createdEntity = await _repository.CreateExerciseTaskAsync(entity, cancellationToken);

        _logger.Info("Exercise task created", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", request.ExerciseId },
            { "taskId", createdEntity.Id },
            { "responsibleUserCount", createdEntity.ResponsibleUsers.Count },
            { "dependencyCount", createdEntity.Dependencies.Count },
            { "result", "success" }
        });

        return createdEntity;
    }

    public async Task<ExerciseTask> CreateExerciseTaskChildAsync(
        Guid parentTaskId,
        CreateExerciseTaskChildRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanFullyEditExerciseTaskAsync(parentTaskId, cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var parentTask = await _repository.GetRequiredByIdAsync(parentTaskId, cancellationToken);

        if (parentTask.ParentId.HasValue)
            throw new InvalidOperationException("Exercise tasks support children only one level deep.");

        var now = DateTime.UtcNow;
        var nextSerialNumber = await _repository.GetNextSerialNumberAsync(parentTask.ExerciseId, cancellationToken);
        var isCompletedStatus = await IsCompletedStatusAsync(_defaultExerciseTaskStatusId, cancellationToken);
        var entity = new ExerciseTask
        {
            Id = Guid.NewGuid(),
            ExerciseId = parentTask.ExerciseId,
            ParentId = parentTaskId,
            SourceId = null,
            SeriesId = request.SeriesId,
            CategoryId = request.CategoryId,
            StatusId = _defaultExerciseTaskStatusId,
            SerialNumber = nextSerialNumber,
            Name = request.Name,
            Description = request.Description,
            Notes = request.Notes,
            DueDate = request.DueDate,
            LastStatusUpdaterId = null,
            CreationTime = now,
            LastUpdateTime = now,
            LastStatusUpdateTime = null,
            CompletionTime = isCompletedStatus ? now : null,
            HasChildren = false,
            Children = new List<ExerciseTask>(),
            ResponsibleUsers = new List<ExerciseTaskResponsibleUser>(),
            Dependencies = new List<TaskDependency>(),
            DependedOnBy = new List<TaskDependency>()
        };

        AttachResponsibleUsers(entity, request.ResponsibleUserIds, now);
        await AttachExistingDependenciesAsync(entity, request.DependsOnTaskIds, now, cancellationToken);

        var createdEntity = await _repository.CreateExerciseTaskAsync(entity, cancellationToken);
        if (!parentTask.HasChildren)
            await _repository.SetExerciseTaskHasChildrenAsync(parentTaskId, true, cancellationToken);

        _logger.Info("Exercise task child created", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", parentTask.ExerciseId },
            { "taskId", createdEntity.Id },
            { "parentTaskId", parentTaskId },
            { "responsibleUserCount", createdEntity.ResponsibleUsers.Count },
            { "dependencyCount", createdEntity.Dependencies.Count },
            { "result", "success" }
        });

        return createdEntity;
    }

    public async Task<List<ExerciseTask>> CreateExerciseTasksAsync(
        List<CreateExerciseTaskRequest> requests,
        CancellationToken cancellationToken = default
    )
    {
        if (requests.Count == 0)
            return new List<ExerciseTask>();

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);

        foreach (var exerciseId in requests.Select(item => item.ExerciseId).Distinct())
        {
            await _authorizationService.EnsureCanCreateExerciseTaskAsync(exerciseId, cancellationToken);
        }

        var groupedRequests = requests.GroupBy(request => request.ExerciseId).ToList();
        var nextSerialNumbers = new Dictionary<Guid, int>();
        foreach (var group in groupedRequests)
        {
            var nextSerialNumber = await _repository.GetNextSerialNumberAsync(group.Key, cancellationToken);
            nextSerialNumbers[group.Key] = nextSerialNumber;
        }

        var now = DateTime.UtcNow;
        var entities = new List<ExerciseTask>(capacity: requests.Count);
        var entitiesByTaskKey = new Dictionary<string, ExerciseTask>(StringComparer.Ordinal);

        _logger.Info("Exercise task bulk create started", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "count", requests.Count },
            { "exerciseCount", groupedRequests.Count },
            { "result", "started" }
        });

        foreach (var request in requests)
        {
            var serialNumber = nextSerialNumbers[request.ExerciseId];
            nextSerialNumbers[request.ExerciseId] = serialNumber + 1;
            var isCompletedStatus = await IsCompletedStatusAsync(_defaultExerciseTaskStatusId, cancellationToken);
            var entity = CreateExerciseTaskEntity(request, serialNumber, now, isCompletedStatus);
            entities.Add(entity);
            RegisterTaskKey(request.TaskKey, entity, entitiesByTaskKey);
        }

        for (var i = 0; i < requests.Count; i++)
        {
            var request = requests[i];
            var entity = entities[i];
            AttachResponsibleUsers(entity, request.ResponsibleUserIds, now);
            await AttachExistingDependenciesAsync(entity, request.DependsOnTaskIds, now, cancellationToken);
            AttachBulkDependencies(entity, request.DependsOnTaskKeys, entitiesByTaskKey, now);
        }

        var createdEntities = await _repository.CreateExerciseTasksAsync(entities, cancellationToken);

        _logger.Info("Exercise task bulk create completed", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "count", createdEntities.Count },
            { "exerciseCount", groupedRequests.Count },
            { "result", "success" }
        });

        return createdEntities;
    }

    public async Task<ExerciseTask> UpdateExerciseTaskAsync(
        Guid taskId,
        UpdateExerciseTaskRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanFullyEditExerciseTaskAsync(taskId, cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(taskId, cancellationToken);
        var now = DateTime.UtcNow;

        entity.SeriesId = request.SeriesId;
        entity.CategoryId = request.CategoryId;
        entity.StatusId = request.StatusId;
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Notes = request.Notes;
        entity.DueDate = request.DueDate;
        entity.LastUpdateTime = now;
        entity.LastStatusUpdaterId = currentUserId;
        entity.LastStatusUpdateTime = now;
        entity.CompletionTime = await IsCompletedStatusAsync(request.StatusId, cancellationToken)
            ? now
            : null;
        var updatedEntity = await _repository.UpdateExerciseTaskAsync(entity, cancellationToken);

        _logger.Info("Exercise task updated", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", entity.ExerciseId },
            { "taskId", taskId },
            { "changedFields", new[] { "SeriesId", "CategoryId", "StatusId", "Name", "Description", "Notes", "DueDate" } },
            { "statusId", request.StatusId },
            { "result", "success" }
        });

        return updatedEntity;
    }

    public async Task<ExerciseTask> UpdateExerciseTaskDueDateAsync(
        Guid taskId,
        DateTime dueDate,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanPartiallyEditExerciseTaskAsync(taskId, cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(taskId, cancellationToken);
        entity.DueDate = dueDate;
        entity.LastUpdateTime = DateTime.UtcNow;
        var updatedEntity = await _repository.UpdateExerciseTaskAsync(entity, cancellationToken);

        _logger.Info("Exercise task due date updated", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", entity.ExerciseId },
            { "taskId", taskId },
            { "changedFields", new[] { "DueDate" } },
            { "result", "success" }
        });

        return updatedEntity;
    }

    public async Task<ExerciseTask> UpdateExerciseTaskSeriesAsync(
        Guid taskId,
        Guid seriesId,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanFullyEditExerciseTaskAsync(taskId, cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(taskId, cancellationToken);
        entity.SeriesId = seriesId;
        entity.LastUpdateTime = DateTime.UtcNow;
        var updatedEntity = await _repository.UpdateExerciseTaskAsync(entity, cancellationToken);

        _logger.Info("Exercise task series updated", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", entity.ExerciseId },
            { "taskId", taskId },
            { "changedFields", new[] { "SeriesId" } },
            { "seriesId", seriesId },
            { "result", "success" }
        });

        return updatedEntity;
    }

    public async Task<ExerciseTask> UpdateExerciseTaskCategoryAsync(
        Guid taskId,
        Guid categoryId,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanFullyEditExerciseTaskAsync(taskId, cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(taskId, cancellationToken);
        entity.CategoryId = categoryId;
        entity.LastUpdateTime = DateTime.UtcNow;
        var updatedEntity = await _repository.UpdateExerciseTaskAsync(entity, cancellationToken);

        _logger.Info("Exercise task category updated", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", entity.ExerciseId },
            { "taskId", taskId },
            { "changedFields", new[] { "CategoryId" } },
            { "categoryId", categoryId },
            { "result", "success" }
        });

        return updatedEntity;
    }

    public async Task<ExerciseTask> UpdateExerciseTaskStatusAsync(
        Guid taskId,
        Guid statusId,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanPartiallyEditExerciseTaskAsync(taskId, cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(taskId, cancellationToken);
        var now = DateTime.UtcNow;

        entity.StatusId = statusId;
        entity.LastUpdateTime = now;
        entity.LastStatusUpdaterId = currentUserId;
        entity.LastStatusUpdateTime = now;
        entity.CompletionTime = await IsCompletedStatusAsync(statusId, cancellationToken)
            ? now
            : null;
        var updatedEntity = await _repository.UpdateExerciseTaskAsync(entity, cancellationToken);

        _logger.Info("Exercise task status updated", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", entity.ExerciseId },
            { "taskId", taskId },
            { "changedFields", new[] { "StatusId" } },
            { "statusId", statusId },
            { "result", "success" }
        });

        return updatedEntity;
    }

    public async Task<ExerciseTask> UpdateExerciseTaskDetailsAsync(
        Guid taskId,
        UpdateExerciseTaskDetailsRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanFullyEditExerciseTaskAsync(taskId, cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(taskId, cancellationToken);
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Notes = request.Notes;
        entity.LastUpdateTime = DateTime.UtcNow;

        var updatedEntity = await _repository.UpdateExerciseTaskAsync(entity, cancellationToken);

        _logger.Info("Exercise task details updated", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", entity.ExerciseId },
            { "taskId", taskId },
            { "changedFields", new[] { "Name", "Description", "Notes" } },
            { "result", "success" }
        });

        return updatedEntity;
    }

    public async Task<List<TaskDependency>> AddExerciseTaskDependenciesAsync(
        Guid taskId,
        List<string> dependsOnIds,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanFullyEditExerciseTaskAsync(taskId, cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var parsedDependsOnIds = dependsOnIds.Select(item => ParseGuidString(item, "DependsOnId")).Distinct().ToList();

        if (parsedDependsOnIds.Count == 0)
            return new List<TaskDependency>();

        var task = await _repository.GetRequiredByIdAsync(taskId, cancellationToken);
        var createdEntities = new List<TaskDependency>(parsedDependsOnIds.Count);

        foreach (var dependsOnId in parsedDependsOnIds)
        {
            if (taskId == dependsOnId)
                throw new InvalidOperationException("A task cannot depend on itself.");

            var dependsOnTask = await _repository.GetRequiredByIdAsync(dependsOnId, cancellationToken);
            if (task.ExerciseId != dependsOnTask.ExerciseId)
                throw new InvalidOperationException("Task dependencies must stay within the same exercise.");

            var entity = new TaskDependency
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                DependsOnId = dependsOnId,
                CreationTime = DateTime.UtcNow
            };
            var createdEntity = await _repository.CreateTaskDependencyAsync(entity, cancellationToken);
            createdEntities.Add(createdEntity);
        }

        _logger.Info("Exercise task dependencies added", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "exerciseId", task.ExerciseId },
            { "taskId", taskId },
            { "count", createdEntities.Count },
            { "result", "success" }
        });

        return createdEntities;
    }

    public async Task DeleteExerciseTaskDependencyAsync(Guid dependencyId, CancellationToken cancellationToken = default)
    {
        await _authorizationService.EnsureCanFullyEditExerciseTaskDependencyAsync(
            dependencyId,
            cancellationToken
        );
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        await _repository.DeleteTaskDependencyAsync(dependencyId, cancellationToken);

        _logger.Info("Exercise task dependency removed", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "dependencyId", dependencyId },
            { "result", "success" }
        });
    }

    public async Task<ExerciseTaskResponsibleUser> AddExerciseTaskResponsibleUserAsync(
        Guid taskId,
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanPartiallyEditExerciseTaskAsync(taskId, cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        _ = await _repository.GetRequiredByIdAsync(taskId, cancellationToken);
        var parsedUserId = ParseGuidString(userId, "UserId");
        var entity = new ExerciseTaskResponsibleUser
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            UserId = parsedUserId,
            CreationTime = DateTime.UtcNow
        };
        var createdEntity = await _repository.CreateExerciseTaskResponsibleUserAsync(entity, cancellationToken);

        _logger.Info("Exercise task responsible user added", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "taskId", taskId },
            { "targetUserId", parsedUserId },
            { "result", "success" }
        });

        return createdEntity;
    }

    public async Task<List<ExerciseTaskResponsibleUser>> BulkUpdateExerciseTaskResponsibleUsersAsync(
        Guid taskId,
        List<string> userIds,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanPartiallyEditExerciseTaskAsync(taskId, cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var parsedUserIds = userIds.Select(item => ParseGuidString(item, "UserId")).Distinct().ToList();
        var updatedEntities = await _repository.ReplaceExerciseTaskResponsibleUsersAsync(
            taskId,
            parsedUserIds,
            DateTime.UtcNow,
            cancellationToken
        );

        _logger.Info("Exercise task responsible users replaced", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "taskId", taskId },
            { "count", updatedEntities.Count },
            { "result", "success" }
        });

        return updatedEntities;
    }

    public async Task DeleteExerciseTaskResponsibleUserAsync(
        Guid responsibilityId,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanPartiallyEditExerciseTaskResponsibleUserAsync(
            responsibilityId,
            cancellationToken
        );
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        await _repository.DeleteExerciseTaskResponsibleUserAsync(responsibilityId, cancellationToken);

        _logger.Info("Exercise task responsible user removed", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "responsibilityId", responsibilityId },
            { "result", "success" }
        });
    }

    public async Task BulkDeleteExerciseTaskResponsibleUsersAsync(
        Guid taskId,
        BulkDeleteExerciseTaskResponsibleUsersRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanPartiallyEditExerciseTaskAsync(taskId, cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var responsibilityIds = request.ResponsibilityIds.Distinct().ToList();
        await _repository.DeleteExerciseTaskResponsibleUsersAsync(
            taskId,
            responsibilityIds,
            cancellationToken
        );

        _logger.Info("Exercise task responsible users bulk removed", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "taskId", taskId },
            { "count", responsibilityIds.Count },
            { "result", "success" }
        });
    }

    public async Task<ExerciseTask> AddAsync(ExerciseTask entity, CancellationToken cancellationToken = default)
    {
        var createdEntity = await _repository.AddAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<ExerciseTask> UpdateAsync(ExerciseTask entity, CancellationToken cancellationToken = default)
    {
        var updatedEntity = await _repository.UpdateAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await DeleteExerciseTaskDeepAsync(id, cancellationToken);
    }

    public async Task DeleteExerciseTaskDeepAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        await _authorizationService.EnsureCanFullyEditExerciseTaskAsync(taskId, cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        _logger.Info("Exercise task deep delete started", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "taskId", taskId },
            { "result", "started" }
        });
        await _repository.DeleteExerciseTaskDeepAsync(taskId, cancellationToken);

        _logger.Info("Exercise task deleted", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "taskId", taskId },
            { "result", "success" }
        });
    }

    private async Task<bool> IsCompletedStatusAsync(Guid statusId, CancellationToken cancellationToken)
    {
        var isCompletedStatus = await _closedListItemRepository.IsHighestDisplayOrderItemForKeyAsync(
            statusId,
            ConstantValues.TaskStatusClosedListKey,
            cancellationToken
        );

        return isCompletedStatus;
    }

    private static void AttachResponsibleUsers(
        ExerciseTask entity,
        List<string>? responsibleUserIds,
        DateTime now
    )
    {
        foreach (var userId in (responsibleUserIds ?? new List<string>())
                     .Select(item => ParseGuidString(item, "UserId"))
                     .Distinct())
        {
            entity.ResponsibleUsers.Add(new ExerciseTaskResponsibleUser
            {
                Id = Guid.NewGuid(),
                TaskId = entity.Id,
                UserId = userId,
                CreationTime = now
            });
        }
    }

    private async Task AttachExistingDependenciesAsync(
        ExerciseTask entity,
        List<string>? dependsOnTaskIds,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        foreach (var dependsOnId in (dependsOnTaskIds ?? new List<string>())
                     .Select(item => ParseGuidString(item, "DependsOnId"))
                     .Distinct())
        {
            if (entity.Id == dependsOnId)
                throw new InvalidOperationException("A task cannot depend on itself.");

            var dependsOnTask = await _repository.GetRequiredByIdAsync(dependsOnId, cancellationToken);
            if (entity.ExerciseId != dependsOnTask.ExerciseId)
                throw new InvalidOperationException("Task dependencies must stay within the same exercise.");

            entity.Dependencies.Add(new TaskDependency
            {
                Id = Guid.NewGuid(),
                TaskId = entity.Id,
                DependsOnId = dependsOnId,
                CreationTime = now
            });
        }
    }

    private static void AttachBulkDependencies(
        ExerciseTask entity,
        List<string>? dependsOnTaskKeys,
        Dictionary<string, ExerciseTask> entitiesByTaskKey,
        DateTime now
    )
    {
        foreach (var dependsOnTaskKey in (dependsOnTaskKeys ?? new List<string>()).Distinct(StringComparer.Ordinal))
        {
            if (!entitiesByTaskKey.TryGetValue(dependsOnTaskKey, out var dependsOnTask))
            {
                throw new InvalidOperationException(
                    $"Task dependency key '{dependsOnTaskKey}' was not found in the bulk create payload."
                );
            }

            if (entity.Id == dependsOnTask.Id)
                throw new InvalidOperationException("A task cannot depend on itself.");

            if (entity.ExerciseId != dependsOnTask.ExerciseId)
                throw new InvalidOperationException("Task dependencies must stay within the same exercise.");

            entity.Dependencies.Add(new TaskDependency
            {
                Id = Guid.NewGuid(),
                TaskId = entity.Id,
                DependsOnId = dependsOnTask.Id,
                CreationTime = now
            });
        }
    }

    private ExerciseTask CreateExerciseTaskEntity(
        CreateExerciseTaskRequest request,
        int serialNumber,
        DateTime now,
        bool isCompletedStatus
    )
    {
        return new ExerciseTask
        {
            Id = Guid.NewGuid(),
            ExerciseId = request.ExerciseId,
            ParentId = null,
            SourceId = null,
            SeriesId = request.SeriesId,
            CategoryId = request.CategoryId,
            StatusId = _defaultExerciseTaskStatusId,
            SerialNumber = serialNumber,
            Name = request.Name,
            Description = request.Description,
            Notes = request.Notes,
            DueDate = request.DueDate,
            LastStatusUpdaterId = null,
            CreationTime = now,
            LastUpdateTime = now,
            LastStatusUpdateTime = null,
            CompletionTime = isCompletedStatus ? now : null,
            HasChildren = false,
            Children = new List<ExerciseTask>(),
            ResponsibleUsers = new List<ExerciseTaskResponsibleUser>(),
            Dependencies = new List<TaskDependency>(),
            DependedOnBy = new List<TaskDependency>()
        };
    }

    private static void RegisterTaskKey(
        string? taskKey,
        ExerciseTask entity,
        Dictionary<string, ExerciseTask> entitiesByTaskKey
    )
    {
        if (string.IsNullOrWhiteSpace(taskKey))
            return;

        if (!entitiesByTaskKey.TryAdd(taskKey, entity))
            throw new InvalidOperationException($"Duplicate task key '{taskKey}' was found in the bulk create payload.");
    }

    private static Guid ParseGuidString(string value, string fieldName)
    {
        if (!Guid.TryParse(value, out var parsedGuid))
            throw new InvalidOperationException($"{fieldName} must be a valid GUID string.");

        return parsedGuid;
    }
}
