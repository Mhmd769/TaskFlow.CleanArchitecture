using MediatR;
using System.Collections.Generic;
using TaskFlow.Application.DTOs.AdminDTOs;

namespace TaskFlow.Application.Features.AdminDashboard.Queries.GetTasksPerUser
{
    public class GetTasksPerUserQuery : IRequest<List<TasksPerUserDto>>
    {
    }
}

