using Coucher.Lib.DAL;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Models.DAL.Tasks;
using Coucher.Shared.Models.DAL.Users;
using HotChocolate;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Lib.Gql;

public sealed class CoucherQuery
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [UseDbContext(typeof(CoucherDbContext))]
    public IQueryable<Exercise> GetExercises([ScopedService] CoucherDbContext dbContext)
    {
        var query = dbContext.Exercises.AsNoTracking();

        return query;
    }

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [UseDbContext(typeof(CoucherDbContext))]
    public IQueryable<ExerciseTask> GetExerciseTasks([ScopedService] CoucherDbContext dbContext)
    {
        var query = dbContext.ExerciseTasks.AsNoTracking();

        return query;
    }

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [UseDbContext(typeof(CoucherDbContext))]
    public IQueryable<UserProfile> GetUsers([ScopedService] CoucherDbContext dbContext)
    {
        var query = dbContext.UserProfiles.AsNoTracking();

        return query;
    }
}
