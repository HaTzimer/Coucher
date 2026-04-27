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
        CreateTaskTemplateRequestModel request,
        CancellationToken cancellationToken = default
    )
    {
        _ = await _currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var nextSerialNumber = await _repository.GetNextSerialNumberAsync(cancellationToken);
        var entity = new TaskTemplate
        {
            Id = Guid.NewGuid(),
            ParentId = null,
            SeriesId = request.SeriesId,
            CategoryId = request.CategoryId,
            SerialNumber = nextSerialNumber,
            Name = request.Name,
            Description = request.Description,
            Notes = request.Notes,
            DefaultWeeksBeforeExerciseStart = request.DefaultWeeksBeforeExerciseStart,
            CreationTime = now,
            LastUpdateTime = now,
            Children = new List<TaskTemplate>(),
            Dependencies = new List<TaskTemplateDependency>(),
            DependedOnBy = new List<TaskTemplateDependency>(),
            Influencers = new List<TaskTemplateInfluencer>()
        };
        var createdEntity = await _repository.CreateTaskTemplateAsync(entity, cancellationToken);

        return createdEntity;
    }

    public async Task<List<TaskTemplate>> CreateTaskTemplatesAsync(
        List<CreateTaskTemplateRequestModel> requests,
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
        for (var i = 0; i < requests.Count; i++)
        {
            var request = requests[i];
            entities.Add(new TaskTemplate
            {
                Id = Guid.NewGuid(),
                ParentId = null,
                SeriesId = request.SeriesId,
                CategoryId = request.CategoryId,
                SerialNumber = nextSerialNumber + i,
                Name = request.Name,
                Description = request.Description,
                Notes = request.Notes,
                DefaultWeeksBeforeExerciseStart = request.DefaultWeeksBeforeExerciseStart,
                CreationTime = now,
                LastUpdateTime = now,
                Children = new List<TaskTemplate>(),
                Dependencies = new List<TaskTemplateDependency>(),
                DependedOnBy = new List<TaskTemplateDependency>(),
                Influencers = new List<TaskTemplateInfluencer>()
            });
        }

        var createdEntities = await _repository.CreateTaskTemplatesAsync(entities, cancellationToken);

        return createdEntities;
    }

    public async Task<TaskTemplate> UpdateTaskTemplateAsync(
        Guid taskTemplateId,
        UpdateTaskTemplateRequestModel request,
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
        entity.LastUpdateTime = DateTime.UtcNow;
        var updatedEntity = await _repository.UpdateTaskTemplateAsync(entity, cancellationToken);

        return updatedEntity;
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

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
    }
}
