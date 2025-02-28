using MediatR;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTask
{
    public record UpdateTaskCommand(
    int Id,
    string Title,
    string? Description,
    Domain.Entities.TaskStatus Status) : IRequest<TaskDto>;
}
