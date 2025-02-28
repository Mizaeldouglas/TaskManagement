using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Features.Tasks.Queries.GetTasksByStatus;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.UnitTests.Application.Queries;

public class GetTasksByStatusQueryHandlerTests
{
    private readonly Mock<ITaskRepository> _mockRepository;
    private readonly Fixture _fixture;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetTasksByStatusQueryHandler _handler;

    public GetTasksByStatusQueryHandlerTests()
    {
        _mockRepository = new Mock<ITaskRepository>();
        _fixture = new Fixture();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetTasksByStatusQueryHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_WithExistingStatus_ShouldReturnTasks()
    {
        // Arrange
        var status = TaskManagement.Domain.Entities.TaskStatus.InProgress;
        var tasks = new List<TaskItem>
        {
            new TaskItem("Tarefa 1", "Descrição 1") { Id = 1, Status = status },
            new TaskItem("Tarefa 2", "Descrição 2") { Id = 2, Status = status }
        };

        var taskDtos = tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            CreationDate = t.CreationDate,
            Status = t.Status.ToString()
        }).ToList();

        var query = new GetTasksByStatusQuery(status);

        _mockRepository.Setup(r => r.GetByStatusAsync(status))
            .ReturnsAsync(tasks);

        _mockMapper.Setup(m => m.Map<IEnumerable<TaskDto>>(It.IsAny<IEnumerable<TaskItem>>()))
            .Returns(taskDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(taskDtos);
    }

    [Fact]
    public async Task Handle_WithNoMatchingStatus_ShouldReturnEmptyList()
    {
        // Arrange
        var status = TaskManagement.Domain.Entities.TaskStatus.Completed;
        var emptyList = new List<TaskItem>();
        var emptyDtos = new List<TaskDto>();

        var query = new GetTasksByStatusQuery(status);

        _mockRepository.Setup(r => r.GetByStatusAsync(status))
            .ReturnsAsync(emptyList);

        _mockMapper.Setup(m => m.Map<IEnumerable<TaskDto>>(It.IsAny<IEnumerable<TaskItem>>()))
            .Returns(emptyDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectStatus()
    {
        // Arrange
        var status = TaskManagement.Domain.Entities.TaskStatus.Pending;
        var query = new GetTasksByStatusQuery(status);

        _mockRepository.Setup(r => r.GetByStatusAsync(status))
            .ReturnsAsync(new List<TaskItem>());

        _mockMapper.Setup(m => m.Map<IEnumerable<TaskDto>>(It.IsAny<IEnumerable<TaskItem>>()))
            .Returns(new List<TaskDto>());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockRepository.Verify(r => r.GetByStatusAsync(status), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapTaskItemsToTaskDtos()
    {
        // Arrange
        var status = TaskManagement.Domain.Entities.TaskStatus.InProgress;
        var tasks = new List<TaskItem>
        {
            new TaskItem("Tarefa 1", "Descrição 1") { Id = 1, Status = status }
        };

        var query = new GetTasksByStatusQuery(status);

        _mockRepository.Setup(r => r.GetByStatusAsync(status))
            .ReturnsAsync(tasks);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockMapper.Verify(m => m.Map<IEnumerable<TaskDto>>(tasks), Times.Once);
    }
}
