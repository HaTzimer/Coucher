namespace Coucher.Shared.Models.WebApi.Requests.Admin;

public sealed class UpdateTaskTemplateRequest
{
    public Guid? SeriesId { get; set; }

    public Guid? CategoryId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool ClearDescription { get; set; }

    public string? Notes { get; set; }

    public bool ClearNotes { get; set; }

    public int? DefaultWeeksBeforeExerciseStart { get; set; }
}
