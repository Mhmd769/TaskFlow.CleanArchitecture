using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Auth.Command.Regestration
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, string>
    {
        private readonly IAuthRepository _authRepo;

        public RegisterUserHandler(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var existing = await _authRepo.GetByEmailAsync(request.RegisterDto.Email);
            if (existing != null)
                throw new Exception("Email already exists");

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = request.RegisterDto.Username,
                FullName = request.RegisterDto.FullName,
                Email = request.RegisterDto.Email,
                PhoneNumber = request.RegisterDto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.RegisterDto.Password),
                Role = UserRole.User
            };

            await _authRepo.AddUserAsync(newUser);
            return "User registered successfully.";
        }
    }
}
