using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Features.Tasks.Commands.UpdateTask;
using TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.UnitTests.Application.Commands;

public class UpdateTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _mockRepository;
    private readonly Fixture _fixture;
    private readonly Mock<IMapper> _mockMapper;
    private readonly UpdateTaskCommandHandler _handler;

    public UpdateTaskCommandHandlerTests()
    {
        _mockRepository = new Mock<ITaskRepository>();
        _mockMapper = new Mock<IMapper>();
        _fixture = new Fixture();
        _handler = new UpdateTaskCommandHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_WithExistingTask_ShouldUpdateAndReturnTaskDto()
    {
        // Arrange
        var existingTask = new TaskItem("Original Title", "Original Description");
        existingTask.Id = 1;

        var command = new UpdateTaskCommand(
            existingTask.Id,
            "Updated Title",
            "Updated Description",
            TaskManagement.Domain.Entities.TaskStatus.InProgress);

        var taskDto = new TaskDto { Id = existingTask.Id, Title = command.Title, Description = command.Description };

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
            t.Id == command.Id &&
            t.Title == command.Title &&
            t.Description == command.Description)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistingTask_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new UpdateTaskCommand(
            99,
            "Updated Title",
            "Updated Description",
            TaskManagement.Domain.Entities.TaskStatus.Completed);

        _mockRepository.Setup(r => r.GetByIdAsync(command.Id))
            .ReturnsAsync((TaskItem?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entidade \"Task\" ({command.Id}) não foi encontrada.");

        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Never);
    }

}