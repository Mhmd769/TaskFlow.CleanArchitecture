using AutoMapper;
using MediatR;
using TaskFlow.Application.DTOs.UserDTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Users.Command.UpdateUser
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateUserHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            // 1️⃣ Get existing user
            var existingUser = await _unitOfWork.Users.GetByIdAsync(request.User.Id);
            if (existingUser == null)
                throw new NotFoundException("User" ,request.User.Id);

            // 2️⃣ Map updated fields from DTO → Entity
            _mapper.Map(request.User, existingUser);

            // 3️⃣ Save changes
            _unitOfWork.Users.Update(existingUser); // optional, depends if your repo tracks entities
            await _unitOfWork.SaveAsync();

            // 4️⃣ Map Entity → DTO for response
            return _mapper.Map<UserDto>(existingUser);
        }
    }
}
