using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.TaskDTOs;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Tasks.Queries.GetTaskById
{
    public class GetTaskByIdHandler : IRequestHandler<GetTaskByIdQuery, TaskDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public GetTaskByIdHandler(IUnitOfWork unitOfWork, IMapper mapper , ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task<TaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"task:{request.TaskId}";
            var cachedTask = await _cache.GetAsync<TaskDto>(cacheKey);
            if (cachedTask != null)
            {
                return cachedTask;
            }

            var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId);
            if (task == null) {
                throw new NotFoundException("Taks" , request.TaskId);
            }

            var taskdto= _mapper.Map<TaskDto>(task);

            await _cache.SetAsync(cacheKey, taskdto, TimeSpan.FromMinutes(10));

            return taskdto;

        }
    }
}
