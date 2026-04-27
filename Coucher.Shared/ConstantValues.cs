using Microsoft.EntityFrameworkCore;

namespace Coucher.Shared;

public static class ConstantValues
{
    public const char RedisKeySeparator = ':';

    // EF mapping/table names
    public const string ClosedListItemTableName = "ClosedListItems";
    public const string UnitTableName = "Units";
    public const string UserProfileTableName = "UserProfiles";
    public const string UserRoleTableName = "UserRoles";
    public const string UserNotificationTableName = "UserNotifications";
    public const string ExerciseTableName = "Exercises";
    public const string ExerciseParticipantTableName = "ExerciseParticipants";
    public const string ExerciseUnitContactTableName = "ExerciseUnitContacts";
    public const string ExerciseInfluencerTableName = "ExerciseInfluencers";
    public const string ExerciseSectionTableName = "ExerciseSections";
    public const string ExerciseTaskTableName = "ExerciseTasks";
    public const string TaskDependencyTableName = "TaskDependencies";
    public const string ExerciseTaskResponsibleUserTableName = "ExerciseTaskResponsibleUsers";
    public const string TaskTemplateTableName = "TaskTemplates";
    public const string TaskTemplateDependencyTableName = "TaskTemplateDependencies";
    public const string TaskTemplateInfluencerTableName = "TaskTemplateInfluencers";

    // Closed-list keys (ClosedListItems.Key)
    public const string ExerciseStatusClosedListKey = "ExerciseStatus";
    public const string TaskStatusClosedListKey = "TaskStatus";
    public const string TaskSeriesClosedListKey = "TaskSeries";
    public const string TaskCategoryClosedListKey = "TaskCategory";
    public const string SectionClosedListKey = "Section";
    public const string InfluencerClosedListKey = "Influencer";
    public const string UnitEchelonClosedListKey = "UnitEchelon";

    // SQL Server doesn't allow multiple cascading actions (including SET NULL) from a single principal table.
    // We default to NO ACTION and handle any delete semantics explicitly in workflows.
    public const DeleteBehavior DeleteBehaviorType = DeleteBehavior.NoAction;
}
