using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.UserDTOs;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUserHandler : IRequestHandler<GetAllUserQuery, List<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public GetAllUserHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<List<UserDto>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = "users:all";

            // 1️⃣ Try to read from Redis
            var cachedUsers = await _cache.GetAsync<List<UserDto>>(cacheKey);
            if (cachedUsers != null)
                return cachedUsers;

            // 2️⃣ Query DB from Repository
            var usersQuery = _unitOfWork.Users.GetAll();

            var userDtos = await _mapper
                .ProjectTo<UserDto>(usersQuery)
                .ToListAsync(cancellationToken);

            if (!userDtos.Any())
                throw new ApplicationException("No users found.");

            // 3️⃣ Store in Redis for 5 minutes
            await _cache.SetAsync(cacheKey, userDtos, TimeSpan.FromMinutes(5));

            return userDtos;
        }
    }
}
