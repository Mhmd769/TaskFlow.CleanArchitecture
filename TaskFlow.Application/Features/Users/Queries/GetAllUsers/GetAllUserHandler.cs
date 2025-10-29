using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.DTOs.UserDTOs;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUserHandler : IRequestHandler<GetAllUserQuery, List<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllUserHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            // IQueryable<User> from repo (AsNoTracking)
            var usersQuery = _unitOfWork.Users.GetAll();

            // Project directly to DTOs and fetch from DB
            var userDtos = await _mapper.ProjectTo<UserDto>(usersQuery)
                                        .ToListAsync(cancellationToken);

            if (!userDtos.Any())
                throw new Exception("No users found");

            return userDtos;
        }
    }
}
