using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.TaskDTOs;

namespace TaskFlow.Application.Features.Tasks.Command.CreateTask
{
    public class CreateTaskCommand : IRequest<TaskDto>
    {
        public CreateTaskDto Task { get; set; }

        public CreateTaskCommand(CreateTaskDto task)
        {
            Task = task;
        }
    }
}
