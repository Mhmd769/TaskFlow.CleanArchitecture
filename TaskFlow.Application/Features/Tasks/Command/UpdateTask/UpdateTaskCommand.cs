using MediatR;
using TaskFlow.Application.DTOs.TaskDTOs;

namespace TaskFlow.Application.Features.Tasks.Command.UpdateTask
{
    public class UpdateTaskCommand : IRequest<TaskDto>
    {
        public UpdateTaskDto Task { get; set; }

        public UpdateTaskCommand(UpdateTaskDto task)
        {
            Task = task;
        }
    }
}
