using MediatR;
using TaskFlow.Application.DTOs.UserDTOs;

namespace TaskFlow.Application.Features.Users.Command.UpdateUser
{
    public class UpdateUserCommand : IRequest<UserDto>
    {
        public UpdateUserCommand(UpdateUserDto dto)
        {
            Dto = dto;
        }

        public Guid UserId { get; set; }
        public UpdateUserDto User { get; set; } = null!;
        public UpdateUserDto Dto { get; }
    }
}
