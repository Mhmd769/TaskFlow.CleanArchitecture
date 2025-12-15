using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.AdminDTOs;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Interfaces;
using TaskStatus = TaskFlow.Domain.Enums.TaskStatus;

namespace TaskFlow.Application.Features.AdminDashboard.Queries.GetTasksOverTime
{
    public class GetTasksOverTimeHandler : IRequestHandler<GetTasksOverTimeQuery, List<TasksOverTimeDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTasksOverTimeHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TasksOverTimeDto>> Handle(GetTasksOverTimeQuery request, CancellationToken cancellationToken)
        {
            var start = (request.StartDate ?? DateTime.UtcNow.AddDays(-30)).Date;
            var end = (request.EndDate ?? DateTime.UtcNow).Date;

            if (end < start)
            {
                (start, end) = (end, start);
            }

            var createdPerDay = await _unitOfWork.Tasks.GetAll()
                .Where(t => t.CreatedAt.Date >= start && t.CreatedAt.Date <= end)
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            // The model does not track completion dates, so we approximate using CreatedAt for tasks whose current status is Completed.
            var completedPerDay = await _unitOfWork.Tasks.GetAll()
                .Where(t => t.Status == TaskStatus.Completed && t.CreatedAt.Date >= start && t.CreatedAt.Date <= end)
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var createdLookup = createdPerDay.ToDictionary(x => x.Date, x => x.Count);
            var completedLookup = completedPerDay.ToDictionary(x => x.Date, x => x.Count);

            var totalDays = (end - start).Days + 1;
            var timeline = new List<TasksOverTimeDto>(totalDays);

            for (var i = 0; i < totalDays; i++)
            {
                var date = start.AddDays(i);

                createdLookup.TryGetValue(date, out var createdCount);
                completedLookup.TryGetValue(date, out var completedCount);

                timeline.Add(new TasksOverTimeDto
                {
                    Date = date,
                    Created = createdCount,
                    Completed = completedCount
                });
            }

            return timeline;
        }
    }
}

