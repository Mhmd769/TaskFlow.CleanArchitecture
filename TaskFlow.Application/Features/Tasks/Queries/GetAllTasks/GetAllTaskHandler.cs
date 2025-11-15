using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public GetAllTaskHandler(IUnitOfWork unitOfWork, IMapper mapper , ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task<List<TaskDto>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = "tasks:all";

            var cachedTasks = await _cache.GetAsync<List<TaskDto>>(cacheKey);
            if (cachedTasks != null)
            {
                return cachedTasks;
            }

            var tasks= _unitOfWork.Tasks.GetAll();
            if (tasks == null)
            {
                throw new AppException("No Tasks Founded");
            }

            var taskdto = await _mapper.ProjectTo<TaskDto>(tasks).ToListAsync(cancellationToken);

            await _cache.SetAsync(cacheKey, taskdto, TimeSpan.FromMinutes(10));

            return taskdto;
        }
    }
}
