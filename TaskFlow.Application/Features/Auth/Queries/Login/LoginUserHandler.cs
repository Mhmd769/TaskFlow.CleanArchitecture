using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.AuthDTOs;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Auth.Queries.Login
{
    public class LoginUserHandler : IRequestHandler<LoginUserQuery, LoginResponseDto>
    {
        private readonly IAuthRepository _authRepo;
        private readonly IJwtService _jwtService;

        public LoginUserHandler(IAuthRepository authRepo, IJwtService jwtService)
        {
            _authRepo = authRepo;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _authRepo.GetByEmailAsync(request.LoginDto.Email);
            if (user == null)
                throw new Exception("Invalid credentials");

            if (!BCrypt.Net.BCrypt.Verify(request.LoginDto.Password, user.PasswordHash))
                throw new Exception("Invalid credentials");

            var token = _jwtService.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                Role = user.Role.ToString(),
                Username = user.Username
            };
        }
    }
}