namespace Coucher.Shared.Models.WebApi.Requests.Admin;

public sealed class UpdateTaskTemplateRequestModel
{
    public Guid? SeriesId { get; set; }
    public Guid? CategoryId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public int DefaultWeeksBeforeExerciseStart { get; set; }
}
