namespace Coacher.Shared.Models.WebApi.Requests.Exercises;

public sealed class CreateExerciseRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public Guid TraineeUnitId { get; set; }
    public Guid TrainerUnitId { get; set; }
    public double? CompressionFactor { get; set; }
    public required List<Guid> InfluencerIds { get; set; }
    public required List<Guid> SectionIds { get; set; }
    public required string ManagerUserId { get; set; }
    public required List<ExerciseUnitContactRequest> TraineeUnitContacts { get; set; }
    public required List<ExerciseUnitContactRequest> TrainerUnitContacts { get; set; }
    public required List<ExerciseParticipantRequest> AdditionalParticipants { get; set; }
}
