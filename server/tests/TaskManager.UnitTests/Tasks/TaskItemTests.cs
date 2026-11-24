using TaskManager.Domain.Tasks;
using TaskManager.Domain.Tasks.Exceptions;

namespace TaskManager.UnitTests.Tasks;

public class TaskItemTests
{
    private static readonly DateTimeOffset CreatedAt =
        new(2026, 11, 24, 8, 0, 0, TimeSpan.Zero);

    private static readonly DateTimeOffset CompletedAt =
        new(2026, 11, 24, 9, 0, 0, TimeSpan.Zero);

    private static readonly DateTimeOffset ReopenedAt =
        new(2026, 11, 24, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Update_WhenTaskIsPending_UpdatesTaskDetails()
    {
        // Arrange
        var task = CreateTask();

        // Act
        task.Update(
            "Updated title",
            "Updated description",
            TaskPriority.High,
            new DateOnly(2026, 12, 1),
            CompletedAt);

        // Assert
        Assert.Equal("Updated title", task.Title);
        Assert.Equal("Updated description", task.Description);
        Assert.Equal(TaskPriority.High, task.Priority);
        Assert.Equal(new DateOnly(2026, 12, 1), task.DueDate);
        Assert.Equal(CompletedAt, task.UpdatedAtUtc);
    }

    [Fact]
    public void Update_WhenTaskIsCompleted_ThrowsException()
    {
        // Arrange
        var task = CreateCompletedTask();

        // Act
        var exception = Assert.Throws<CompletedTaskCannotBeEditedException>(
            () => task.Update(
                "Updated title",
                "Updated description",
                TaskPriority.High,
                null,
                ReopenedAt));

        // Assert
        Assert.Equal(task.Id, exception.TaskId);
    }

    [Fact]
    public void Update_WhenTaskIsCompleted_PreservesOriginalDetails()
    {
        // Arrange
        var task = CreateCompletedTask();

        var originalTitle = task.Title;
        var originalDescription = task.Description;
        var originalPriority = task.Priority;
        var originalDueDate = task.DueDate;

        // Act
        Assert.Throws<CompletedTaskCannotBeEditedException>(
            () => task.Update(
                "Updated title",
                "Updated description",
                TaskPriority.High,
                null,
                ReopenedAt));

        // Assert
        Assert.Equal(originalTitle, task.Title);
        Assert.Equal(originalDescription, task.Description);
        Assert.Equal(originalPriority, task.Priority);
        Assert.Equal(originalDueDate, task.DueDate);
    }

    [Fact]
    public void Update_WhenTaskIsCompleted_PreservesAuditTimestamps()
    {
        // Arrange
        var task = CreateCompletedTask();

        var originalUpdatedAt = task.UpdatedAtUtc;
        var originalCompletedAt = task.CompletedAtUtc;

        // Act
        Assert.Throws<CompletedTaskCannotBeEditedException>(
            () => task.Update(
                "Updated title",
                "Updated description",
                TaskPriority.High,
                null,
                ReopenedAt));

        // Assert
        Assert.Equal(originalUpdatedAt, task.UpdatedAtUtc);
        Assert.Equal(originalCompletedAt, task.CompletedAtUtc);
    }

    [Fact]
    public void Update_AfterTaskIsReopened_AllowsChanges()
    {
        // Arrange
        var task = CreateCompletedTask();

        task.Reopen(ReopenedAt);

        // Act
        task.Update(
            "Updated title",
            "Updated description",
            TaskPriority.High,
            null,
            ReopenedAt.AddMinutes(5));

        // Assert
        Assert.Equal("Updated title", task.Title);
        Assert.Equal("Updated description", task.Description);
        Assert.Equal(TaskPriority.High, task.Priority);
        Assert.False(task.IsCompleted);
        Assert.Null(task.CompletedAtUtc);
        Assert.Equal(ReopenedAt.AddMinutes(5), task.UpdatedAtUtc);
    }

    private static TaskItem CreateTask()
    {
        return TaskItem.Create(
            "Original title",
            "Original description",
            TaskPriority.Medium,
            new DateOnly(2026, 11, 30),
            CreatedAt);
    }

    private static TaskItem CreateCompletedTask()
    {
        var task = CreateTask();
        task.MarkCompleted(CompletedAt);

        return task;
    }
}
