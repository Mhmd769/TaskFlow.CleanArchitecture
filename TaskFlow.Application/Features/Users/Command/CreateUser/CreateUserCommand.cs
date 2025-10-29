using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TaskFlow.Application.DTOs.UserDTOs;

namespace TaskFlow.Application.Features.Users.Command.CreateUser
{


    public class CreateUserCommand : IRequest<UserDto>
    {
        public CreateUserDto User { get; set; } = null!;
    }
}
