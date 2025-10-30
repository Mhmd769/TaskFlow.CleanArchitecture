using MediatR;
using TaskFlow.Application.DTOs.TaskDTOs;

public class GetTaskByIdQuery : IRequest<TaskDto>
{
    public Guid TaskId { get; set; }
}
