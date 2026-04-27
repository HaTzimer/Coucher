namespace Coucher.Shared.Models.WebApi.Requests.Exercises;

public sealed class UpdateExerciseRequestModel
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public Guid? StatusId { get; set; }
    public Guid TraineeUnitId { get; set; }
    public Guid TrainerUnitId { get; set; }
    public double? CompressionFactor { get; set; }
}
