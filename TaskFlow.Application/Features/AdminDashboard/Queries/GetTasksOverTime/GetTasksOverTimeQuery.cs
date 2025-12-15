using MediatR;
using System;
using System.Collections.Generic;
using TaskFlow.Application.DTOs.AdminDTOs;

namespace TaskFlow.Application.Features.AdminDashboard.Queries.GetTasksOverTime
{
    public class GetTasksOverTimeQuery : IRequest<List<TasksOverTimeDto>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

