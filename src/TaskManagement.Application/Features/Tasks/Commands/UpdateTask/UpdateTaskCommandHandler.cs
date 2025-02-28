using AutoMapper;
using MediatR;
using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTask
{
    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskDto>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public UpdateTaskCommandHandler(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<TaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var taskEntity = await _taskRepository.GetByIdAsync(request.Id);

            if (taskEntity == null)
                throw new NotFoundException(nameof(Task), request.Id);

            // Atualizar propriedades
            taskEntity.Title = request.Title;
            taskEntity.Description = request.Description;

            await _taskRepository.UpdateAsync(taskEntity);

            return _mapper.Map<TaskDto>(taskEntity);
        }
    }
}