using TaskManager.Domain.Tasks;

namespace TaskManager.Application.Tasks.DTOs;

public sealed record TaskResponse(
    Guid Id,
    string Title,
    string? Description,
    TaskPriority Priority,
    DateOnly? DueDate,
    bool IsCompleted,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    DateTimeOffset? CompletedAtUtc);