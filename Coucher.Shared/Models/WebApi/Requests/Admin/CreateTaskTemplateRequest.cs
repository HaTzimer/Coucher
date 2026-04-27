namespace Coucher.Shared.Models.WebApi.Requests.Admin;

public sealed class CreateTaskTemplateRequest
{
    public required string TemplateKey { get; set; }
    public Guid? SeriesId { get; set; }
    public Guid? CategoryId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public int DefaultWeeksBeforeExerciseStart { get; set; }
    public List<string>? DependsOnTemplateKeys { get; set; }
    public List<CreateTaskTemplateRequest>? Children { get; set; }
}
