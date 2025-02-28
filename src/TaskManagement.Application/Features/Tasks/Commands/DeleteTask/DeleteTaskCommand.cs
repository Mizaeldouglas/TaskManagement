using MediatR;

namespace TaskManagement.Application.Features.Tasks.Commands.DeleteTask
{
    public record DeleteTaskCommand(int Id) : IRequest;
}
