using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.DTOs.ProjectDTOs;
using TaskFlow.Application.DTOs.TaskDTOs;
using TaskFlow.Application.Features.Projects.Command.CreateProject;
using TaskFlow.Application.Features.Projects.Command.DeleteProjects;
using TaskFlow.Application.Features.Projects.Command.UpdateProjects;
using TaskFlow.Application.Features.Projects.Queries.GetAllProjects;
using TaskFlow.Application.Features.Projects.Queries.GetPrpjectById;
using TaskFlow.Application.Features.Tasks.Command.CreateTask;
using TaskFlow.Application.Features.Tasks.Command.DeleteTask;
using TaskFlow.Application.Features.Tasks.Command.UpdateTask;
using TaskFlow.Application.Features.Tasks.Queries.GetAllTasks;

namespace TaskFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Tasks = await _mediator.Send(new GetAllTasksQuery());
            return Ok(Tasks);
        }

        [HttpGet("{Id}")]

        public async Task<IActionResult> GetById(Guid Id)
        {
            var Task = await _mediator.Send(new GetTaskByIdQuery { TaskId = Id });
            return Ok(Task);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
        {
            var task = await _mediator.Send(new CreateTaskCommand(dto));
            return Ok(task);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateTaskDto dto)
        {
            var command = new UpdateTaskCommand(dto);
            var Task = await _mediator.Send(command);
            return Ok(Task);
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(Guid Id)
        {   
            var task = await _mediator.Send(new DeleteTaskCommand { TaskId = Id });
            return Ok(task);
        }
    }
}
