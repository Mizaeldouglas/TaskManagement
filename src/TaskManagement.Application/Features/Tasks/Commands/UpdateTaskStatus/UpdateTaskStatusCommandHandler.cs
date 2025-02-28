using AutoMapper;
using MediatR;
using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus
{
    public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, TaskDto>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public UpdateTaskStatusCommandHandler(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        // src/TaskManagement.Application/Features/Tasks/Commands/UpdateTaskStatus/UpdateTaskStatusCommandHandler.cs
        public async Task<TaskDto> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.Id);
            if (task == null)
            {
                throw new NotFoundException("Tarefa", request.Id);
            }

            if (!Enum.TryParse<Domain.Entities.TaskStatus>(request.Status.Trim(), true, out var status))
            {
                throw new ValidationException($"Status inválido: {request.Status}");
            }

            // Se o status for alterado para Completed, definir a data de conclusão
            if (status == Domain.Entities.TaskStatus.Completed && task.CompletionDate == null)
            {
                task.CompletionDate = DateTime.UtcNow; // Usar UTC agora para consistência
            }
            // Se o status for alterado de Completed para outro estado, remover a data de conclusão
            else if (status != Domain.Entities.TaskStatus.Completed && task.Status == Domain.Entities.TaskStatus.Completed)
            {
                task.CompletionDate = null;
            }

            task.Status = status;
            await _taskRepository.UpdateAsync(task);

            return _mapper.Map<TaskDto>(task);
        }

    }
}
