using Coucher.Shared.Interfaces.Repositories;
using Coucher.Shared.Interfaces.Services;
using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared.Models.WebApi.Requests.Admin;

namespace Coucher.Lib.Services;

public sealed class TaskTemplateService : ITaskTemplateService
{
    private readonly ITaskTemplateRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public TaskTemplateService(ITaskTemplateRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<List<TaskTemplate>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.ListAsync(cancellationToken);

        return items;
    }

    public async Task<TaskTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<TaskTemplate> GetRequiredByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetRequiredByIdAsync(id, cancellationToken);

        return entity;
    }

    public async Task<TaskTemplate> CreateTaskTemplateAsync(
        CreateTaskTemplateRequest request,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var nextSerialNumber = await _repository.GetNextSerialNumberAsync(cancellationToken);
        var nodesByKey = new Dictionary<string, TaskTemplate>(StringComparer.Ordinal);
        var createdNodes = new List<(TaskTemplate Entity, TaskTemplateNodeRequestBase Request)>();
        var entity = BuildTaskTemplateTree(request, now, ref nextSerialNumber, nodesByKey, createdNodes);
        WireDependencies(nodesByKey, createdNodes, now);
        var createdEntity = await _repository.CreateTaskTemplateAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<List<TaskTemplate>> CreateTaskTemplatesAsync(
        List<CreateTaskTemplateRequest> requests,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        if (requests.Count == 0)
        {
            return new List<TaskTemplate>();
        }

        var now = DateTime.UtcNow;
        var nextSerialNumber = await _repository.GetNextSerialNumberAsync(cancellationToken);
        var entities = new List<TaskTemplate>(capacity: requests.Count);
        foreach (var request in requests)
        {
            var nodesByKey = new Dictionary<string, TaskTemplate>(StringComparer.Ordinal);
            var createdNodes = new List<(TaskTemplate Entity, TaskTemplateNodeRequestBase Request)>();
            var entity = BuildTaskTemplateTree(request, now, ref nextSerialNumber, nodesByKey, createdNodes);
            WireDependencies(nodesByKey, createdNodes, now);
            entities.Add(entity);
        }

        var createdEntities = await _repository.CreateTaskTemplatesAsync(entities, cancellationToken);

        return createdEntities;
    }

    public async Task<TaskTemplate> UpdateTaskTemplateAsync(
        Guid taskTemplateId,
        UpdateTaskTemplateRequest request,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(taskTemplateId, cancellationToken);
        entity.SeriesId = request.SeriesId;
        entity.CategoryId = request.CategoryId;
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Notes = request.Notes;
        entity.DefaultWeeksBeforeExerciseStart = request.DefaultWeeksBeforeExerciseStart;
        var now = DateTime.UtcNow;
        entity.LastUpdateTime = now;
        var updatedEntity = await _repository.UpdateTaskTemplateAsync(entity, cancellationToken);
        _ = await _repository.ReplaceTaskTemplateInfluencersAsync(
            taskTemplateId,
            request.InfluencerIds ?? new List<Guid>(),
            now,
            cancellationToken
        );

        return updatedEntity;
    }

    public async Task<TaskTemplate> UpdateTaskTemplateSeriesAsync(
        Guid taskTemplateId,
        Guid? seriesId,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(taskTemplateId, cancellationToken);
        entity.SeriesId = seriesId;
        entity.LastUpdateTime = DateTime.UtcNow;
        var updatedEntity = await _repository.UpdateTaskTemplateAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<TaskTemplate> UpdateTaskTemplateCategoryAsync(
        Guid taskTemplateId,
        Guid? categoryId,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(taskTemplateId, cancellationToken);
        entity.CategoryId = categoryId;
        entity.LastUpdateTime = DateTime.UtcNow;
        var updatedEntity = await _repository.UpdateTaskTemplateAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<TaskTemplate> UpdateTaskTemplateDefaultWeeksBeforeExerciseStartAsync(
        Guid taskTemplateId,
        int defaultWeeksBeforeExerciseStart,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(taskTemplateId, cancellationToken);
        entity.DefaultWeeksBeforeExerciseStart = defaultWeeksBeforeExerciseStart;
        entity.LastUpdateTime = DateTime.UtcNow;
        var updatedEntity = await _repository.UpdateTaskTemplateAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<TaskTemplate> UpdateTaskTemplateDetailsAsync(
        Guid taskTemplateId,
        UpdateTaskTemplateDetailsRequest request,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(taskTemplateId, cancellationToken);
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Notes = request.Notes;
        entity.LastUpdateTime = DateTime.UtcNow;
        var updatedEntity = await _repository.UpdateTaskTemplateAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public async Task<TaskTemplate> AddTaskTemplateChildAsync(
        Guid taskTemplateId,
        CreateTaskTemplateChildRequest request,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var parentTemplate = await _repository.GetRequiredByIdAsync(taskTemplateId, cancellationToken);
        if (parentTemplate.ParentId.HasValue)
        {
            throw new InvalidOperationException("Task templates support children only one level deep.");
        }

        var now = DateTime.UtcNow;
        var nextSerialNumber = await _repository.GetNextSerialNumberAsync(cancellationToken);
        var nodesByKey = new Dictionary<string, TaskTemplate>(StringComparer.Ordinal);
        var createdNodes = new List<(TaskTemplate Entity, TaskTemplateNodeRequestBase Request)>();
        var entity = CreateTaskTemplateEntity(request, taskTemplateId, now, ref nextSerialNumber);
        RegisterTemplateKey(request.TemplateKey, entity, nodesByKey);
        createdNodes.Add((entity, request));
        WireDependencies(nodesByKey, createdNodes, now);
        var createdEntity = await _repository.CreateTaskTemplateAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<TaskTemplateDependency> AddTaskTemplateDependencyAsync(
        Guid taskTemplateId,
        Guid dependsOnId,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        _ = await _repository.GetRequiredByIdAsync(taskTemplateId, cancellationToken);
        _ = await _repository.GetRequiredByIdAsync(dependsOnId, cancellationToken);

        if (taskTemplateId == dependsOnId)
        {
            throw new InvalidOperationException("A task template cannot depend on itself.");
        }

        var entity = new TaskTemplateDependency
        {
            Id = Guid.NewGuid(),
            TemplateId = taskTemplateId,
            DependsOnId = dependsOnId,
            CreationTime = DateTime.UtcNow
        };
        var createdEntity = await _repository.CreateTaskTemplateDependencyAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<List<TaskTemplateInfluencer>> AddTaskTemplateInfluencersAsync(
        Guid taskTemplateId,
        List<Guid> influencerIds,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var createdEntities = await _repository.CreateTaskTemplateInfluencersAsync(
            taskTemplateId,
            influencerIds,
            DateTime.UtcNow,
            cancellationToken
        );

        return createdEntities;
    }

    public async Task DeleteTaskTemplateDependencyAsync(
        Guid dependencyId,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        await _repository.DeleteTaskTemplateDependencyAsync(dependencyId, cancellationToken);
    }

    public async Task DeleteTaskTemplateInfluencerAsync(
        Guid influencerLinkId,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        await _repository.DeleteTaskTemplateInfluencerAsync(influencerLinkId, cancellationToken);
    }

    public async Task<TaskTemplate> ArchiveTaskTemplateAsync(
        Guid taskTemplateId,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var archivedEntity = await _repository.ArchiveTaskTemplateAsync(taskTemplateId, cancellationToken);

        return archivedEntity;
    }

    public async Task<TaskTemplate> UnarchiveTaskTemplateAsync(
        Guid taskTemplateId,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var unarchivedEntity = await _repository.UnarchiveTaskTemplateAsync(taskTemplateId, cancellationToken);

        return unarchivedEntity;
    }

    public async Task<TaskTemplate> AddAsync(TaskTemplate entity, CancellationToken cancellationToken = default)
    {
        var createdEntity = await _repository.AddAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<TaskTemplate> UpdateAsync(TaskTemplate entity, CancellationToken cancellationToken = default)
    {
        var updatedEntity = await _repository.UpdateAsync(entity, cancellationToken);

        return updatedEntity;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Task templates use archive and unarchive operations instead of physical delete.");
    }

    private static TaskTemplate BuildTaskTemplateTree(
        CreateTaskTemplateRequest request,
        DateTime now,
        ref int nextSerialNumber,
        Dictionary<string, TaskTemplate> nodesByKey,
        List<(TaskTemplate Entity, TaskTemplateNodeRequestBase Request)> createdNodes
    )
    {
        var entity = CreateTaskTemplateEntity(request, parentId: null, now, ref nextSerialNumber);
        RegisterTemplateKey(request.TemplateKey, entity, nodesByKey);
        createdNodes.Add((entity, request));

        foreach (var childRequest in request.Children ?? new List<CreateTaskTemplateChildRequest>())
        {
            var childEntity = CreateTaskTemplateEntity(childRequest, entity.Id, now, ref nextSerialNumber);
            RegisterTemplateKey(childRequest.TemplateKey, childEntity, nodesByKey);
            createdNodes.Add((childEntity, childRequest));
            entity.Children.Add(childEntity);
        }

        return entity;
    }

    private static TaskTemplate CreateTaskTemplateEntity(
        TaskTemplateNodeRequestBase request,
        Guid? parentId,
        DateTime now,
        ref int nextSerialNumber
    )
    {
        var entity = new TaskTemplate
        {
            Id = Guid.NewGuid(),
            ParentId = parentId,
            SeriesId = request.SeriesId,
            CategoryId = request.CategoryId,
            SerialNumber = nextSerialNumber,
            Name = request.Name,
            Description = request.Description,
            Notes = request.Notes,
            DefaultWeeksBeforeExerciseStart = request.DefaultWeeksBeforeExerciseStart,
            IsArchive = false,
            CreationTime = now,
            LastUpdateTime = now,
            Children = new List<TaskTemplate>(),
            Dependencies = new List<TaskTemplateDependency>(),
            DependedOnBy = new List<TaskTemplateDependency>(),
            Influencers = BuildTaskTemplateInfluencers(request.InfluencerIds, now)
        };

        nextSerialNumber++;

        return entity;
    }

    private static void WireDependencies(
        Dictionary<string, TaskTemplate> nodesByKey,
        List<(TaskTemplate Entity, TaskTemplateNodeRequestBase Request)> createdNodes,
        DateTime now
    )
    {
        foreach (var (entity, request) in createdNodes)
        {
            foreach (var dependsOnTemplateKey in request.DependsOnTemplateKeys?.Distinct() ?? Enumerable.Empty<string>())
            {
                if (!nodesByKey.TryGetValue(dependsOnTemplateKey, out var dependsOnEntity))
                {
                    throw new InvalidOperationException(
                        $"Task template dependency key '{dependsOnTemplateKey}' was not found in the create payload."
                    );
                }

                if (dependsOnEntity.Id == entity.Id)
                {
                    throw new InvalidOperationException("A task template cannot depend on itself.");
                }

                entity.Dependencies.Add(new TaskTemplateDependency
                {
                    Id = Guid.NewGuid(),
                    TemplateId = entity.Id,
                    DependsOnId = dependsOnEntity.Id,
                    CreationTime = now
                });
            }
        }
    }

    private static void RegisterTemplateKey(
        string? templateKey,
        TaskTemplate entity,
        Dictionary<string, TaskTemplate> nodesByKey
    )
    {
        if (string.IsNullOrWhiteSpace(templateKey))
        {
            return;
        }

        if (nodesByKey.ContainsKey(templateKey))
        {
            throw new InvalidOperationException($"Duplicate task template key '{templateKey}' was found in the create payload.");
        }

        nodesByKey.Add(templateKey, entity);
    }

    private static List<TaskTemplateInfluencer> BuildTaskTemplateInfluencers(List<Guid>? influencerIds, DateTime now)
    {
        var entities = (influencerIds ?? new List<Guid>())
            .Distinct()
            .Select(influencerId => new TaskTemplateInfluencer
            {
                Id = Guid.NewGuid(),
                TemplateId = null,
                InfluencerId = influencerId,
                CreationTime = now
            })
            .ToList();

        return entities;
    }
}
