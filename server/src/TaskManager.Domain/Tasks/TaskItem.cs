using TaskManager.Domain.Tasks.Exceptions;

namespace TaskManager.Domain.Tasks;

public sealed class TaskItem
{
    // EF Core uses this constructor when materializing entities.
    private TaskItem()
    {
    }

    private TaskItem(
        Guid id,
        string title,
        string? description,
        TaskPriority priority,
        DateOnly? dueDate,
        DateTimeOffset nowUtc)
    {
        Id = id;
        Title = title;
        Description = description;
        Priority = priority;
        DueDate = dueDate;
        IsCompleted = false;
        CreatedAtUtc = nowUtc;
        UpdatedAtUtc = nowUtc;
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public TaskPriority Priority { get; private set; }

    public DateOnly? DueDate { get; private set; }

    public bool IsCompleted { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public DateTimeOffset? CompletedAtUtc { get; private set; }

    public static TaskItem Create(
        string title,
        string? description,
        TaskPriority priority,
        DateOnly? dueDate,
        DateTimeOffset nowUtc)
    {
        ValidateTitle(title);
        ValidatePriority(priority);
        ValidateUtc(nowUtc);

        return new TaskItem(
            Guid.NewGuid(),
            title.Trim(),
            NormalizeDescription(description),
            priority,
            dueDate,
            nowUtc);
    }

    
public void Update(
    string title,
    string? description,
    TaskPriority priority,
    DateOnly? dueDate,
    DateTimeOffset nowUtc)
{
    if (IsCompleted)
    {
        throw new CompletedTaskCannotBeEditedException(Id);
    }

    ValidateTitle(title);
    ValidatePriority(priority);
    ValidateUtc(nowUtc);

    Title = title.Trim();
    Description = NormalizeDescription(description);
    Priority = priority;
    DueDate = dueDate;
    UpdatedAtUtc = nowUtc;
}


    public void MarkCompleted(DateTimeOffset nowUtc)
    {
        ValidateUtc(nowUtc);

        if (IsCompleted)
            return;

        IsCompleted = true;
        CompletedAtUtc = nowUtc;
        UpdatedAtUtc = nowUtc;
    }

    public void Reopen(DateTimeOffset nowUtc)
    {
        ValidateUtc(nowUtc);

        if (!IsCompleted)
            return;

        IsCompleted = false;
        CompletedAtUtc = null;
        UpdatedAtUtc = nowUtc;
    }

    public bool IsOverdue(DateOnly today)
    {
        return !IsCompleted
            && DueDate.HasValue
            && today > DueDate.Value;
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(
                "Task title is required.",
                nameof(title));
        }
    }

    private static void ValidatePriority(TaskPriority priority)
    {
        if (!Enum.IsDefined(priority))
        {
            throw new ArgumentOutOfRangeException(
                nameof(priority),
                "Invalid task priority.");
        }
    }

    private static void ValidateUtc(DateTimeOffset value)
    {
        if (value.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException(
                "Timestamp must be in UTC.");
        }
    }

    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
    }
}
