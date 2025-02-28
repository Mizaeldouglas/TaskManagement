using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.UnitTests.Application.Commands;

public class UpdateTaskStatusCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Fixture _fixture;
    private readonly UpdateTaskStatusCommandHandler _handler;

    public UpdateTaskStatusCommandHandlerTests()
    {
        _mockRepository = new Mock<ITaskRepository>();
        _mockMapper = new Mock<IMapper>();
        _fixture = new Fixture();
        _handler = new UpdateTaskStatusCommandHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_WithExistingTask_ShouldUpdateStatusAndReturnTaskDto()
    {
        // Arrange
        var existingTask = new TaskItem("Test Task", "Description");
        existingTask.Id = 1;
        existingTask.Status = TaskManagement.Domain.Entities.TaskStatus.Pending;

        var command = new UpdateTaskStatusCommand
        {
            Id = existingTask.Id,
            Status = "InProgress"
        };

        var taskDto = new TaskDto
        {
            Id = existingTask.Id,
            Title = existingTask.Title,
            Status = command.Status
        };

        _mockRepository.Setup(r => r.GetByIdAsync(command.Id))
            .ReturnsAsync(existingTask);

        _mockMapper.Setup(m => m.Map<TaskDto>(It.IsAny<TaskItem>()))
            .Returns(taskDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(taskDto);

        _mockRepository.Verify(r => r.UpdateAsync(It.Is<TaskItem>(t =>
            t.Id == existingTask.Id &&
            t.Status == TaskManagement.Domain.Entities.TaskStatus.InProgress)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistingTask_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new UpdateTaskStatusCommand
        {
            Id = 99,
            Status = "Completed"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(command.Id))
            .ReturnsAsync((TaskItem?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entidade \"Tarefa\" ({command.Id}) não foi encontrada.");

        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Never);
    }


    [Fact]
    public async Task Handle_WithInvalidStatus_ShouldThrowValidationException()
    {
        // Arrange
        var existingTask = new TaskItem("Test Task", "Description");
        existingTask.Id = 1;

        var command = new UpdateTaskStatusCommand
        {
            Id = existingTask.Id,
            Status = "InvalidStatus"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(command.Id))
            .ReturnsAsync(existingTask);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<ValidationException>()
            .WithMessage("Status inválido: InvalidStatus");

        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenChangingStatusToCompleted_ShouldSetCompletionDate()
    {
        // Arrange
        var existingTask = new TaskItem("Test Task", "Description");
        existingTask.Id = 1;
        existingTask.Status = TaskManagement.Domain.Entities.TaskStatus.InProgress;
        existingTask.CompletionDate = null;

        var command = new UpdateTaskStatusCommand
        {
            Id = existingTask.Id,
            Status = "Completed"
        };

        var taskDto = new TaskDto { Id = existingTask.Id, Title = existingTask.Title };

        _mockRepository.Setup(r => r.GetByIdAsync(command.Id))
            .ReturnsAsync(existingTask);

        _mockMapper.Setup(m => m.Map<TaskDto>(It.IsAny<TaskItem>()))
            .Returns(taskDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<TaskItem>(t =>
            t.Id == existingTask.Id &&
            t.Status == TaskManagement.Domain.Entities.TaskStatus.Completed &&
            t.CompletionDate != null)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenChangingStatusFromCompleted_ShouldClearCompletionDate()
    {
        // Arrange
        var existingTask = new TaskItem("Test Task", "Description");
        existingTask.Id = 1;
        existingTask.Status = TaskManagement.Domain.Entities.TaskStatus.Completed;
        existingTask.CompletionDate = DateTime.UtcNow;

        var command = new UpdateTaskStatusCommand
        {
            Id = existingTask.Id,
            Status = "InProgress"
        };

        var taskDto = new TaskDto { Id = existingTask.Id, Title = existingTask.Title };

        _mockRepository.Setup(r => r.GetByIdAsync(command.Id))
            .ReturnsAsync(existingTask);

        _mockMapper.Setup(m => m.Map<TaskDto>(It.IsAny<TaskItem>()))
            .Returns(taskDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<TaskItem>(t =>
            t.Id == existingTask.Id &&
            t.Status == TaskManagement.Domain.Entities.TaskStatus.InProgress &&
            t.CompletionDate == null)),
            Times.Once);
    }
}
