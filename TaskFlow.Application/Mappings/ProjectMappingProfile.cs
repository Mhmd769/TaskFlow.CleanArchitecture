using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.ProjectDTOs;
using TaskFlow.Application.DTOs.UserDTOs;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Mappings
{
    public class ProjectMappingProfile : Profile
    {
        public ProjectMappingProfile() 
        {
            // Entity ↔ DTO
            CreateMap<Project, ProjectDto>().ReverseMap();

            // Create DTO → Entity (no reverse needed)
            CreateMap<CreateProjectDto, Project>();


            // -----------------------
            CreateMap<UpdateProjectDto, Project>();

        }
    }
}
