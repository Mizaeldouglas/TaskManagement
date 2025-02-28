using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Features.Tasks.Queries.GetTaskById;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.UnitTests.Application.Queries;

public class GetTaskByIdQueryHandlerTests
{
    private readonly Mock<ITaskRepository> _mockRepository;
    private readonly Fixture _fixture;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetTaskByIdQueryHandler _handler;

    public GetTaskByIdQueryHandlerTests()
    {
        _mockRepository = new Mock<ITaskRepository>();
        _fixture = new Fixture();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetTaskByIdQueryHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_WithExistingId_ShouldReturnTask()
    {
        // Arrange
        var taskItem = new TaskItem("Test Task", "Test Description");
        taskItem.Id = 1;

        var taskDto = new TaskDto
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            Description = taskItem.Description,
            CreationDate = taskItem.CreationDate,
            Status = taskItem.Status.ToString()
        };

        var query = new GetTaskByIdQuery(taskItem.Id);

        _mockRepository.Setup(r => r.GetByIdAsync(query.Id))
            .ReturnsAsync(taskItem);

        _mockMapper.Setup(m => m.Map<TaskDto>(It.IsAny<TaskItem>()))
            .Returns(taskDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(taskItem.Id);
        result.Title.Should().Be(taskItem.Title);
        result.Description.Should().Be(taskItem.Description);
    }

    [Fact]
    public async Task Handle_WithNonExistingId_ShouldThrowNotFoundException()
    {
        // Arrange
        var query = new GetTaskByIdQuery(999);
        _mockRepository.Setup(r => r.GetByIdAsync(query.Id))
            .ReturnsAsync((TaskItem?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(query, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entidade \"Task\" ({query.Id}) não foi encontrada.");
    }
}