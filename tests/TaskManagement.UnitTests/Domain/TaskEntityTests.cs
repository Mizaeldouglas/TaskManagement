using FluentAssertions;
using TaskManagement.Domain.Entities;

namespace TaskManagement.UnitTests.Domain;

public class TaskEntityTests
{
    [Fact]
    public void Create_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var task = new TaskItem("Teste de Tarefa", "Descrição da tarefa");

        // Assert
        task.Title.Should().Be("Teste de Tarefa");
        task.Description.Should().Be("Descrição da tarefa");
        task.Status.Should().Be(TaskManagement.Domain.Entities.TaskStatus.Pending);
        task.CreationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        task.CompletionDate.Should().BeNull();
    }

    [Fact]
    public void MarkAsCompleted_ShouldSetCompletionDate()
    {
        // Arrange
        var task = new TaskItem("Tarefa", "Descrição");

        // Act
        task.MarkAsCompleted();

        // Assert
        task.Status.Should().Be(TaskManagement.Domain.Entities.TaskStatus.Completed);
        task.CompletionDate.Should().NotBeNull();
        task.CompletionDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void MarkAsInProgress_FromCompleted_ShouldThrowException()
    {
        // Arrange
        var task = new TaskItem("Tarefa", "Descrição");
        task.MarkAsCompleted();

        // Act & Assert
        task.Invoking(t => t.MarkAsInProgress())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot mark a completed task as in progress");
    }

    [Fact]
    public void MarkAsPending_FromCompleted_ShouldRemoveCompletionDate()
    {
        // Arrange
        var task = new TaskItem("Tarefa", "Descrição");
        task.MarkAsCompleted();

        // Act
        task.MarkAsPending();

        // Assert
        task.Status.Should().Be(TaskManagement.Domain.Entities.TaskStatus.Pending);
        task.CompletionDate.Should().BeNull();
    }
}