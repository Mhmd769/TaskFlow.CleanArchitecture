using MediatR;
using TaskFlow.Application.DTOs.AdminDTOs;

namespace TaskFlow.Application.Features.AdminDashboard.Queries.GetTasksByStatus
{
    public class GetTasksByStatusQuery : IRequest<TasksByStatusDto>
    {
    }
}

