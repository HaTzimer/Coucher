namespace Coucher.Shared.Models.WebApi.Requests.Tasks;

public sealed class UpdateExerciseTaskRequest
{
    public Guid? SeriesId { get; set; }

    public Guid? CategoryId { get; set; }

    public Guid? StatusId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool ClearDescription { get; set; }

    public string? Notes { get; set; }

    public bool ClearNotes { get; set; }

    public DateTime? DueDate { get; set; }
}
