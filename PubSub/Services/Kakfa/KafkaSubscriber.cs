using Confluent.Kafka;
using PubSub.Context;
using PubSub.Interfaces;
using PubSub.Models;
using System.Text.Json;

namespace PubSub.Services.Kafka
{
    public class KafkaSubscriber : ISubscriber
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly onboardingContext _dbContext;
        private readonly string _subscriberId;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public KafkaSubscriber(
            string bootstrapServers,
            string topic,
            string groupId,
            onboardingContext dbContext,
            string subscriberId)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<string, string>(config).Build();
            _consumer.Subscribe(topic);
            _dbContext = dbContext;
            _subscriberId = subscriberId;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartAsync()
        {
            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(_cancellationTokenSource.Token);
                    var message = JsonSerializer.Deserialize<Message>(consumeResult.Message.Value);
                    message.Source = "Kafka";

                    //var messageLog = new MessageLog
                    //{
                    //    MessageId = message.MessageId,
                    //    SubscriberId = _subscriberId,
                    //    Content = message.Content,
                    //    Timestamp = message.Timestamp,
                    //    Source = "Kafka"
                    //};

                    await _dbContext.Messages.AddAsync(message);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (OperationCanceledException)
            {
                _consumer.Close();
            }
        }

        public Task StopAsync()
        {
            _cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }
    }
}