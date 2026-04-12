namespace Coucher.Shared.Models.DAL.Tasks;

public sealed class TaskDependency
{
    public Guid Id { get; set; }
    public Guid ExerciseTaskId { get; set; }
    public Guid DependsOnTaskId { get; set; }
    public required ExerciseTask ExerciseTask { get; set; }
    public required ExerciseTask DependsOnTask { get; set; }
}
