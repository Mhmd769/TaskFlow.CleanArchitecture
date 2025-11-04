using AutoMapper;
using TaskFlow.Application.DTOs.ProjectDTOs;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Mappings
{
    public class ProjectMappingProfile : Profile
    {
        public ProjectMappingProfile()
        {
            // Entity -> DTO
            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.FullName))
                .ForMember(dest => dest.TaskCount, opt => opt.MapFrom(src => src.Tasks.Count))
                .ReverseMap();

            // Create DTO -> Entity
            CreateMap<CreateProjectDto, Project>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.Tasks, opt => opt.Ignore());

            // Update DTO -> Entity
            CreateMap<UpdateProjectDto, Project>()
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.Tasks, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}
