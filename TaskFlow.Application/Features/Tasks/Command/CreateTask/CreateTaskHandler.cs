using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.KafkaDTOs;
using TaskFlow.Application.DTOs.TaskDTOs;
using TaskFlow.Application.DTOs.UserDTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Tasks.Command.CreateTask
{
    public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, TaskDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITaskEventProducer _eventProducer; // <-- Kafka Producer

        public CreateTaskHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITaskEventProducer eventProducer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _eventProducer = eventProducer;
        }

        public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            // Map Task data
            var task = _mapper.Map<TaskItem>(request.Task);

            // Assign Users
            task.AssignedUsers = request.Task.AssignedUsers?
                .Select(x => new TaskAssignedUser { UserId = x.Id })
                .ToList() ?? new List<TaskAssignedUser>();

            // Save Task
            await _unitOfWork.Tasks.AddAsync(task);
            await _unitOfWork.SaveAsync();

            // --------------------------
            // FETCH FULL USER DATA
            // --------------------------
            var assignedUsers = new List<UserDto>();

            foreach (var au in task.AssignedUsers)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(au.UserId);

                if (user != null)
                {
                    assignedUsers.Add(new UserDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                    });
                }
            }

            // --------------------------
            // BUILD KAFKA EVENT
            // --------------------------
            var taskAssignedEvent = new TaskAssignedEvent
            {
                TaskId = task.Id,
                TaskTitle = task.Title,
                ProjectId = task.ProjectId,
                DueDate = task.DueDate,
                AssignedUsers = assignedUsers
            };

            // --------------------------
            // PUBLISH TO KAFKA
            // --------------------------
            await _eventProducer.PublishTaskAssignedAsync(taskAssignedEvent);

            // Map to DTO for response
            return _mapper.Map<TaskDto>(task);
        }
    }
}
