using MediatR;
using TaskFlow.Application.DTOs.UserDTOs;

namespace TaskFlow.Application.Features.Users.Command.UpdateUser
{
    public class UpdateUserCommand : IRequest<UserDto>
    {
        public UpdateUserDto User { get; set; }

        public UpdateUserCommand(UpdateUserDto dto)
        {
            User = dto; // assign here
        }
    }
}
