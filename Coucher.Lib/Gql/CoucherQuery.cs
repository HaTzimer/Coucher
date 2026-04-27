using Coucher.Lib.DAL;
using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Models.DAL.Notifications;
using Coucher.Shared.Models.DAL.Tasks;
using Coucher.Shared.Models.DAL.Users;
using Coucher.Shared.Interfaces.Services;
using HotChocolate;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Lib.Gql;

public sealed class CoucherQuery
{
    [GraphQLDescription("Returns configurable closed-list entries used by dropdowns, statuses, and catalogs.")]
    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<ClosedListItem>> GetClosedListItems(
        CoucherDbContext dbContext,
        [Service] IGraphQlCurrentUserService currentUserService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAuthenticatedAsync(currentUserService, cancellationToken);
        var query = dbContext.ClosedListItems.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns organizational units that can be assigned to users and exercises.")]
    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<Unit>> GetUnits(
        CoucherDbContext dbContext,
        [Service] IGraphQlCurrentUserService currentUserService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAuthenticatedAsync(currentUserService, cancellationToken);
        var query = dbContext.Units.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns exercises with their core planning and execution data.")]
    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<Exercise>> GetExercises(
        CoucherDbContext dbContext,
        [Service] IGraphQlCurrentUserService currentUserService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAuthenticatedAsync(currentUserService, cancellationToken);
        var query = dbContext.Exercises.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns user-to-exercise participation records.")]
    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<ExerciseParticipant>> GetExerciseParticipants(
        CoucherDbContext dbContext,
        [Service] IGraphQlCurrentUserService currentUserService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAuthenticatedAsync(currentUserService, cancellationToken);
        var query = dbContext.ExerciseParticipants.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns exercise unit contacts that are stored outside the participant list.")]
    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<ExerciseUnitContact>> GetExerciseUnitContacts(
        CoucherDbContext dbContext,
        [Service] IGraphQlCurrentUserService currentUserService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAuthenticatedAsync(currentUserService, cancellationToken);
        var query = dbContext.ExerciseUnitContacts.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns influencer links selected for each exercise.")]
    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<ExerciseInfluencer>> GetExerciseInfluencers(
        CoucherDbContext dbContext,
        [Service] IGraphQlCurrentUserService currentUserService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAuthenticatedAsync(currentUserService, cancellationToken);
        var query = dbContext.ExerciseInfluencers.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns section links selected for each exercise.")]
    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<ExerciseSection>> GetExerciseSections(
        CoucherDbContext dbContext,
        [Service] IGraphQlCurrentUserService currentUserService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAuthenticatedAsync(currentUserService, cancellationToken);
        var query = dbContext.ExerciseSections.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns tasks that belong to exercises.")]
    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<ExerciseTask>> GetExerciseTasks(
        CoucherDbContext dbContext,
        [Service] IGraphQlCurrentUserService currentUserService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAuthenticatedAsync(currentUserService, cancellationToken);
        var query = dbContext.ExerciseTasks.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns links between exercise tasks and their responsible users.")]
    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<ExerciseTaskResponsibleUser>> GetExerciseTaskResponsibleUsers(
        CoucherDbContext dbContext,
        [Service] IGraphQlCurrentUserService currentUserService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAuthenticatedAsync(currentUserService, cancellationToken);
        var query = dbContext.ExerciseTaskResponsibleUsers.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns dependency links between exercise tasks.")]
    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<TaskDependency>> GetTaskDependencies(
        CoucherDbContext dbContext,
        [Service] IGraphQlCurrentUserService currentUserService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAuthenticatedAsync(currentUserService, cancellationToken);
        var query = dbContext.TaskDependencies.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns reusable task templates that can be imported into exercises.")]
    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<TaskTemplate>> GetTaskTemplates(
        CoucherDbContext dbContext,
        [Service] IGraphQlCurrentUserService currentUserService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAuthenticatedAsync(currentUserService, cancellationToken);
        var query = dbContext.TaskTemplates.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns dependency links between task templates.")]
    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<TaskTemplateDependency>> GetTaskTemplateDependencies(
        CoucherDbContext dbContext,
        [Service] IGraphQlCurrentUserService currentUserService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAuthenticatedAsync(currentUserService, cancellationToken);
        var query = dbContext.TaskTemplateDependencies.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns influencer links attached to task templates.")]
    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<TaskTemplateInfluencer>> GetTaskTemplateInfluencers(
        CoucherDbContext dbContext,
        [Service] IGraphQlCurrentUserService currentUserService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAuthenticatedAsync(currentUserService, cancellationToken);
        var query = dbContext.TaskTemplateInfluencers.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns user profiles.")]
    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<UserProfile>> GetUsers(
        CoucherDbContext dbContext,
        [Service] IGraphQlCurrentUserService currentUserService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAuthenticatedAsync(currentUserService, cancellationToken);
        var query = dbContext.UserProfiles.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns global role assignments for users.")]
    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<UserRole>> GetUserRoles(
        CoucherDbContext dbContext,
        [Service] IGraphQlCurrentUserService currentUserService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAuthenticatedAsync(currentUserService, cancellationToken);
        var query = dbContext.UserRoles.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns notifications shown to users.")]
    [UseDbContext(typeof(CoucherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<UserNotification>> GetUserNotifications(
        CoucherDbContext dbContext,
        [Service] IGraphQlCurrentUserService currentUserService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAuthenticatedAsync(currentUserService, cancellationToken);
        var query = dbContext.UserNotifications.AsNoTracking();

        return query;
    }

    private static async Task EnsureAuthenticatedAsync(
        IGraphQlCurrentUserService currentUserService,
        CancellationToken cancellationToken
    )
    {
        _ = await currentUserService.GetRequiredCurrentUserIdAsync(cancellationToken);
    }
}
