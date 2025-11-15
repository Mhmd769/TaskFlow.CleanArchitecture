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

namespace TaskFlow.Application.Features.Tasks.Command.DeleteTask
{
    public class DeleteTaskHandler : IRequestHandler<DeleteTaskCommand, TaskDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;  

        public DeleteTaskHandler(IUnitOfWork unitOfWork, IMapper mapper ,ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task<TaskDto> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId);

            if (task == null)
            {
                throw new NotFoundException("Task", request.TaskId);
            }

            _unitOfWork.Tasks.Delete(task);
            await _unitOfWork.SaveAsync();


            string cacheById = $"task:{request.TaskId}";
            string cacheAll = "tasks:all";

            await _cache.RemoveAsync(cacheById);
            await _cache.RemoveAsync(cacheAll);

            return _mapper.Map<TaskDto>(task);

        }
    }
}
