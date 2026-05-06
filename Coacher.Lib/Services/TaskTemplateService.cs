using System.Net;
using System.Diagnostics.CodeAnalysis;
using Augustus.Infra.Core.Shared.Exceptions;
using Augustus.Infra.Core.Shared.Interfaces;
using Coacher.Shared.Interfaces.Repositories;
using Coacher.Shared.Interfaces.Services;
using Coacher.Shared.Models.DAL.Admin;
using Coacher.Shared.Models.WebApi.Requests.Admin;

namespace Coacher.Lib.Services;

public sealed class TaskTemplateService : ITaskTemplateService
{
    private readonly IAugustusLogger _logger;
    private readonly ITaskTemplateRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICoacherAuthorizationService _authorizationService;

    public TaskTemplateService(
        IAugustusLogger logger,
        ITaskTemplateRepository repository,
        ICurrentUserService currentUserService,
        ICoacherAuthorizationService authorizationService
    )
    {
        _logger = logger;
        _repository = repository;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
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
        await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var nextSerialNumber = await _repository.GetNextSerialNumberAsync(cancellationToken);
        var nodesByKey = new Dictionary<string, TaskTemplate>(StringComparer.Ordinal);
        var createdNodes = new List<(TaskTemplate Entity, TaskTemplateNodeRequestBase Request)>();
        var entity = BuildTaskTemplateTree(request, now, ref nextSerialNumber, nodesByKey, createdNodes);
        WireDependencies(nodesByKey, createdNodes, now);
        var createdEntity = await _repository.CreateTaskTemplateAsync(entity, cancellationToken);

        _logger.Info("Task template created", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "taskTemplateId", createdEntity.Id },
            { "childCount", createdEntity.Children.Count },
            { "dependencyCount", createdNodes.Sum(item => item.Entity.Dependencies.Count) },
            { "result", "success" }
        });

        return createdEntity;
    }

    public async Task<List<TaskTemplate>> CreateTaskTemplatesAsync(
        List<CreateTaskTemplateRequest> requests,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        if (requests.Count == 0)
            return new List<TaskTemplate>();

        var now = DateTime.UtcNow;
        var nextSerialNumber = await _repository.GetNextSerialNumberAsync(cancellationToken);
        var entities = new List<TaskTemplate>(capacity: requests.Count);

        _logger.Info("Task template bulk create started", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "count", requests.Count },
            { "result", "started" }
        });

        foreach (var request in requests)
        {
            var nodesByKey = new Dictionary<string, TaskTemplate>(StringComparer.Ordinal);
            var createdNodes = new List<(TaskTemplate Entity, TaskTemplateNodeRequestBase Request)>();
            var entity = BuildTaskTemplateTree(request, now, ref nextSerialNumber, nodesByKey, createdNodes);
            WireDependencies(nodesByKey, createdNodes, now);
            entities.Add(entity);
        }

        var createdEntities = await _repository.CreateTaskTemplatesAsync(entities, cancellationToken);

        _logger.Info("Task template bulk create completed", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "count", createdEntities.Count },
            { "result", "success" }
        });

        return createdEntities;
    }

    public async Task<TaskTemplate> UpdateTaskTemplateAsync(
        Guid taskTemplateId,
        UpdateTaskTemplateRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var entity = await _repository.GetRequiredByIdAsync(taskTemplateId, cancellationToken);

        ValidateUpdateTaskTemplateRequest(request);

        var changedFields = ApplyTaskTemplateUpdates(entity, request);
        if (changedFields.Count == 0)
            return entity;

        entity.LastUpdateTime = DateTime.UtcNow;

        var updatedEntity = await _repository.UpdateTaskTemplateAsync(entity, cancellationToken);

        _logger.Info("Task template updated", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "taskTemplateId", taskTemplateId },
            { "changedFields", changedFields.ToArray() },
            { "result", "success" }
        });

        return updatedEntity;
    }

    public async Task<TaskTemplate> AddTaskTemplateChildAsync(
        Guid taskTemplateId,
        CreateTaskTemplateChildRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var parentTemplate = await _repository.GetRequiredByIdAsync(taskTemplateId, cancellationToken);
        if (parentTemplate.ParentId.HasValue)
            ThrowConflict("Task templates support children only one level deep.", ("taskTemplateId", taskTemplateId));

        var now = DateTime.UtcNow;
        var nextSerialNumber = await _repository.GetNextSerialNumberAsync(cancellationToken);
        var nodesByKey = new Dictionary<string, TaskTemplate>(StringComparer.Ordinal);
        var createdNodes = new List<(TaskTemplate Entity, TaskTemplateNodeRequestBase Request)>();
        var entity = CreateTaskTemplateEntity(request, taskTemplateId, now, ref nextSerialNumber);
        RegisterTemplateKey(request.TemplateKey, entity, nodesByKey);
        createdNodes.Add((entity, request));
        WireDependencies(nodesByKey, createdNodes, now);
        var createdEntity = await _repository.CreateTaskTemplateAsync(entity, cancellationToken);

        _logger.Info("Task template child created", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "taskTemplateId", createdEntity.Id },
            { "parentTaskTemplateId", taskTemplateId },
            { "dependencyCount", createdEntity.Dependencies.Count },
            { "result", "success" }
        });

        return createdEntity;
    }

    public async Task<List<TaskTemplateDependency>> AddTaskTemplateDependenciesAsync(
        Guid taskTemplateId,
        List<Guid> dependsOnIds,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        _ = await _repository.GetRequiredByIdAsync(taskTemplateId, cancellationToken);
        var normalizedDependsOnIds = dependsOnIds.Distinct().ToList();
        if (normalizedDependsOnIds.Count == 0)
            return new List<TaskTemplateDependency>();

        foreach (var dependsOnId in normalizedDependsOnIds)
        {
            _ = await _repository.GetRequiredByIdAsync(dependsOnId, cancellationToken);
            if (taskTemplateId == dependsOnId)
                ThrowConflict(
                    "A task template cannot depend on itself.",
                    ("taskTemplateId", taskTemplateId),
                    ("dependsOnId", dependsOnId)
                );
        }

        var now = DateTime.UtcNow;
        var entities = normalizedDependsOnIds.Select(dependsOnId => new TaskTemplateDependency
            {
                Id = Guid.NewGuid(),
                TemplateId = taskTemplateId,
                DependsOnId = dependsOnId,
                CreationTime = now
            })
            .ToList();
        var createdEntities = await _repository.CreateTaskTemplateDependenciesAsync(
            entities,
            cancellationToken
        );

        _logger.Info("Task template dependencies added", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "taskTemplateId", taskTemplateId },
            { "count", createdEntities.Count },
            { "result", "success" }
        });

        return createdEntities;
    }

    public async Task<List<TaskTemplateInfluencer>> AddTaskTemplateInfluencersAsync(
        Guid taskTemplateId,
        List<Guid> influencerIds,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);
        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var createdEntities = await _repository.CreateTaskTemplateInfluencersAsync(
            taskTemplateId,
            influencerIds,
            DateTime.UtcNow,
            cancellationToken
        );

        _logger.Info("Task template influencers added", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "taskTemplateId", taskTemplateId },
            { "count", createdEntities.Count },
            { "result", "success" }
        });

        return createdEntities;
    }

    public async Task DeleteTaskTemplateDependenciesAsync(
        Guid taskTemplateId,
        List<Guid> dependencyIds,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var normalizedDependencyIds = dependencyIds.Distinct().ToList();
        if (normalizedDependencyIds.Count == 0)
            return;

        await _repository.DeleteTaskTemplateDependenciesAsync(
            taskTemplateId,
            normalizedDependencyIds,
            cancellationToken
        );

        _logger.Info("Task template dependencies removed", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "taskTemplateId", taskTemplateId },
            { "count", normalizedDependencyIds.Count },
            { "result", "success" }
        });
    }

    public async Task DeleteTaskTemplateInfluencersAsync(
        Guid taskTemplateId,
        List<Guid> influencerLinkIds,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var normalizedInfluencerLinkIds = influencerLinkIds.Distinct().ToList();
        if (normalizedInfluencerLinkIds.Count == 0)
            return;

        await _repository.DeleteTaskTemplateInfluencersAsync(
            taskTemplateId,
            normalizedInfluencerLinkIds,
            cancellationToken
        );

        _logger.Info("Task template influencers removed", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "taskTemplateId", taskTemplateId },
            { "count", normalizedInfluencerLinkIds.Count },
            { "result", "success" }
        });
    }

    public async Task<TaskTemplate> SetTaskTemplateArchiveStateAsync(
        Guid taskTemplateId,
        bool isArchived,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);

        var currentUserId = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        if (isArchived)
        {
            var archivedEntity = await _repository.ArchiveTaskTemplateAsync(taskTemplateId, cancellationToken);

            _logger.Info("Task template archived", new Dictionary<string, object>
            {
                { "userId", currentUserId },
                { "taskTemplateId", taskTemplateId },
                { "result", "success" }
            });

            return archivedEntity;
        }

        var unarchivedEntity = await _repository.UnarchiveTaskTemplateAsync(taskTemplateId, cancellationToken);

        _logger.Info("Task template unarchived", new Dictionary<string, object>
        {
            { "userId", currentUserId },
            { "taskTemplateId", taskTemplateId },
            { "result", "success" }
        });

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
        var exception = new HttpStatusCodeException(
            "Task templates use archive and unarchive operations instead of physical delete.",
            new Dictionary<string, object?>
            {
                { "taskTemplateId", id }
            },
            HttpStatusCode.BadRequest
        );

        _logger.Error(exception);

        throw exception;
    }

    private void ValidateUpdateTaskTemplateRequest(UpdateTaskTemplateRequest request)
    {
        if (request.Description is not null && request.ClearDescription)
            ThrowBadRequest(
                "Description cannot be updated and cleared in the same request.",
                ("clearDescription", request.ClearDescription)
            );

        if (request.Notes is not null && request.ClearNotes)
            ThrowBadRequest(
                "Notes cannot be updated and cleared in the same request.",
                ("clearNotes", request.ClearNotes)
            );
    }

    private static List<string> ApplyTaskTemplateUpdates(
        TaskTemplate entity,
        UpdateTaskTemplateRequest request
    )
    {
        var changedFields = new List<string>();

        if (request.SeriesId.HasValue && entity.SeriesId != request.SeriesId.Value)
        {
            entity.SeriesId = request.SeriesId.Value;
            changedFields.Add("SeriesId");
        }

        if (request.CategoryId.HasValue && entity.CategoryId != request.CategoryId.Value)
        {
            entity.CategoryId = request.CategoryId.Value;
            changedFields.Add("CategoryId");
        }

        if (request.Name is not null && entity.Name != request.Name)
        {
            entity.Name = request.Name;
            changedFields.Add("Name");
        }

        if (request.ClearDescription)
        {
            if (entity.Description is not null)
            {
                entity.Description = null;
                changedFields.Add("Description");
            }
        }

        if (request.Description is not null && entity.Description != request.Description)
        {
            entity.Description = request.Description;
            changedFields.Add("Description");
        }

        if (request.ClearNotes)
        {
            if (entity.Notes is not null)
            {
                entity.Notes = null;
                changedFields.Add("Notes");
            }
        }

        if (request.Notes is not null && entity.Notes != request.Notes)
        {
            entity.Notes = request.Notes;
            changedFields.Add("Notes");
        }

        if (request.DefaultTimeBeforeExerciseToStart.HasValue
            && entity.DefaultTimeBeforeExerciseToStart != request.DefaultTimeBeforeExerciseToStart.Value)
        {
            entity.DefaultTimeBeforeExerciseToStart = request.DefaultTimeBeforeExerciseToStart.Value;
            changedFields.Add(nameof(entity.DefaultTimeBeforeExerciseToStart));
        }

        return changedFields;
    }

    private TaskTemplate BuildTaskTemplateTree(
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
            DefaultTimeBeforeExerciseToStart = request.DefaultTimeBeforeExerciseToStart,
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

    private void WireDependencies(
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
                    ThrowBadRequest(
                        $"Task template dependency key '{dependsOnTemplateKey}' was not found in the create payload.",
                        ("dependsOnTemplateKey", dependsOnTemplateKey)
                    );

                if (dependsOnEntity.Id == entity.Id)
                    ThrowConflict("A task template cannot depend on itself.", ("taskTemplateId", entity.Id));

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

    private void RegisterTemplateKey(
        string? templateKey,
        TaskTemplate entity,
        Dictionary<string, TaskTemplate> nodesByKey
    )
    {
        if (string.IsNullOrWhiteSpace(templateKey))
            return;

        if (nodesByKey.ContainsKey(templateKey))
            ThrowBadRequest(
                $"Duplicate task template key '{templateKey}' was found in the create payload.",
                ("templateKey", templateKey)
            );

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

    [DoesNotReturn]
    private void ThrowBadRequest(string message, params (string Key, object? Value)[] entries)
    {
        var exception = new HttpStatusCodeException(
            message,
            entries.ToDictionary(item => item.Key, item => item.Value),
            HttpStatusCode.BadRequest
        );

        _logger.Error(exception);

        throw exception;
    }

    [DoesNotReturn]
    private void ThrowConflict(string message, params (string Key, object? Value)[] entries)
    {
        var exception = new DataConflictException(
            message,
            parameters: entries.ToDictionary(item => item.Key, item => item.Value)
        );

        _logger.Error(exception);

        throw exception;
    }
}

