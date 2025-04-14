using Confluent.Kafka;
using PubSub.Interfaces;
using PubSub.Models;
using System.Text.Json;

namespace PubSub.Services.Kafka
{
    public class KafkaPublisher : IPublisher, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topic;

        public KafkaPublisher(string bootstrapServers, string topic)
        {
            _topic = topic;
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };
            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task PublishMessageAsync(Message message)
        {
            await _producer.ProduceAsync(_topic, new Message<string, string>
            {
                Key = message.MessageId,
                Value = JsonSerializer.Serialize(message)
            });
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}