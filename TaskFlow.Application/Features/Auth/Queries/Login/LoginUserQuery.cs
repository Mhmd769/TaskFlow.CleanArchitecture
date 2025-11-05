using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.AuthDTOs;

namespace TaskFlow.Application.Features.Auth.Queries.Login
{
    public class LoginUserQuery : IRequest<LoginResponseDto>
    {
        public LoginDto LoginDto { get; set; } = null!;
    }
}
