using MediatR;
using System;
using System.Collections.Generic;
using TaskFlow.Application.DTOs.AdminDTOs;

namespace TaskFlow.Application.Features.AdminDashboard.Queries.GetCalendarTasks
{
    public class GetCalendarTasksQuery : IRequest<List<CalendarTaskDto>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

