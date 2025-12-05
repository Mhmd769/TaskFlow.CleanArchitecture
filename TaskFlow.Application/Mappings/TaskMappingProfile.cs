using System;
using AutoMapper;
using TaskFlow.Application.DTOs.TaskDTOs;
using TaskFlow.Domain.Entities;

public class TaskMappingProfile : Profile
{
    public TaskMappingProfile()
    {
        // Create
        CreateMap<CreateTaskDto, TaskItem>()
            .ForMember(dest => dest.AssignedUsers, opt => opt.Ignore());

        // Update
        CreateMap<UpdateTaskDto, TaskItem>()
            .ForMember(dest => dest.AssignedUsers, opt => opt.Ignore());

        // Entity → DTO
        CreateMap<TaskItem, TaskDto>()
            .ForMember(dest => dest.AssignedUserIds,
                       opt => opt.MapFrom(src => src.AssignedUsers != null
                           ? src.AssignedUsers.Select(au => au.UserId).ToList()
                           : new List<Guid>()))
            .ForMember(dest => dest.AssignedUserNames,
                       opt => opt.MapFrom(src => src.AssignedUsers != null
                           ? src.AssignedUsers.Select(u => u.User.FullName).ToList()
                           : new List<string>()));
        ;
    }
}
