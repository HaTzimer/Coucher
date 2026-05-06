using Augustus.Infra.Core.Shared.Interfaces;
using Coacher.Lib.DAL;
using Coacher.Lib.Mocking;
using Coacher.Shared.Interfaces.Services;
using Coacher.Shared.Models.Internal.Mocking;
using Microsoft.EntityFrameworkCore;

namespace Coacher.Lib.Services;

public sealed class MockSeedService : IMockSeedService
{
    private readonly IAugustusLogger _logger;
    private readonly IDbContextFactory<CoacherDbContext> _dbContextFactory;
    private readonly MockDataGenerator _generator;

    public MockSeedService(
        IAugustusLogger logger,
        IDbContextFactory<CoacherDbContext> dbContextFactory
    )
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _generator = new MockDataGenerator();
    }

    public async Task<MockSeedSummary> SeedAsync(MockSeedOptions options, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        if (ShouldLogMockSeed())
            _logger.Info("Mock seed started", new Dictionary<string, object>
            {
                { "resetExisting", options.ResetExisting },
                { "result", "started" }
            });

        var hasAnyData = await dbContext.UserProfiles.AsNoTracking().AnyAsync(cancellationToken)
                         || await dbContext.Exercises.AsNoTracking().AnyAsync(cancellationToken);

        if (hasAnyData && !options.ResetExisting)
        {
            var summary = new MockSeedSummary
            {
                WasReset = false,
                Note = "Database already contains data. Re-run with ResetExisting=true to wipe and seed."
            };

            if (ShouldLogMockSeed())
                _logger.Info("Mock seed skipped", new Dictionary<string, object>
                {
                    { "resetExisting", options.ResetExisting },
                    { "result", "skipped" }
                });

            return summary;
        }

        var data = _generator.Generate(options);

        await using var tx = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        if (options.ResetExisting)
            await ResetAsync(dbContext, cancellationToken);

        dbContext.ClosedListItems.AddRange(data.ClosedListItems);
        dbContext.Units.AddRange(data.Units);
        dbContext.UserProfiles.AddRange(data.Users);
        dbContext.ExternalIds.AddRange(data.ExternalIds);
        dbContext.UserRoles.AddRange(data.UserRoles);
        dbContext.Exercises.AddRange(data.Exercises);
        dbContext.ExerciseParticipants.AddRange(data.ExerciseParticipants);
        dbContext.ExerciseUnitContacts.AddRange(data.ExerciseUnitContacts);
        dbContext.ExerciseInfluencers.AddRange(data.ExerciseInfluencers);
        dbContext.ExerciseSections.AddRange(data.ExerciseSections);
        dbContext.TaskTemplates.AddRange(data.TaskTemplates);
        dbContext.TaskTemplateDependencies.AddRange(data.TaskTemplateDependencies);
        dbContext.TaskTemplateInfluencers.AddRange(data.TaskTemplateInfluencers);
        dbContext.ExerciseTasks.AddRange(data.ExerciseTasks);
        dbContext.ExerciseTaskResponsibleUsers.AddRange(data.ExerciseTaskResponsibleUsers);
        dbContext.TaskDependencies.AddRange(data.TaskDependencies);
        dbContext.UserNotifications.AddRange(data.UserNotifications);

        await dbContext.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);

        var summaryResult = new MockSeedSummary
        {
            WasReset = options.ResetExisting,
            ClosedListItemCount = data.ClosedListItems.Count,
            UnitCount = data.Units.Count,
            UserCount = data.Users.Count,
            ExternalIdCount = data.ExternalIds.Count,
            UserRoleCount = data.UserRoles.Count,
            ExerciseCount = data.Exercises.Count,
            ExerciseParticipantCount = data.ExerciseParticipants.Count,
            ExerciseUnitContactCount = data.ExerciseUnitContacts.Count,
            ExerciseInfluencerCount = data.ExerciseInfluencers.Count,
            ExerciseSectionCount = data.ExerciseSections.Count,
            TaskTemplateCount = data.TaskTemplates.Count,
            TaskTemplateDependencyCount = data.TaskTemplateDependencies.Count,
            TaskTemplateInfluencerCount = data.TaskTemplateInfluencers.Count,
            ExerciseTaskCount = data.ExerciseTasks.Count,
            ExerciseTaskResponsibleUserCount = data.ExerciseTaskResponsibleUsers.Count,
            TaskDependencyCount = data.TaskDependencies.Count,
            UserNotificationCount = data.UserNotifications.Count,
            Note = null
        };

        if (ShouldLogMockSeed())
            _logger.Info("Mock seed completed", new Dictionary<string, object>
            {
                { "resetExisting", options.ResetExisting },
                { "exerciseCount", data.Exercises.Count },
                { "exerciseTaskCount", data.ExerciseTasks.Count },
                { "taskTemplateCount", data.TaskTemplates.Count },
                { "userCount", data.Users.Count },
                { "result", "success" }
            });

        return summaryResult;
    }

    private static async Task ResetAsync(CoacherDbContext dbContext, CancellationToken cancellationToken)
    {
        // Delete children before parents (FKs are NO ACTION by default in this repo).
        var statements = new[]
        {
            "DELETE FROM [TaskDependencies]",
            "DELETE FROM [ExerciseTaskResponsibleUsers]",
            "DELETE FROM [ExerciseTasks]",
            "DELETE FROM [ExerciseInfluencers]",
            "DELETE FROM [ExerciseSections]",
            "DELETE FROM [ExerciseParticipants]",
            "DELETE FROM [ExerciseUnitContacts]",
            "DELETE FROM [UserNotifications]",
            "DELETE FROM [Exercises]",
            "DELETE FROM [TaskTemplateDependencies]",
            "DELETE FROM [TaskTemplateInfluencers]",
            "DELETE FROM [TaskTemplates]",
            "DELETE FROM [ExternalIds]",
            "DELETE FROM [UserRoles]",
            "DELETE FROM [UserProfiles]",
            "DELETE FROM [Units]",
            "DELETE FROM [ClosedListItems]"
        };

        foreach (var sql in statements)
        {
            await dbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }
    }

    private static bool ShouldLogMockSeed()
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        return string.Equals(environmentName, "Development", StringComparison.OrdinalIgnoreCase)
               || string.Equals(environmentName, "Local", StringComparison.OrdinalIgnoreCase);
    }
}
