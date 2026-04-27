using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.WebApi.Requests.Tasks;
using Coucher.Shared.Models.DAL.Tasks;
using Coucher.Shared;

namespace Coucher.Lib.Services;

public sealed class ExerciseTaskService : IExerciseTaskService
{
    private readonly IExerciseTaskRepository _repository;
    private readonly IClosedListItemRepository _closedListItemRepository;
    private readonly ICurrentUserService _currentUserService;

    public ExerciseTaskService(
        IExerciseTaskRepository repository,
        IClosedListItemRepository closedListItemRepository,
        ICurrentUserService currentUserService
    )
    {
        _repository = repository;
        _closedListItemRepository = closedListItemRepository;
        _currentUserService = currentUserService;
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
        CreateExerciseTaskRequestModel request,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var nextSerialNumber = await _repository.GetNextSerialNumberAsync(request.ExerciseId, cancellationToken);
        var isCompletedStatus = await IsCompletedStatusAsync(request.StatusId, cancellationToken);
        var entity = new ExerciseTask
        {
            Id = Guid.NewGuid(),
            ExerciseId = request.ExerciseId,
            ParentId = null,
            SourceId = null,
            SeriesId = request.SeriesId,
            CategoryId = request.CategoryId,
            StatusId = request.StatusId,
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
        List<CreateExerciseTaskRequestModel> requests,
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
            var isCompletedStatus = await IsCompletedStatusAsync(request.StatusId, cancellationToken);

            entities.Add(new ExerciseTask
            {
                Id = Guid.NewGuid(),
                ExerciseId = request.ExerciseId,
                ParentId = null,
                SourceId = null,
                SeriesId = request.SeriesId,
                CategoryId = request.CategoryId,
                StatusId = request.StatusId,
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

    public async Task<ExerciseTask> UpdateExerciseTaskAsync(
        Guid taskId,
        UpdateExerciseTaskRequestModel request,
        CancellationToken cancellationToken = default
    )
    {
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(taskId, cancellationToken);
        var now = DateTime.UtcNow;
        var isStatusChanged = entity.StatusId != request.StatusId;

        entity.SeriesId = request.SeriesId;
        entity.CategoryId = request.CategoryId;
        entity.StatusId = request.StatusId;
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Notes = request.Notes;
        entity.DueDate = request.DueDate;
        entity.LastUpdateTime = now;

        if (isStatusChanged)
        {
            entity.LastStatusUpdaterId = currentUserId;
            entity.LastStatusUpdateTime = now;
            entity.CompletionTime = await IsCompletedStatusAsync(request.StatusId, cancellationToken)
                ? now
                : null;
        }

        var updatedEntity = await _repository.UpdateExerciseTaskAsync(entity, cancellationToken);

        return updatedEntity;
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
        await _repository.DeleteAsync(id, cancellationToken);
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
}
