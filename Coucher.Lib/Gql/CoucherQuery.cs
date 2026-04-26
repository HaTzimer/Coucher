using Coucher.Lib.DAL;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Models.DAL.Tasks;
using Coucher.Shared.Models.DAL.Users;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Lib.Gql;

public sealed class CoucherQuery
{
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
    public IQueryable<ExerciseTask> GetExerciseTasks(CoucherDbContext dbContext)
    {
        var query = dbContext.ExerciseTasks.AsNoTracking();

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
}
