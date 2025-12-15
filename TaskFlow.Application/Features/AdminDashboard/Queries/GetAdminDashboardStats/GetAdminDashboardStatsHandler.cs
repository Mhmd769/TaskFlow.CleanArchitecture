using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.DTOs.AdminDTOs;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Interfaces;
using TaskStatus = TaskFlow.Domain.Enums.TaskStatus;

namespace TaskFlow.Application.Features.AdminDashboard.Queries.GetAdminDashboardStats
{
    public class GetAdminDashboardStatsHandler : IRequestHandler<GetAdminDashboardStatsQuery, AdminDashboardStatsDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAdminDashboardStatsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminDashboardStatsDto> Handle(GetAdminDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var usersCount = await _unitOfWork.Users.GetAll().CountAsync(cancellationToken);
            var projectsCount = await _unitOfWork.Projects.GetAll().CountAsync(cancellationToken);
            var tasks = await _unitOfWork.Tasks.GetAll().ToListAsync(cancellationToken);

            return new AdminDashboardStatsDto
            {
                TotalUsers = usersCount,
                TotalProjects = projectsCount,
                TotalTasks = tasks.Count,
                PendingTasks = tasks.Count(t => t.Status == TaskStatus.Pending),
                InProgressTasks = tasks.Count(t => t.Status == TaskStatus.InProgress),
                CompletedTasks = tasks.Count(t => t.Status == TaskStatus.Completed),
                CancelledTasks = tasks.Count(t => t.Status == TaskStatus.Cancelled),
                OverdueTasks = tasks.Count(t => t.DueDate != null && t.DueDate < now && t.Status != TaskStatus.Completed)
            };
        }
    }
}

