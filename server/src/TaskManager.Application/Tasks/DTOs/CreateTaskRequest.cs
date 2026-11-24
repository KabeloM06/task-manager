using TaskManager.Domain.Tasks;

namespace TaskManager.Application.Tasks.DTOs;

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateOnly? DueDate);