using AutoMapper;
using TaskFlow.Application.DTOs.TaskDTOs;
using TaskFlow.Domain.Entities;

public class TaskMappingProfile : Profile
{
    public TaskMappingProfile()
    {
        // Entity → DTO
        CreateMap<TaskItem, TaskDto>().ReverseMap();

        // DTO → Entity (Create)
        CreateMap<CreateTaskDto, TaskItem>()
            .ForMember(dest => dest.AssignedUsers, opt => opt.Ignore());

        // DTO → Entity (Update)
        CreateMap<UpdateTaskDto, TaskItem>();
    }
}
