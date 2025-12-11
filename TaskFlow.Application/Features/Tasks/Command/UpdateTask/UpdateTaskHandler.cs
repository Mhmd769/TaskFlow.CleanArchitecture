using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.TaskDTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Tasks.Command.UpdateTask
{
    public class UpdateTaskHandler : IRequestHandler<UpdateTaskCommand, TaskDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        private readonly INotificationRepository _repo;


        public UpdateTaskHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache, INotificationRepository repo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
            _repo = repo;


        }

        public async Task<TaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Task;

            // 1️⃣ Fetch task with tracking (using Query() instead of GetAll() to get tracked entity)
            var task = await _unitOfWork.Tasks.Query()
                .Include(t => t.Project)
                .Include(t => t.AssignedUsers)
                    .ThenInclude(au => au.User)
                .FirstOrDefaultAsync(t => t.Id == dto.Id, cancellationToken);

            if (task == null)
                throw new NotFoundException("Task", dto.Id);

            // 2️⃣ Map simple fields
            task.Title = dto.Title;
            task.Description = dto.Description;
            task.ProjectId = dto.ProjectId;
            task.Status = dto.Status;
            task.DueDate = dto.DueDate;

            // 3️⃣ Update assigned users - clear existing and add new ones
            // Since the entity is tracked, EF Core will detect these changes
            task.AssignedUsers.Clear();
            
            foreach (var userId in dto.AssignedUserIds)
            {
                task.AssignedUsers.Add(new TaskAssignedUser 
                { 
                    TaskId = task.Id, 
                    UserId = userId 
                });
            }

            if (dto.AssignedUserIds != null && dto.AssignedUserIds.Any())
            {
                var notifications = dto.AssignedUserIds.Select(userId => new Notification
                {
                    UserId = userId.ToString(), // convert Guid to string
                    Message = $"Project manager update the task you are assigned in check it.: {task.Title}",
                    Link = $"/tasks/{task.Id}",
                    IsRead = false,
                    CreatedAt = System.DateTime.UtcNow
                }).ToList();

                await _repo.AddRangeAsync(notifications);
                await _repo.SaveAsync();
            }

            // 4️⃣ Save changes (entity is already tracked, so Update() will work correctly)
            await _unitOfWork.SaveAsync();

            // 5️⃣ Reload task with all relationships to ensure proper mapping
            var updatedTask = await _unitOfWork.Tasks.GetAll()
                .Include(t => t.Project)
                .Include(t => t.AssignedUsers)
                    .ThenInclude(au => au.User)
                .FirstOrDefaultAsync(t => t.Id == dto.Id, cancellationToken);

            if (updatedTask == null)
                throw new NotFoundException("Task", dto.Id);

            // 6️⃣ Clear cache
            await _cache.RemoveAsync($"task:{dto.Id}");
            await _cache.RemoveAsync("tasks:all");

            // 7️⃣ Return DTO
            return _mapper.Map<TaskDto>(updatedTask);
        }
    }
}
