using MediatR;
using System.Threading.Tasks;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Commands.DeleteTask
{
    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand>
    {
        private readonly ITaskRepository _taskRepository;

        public DeleteTaskCommandHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var taskItem = await _taskRepository.GetByIdAsync(request.Id);

            if (taskItem == null)
                throw new NotFoundException(nameof(Task), request.Id);


            await _taskRepository.DeleteAsync(request.Id);
        }
    }
}
