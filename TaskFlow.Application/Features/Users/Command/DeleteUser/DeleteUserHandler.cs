using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.UserDTOs;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Users.Command.DeleteUser
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeleteUserHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            // Fetch the user
            var user = await _unitOfWork.Users.GetByIdAsync(request.userid);
            if (user == null)
                throw new Exception("User not found");

            // Map before deleting
            var deletedUserDto = _mapper.Map<UserDto>(user);

            // Delete and save changes
            _unitOfWork.Users.Delete(user);
            await _unitOfWork.SaveAsync();

            return deletedUserDto;
        }
    }
}
