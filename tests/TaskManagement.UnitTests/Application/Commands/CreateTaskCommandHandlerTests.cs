using AutoFixture;
using FluentAssertions;
using Moq;
using AutoMapper;
using TaskManagement.Application.Features.Tasks.Commands.CreateTask;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Application.DTOs;

namespace TaskManagement.UnitTests.Application.Commands;

public class CreateTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Fixture _fixture;
    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandHandlerTests()
    {
        _mockRepository = new Mock<ITaskRepository>();
        _mockMapper = new Mock<IMapper>();
        _fixture = new Fixture();
        _handler = new CreateTaskCommandHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateTask_AndReturnTaskDto()
    {
        // Arrange
        var command = _fixture.Create<CreateTaskCommand>();
        var taskItem = new TaskItem(command.Title, command.Description);
        var taskDto = _fixture.Create<TaskDto>();

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(taskItem.Id);

        _mockMapper.Setup(m => m.Map<TaskDto>(It.IsAny<TaskItem>()))
            .Returns(taskDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(taskDto);
        _mockRepository.Verify(r => r.AddAsync(It.Is<TaskItem>(t =>
            t.Title == command.Title &&
            t.Description == command.Description)),
            Times.Once);
        _mockMapper.Verify(m => m.Map<TaskDto>(It.IsAny<TaskItem>()), Times.Once);
    }
}
