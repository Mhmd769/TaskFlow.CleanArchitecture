using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        private readonly ITaskEventProducer _eventProducer;

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
            var dto = request.Task;

            // 1️⃣ Create Task entity
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                ProjectId = dto.ProjectId,
                DueDate = dto.DueDate
            };

            // 2️⃣ Assign users
            task.AssignedUsers = dto.AssignedUserIds
                .Select(id => new TaskAssignedUser { UserId = id })
                .ToList();

            // 3️⃣ Save task
            await _unitOfWork.Tasks.AddAsync(task);
            await _unitOfWork.SaveAsync();

            // 4️⃣ Load full task with Project and Users
            var fullTask = await _unitOfWork.Tasks.GetAll()
                .Include(t => t.Project)
                .Include(t => t.AssignedUsers)
                    .ThenInclude(au => au.User)
                .FirstOrDefaultAsync(t => t.Id == task.Id, cancellationToken);

            if (fullTask == null)
                throw new System.Exception("Task creation failed");

            // 5️⃣ Publish Kafka event
            var assignedUsers = fullTask.AssignedUsers
                .Select(u => new UserDto
                {
                    Id = u.UserId,
                    FullName = u.User.FullName,
                    Email = u.User.Email
                }).ToList();

            await _eventProducer.PublishTaskAssignedAsync(new TaskAssignedEvent
            {
                TaskId = fullTask.Id,
                TaskTitle = fullTask.Title,
                ProjectId = fullTask.ProjectId,
                DueDate = fullTask.DueDate,
                AssignedUsers = assignedUsers
            });

            // 6️⃣ Return DTO for frontend
            return _mapper.Map<TaskDto>(fullTask);
        }
    }
}
