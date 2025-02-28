using MediatR;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTasksByStatus
{
    public record GetTasksByStatusQuery(Domain.Entities.TaskStatus Status) : IRequest<IEnumerable<TaskDto>>;
}
