using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.DTOs.ProjectDTOs;
using TaskFlow.Application.Features.Projects.Command.CreateProject;
using TaskFlow.Application.Features.Projects.Command.DeleteProjects;
using TaskFlow.Application.Features.Projects.Command.UpdateProjects;
using TaskFlow.Application.Features.Projects.Queries.GetAllProjects;
using TaskFlow.Application.Features.Projects.Queries.GetPrpjectById;

namespace TaskFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var projects = await _mediator.Send(new GetAllProjectsQuery());
            return Ok(projects);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var project = await _mediator.Send(new GetProjectByIdQuery{ ProjectId=Id});
            return Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
        {
            var project = await _mediator.Send(new CreateProjectCommand(dto));
            return Ok(project);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateProjectDto dto)
        {
            var project = await _mediator.Send(new UpdateProjectCommand(dto));
            return Ok(project);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var project = await _mediator.Send(new DeleteProjectCommand { ProjectId = Id });
            return Ok(project);
        }
    }
}
