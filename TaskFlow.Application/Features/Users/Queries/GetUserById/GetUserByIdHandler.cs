using AutoMapper;
using MediatR;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.UserDTOs;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Users.Queries.GetUserById
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;


        public GetUserByIdHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)

        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;

        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = $"user:{request.UserId}";

            // 1️⃣ Try cache
            var cachedUser = await _cache.GetAsync<UserDto>(cacheKey);
            if (cachedUser != null)
                return cachedUser;

            // 2️⃣ Load from DB
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null)
                throw new NotFoundException("User", request.UserId);

            var dto = _mapper.Map<UserDto>(user);

            // 3️⃣ Save to cache
            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));

            return dto;
        }

    }
}
