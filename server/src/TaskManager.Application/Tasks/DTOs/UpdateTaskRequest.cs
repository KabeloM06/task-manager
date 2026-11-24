using TaskManager.Domain.Tasks;

namespace TaskManager.Application.Tasks.DTOs;

public sealed record UpdateTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateOnly? DueDate);