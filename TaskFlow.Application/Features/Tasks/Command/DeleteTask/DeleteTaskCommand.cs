using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.TaskDTOs;

namespace TaskFlow.Application.Features.Tasks.Command.DeleteTask
{
    public class DeleteTaskCommand : IRequest<TaskDto>
    {
        public Guid TaskId { get; set; }
    }
}
