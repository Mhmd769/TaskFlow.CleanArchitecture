using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using TaskFlow.Application.DTOs.TaskDTOs;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Tasks.Command.UpdateTask
{
    public class UpdateTaskHandler : IRequestHandler<UpdateTaskCommand, TaskDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateTaskHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<TaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var existingtask = await _unitOfWork.Tasks.GetByIdAsync(request.Task.Id);

            if (existingtask != null)
            {
                throw new Exception("Task not found");
            }
            _mapper.Map(request.Task, existingtask);

            _unitOfWork.Tasks.Update(existingtask);

            await _unitOfWork.SaveAsync();

            return _mapper.Map<TaskDto>(existingtask);

        }
    }
}
