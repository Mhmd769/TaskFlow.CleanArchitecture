using MediatR;
using TaskFlow.Application.DTOs.AdminDTOs;

namespace TaskFlow.Application.Features.AdminDashboard.Queries.GetAdminDashboardStats
{
    public class GetAdminDashboardStatsQuery : IRequest<AdminDashboardStatsDto>
    {
    }
}

