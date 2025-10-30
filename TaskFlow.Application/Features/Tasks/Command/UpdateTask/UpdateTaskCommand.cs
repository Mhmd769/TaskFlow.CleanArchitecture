using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.TaskDTOs;

namespace TaskFlow.Application.Features.Tasks.Command.UpdateTask
{
    public class UpdateTaskCommand : IRequest<TaskDto>
    {
        public TaskDto Task { get; set; }
        public UpdateTaskCommand(TaskDto task)
        {
            Task = task;
        }
    }
}
