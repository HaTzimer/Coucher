using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Models.DAL.Notifications;
using Coucher.Shared.Models.DAL.Tasks;
using Coucher.Shared.Models.DAL.Users;
using Coucher.Shared.Models.Internal.Projections.Exercises;
using Microsoft.EntityFrameworkCore;

namespace Coucher.Lib.DAL;

public sealed class CoucherDbContext : DbContext
{
    public CoucherDbContext(DbContextOptions<CoucherDbContext> options)
        : base(options)
    {
    }

    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<ExerciseParticipant> ExerciseParticipants => Set<ExerciseParticipant>();
    public DbSet<ExerciseInfluencerLink> ExerciseInfluencerLinks => Set<ExerciseInfluencerLink>();
    public DbSet<ExerciseThreatArenaLink> ExerciseThreatArenaLinks => Set<ExerciseThreatArenaLink>();
    public DbSet<ExerciseTask> ExerciseTasks => Set<ExerciseTask>();
    public DbSet<TaskDependency> TaskDependencies => Set<TaskDependency>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<UserNotification> UserNotifications => Set<UserNotification>();
    public DbSet<ClosedListItem> ClosedListItems => Set<ClosedListItem>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<FixedTaskTemplate> FixedTaskTemplates => Set<FixedTaskTemplate>();
    public DbSet<FixedTaskTemplateDependency> FixedTaskTemplateDependencies => Set<FixedTaskTemplateDependency>();
    public DbSet<ExerciseSummary> ExerciseSummaries => Set<ExerciseSummary>();
}
