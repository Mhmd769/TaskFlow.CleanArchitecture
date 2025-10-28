using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Features.Project.Command.CreateProject
{
    public class CreateProjectCommand : IRequest<ProjectDto>
    {
        public CreateProjectDto Project { get; set; } = null!;
    }
}
