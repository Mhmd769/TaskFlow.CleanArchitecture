using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using TaskFlow.Application.DTOs.KafkaDTOs;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Infrastructure.Messaging
{
    public class TaskEventProducer : ITaskEventProducer
    {
        private readonly string _topic;
        private readonly IProducer<string, string> _producer;
        private readonly IEmailService _emailService;

        public TaskEventProducer(IConfiguration config, IEmailService emailService)
        {
            var settings = new ProducerConfig
            {
                BootstrapServers = config["Kafka:BootstrapServers"]
            };

            _topic = config["Kafka:Topics:TaskAssigned"];
            _producer = new ProducerBuilder<string, string>(settings).Build();
            _emailService = emailService;
        }

        public async Task PublishTaskAssignedAsync(TaskAssignedEvent evt)
        {
            // Serialize event
            var json = JsonSerializer.Serialize(evt);

            // Produce message to Kafka
            await _producer.ProduceAsync(_topic, new Message<string, string>
            {
                Key = evt.TaskId.ToString(),
                Value = json
            });

            // Send email to assigned users
            foreach (var user in evt.AssignedUsers)
            {
                string subject = $"New Task Assigned: {evt.TaskTitle}";
                string body = $"Hello {user.FullName},\n\n" +
                              $"You have been assigned a new task:\n\n" +
                              $"Title: {evt.TaskTitle}\n" +
                              $"Due Date: {evt.DueDate?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"}\n\n" +
                              $"Please check your TaskFlow dashboard for details.\n\n" +
                              $"Best regards,\nTaskFlow Team";

                // Fire-and-forget (or you can await)
                await _emailService.SendEmailAsync(user.Email, subject, body);
            }
        }
    }
}
