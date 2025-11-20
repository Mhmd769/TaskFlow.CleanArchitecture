
using TaskFlow.Application.DTOs.KafkaDTOs;

public interface ITaskEventProducer
{
    Task PublishTaskAssignedAsync(TaskAssignedEvent evt);
}