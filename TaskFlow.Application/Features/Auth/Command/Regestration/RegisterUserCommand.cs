using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.AuthDTOs;

namespace TaskFlow.Application.Features.Auth.Command.Regestration
{
    public class RegisterUserCommand : IRequest<string>
    {
        public RegisterDto RegisterDto { get; set; } = null!;
    }
}
