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

            var cachedUser = await _cache.GetAsync<UserDto>(cacheKey);
            
            if(cachedUser != null)
                return cachedUser;


            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null)
                throw new NotFoundException("User", request.UserId);

            var  userDtos = _mapper.Map<UserDto>(user);
            await _cache.SetAsync(cacheKey, userDtos, TimeSpan.FromMinutes(5));


            return userDtos;
        }
    }
}
