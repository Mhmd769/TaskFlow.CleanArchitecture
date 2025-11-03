using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.ProjectDTOs;

namespace TaskFlow.Application.Features.Projects.Command.CreateProject
{
    public class CreateProjectCommand : IRequest<ProjectDto>
    {
        public CreateProjectDto Project { get; set; } = null!;

        public CreateProjectCommand(CreateProjectDto dto)
        {
            Project = dto;
        }

    }
}
