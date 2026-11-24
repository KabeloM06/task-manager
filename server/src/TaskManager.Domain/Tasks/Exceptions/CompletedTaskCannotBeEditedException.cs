namespace TaskManager.Domain.Tasks.Exceptions;

public sealed class CompletedTaskCannotBeEditedException(Guid taskId)
    : InvalidOperationException(
        $"Completed task '{taskId}' cannot be edited. Reopen the task before making changes.")
{
    public Guid TaskId { get; } = taskId;
}
