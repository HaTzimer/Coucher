namespace Coucher.Shared.Models.WebApi.Requests.Admin;

public abstract class TaskTemplateNodeRequestBase
{
    public string? TemplateKey { get; set; }

    public Guid? SeriesId { get; set; }

    public Guid? CategoryId { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public string? Notes { get; set; }

    public int DefaultWeeksBeforeExerciseStart { get; set; }

    public List<Guid>? InfluencerIds { get; set; }

    public List<string>? DependsOnTemplateKeys { get; set; }
}
