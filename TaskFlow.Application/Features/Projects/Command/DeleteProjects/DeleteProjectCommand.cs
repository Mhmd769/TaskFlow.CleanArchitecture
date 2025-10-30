using MediatR;
using TaskFlow.Application.DTOs.ProjectDTOs;

namespace TaskFlow.Application.Features.Projects.Command.DeleteProjects
{
    public class DeleteProjectCommand : IRequest<ProjectDto>
    {
        public Guid ProjectId { get; set; }

        public DeleteProjectCommand(Guid projectId)
        {
            ProjectId = projectId;
        }
    }
}
