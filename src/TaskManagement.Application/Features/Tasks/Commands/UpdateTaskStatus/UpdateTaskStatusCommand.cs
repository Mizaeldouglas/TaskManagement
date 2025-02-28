using MediatR;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus
{
    public class UpdateTaskStatusCommand : IRequest<TaskDto>
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}