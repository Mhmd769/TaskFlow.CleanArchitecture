using AutoMapper;
using MediatR;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.UserDTOs;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Users.Command.DeleteUser
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public DeleteUserHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<UserDto> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            // Get user
            var user = await _unitOfWork.Users.GetByIdAsync(request.userid);
            if (user == null)
                throw new NotFoundException("User", request.userid);

            var userDto = _mapper.Map<UserDto>(user);

            // Delete
            _unitOfWork.Users.Delete(user);
            await _unitOfWork.SaveAsync();

            // Cache keys
            string cacheById = $"user:{request.userid}";
            string cacheAll = "users:all";
            string projectsCache = "projects:all"; // ✅ Add this
            string taskcache = "tasks:all";

            // Remove old caches
            await _cache.RemoveAsync(cacheById);
            await _cache.RemoveAsync(cacheAll);
            await _cache.RemoveAsync(projectsCache); // ✅ Invalidate projects cache
            await _cache.RemoveAsync(taskcache);

            return userDto;
        }
    }
}
