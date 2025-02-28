using AutoFixture;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagement.Api.Controllers;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Features.Tasks.Commands.CreateTask;
using TaskManagement.Application.Features.Tasks.Commands.DeleteTask;
using TaskManagement.Application.Features.Tasks.Commands.UpdateTask;
using TaskManagement.Application.Features.Tasks.Queries.GetAllTasks;
using TaskManagement.Application.Features.Tasks.Queries.GetTaskById;
using TaskManagement.Domain.Exceptions;

namespace TaskManagement.UnitTests.Api;

public class TasksControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Fixture _fixture;
    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _fixture = new Fixture();
        _controller = new TasksController(_mockMediator.Object);
    }

    [Fact]
    public async Task GetTasks_ShouldReturnOkResult_WithTasks()
    {
        // Arrange
        var tasks = _fixture.CreateMany<TaskDto>(3).ToList();
        _mockMediator.Setup(m => m.Send(It.IsAny<GetAllTasksQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _controller.GetTasks();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnValue = okResult.Value.Should().BeAssignableTo<IEnumerable<TaskDto>>().Subject;
        returnValue.Count().Should().Be(3);
    }

    [Fact]
    public async Task GetTask_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var task = _fixture.Create<TaskDto>();
        _mockMediator.Setup(m => m.Send(It.IsAny<GetTaskByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        // Act
        var result = await _controller.GetTask(task.Id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnValue = okResult.Value.Should().BeAssignableTo<TaskDto>().Subject;
        returnValue.Should().Be(task);
    }

    [Fact]
    public async Task GetTask_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        int taskId = 999;

        

        // Act & Assert
        await _controller.Invoking(c => c.GetTask(taskId))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entidade \"Tarefa\" ({taskId}) não foi encontrada.");
    }

    [Fact]
    public async Task CreateTask_WithValidCommand_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var command = _fixture.Create<CreateTaskCommand>();
        var taskDto = _fixture.Create<TaskDto>();
        _mockMediator.Setup(m => m.Send(It.IsAny<CreateTaskCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskDto);

        // Act
        var result = await _controller.CreateTask(command);

        // Assert
        var createdAtActionResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtActionResult.ActionName.Should().Be(nameof(_controller.GetTask));
        createdAtActionResult.RouteValues.Should().ContainKey("id")
            .WhoseValue.Should().Be(taskDto.Id);
        createdAtActionResult.Value.Should().Be(taskDto);
    }

    [Fact]
    public async Task UpdateTask_WithValidCommand_ShouldReturnOk()
    {
        // Arrange
        var taskDto = _fixture.Create<TaskDto>();
        var command = new UpdateTaskCommand(
            taskDto.Id,
            taskDto.Title,
            taskDto.Description,
            TaskManagement.Domain.Entities.TaskStatus.InProgress
        );

        _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskDto);

        // Act
        var result = await _controller.UpdateTask(command.Id, command);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnValue = okResult.Value.Should().BeAssignableTo<TaskDto>().Subject;
        returnValue.Should().Be(taskDto);
    }

    [Fact]
    public async Task UpdateTask_WithMismatchedIds_ShouldThrowBadRequestException()
    {
        // Arrange
        var command = _fixture.Create<UpdateTaskCommand>();
        int differentId = command.Id + 1;

        // Act & Assert
        await _controller.Invoking(c => c.UpdateTask(differentId, command))
            .Should().ThrowAsync<BadRequestException>()
            .WithMessage("O ID na URL deve corresponder ao ID no corpo da requisição");
    }

    [Fact]
    public async Task DeleteTask_ShouldReturnNoContent()
    {
        // Arrange
        int taskId = 1;
        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteTaskCommand>(), It.IsAny<CancellationToken>()));

        // Act
        var result = await _controller.DeleteTask(taskId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }


}