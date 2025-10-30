using AutoMapper;
using TaskFlow.Application.DTOs.UserDTOs;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            // -----------------------
            // Entity ↔ DTO (Read)
            // -----------------------
            CreateMap<User, UserDto>().ReverseMap();
            // Now you can map User → UserDto and UserDto → User automatically

            // -----------------------
            // DTO → Entity (Create)
            // -----------------------
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            // -----------------------
            // DTO → Entity (Update)
            // -----------------------
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        }

    }
}
