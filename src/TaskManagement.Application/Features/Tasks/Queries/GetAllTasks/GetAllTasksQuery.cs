using MediatR;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Features.Tasks.Queries.GetAllTasks
{
    public record GetAllTasksQuery : IRequest<IEnumerable<TaskDto>>;
}
