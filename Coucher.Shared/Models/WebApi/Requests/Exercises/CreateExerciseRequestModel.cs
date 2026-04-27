namespace Coucher.Shared.Models.WebApi.Requests.Exercises;

public sealed class CreateExerciseRequestModel
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public Guid? StatusId { get; set; }
    public Guid TraineeUnitId { get; set; }
    public Guid TrainerUnitId { get; set; }
    public double? CompressionFactor { get; set; }
    public required List<Guid> InfluencerIds { get; set; }
    public required List<Guid> SectionIds { get; set; }
    public required ExerciseParticipantRequestModel Manager { get; set; }
    public required List<ExerciseUnitContactRequestModel> TraineeUnitContacts { get; set; }
    public required List<ExerciseUnitContactRequestModel> TrainerUnitContacts { get; set; }
    public required List<ExerciseParticipantRequestModel> AdditionalParticipants { get; set; }
}
