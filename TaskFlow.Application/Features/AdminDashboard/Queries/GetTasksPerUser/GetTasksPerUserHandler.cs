using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.AdminDTOs;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.AdminDashboard.Queries.GetTasksPerUser
{
    public class GetTasksPerUserHandler : IRequestHandler<GetTasksPerUserQuery, List<TasksPerUserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTasksPerUserHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TasksPerUserDto>> Handle(GetTasksPerUserQuery request, CancellationToken cancellationToken)
        {
            var perUser = await _unitOfWork.Tasks.GetAll()
                .SelectMany(t => t.AssignedUsers.Select(au => new { au.UserId, au.User.FullName }))
                .GroupBy(x => new { x.UserId, x.FullName })
                .Select(g => new TasksPerUserDto
                {
                    UserId = g.Key.UserId,
                    UserName = g.Key.FullName,
                    TaskCount = g.Count()
                })
                .OrderByDescending(x => x.TaskCount)
                .ToListAsync(cancellationToken);

            return perUser;
        }
    }
}

