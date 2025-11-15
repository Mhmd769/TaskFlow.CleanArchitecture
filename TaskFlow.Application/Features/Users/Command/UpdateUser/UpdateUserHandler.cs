using AutoMapper;
using MediatR;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.UserDTOs;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Users.Command.UpdateUser
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public UpdateUserHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            // 1️⃣ Get user
            var existingUser = await _unitOfWork.Users.GetByIdAsync(request.User.Id);
            if (existingUser == null)
                throw new NotFoundException("User", request.User.Id);

            // 2️⃣ Map update
            _mapper.Map(request.User, existingUser);

            // 3️⃣ Save
            _unitOfWork.Users.Update(existingUser);
            await _unitOfWork.SaveAsync();

            // 4️⃣ ❗Invalidate cache
            string cacheById = $"user:{request.User.Id}";
            string cacheAll = "users:all";

            await _cache.RemoveAsync(cacheById);
            await _cache.RemoveAsync(cacheAll);

            // Return updated DTO
            return _mapper.Map<UserDto>(existingUser);
        }
    }
}
