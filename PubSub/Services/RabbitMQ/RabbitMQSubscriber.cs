using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore.Metadata;
using PubSub.Context;
using PubSub.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace PubSub.Services.RabbitMQ
{
    public class RabbitMQSubscriber : ISubscriber, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly onboardingContext _dbContext;
        private readonly string _subscriberId;
        private readonly string _queueName;

        public RabbitMQSubscriber(
            string hostName,
            string exchangeName,
            onboardingContext dbContext,
            string subscriberId)
        {
            _dbContext = dbContext;
            _subscriberId = subscriberId;

            var factory = new ConnectionFactory { HostName = hostName };
            _connection = (IConnection?)factory.CreateConnectionAsync();
            _channel = _connection.CreateModel();

            // Declare exchange
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);

            // Create queue with random name
            _queueName = _channel.QueueDeclare().QueueName;

            // Bind queue to exchange
            _channel.QueueBind(_queueName, exchangeName, "");
        }

        public Task StartAsync()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<Message>(messageJson);

                    var messageLog = new MessageLog
                    {
                        MessageId = message.MessageId,
                        SubscriberId = _subscriberId,
                        Content = message.Content,
                        Timestamp = message.Timestamp,
                        Source = "RabbitMQ"
                    };

                    await _dbContext.MessageLogs.AddAsync(messageLog);
                    await _dbContext.SaveChangesAsync();

                    // Acknowledge the message
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    // Negative acknowledgment - message will be requeued
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            // Start consuming
            _channel.BasicConsume(queue: _queueName,
                                autoAck: false,
                                consumer: consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}