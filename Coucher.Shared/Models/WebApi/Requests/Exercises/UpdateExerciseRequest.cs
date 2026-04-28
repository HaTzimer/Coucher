namespace Coucher.Shared.Models.WebApi.Requests.Exercises;

public sealed class UpdateExerciseRequest
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool ClearDescription { get; set; }

    public DateOnly? EndDate { get; set; }

    public Guid? StatusId { get; set; }

    public Guid? TraineeUnitId { get; set; }

    public Guid? TrainerUnitId { get; set; }
}
