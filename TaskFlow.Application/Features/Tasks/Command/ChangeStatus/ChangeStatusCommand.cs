using MediatR;
using System;
using TaskFlow.Application.DTOs.TaskDTOs;

namespace TaskFlow.Application.Features.Tasks.Command.ChangeStatus
{
    public class ChangeStatusCommand : IRequest<TaskDto>
    {
        public Guid TaskId { get; set; }
        public int NewStatus { get; set; }

        public ChangeStatusCommand(Guid taskId, int newStatus)
        {
            TaskId = taskId;
            NewStatus = newStatus;
        }
    }
}
