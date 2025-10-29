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
            // Entity → DTO (Read)
            // -----------------------
            CreateMap<User, UserDto>();

            // -----------------------
            // DTO → Entity (Create)
            // -----------------------
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            // PasswordHash handled manually before saving

            // -----------------------
            // DTO → Entity (Update)
            // -----------------------
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            // Usually we don’t update password here
        }
    }
}
