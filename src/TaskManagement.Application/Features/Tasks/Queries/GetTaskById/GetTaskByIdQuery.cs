using MediatR;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTaskById
{
    public record GetTaskByIdQuery(int Id) : IRequest<TaskDto>;
}
