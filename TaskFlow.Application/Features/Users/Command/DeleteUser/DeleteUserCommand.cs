using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.UserDTOs;

namespace TaskFlow.Application.Features.Users.Command.DeleteUser
{
    public class DeleteUserCommand : IRequest <UserDto>
    {
        public Guid userid { get; set; }
    }
}
