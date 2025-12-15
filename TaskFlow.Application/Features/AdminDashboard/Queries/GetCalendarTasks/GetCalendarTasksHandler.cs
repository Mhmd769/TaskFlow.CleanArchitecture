using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.AdminDTOs;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.AdminDashboard.Queries.GetCalendarTasks
{
    public class GetCalendarTasksHandler : IRequestHandler<GetCalendarTasksQuery, List<CalendarTaskDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCalendarTasksHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CalendarTaskDto>> Handle(GetCalendarTasksQuery request, CancellationToken cancellationToken)
        {
            var start = (request.StartDate ?? DateTime.UtcNow.AddDays(-15)).Date;
            var end = (request.EndDate ?? DateTime.UtcNow.AddDays(30)).Date;

            if (end < start)
            {
                (start, end) = (end, start);
            }

            var tasks = await _unitOfWork.Tasks.GetAll()
                .Where(t => t.DueDate != null && t.DueDate.Value.Date >= start && t.DueDate.Value.Date <= end)
                .Select(t => new CalendarTaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
                    DueDate = t.DueDate,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project.Name
                })
                .OrderBy(t => t.DueDate)
                .ToListAsync(cancellationToken);

            return tasks;
        }
    }
}

