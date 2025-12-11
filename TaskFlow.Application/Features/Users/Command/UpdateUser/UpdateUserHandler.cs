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
            var existingUser = await _unitOfWork.Users.GetByIdAsync(request.User.Id);
            if (existingUser == null)
                throw new NotFoundException("User", request.User.Id);

            _mapper.Map(request.User, existingUser);

            _unitOfWork.Users.Update(existingUser);
            await _unitOfWork.SaveAsync();

            // Cache keys
            string cacheById = $"user:{existingUser.Id}";
            string cacheAll = "users:all";
            string projectsCache = "projects:all"; // ✅ Add this
            string taskcache = "tasks:all";

            // Remove old caches
            await _cache.RemoveAsync(cacheById);
            await _cache.RemoveAsync(cacheAll);
            await _cache.RemoveAsync(projectsCache); // ✅ Invalidate projects cache
            await _cache.RemoveAsync(taskcache);

            // Cache updated user
            var updatedDto = _mapper.Map<UserDto>(existingUser);
            await _cache.SetAsync(cacheById, updatedDto, TimeSpan.FromMinutes(5));

            return updatedDto;
        }



    }
}
