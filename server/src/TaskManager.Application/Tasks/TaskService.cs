using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Abstractions.Time;
using TaskManager.Application.Tasks.DTOs;
using TaskManager.Application.Tasks.Exceptions;
using TaskManager.Domain.Tasks;

namespace TaskManager.Application.Tasks;

public sealed class TaskService(
    ITaskRepository repository,
    IClock clock)
{
    public async Task<TaskResponse> CreateTaskAsync(
        CreateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var task = TaskItem.Create(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            clock.UtcNow);

        await repository.AddAsync(task, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(task);
    }

    public async Task<IReadOnlyList<TaskResponse>> GetAllTasksAsync(
        CancellationToken cancellationToken = default)
    {
        var tasks = await repository.GetAllAsync(cancellationToken);

        return tasks.Select(MapToResponse).ToList();
    }

    public async Task<TaskResponse> GetTaskByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var task = await GetTaskOrThrowAsync(id, cancellationToken);

        return MapToResponse(task);
    }

    public async Task<TaskResponse> UpdateTaskAsync(
        Guid id,
        UpdateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var task = await GetTaskOrThrowAsync(id, cancellationToken);

        task.Update(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            clock.UtcNow);

        await repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(task);
    }

    public async Task<TaskResponse> CompleteTaskAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var task = await GetTaskOrThrowAsync(id, cancellationToken);

        task.MarkCompleted(clock.UtcNow);

        await repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(task);
    }

    public async Task<TaskResponse> ReopenTaskAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var task = await GetTaskOrThrowAsync(id, cancellationToken);

        task.Reopen(clock.UtcNow);

        await repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(task);
    }

    public async Task DeleteTaskAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var task = await GetTaskOrThrowAsync(id, cancellationToken);

        repository.Remove(task);

        await repository.SaveChangesAsync(cancellationToken);
    }

    private async Task<TaskItem> GetTaskOrThrowAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new TaskNotFoundException(id);
    }

    private static TaskResponse MapToResponse(TaskItem task)
    {
        return new TaskResponse(
            task.Id,
            task.Title,
            task.Description,
            task.Priority,
            task.DueDate,
            task.IsCompleted,
            task.CreatedAtUtc,
            task.UpdatedAtUtc,
            task.CompletedAtUtc);
    }
}