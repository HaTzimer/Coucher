using Coucher.Shared.Models.Enums;

namespace Coucher.WebApi.Models.WebApi.Requests.Exercises;

public sealed class CreateExerciseRequestModel
{
    public required string Name { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public UnitEchelon TraineeUnitEchelon { get; set; }
    public UnitEchelon TrainerUnitEchelon { get; set; }
    public bool IsOperationalTerrainExercise { get; set; }
    public required List<ExerciseInfluencer> Influencers { get; set; }
    public required List<ThreatArenaCode> ThreatArenas { get; set; }
    public required ExerciseParticipantRequestModel Manager { get; set; }
    public ExerciseParticipantRequestModel? TraineeUnitContact { get; set; }
    public required List<ExerciseParticipantRequestModel> AdditionalParticipants { get; set; }
}
