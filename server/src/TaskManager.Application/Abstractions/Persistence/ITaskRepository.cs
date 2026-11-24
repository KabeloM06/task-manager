using TaskManager.Domain.Tasks;

namespace TaskManager.Application.Abstractions.Persistence;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItem>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        TaskItem task,
        CancellationToken cancellationToken = default);

    void Remove(TaskItem task);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
