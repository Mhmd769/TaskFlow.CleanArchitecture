using MediatR;
using System.Collections.Generic;
using TaskFlow.Application.DTOs.AdminDTOs;

namespace TaskFlow.Application.Features.AdminDashboard.Queries.GetTasksPerProject
{
    public class GetTasksPerProjectQuery : IRequest<List<TasksPerProjectDto>>
    {
    }
}

