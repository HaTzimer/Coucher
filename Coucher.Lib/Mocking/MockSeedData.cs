using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Models.DAL.Notifications;
using Coucher.Shared.Models.DAL.Tasks;
using Coucher.Shared.Models.DAL.Users;

namespace Coucher.Lib.Mocking;

internal sealed class MockSeedData
{
    public required List<ClosedListItem> ClosedListItems { get; init; }
    public required List<Unit> Units { get; init; }
    public required List<UserProfile> Users { get; init; }
    public required List<UserRole> UserRoles { get; init; }
    public required List<Exercise> Exercises { get; init; }
    public required List<ExerciseParticipant> ExerciseParticipants { get; init; }
    public required List<ExerciseUnitContact> ExerciseUnitContacts { get; init; }
    public required List<ExerciseInfluencer> ExerciseInfluencers { get; init; }
    public required List<ExerciseSection> ExerciseSections { get; init; }
    public required List<TaskTemplate> TaskTemplates { get; init; }
    public required List<TaskTemplateDependency> TaskTemplateDependencies { get; init; }
    public required List<TaskTemplateInfluencer> TaskTemplateInfluencers { get; init; }
    public required List<ExerciseTask> ExerciseTasks { get; init; }
    public required List<ExerciseTaskResponsibleUser> ExerciseTaskResponsibleUsers { get; init; }
    public required List<TaskDependency> TaskDependencies { get; init; }
    public required List<UserNotification> UserNotifications { get; init; }
}

