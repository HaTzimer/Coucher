namespace Coucher.Shared.Models.WebApi.Requests.Common;

public sealed class SeedMocksRequestModel
{
    public bool ResetExisting { get; set; }
    public int? UserCount { get; set; }
    public int? ExerciseCount { get; set; }
    public int? AdditionalParticipantsPerExercise { get; set; }
    public int? TaskTemplateCount { get; set; }
    public int? TasksPerExercise { get; set; }
    public int? NotificationsPerUser { get; set; }
}

