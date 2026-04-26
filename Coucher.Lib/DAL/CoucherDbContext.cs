using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Models.DAL.Notifications;
using Coucher.Shared.Models.DAL.Tasks;
using Coucher.Shared.Models.DAL.Users;
using Coucher.Shared.Models.Internal.Projections.Exercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Coucher.Lib.DAL;

public sealed class CoucherDbContext : DbContext
{
    public CoucherDbContext(DbContextOptions<CoucherDbContext> options)
        : base(options)
    {
    }

    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<ExerciseParticipant> ExerciseParticipants => Set<ExerciseParticipant>();
    public DbSet<ExerciseInfluencer> ExerciseInfluencers => Set<ExerciseInfluencer>();
    public DbSet<ExerciseSection> ExerciseSections => Set<ExerciseSection>();
    public DbSet<ExerciseTask> ExerciseTasks => Set<ExerciseTask>();
    public DbSet<TaskDependency> TaskDependencies => Set<TaskDependency>();
    public DbSet<ExerciseTaskResponsibleUser> ExerciseTaskResponsibleUsers => Set<ExerciseTaskResponsibleUser>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<UserNotification> UserNotifications => Set<UserNotification>();
    public DbSet<ClosedListItem> ClosedListItems => Set<ClosedListItem>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<TaskTemplate> TaskTemplates => Set<TaskTemplate>();
    public DbSet<TaskTemplateDependency> TaskTemplateDependencies => Set<TaskTemplateDependency>();
    public DbSet<TaskTemplateInfluencer> TaskTemplateInfluencers => Set<TaskTemplateInfluencer>();
    public DbSet<ExerciseSummary> ExerciseSummaries => Set<ExerciseSummary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // EF Core 7 SQL Server provider doesn't map DateOnly automatically.
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
            dateTime => DateOnly.FromDateTime(dateTime)
        );

        modelBuilder.Entity<Exercise>()
            .Property(e => e.StartDate)
            .HasConversion(dateOnlyConverter)
            .HasColumnType("date");

        modelBuilder.Entity<Exercise>()
            .Property(e => e.EndDate)
            .HasConversion(dateOnlyConverter)
            .HasColumnType("date");

        modelBuilder.Entity<ExerciseSummary>()
            .Property(e => e.StartDate)
            .HasConversion(dateOnlyConverter)
            .HasColumnType("date");

        modelBuilder.Entity<ExerciseSummary>()
            .Property(e => e.EndDate)
            .HasConversion(dateOnlyConverter)
            .HasColumnType("date");
    }
}
