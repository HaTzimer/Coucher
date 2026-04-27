namespace Coucher.Shared.Models.Internal.Mocking;

public sealed class MockSeedSummary
{
    public bool WasReset { get; set; }
    public int ClosedListItemCount { get; set; }
    public int UnitCount { get; set; }
    public int UserCount { get; set; }
    public int UserRoleCount { get; set; }
    public int ExerciseCount { get; set; }
    public int ExerciseParticipantCount { get; set; }
    public int ExerciseUnitContactCount { get; set; }
    public int ExerciseInfluencerCount { get; set; }
    public int ExerciseSectionCount { get; set; }
    public int TaskTemplateCount { get; set; }
    public int TaskTemplateDependencyCount { get; set; }
    public int TaskTemplateInfluencerCount { get; set; }
    public int ExerciseTaskCount { get; set; }
    public int ExerciseTaskResponsibleUserCount { get; set; }
    public int TaskDependencyCount { get; set; }
    public int UserNotificationCount { get; set; }
    public string? Note { get; set; }
}

