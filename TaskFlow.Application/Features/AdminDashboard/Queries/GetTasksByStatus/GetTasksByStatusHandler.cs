using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.DTOs.AdminDTOs;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Interfaces;
using TaskStatus = TaskFlow.Domain.Enums.TaskStatus;

namespace TaskFlow.Application.Features.AdminDashboard.Queries.GetTasksByStatus
{
    public class GetTasksByStatusHandler : IRequestHandler<GetTasksByStatusQuery, TasksByStatusDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTasksByStatusHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TasksByStatusDto> Handle(GetTasksByStatusQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var statusCounts = await _unitOfWork.Tasks.GetAll()
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Pending = g.Count(t => t.Status == TaskStatus.Pending),
                    InProgress = g.Count(t => t.Status == TaskStatus.InProgress),
                    Completed = g.Count(t => t.Status == TaskStatus.Completed),
                    Cancelled = g.Count(t => t.Status == TaskStatus.Cancelled),
                    Overdue = g.Count(t => t.DueDate != null && t.DueDate < now && t.Status != TaskStatus.Completed)
                })
                .FirstOrDefaultAsync(cancellationToken);

            return new TasksByStatusDto
            {
                Pending = statusCounts?.Pending ?? 0,
                InProgress = statusCounts?.InProgress ?? 0,
                Completed = statusCounts?.Completed ?? 0,
                Cancelled = statusCounts?.Cancelled ?? 0,
                Overdue = statusCounts?.Overdue ?? 0
            };
        }
    }
}

