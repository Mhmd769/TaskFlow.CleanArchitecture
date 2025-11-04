using AutoMapper;
using MediatR;
using TaskFlow.Application.DTOs.TaskDTOs;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Tasks.Command.UpdateTask
{
    public class UpdateTaskHandler : IRequestHandler<UpdateTaskCommand, TaskDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateTaskHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<TaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Task;

            // 1️⃣ Fetch existing task
            var existingTask = await _unitOfWork.Tasks.GetByIdAsync(dto.Id);
            if (existingTask == null)
                throw new NotFoundException("Task", dto.Id);

            // 2️⃣ Map DTO → Entity (updates fields)
            _mapper.Map(dto, existingTask);

            // 3️⃣ Update entity
            _unitOfWork.Tasks.Update(existingTask);
            await _unitOfWork.SaveAsync();

            // 4️⃣ Return updated DTO
            return _mapper.Map<TaskDto>(existingTask);
        }
    }
}
