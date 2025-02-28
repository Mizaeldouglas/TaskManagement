using AutoFixture;
using FluentAssertions;
using Moq;
using TaskManagement.Application.Features.Tasks.Commands.DeleteTask;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.UnitTests.Application.Commands;

public class DeleteTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _mockRepository;
    private readonly Fixture _fixture;
    private readonly DeleteTaskCommandHandler _handler;

    public DeleteTaskCommandHandlerTests()
    {
        _mockRepository = new Mock<ITaskRepository>();
        _fixture = new Fixture();
        _handler = new DeleteTaskCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_WithExistingTask_ShouldDeleteTask()
    {
        // Arrange
        var existingTask = new TaskItem("Test Task", "Description");
        existingTask.Id = 1;

        var command = new DeleteTaskCommand(existingTask.Id);

        _mockRepository.Setup(r => r.GetByIdAsync(command.Id))
            .ReturnsAsync(existingTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(existingTask.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistingTask_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new DeleteTaskCommand(99);

        _mockRepository.Setup(r => r.GetByIdAsync(command.Id))
            .ReturnsAsync((TaskItem?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entidade \"Task\" ({command.Id}) não foi encontrada.");

        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }
}
