using MediatR;
using TaskFlow.Application.DTOs.ProjectDTOs;

namespace TaskFlow.Application.Features.Projects.Command.UpdateProjects
{
    public class UpdateProjectCommand : IRequest<ProjectDto>
    {
        public UpdateProjectDto Project { get; set; }

        public UpdateProjectCommand(UpdateProjectDto project)
        {
            Project = project;
        }
    }
}
