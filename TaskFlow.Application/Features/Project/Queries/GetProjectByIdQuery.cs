﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.ProjectDTOs;

namespace TaskFlow.Application.Features.Project.Queries
{
    public class GetProjectByIdQuery : IRequest<ProjectDto>
    {
        public Guid ProjectId { get; set; }
    }
}
