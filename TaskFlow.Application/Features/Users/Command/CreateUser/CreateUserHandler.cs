using AutoMapper;
using MediatR;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.UserDTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Users.Command.CreateUser
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public CreateUserHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
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

            // Bust list cache and prime single-user cache to avoid stale responses
            await _cache.RemoveAsync("users:all");
            await _cache.SetAsync($"user:{result.Id}", result, TimeSpan.FromMinutes(5));

            return result;
        }
    }
}
