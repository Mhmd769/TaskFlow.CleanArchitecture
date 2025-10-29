using MediatR;
using TaskFlow.Application.DTOs.UserDTOs;

namespace TaskFlow.Application.Features.Users.Command.UpdateUser
{
    public class UpdateUserCommand : IRequest<UserDto>
    {
        public Guid UserId { get; set; }
        public UpdateUserDto User { get; set; } = null!;
    }
}
