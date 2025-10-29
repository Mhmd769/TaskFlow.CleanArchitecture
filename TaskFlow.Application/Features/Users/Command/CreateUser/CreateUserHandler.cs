using AutoMapper;
using MediatR;
using TaskFlow.Application.DTOs.UserDTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Users.Command.CreateUser
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateUserHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Map from DTO → Entity
            var user = _mapper.Map<User>(request.User);

            // Hash the password (since AutoMapper ignores it)
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.User.Password);

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveAsync();

            // Map from Entity → DTO
            var result = _mapper.Map<UserDto>(user);
            return result;
        }
    }
}
