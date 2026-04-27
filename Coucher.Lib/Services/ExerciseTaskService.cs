using Augustus.Infra.Core.Shared.Interfaces;
using Coucher.Shared;
using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.WebApi.Requests.Tasks;
using Coucher.Shared.Models.DAL.Tasks;

namespace Coucher.Lib.Services;

public sealed class ExerciseTaskService : IExerciseTaskService
{
    private readonly IExerciseTaskRepository _repository;
    private readonly IClosedListItemRepository _closedListItemRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly Guid _defaultExerciseTaskStatusId;

    public ExerciseTaskService(
        IExerciseTaskRepository repository,
        IClosedListItemRepository closedListItemRepository,
        ICurrentUserService currentUserService,
        IAugustusConfiguration config
    )
    {
        _repository = repository;
        _closedListItemRepository = closedListItemRepository;
        _currentUserService = currentUserService;
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
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var nextSerialNumber = await _repository.GetNextSerialNumberAsync(request.ExerciseId, cancellationToken);
        var isCompletedStatus = await IsCompletedStatusAsync(_defaultExerciseTaskStatusId, cancellationToken);
        var entity = new ExerciseTask
        {
            Id = Guid.NewGuid(),
            ExerciseId = request.ExerciseId,
            ParentId = null,
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
        var createdEntity = await _repository.CreateExerciseTaskAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<List<ExerciseTask>> CreateExerciseTasksAsync(
        List<CreateExerciseTaskRequest> requests,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        if (requests.Count == 0)
        {
            return new List<ExerciseTask>();
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
        foreach (var request in requests)
        {
            var serialNumber = nextSerialNumbers[request.ExerciseId];
            nextSerialNumbers[request.ExerciseId] = serialNumber + 1;
            var isCompletedStatus = await IsCompletedStatusAsync(_defaultExerciseTaskStatusId, cancellationToken);

            entities.Add(new ExerciseTask
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
            });
        }

        var createdEntities = await _repository.CreateExerciseTasksAsync(entities, cancellationToken);

        return createdEntities;
    }

    public async Task<ExerciseTask> UpdateExerciseTaskSeriesAsync(
        Guid taskId,
        UpdateExerciseTaskSeriesRequest request,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(taskId, cancellationToken);
        entity.SeriesId = request.SeriesId;
        entity.LastUpdateTime = DateTime.UtcNow;
        var updatedEntity = await _repository.UpdateExerciseTaskAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<ExerciseTask> UpdateExerciseTaskCategoryAsync(
        Guid taskId,
        UpdateExerciseTaskCategoryRequest request,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(taskId, cancellationToken);
        entity.CategoryId = request.CategoryId;
        entity.LastUpdateTime = DateTime.UtcNow;
        var updatedEntity = await _repository.UpdateExerciseTaskAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<ExerciseTask> UpdateExerciseTaskStatusAsync(
        Guid taskId,
        UpdateExerciseTaskStatusRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(taskId, cancellationToken);
        var now = DateTime.UtcNow;

        entity.StatusId = request.StatusId;
        entity.LastUpdateTime = now;
        entity.LastStatusUpdaterId = currentUserId;
        entity.LastStatusUpdateTime = now;
        entity.CompletionTime = await IsCompletedStatusAsync(request.StatusId, cancellationToken)
            ? now
            : null;
        var updatedEntity = await _repository.UpdateExerciseTaskAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<ExerciseTask> UpdateExerciseTaskDetailsAsync(
        Guid taskId,
        UpdateExerciseTaskDetailsRequest request,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(taskId, cancellationToken);
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Notes = request.Notes;
        entity.LastUpdateTime = DateTime.UtcNow;

        var updatedEntity = await _repository.UpdateExerciseTaskAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<TaskDependency> AddExerciseTaskDependencyAsync(
        Guid taskId,
        AddExerciseTaskDependencyRequest request,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        if (taskId == request.DependsOnId)
        {
            throw new InvalidOperationException("A task cannot depend on itself.");
        }

        var task = await _repository.GetRequiredByIdAsync(taskId, cancellationToken);
        var dependsOnTask = await _repository.GetRequiredByIdAsync(request.DependsOnId, cancellationToken);
        if (task.ExerciseId != dependsOnTask.ExerciseId)
        {
            throw new InvalidOperationException("Task dependencies must stay within the same exercise.");
        }

        var entity = new TaskDependency
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            DependsOnId = request.DependsOnId,
            CreationTime = DateTime.UtcNow
        };
        var createdEntity = await _repository.CreateTaskDependencyAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task DeleteExerciseTaskDependencyAsync(Guid dependencyId, CancellationToken cancellationToken = default)
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        await _repository.DeleteTaskDependencyAsync(dependencyId, cancellationToken);
    }

    public async Task<ExerciseTaskResponsibleUser> AddExerciseTaskResponsibleUserAsync(
        Guid taskId,
        AddExerciseTaskResponsibleUserRequest request,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        _ = await _repository.GetRequiredByIdAsync(taskId, cancellationToken);
        var entity = new ExerciseTaskResponsibleUser
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            UserId = ParseUserId(request.UserId),
            CreationTime = DateTime.UtcNow
        };
        var createdEntity = await _repository.CreateExerciseTaskResponsibleUserAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<List<ExerciseTaskResponsibleUser>> BulkUpdateExerciseTaskResponsibleUsersAsync(
        Guid taskId,
        BulkUpdateExerciseTaskResponsibleUsersRequest request,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var userIds = request.UserIds.Select(ParseUserId).Distinct().ToList();
        var updatedEntities = await _repository.ReplaceExerciseTaskResponsibleUsersAsync(
            taskId,
            userIds,
            DateTime.UtcNow,
            cancellationToken
        );

        return updatedEntities;
    }

    public async Task DeleteExerciseTaskResponsibleUserAsync(
        Guid responsibilityId,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        await _repository.DeleteExerciseTaskResponsibleUserAsync(responsibilityId, cancellationToken);
    }

    public async Task BulkDeleteExerciseTaskResponsibleUsersAsync(
        Guid taskId,
        BulkDeleteExerciseTaskResponsibleUsersRequest request,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        await _repository.DeleteExerciseTaskResponsibleUsersAsync(
            taskId,
            request.ResponsibilityIds.Distinct().ToList(),
            cancellationToken
        );
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
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        await _repository.DeleteExerciseTaskDeepAsync(taskId, cancellationToken);
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

    private static Guid ParseUserId(string userId)
    {
        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            throw new InvalidOperationException("UserId must be a valid GUID string.");
        }

        return parsedUserId;
    }
}
