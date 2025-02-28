using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Features.Tasks.Queries.GetAllTasks;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.UnitTests.Application.Queries;

public class GetAllTasksQueryHandlerTests
{
    private readonly Mock<ITaskRepository> _mockRepository;
    private readonly Fixture _fixture;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetAllTasksQueryHandler _handler;

    public GetAllTasksQueryHandlerTests()
    {
        _mockRepository = new Mock<ITaskRepository>();
        _fixture = new Fixture();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetAllTasksQueryHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllTasks()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new TaskItem("Tarefa 1", "Descrição 1") { Id = 1 },
            new TaskItem("Tarefa 2", "Descrição 2") { Id = 2 },
            new TaskItem("Tarefa 3", "Descrição 3") { Id = 3 }
        };

        var taskDtos = tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            CreationDate = t.CreationDate,
            Status = t.Status.ToString()
        }).ToList();

        var query = new GetAllTasksQuery();

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(tasks);

        _mockMapper.Setup(m => m.Map<IEnumerable<TaskDto>>(It.IsAny<IEnumerable<TaskItem>>()))
            .Returns(taskDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(taskDtos);
    }

    [Fact]
    public async Task Handle_WithEmptyRepository_ShouldReturnEmptyList()
    {
        // Arrange
        var emptyList = new List<TaskItem>();
        var emptyDtos = new List<TaskDto>();
        var query = new GetAllTasksQuery();

        _mockRepository.Setup(r => r.GetAllAsync())
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
    public async Task Handle_ShouldCallRepositoryGetAllAsync()
    {
        // Arrange
        var query = new GetAllTasksQuery();
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<TaskItem>());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapTaskItemsToTaskDtos()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new TaskItem("Tarefa 1", "Descrição 1") { Id = 1 }
        };

        var query = new GetAllTasksQuery();

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(tasks);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockMapper.Verify(m => m.Map<IEnumerable<TaskDto>>(tasks), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedData()
    {
        // Arrange
        // Criação manual de TaskItems para evitar a exceção do AutoFixture
        var tasks = new List<TaskItem>
    {
        new TaskItem("Tarefa 1", "Descrição 1") { Id = 1 },
        new TaskItem("Tarefa 2", "Descrição 2") { Id = 2 },
        new TaskItem("Tarefa 3", "Descrição 3") { Id = 3 }
    };

        var taskDtos = _fixture.CreateMany<TaskDto>(3).ToList();
        var query = new GetAllTasksQuery();

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(tasks);

        _mockMapper.Setup(m => m.Map<IEnumerable<TaskDto>>(tasks))
            .Returns(taskDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(taskDtos);
    }


    [Fact]
    public async Task Handle_WhenRepositoryThrows_ShouldPropagateException()
    {
        // Arrange
        var query = new GetAllTasksQuery();
        var expectedException = new InvalidOperationException("Database error");

        _mockRepository.Setup(r => r.GetAllAsync())
            .ThrowsAsync(expectedException);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(query, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Database error");
    }
}
