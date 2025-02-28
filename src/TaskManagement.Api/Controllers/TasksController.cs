using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Features.Tasks.Commands.CreateTask;
using TaskManagement.Application.Features.Tasks.Commands.DeleteTask;
using TaskManagement.Application.Features.Tasks.Commands.UpdateTask;
using TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;
using TaskManagement.Application.Features.Tasks.Queries.GetAllTasks;
using TaskManagement.Application.Features.Tasks.Queries.GetTaskById;
using TaskManagement.Application.Features.Tasks.Queries.GetTasksByStatus;
using TaskManagement.Domain.Exceptions;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtém todas as tarefas.
        /// </summary>
        /// <returns>Lista de todas as tarefas</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
        {
            var tasks = await _mediator.Send(new GetAllTasksQuery());
            return Ok(tasks);
        }

        /// <summary>
        /// Obtém tarefas filtradas por status.
        /// </summary>
        /// <param name="status">Status da tarefa (Pending, InProgress, Completed)</param>
        /// <returns>Lista de tarefas com o status especificado</returns>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksByStatus(Domain.Entities.TaskStatus status)
        {
            if (!Enum.IsDefined(typeof(Domain.Entities.TaskStatus), status))
                throw new ValidationException($"Status inválido. Valores aceitos: {string.Join(", ", Enum.GetNames(typeof(Domain.Entities.TaskStatus)))}");

            var tasks = await _mediator.Send(new GetTasksByStatusQuery(status));
            return Ok(tasks);
        }

        /// <summary>
        /// Obtém uma tarefa pelo ID.
        /// </summary>
        /// <param name="id">ID da tarefa</param>
        /// <returns>Detalhes da tarefa</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskDto>> GetTask(int id)
        {
            var taskItem = await _mediator.Send(new GetTaskByIdQuery(id));
            if (taskItem == null)
                throw new NotFoundException("Tarefa", id);

            return Ok(taskItem);
        }

        /// <summary>
        /// Cria uma nova tarefa.
        /// </summary>
        /// <param name="command">Dados da tarefa</param>
        /// <returns>Tarefa recém-criada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskCommand command)
        {
            var taskItem = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTask), new { id = taskItem.Id }, taskItem);
        }

        /// <summary>
        /// Atualiza uma tarefa existente.
        /// </summary>
        /// <param name="id">ID da tarefa</param>
        /// <param name="command">Dados atualizados da tarefa</param>
        /// <returns>Tarefa atualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskDto>> UpdateTask(int id, [FromBody] UpdateTaskCommand command)
        {
            if (id != command.Id)
            {
                throw new BadRequestException("O ID na URL deve corresponder ao ID no corpo da requisição");
            }

            var taskItem = await _mediator.Send(command);
            return Ok(taskItem);
        }

        /// <summary>
        /// Remove uma tarefa.
        /// </summary>
        /// <param name="id">ID da tarefa</param>
        /// <returns>Sem conteúdo</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask(int id)
        {
            await _mediator.Send(new DeleteTaskCommand(id));
            return NoContent();
        }

        /// <summary>
        /// Atualiza o status de uma tarefa.
        /// </summary>
        /// <param name="id">ID da tarefa</param>
        /// <param name="command">Novo status da tarefa</param>
        /// <returns>Sem conteúdo</returns>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskDto>> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusCommand command)
        {
            if (id != command.Id)
            {
                throw new BadRequestException("O ID na URL deve corresponder ao ID no corpo da requisição");
            }

            var taskItem = await _mediator.Send(command);
            return Ok(taskItem);
        }
    }
}
