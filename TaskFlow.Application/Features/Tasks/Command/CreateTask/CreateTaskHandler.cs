using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.TaskDTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Tasks.Command.CreateTask
{
    public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, TaskDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateTaskHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = _mapper.Map<TaskItem>(request.Task);

            // Map multiple assigned users
            task.AssignedUsers = request.Task.AssignedUsers?
                .Select(u => new TaskAssignedUser { UserId = u.Id })
                .ToList() ?? new List<TaskAssignedUser>();

            await _unitOfWork.Tasks.AddAsync(task);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<TaskDto>(task);
        }

    }
}
