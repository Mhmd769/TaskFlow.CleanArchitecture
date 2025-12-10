using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.TaskDTOs;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Tasks.Command.ChangeStatus
{
    public class ChangeStatusHandler : IRequestHandler<ChangeStatusCommand, TaskDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public ChangeStatusHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<TaskDto> Handle(ChangeStatusCommand request, CancellationToken cancellationToken)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId);
            if (task == null)
                throw new NotFoundException("Task", request.TaskId);

            // Update status
            task.Status = (Domain.Enums.TaskStatus)request.NewStatus;
            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.SaveAsync();

            // Load assigned users for DTO mapping if needed
            var updatedTask = await _unitOfWork.Tasks.Query()
                .Where(t => t.Id == request.TaskId)
                .Include(t => t.AssignedUsers)
                    .ThenInclude(au => au.User)
                .FirstOrDefaultAsync(cancellationToken);

            var dto = _mapper.Map<TaskDto>(updatedTask);

            // Invalidate caches so consumers see the new status
            await _cache.RemoveAsync($"task:{request.TaskId}");
            await _cache.RemoveAsync("tasks:all");
            await _cache.SetAsync($"task:{request.TaskId}", dto, TimeSpan.FromMinutes(10));

            return dto;
        }
    }
}
