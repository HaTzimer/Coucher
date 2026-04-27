using Coucher.Lib.DAL;
using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Models.DAL.Notifications;
using Coucher.Shared.Models.DAL.Tasks;
using Coucher.Shared.Models.DAL.Users;
using Coucher.Shared.Models.Internal.Projections.Exercises;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Lib.Gql;

public sealed class CoucherQuery
{
    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<ClosedListItem> GetClosedListItems(CoucherDbContext dbContext)
    {
        var query = dbContext.ClosedListItems.AsNoTracking();

        return query;
    }

    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Unit> GetUnits(CoucherDbContext dbContext)
    {
        var query = dbContext.Units.AsNoTracking();

        return query;
    }

    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Exercise> GetExercises(CoucherDbContext dbContext)
    {
        var query = dbContext.Exercises.AsNoTracking();

        return query;
    }

    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<ExerciseParticipant> GetExerciseParticipants(CoucherDbContext dbContext)
    {
        var query = dbContext.ExerciseParticipants.AsNoTracking();

        return query;
    }

    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<ExerciseUnitContact> GetExerciseUnitContacts(CoucherDbContext dbContext)
    {
        var query = dbContext.ExerciseUnitContacts.AsNoTracking();

        return query;
    }

    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<ExerciseInfluencer> GetExerciseInfluencers(CoucherDbContext dbContext)
    {
        var query = dbContext.ExerciseInfluencers.AsNoTracking();

        return query;
    }

    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<ExerciseSection> GetExerciseSections(CoucherDbContext dbContext)
    {
        var query = dbContext.ExerciseSections.AsNoTracking();

        return query;
    }

    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<ExerciseTask> GetExerciseTasks(CoucherDbContext dbContext)
    {
        var query = dbContext.ExerciseTasks.AsNoTracking();

        return query;
    }

    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<ExerciseTaskResponsibleUser> GetExerciseTaskResponsibleUsers(CoucherDbContext dbContext)
    {
        var query = dbContext.ExerciseTaskResponsibleUsers.AsNoTracking();

        return query;
    }

    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<TaskDependency> GetTaskDependencies(CoucherDbContext dbContext)
    {
        var query = dbContext.TaskDependencies.AsNoTracking();

        return query;
    }

    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<TaskTemplate> GetTaskTemplates(CoucherDbContext dbContext)
    {
        var query = dbContext.TaskTemplates.AsNoTracking();

        return query;
    }

    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<TaskTemplateDependency> GetTaskTemplateDependencies(CoucherDbContext dbContext)
    {
        var query = dbContext.TaskTemplateDependencies.AsNoTracking();

        return query;
    }

    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<TaskTemplateInfluencer> GetTaskTemplateInfluencers(CoucherDbContext dbContext)
    {
        var query = dbContext.TaskTemplateInfluencers.AsNoTracking();

        return query;
    }

    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<UserProfile> GetUsers(CoucherDbContext dbContext)
    {
        var query = dbContext.UserProfiles.AsNoTracking();

        return query;
    }

    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<UserRole> GetUserRoles(CoucherDbContext dbContext)
    {
        var query = dbContext.UserRoles.AsNoTracking();

        return query;
    }

    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<UserNotification> GetUserNotifications(CoucherDbContext dbContext)
    {
        var query = dbContext.UserNotifications.AsNoTracking();

        return query;
    }

    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<ExerciseSummary> GetExerciseSummaries(CoucherDbContext dbContext)
    {
        var query = dbContext.ExerciseSummaries.AsNoTracking();

        return query;
    }
}
