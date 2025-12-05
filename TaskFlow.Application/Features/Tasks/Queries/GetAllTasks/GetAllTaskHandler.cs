using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.TaskDTOs;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Tasks.Queries.GetAllTasks
{
    public class GetAllTaskHandler : IRequestHandler<GetAllTasksQuery, List<TaskDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public GetAllTaskHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<List<TaskDto>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = "tasks:all";

            // 1️⃣ Check cache
            var cachedTasks = await _cache.GetAsync<List<TaskDto>>(cacheKey);
            if (cachedTasks != null)
                return cachedTasks;

            // 2️⃣ Load tasks with Project and AssignedUsers -> User
            var tasks = await _unitOfWork.Tasks.GetAll()
                .Include(t => t.Project)
                .Include(t => t.AssignedUsers)
                    .ThenInclude(au => au.User)
                .ToListAsync(cancellationToken);

            if (!tasks.Any())
                throw new AppException("No tasks found");

            // 3️⃣ Map manually to ensure ProjectName and AssignedUserNames
            var taskDtos = tasks.Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                DueDate = t.DueDate,
                ProjectId = t.ProjectId,
                ProjectName = t.Project?.Name ?? "N/A",
                AssignedUserIds = t.AssignedUsers.Select(au => au.UserId).ToList(),
                AssignedUserNames = t.AssignedUsers.Select(au => au.User.FullName).ToList()
            }).ToList();

            // 4️⃣ Cache the result
            await _cache.SetAsync(cacheKey, taskDtos, TimeSpan.FromMinutes(10));

            return taskDtos;
        }
    }
}
