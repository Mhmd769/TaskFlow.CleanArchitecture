using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.AdminDTOs;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.AdminDashboard.Queries.GetTasksPerProject
{
    public class GetTasksPerProjectHandler : IRequestHandler<GetTasksPerProjectQuery, List<TasksPerProjectDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTasksPerProjectHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TasksPerProjectDto>> Handle(GetTasksPerProjectQuery request, CancellationToken cancellationToken)
        {
            var perProject = await _unitOfWork.Tasks.GetAll()
                .GroupBy(t => new { t.ProjectId, t.Project.Name })
                .Select(g => new TasksPerProjectDto
                {
                    ProjectId = g.Key.ProjectId,
                    ProjectName = g.Key.Name,
                    TaskCount = g.Count()
                })
                .OrderByDescending(p => p.TaskCount)
                .ToListAsync(cancellationToken);

            return perProject;
        }
    }
}

