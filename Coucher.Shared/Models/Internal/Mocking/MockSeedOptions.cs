namespace Coucher.Shared.Models.Internal.Mocking;

public sealed class MockSeedOptions
{
    public bool ResetExisting { get; set; }
    public int? UserCount { get; set; }
    public int? ExerciseCount { get; set; }
    public int? AdditionalParticipantsPerExercise { get; set; }
    public int? TaskTemplateCount { get; set; }
    public int? TasksPerExercise { get; set; }
    public int? NotificationsPerUser { get; set; }
}

