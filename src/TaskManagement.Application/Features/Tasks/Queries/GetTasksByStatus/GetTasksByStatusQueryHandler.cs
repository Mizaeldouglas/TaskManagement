using AutoMapper;
using MediatR;
using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTasksByStatus
{
    public class GetTasksByStatusQueryHandler : IRequestHandler<GetTasksByStatusQuery, IEnumerable<TaskDto>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public GetTasksByStatusQueryHandler(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaskDto>> Handle(GetTasksByStatusQuery request, CancellationToken cancellationToken)
        {
            var tasks = await _taskRepository.GetByStatusAsync(request.Status);
            return _mapper.Map<IEnumerable<TaskDto>>(tasks);
        }
    }
}
