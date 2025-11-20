using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs.KafkaDTOs;

namespace TaskFlow.Infrastructure.Messaging
{
    public class TaskEventProducer : ITaskEventProducer
    {
        private readonly string _topic;
        private readonly IProducer<string, string> _producer;

        public TaskEventProducer(IConfiguration config)
        {
            var settings = new ProducerConfig
            {
                BootstrapServers = config["Kafka:BootstrapServers"]
            };

            _topic = config["Kafka:Topics:TaskAssigned"];

            _producer = new ProducerBuilder<string, string>(settings).Build();
        }

        public async Task PublishTaskAssignedAsync(TaskAssignedEvent evt)
        {
            var json = JsonSerializer.Serialize(evt);

            await _producer.ProduceAsync(_topic, new Message<string, string>
            {
                Key = evt.TaskId.ToString(),
                Value = json
            });
        }
    }
}
