using Coacher.Lib.DAL;
using Coacher.Shared.Interfaces.Services;
using Coacher.Shared.Models.DAL.Admin;
using Coacher.Shared.Models.DAL.Exercises;
using Coacher.Shared.Models.DAL.Notifications;
using Coacher.Shared.Models.DAL.Tasks;
using Coacher.Shared.Models.DAL.Users;
using Coacher.Shared.Models.Internal.Authorization;
using HotChocolate;
using Microsoft.EntityFrameworkCore;

namespace Coacher.Lib.Gql;

public sealed class CoacherQuery
{
    [GraphQLDescription("Returns configurable closed-list entries used by dropdowns, statuses, and catalogs.")]
    [UseDbContext(typeof(CoacherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<ClosedListItem>> GetClosedListItems(
        CoacherDbContext dbContext,
        [Service] ICoacherAuthorizationService authorizationService,
        CancellationToken cancellationToken
    )
    {
        _ = await authorizationService.GetCurrentAuthorizationSnapshotAsync(cancellationToken);
        var query = dbContext.ClosedListItems.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns organizational units that can be assigned to users and exercises.")]
    [UseDbContext(typeof(CoacherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<Unit>> GetUnits(
        CoacherDbContext dbContext,
        [Service] ICoacherAuthorizationService authorizationService,
        CancellationToken cancellationToken
    )
    {
        _ = await authorizationService.GetCurrentAuthorizationSnapshotAsync(cancellationToken);
        var query = dbContext.Units.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns exercises with their core planning and execution data.")]
    [UseDbContext(typeof(CoacherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<Exercise>> GetExercises(
        CoacherDbContext dbContext,
        [Service] ICoacherAuthorizationService authorizationService,
        CancellationToken cancellationToken
    )
    {
        var snapshot = await authorizationService.GetCurrentAuthorizationSnapshotAsync(cancellationToken);
        var visibleExerciseIds = GetVisibleExerciseIdsQuery(dbContext, snapshot);
        var query = dbContext.Exercises
            .AsNoTracking()
            .Where(item => visibleExerciseIds.Contains(item.Id));

        return query;
    }

    [GraphQLDescription("Returns user-to-exercise participation records.")]
    [UseDbContext(typeof(CoacherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<ExerciseParticipant>> GetExerciseParticipants(
        CoacherDbContext dbContext,
        [Service] ICoacherAuthorizationService authorizationService,
        CancellationToken cancellationToken
    )
    {
        var snapshot = await authorizationService.GetCurrentAuthorizationSnapshotAsync(cancellationToken);
        var visibleExerciseIds = GetVisibleExerciseIdsQuery(dbContext, snapshot);
        var query = dbContext.ExerciseParticipants
            .AsNoTracking()
            .Where(item => item.ExerciseId.HasValue && visibleExerciseIds.Contains(item.ExerciseId.Value));

        return query;
    }

    [GraphQLDescription("Returns exercise unit contacts that are stored outside the participant list.")]
    [UseDbContext(typeof(CoacherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<ExerciseUnitContact>> GetExerciseUnitContacts(
        CoacherDbContext dbContext,
        [Service] ICoacherAuthorizationService authorizationService,
        CancellationToken cancellationToken
    )
    {
        var snapshot = await authorizationService.GetCurrentAuthorizationSnapshotAsync(cancellationToken);
        var visibleExerciseIds = GetVisibleExerciseIdsQuery(dbContext, snapshot);
        var query = dbContext.ExerciseUnitContacts
            .AsNoTracking()
            .Where(item => item.ExerciseId.HasValue && visibleExerciseIds.Contains(item.ExerciseId.Value));

        return query;
    }

    [GraphQLDescription("Returns influencer links selected for each exercise.")]
    [UseDbContext(typeof(CoacherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<ExerciseInfluencer>> GetExerciseInfluencers(
        CoacherDbContext dbContext,
        [Service] ICoacherAuthorizationService authorizationService,
        CancellationToken cancellationToken
    )
    {
        var snapshot = await authorizationService.GetCurrentAuthorizationSnapshotAsync(cancellationToken);
        var visibleExerciseIds = GetVisibleExerciseIdsQuery(dbContext, snapshot);
        var query = dbContext.ExerciseInfluencers
            .AsNoTracking()
            .Where(item => item.ExerciseId.HasValue && visibleExerciseIds.Contains(item.ExerciseId.Value));

        return query;
    }

    [GraphQLDescription("Returns section links selected for each exercise.")]
    [UseDbContext(typeof(CoacherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<ExerciseSection>> GetExerciseSections(
        CoacherDbContext dbContext,
        [Service] ICoacherAuthorizationService authorizationService,
        CancellationToken cancellationToken
    )
    {
        var snapshot = await authorizationService.GetCurrentAuthorizationSnapshotAsync(cancellationToken);
        var visibleExerciseIds = GetVisibleExerciseIdsQuery(dbContext, snapshot);
        var query = dbContext.ExerciseSections
            .AsNoTracking()
            .Where(item => item.ExerciseId.HasValue && visibleExerciseIds.Contains(item.ExerciseId.Value));

        return query;
    }

    [GraphQLDescription("Returns tasks that belong to exercises.")]
    [UseDbContext(typeof(CoacherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<ExerciseTask>> GetExerciseTasks(
        CoacherDbContext dbContext,
        [Service] ICoacherAuthorizationService authorizationService,
        CancellationToken cancellationToken
    )
    {
        var snapshot = await authorizationService.GetCurrentAuthorizationSnapshotAsync(cancellationToken);
        var visibleExerciseIds = GetVisibleExerciseIdsQuery(dbContext, snapshot);
        var query = dbContext.ExerciseTasks
            .AsNoTracking()
            .Where(item => visibleExerciseIds.Contains(item.ExerciseId));

        return query;
    }

    [GraphQLDescription("Returns links between exercise tasks and their responsible users.")]
    [UseDbContext(typeof(CoacherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<ExerciseTaskResponsibleUser>> GetExerciseTaskResponsibleUsers(
        CoacherDbContext dbContext,
        [Service] ICoacherAuthorizationService authorizationService,
        CancellationToken cancellationToken
    )
    {
        var snapshot = await authorizationService.GetCurrentAuthorizationSnapshotAsync(cancellationToken);
        var visibleTaskIds = GetVisibleTaskIdsQuery(dbContext, snapshot);
        var query = dbContext.ExerciseTaskResponsibleUsers
            .AsNoTracking()
            .Where(item => item.TaskId.HasValue && visibleTaskIds.Contains(item.TaskId.Value));

        return query;
    }

    [GraphQLDescription("Returns dependency links between exercise tasks.")]
    [UseDbContext(typeof(CoacherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<TaskDependency>> GetTaskDependencies(
        CoacherDbContext dbContext,
        [Service] ICoacherAuthorizationService authorizationService,
        CancellationToken cancellationToken
    )
    {
        var snapshot = await authorizationService.GetCurrentAuthorizationSnapshotAsync(cancellationToken);
        var visibleTaskIds = GetVisibleTaskIdsQuery(dbContext, snapshot);
        var query = dbContext.TaskDependencies
            .AsNoTracking()
            .Where(item => item.TaskId.HasValue && visibleTaskIds.Contains(item.TaskId.Value));

        return query;
    }

    [GraphQLDescription("Returns reusable task templates that can be imported into exercises.")]
    [UseDbContext(typeof(CoacherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<TaskTemplate>> GetTaskTemplates(
        CoacherDbContext dbContext,
        [Service] ICoacherAuthorizationService authorizationService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAdminAccessAsync(authorizationService, cancellationToken);
        var query = dbContext.TaskTemplates.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns dependency links between task templates.")]
    [UseDbContext(typeof(CoacherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<TaskTemplateDependency>> GetTaskTemplateDependencies(
        CoacherDbContext dbContext,
        [Service] ICoacherAuthorizationService authorizationService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAdminAccessAsync(authorizationService, cancellationToken);
        var query = dbContext.TaskTemplateDependencies.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns influencer links attached to task templates.")]
    [UseDbContext(typeof(CoacherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<TaskTemplateInfluencer>> GetTaskTemplateInfluencers(
        CoacherDbContext dbContext,
        [Service] ICoacherAuthorizationService authorizationService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAdminAccessAsync(authorizationService, cancellationToken);
        var query = dbContext.TaskTemplateInfluencers.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns user profiles.")]
    [UseDbContext(typeof(CoacherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<UserProfile>> GetUsers(
        CoacherDbContext dbContext,
        [Service] ICoacherAuthorizationService authorizationService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAdminAccessAsync(authorizationService, cancellationToken);
        var query = dbContext.UserProfiles.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns external identifiers attached to users.")]
    [UseDbContext(typeof(CoacherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<ExternalId>> GetExternalIds(
        CoacherDbContext dbContext,
        [Service] ICoacherAuthorizationService authorizationService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAdminAccessAsync(authorizationService, cancellationToken);
        var query = dbContext.ExternalIds.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns global role assignments for users.")]
    [UseDbContext(typeof(CoacherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<UserRole>> GetUserRoles(
        CoacherDbContext dbContext,
        [Service] ICoacherAuthorizationService authorizationService,
        CancellationToken cancellationToken
    )
    {
        await EnsureAdminAccessAsync(authorizationService, cancellationToken);
        var query = dbContext.UserRoles.AsNoTracking();

        return query;
    }

    [GraphQLDescription("Returns notifications shown to users.")]
    [UseDbContext(typeof(CoacherDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<UserNotification>> GetUserNotifications(
        CoacherDbContext dbContext,
        [Service] ICoacherAuthorizationService authorizationService,
        CancellationToken cancellationToken
    )
    {
        var snapshot = await authorizationService.GetCurrentAuthorizationSnapshotAsync(cancellationToken);
        var query = dbContext.UserNotifications
            .AsNoTracking()
            .Where(item => item.UserId == snapshot.UserId);

        return query;
    }

    private static IQueryable<Guid> GetVisibleExerciseIdsQuery(
        CoacherDbContext dbContext,
        CurrentAuthorizationSnapshot snapshot
    )
    {
        var exercises = dbContext.Exercises.AsNoTracking();
        if (snapshot.IsAdmin || snapshot.IsAuditor)
        {
            var allExerciseIds = exercises.Select(item => item.Id);

            return allExerciseIds;
        }

        var visibleExerciseIds = exercises
            .Where(item =>
                item.CreatedByUserId == snapshot.UserId
                || item.Participants.Any(participant => participant.UserId == snapshot.UserId)
            )
            .Select(item => item.Id);

        return visibleExerciseIds;
    }

    private static IQueryable<Guid> GetVisibleTaskIdsQuery(
        CoacherDbContext dbContext,
        CurrentAuthorizationSnapshot snapshot
    )
    {
        var visibleExerciseIds = GetVisibleExerciseIdsQuery(dbContext, snapshot);
        var visibleTaskIds = dbContext.ExerciseTasks
            .AsNoTracking()
            .Where(item => visibleExerciseIds.Contains(item.ExerciseId))
            .Select(item => item.Id);

        return visibleTaskIds;
    }

    private static async Task EnsureAdminAccessAsync(
        ICoacherAuthorizationService authorizationService,
        CancellationToken cancellationToken
    )
    {
        await authorizationService.EnsureCanAccessAdminSurfaceAsync(cancellationToken);
    }
}
