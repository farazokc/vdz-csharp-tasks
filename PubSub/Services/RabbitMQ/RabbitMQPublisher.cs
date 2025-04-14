using Microsoft.EntityFrameworkCore.Metadata;
using PubSub.Interfaces;
using PubSub.Models;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace PubSub.Services.RabbitMQ
{
    public class RabbitMQPublisher : IPublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchangeName;

        public RabbitMQPublisher(string hostName, string exchangeName)
        {
            _exchangeName = exchangeName;
            var factory = new ConnectionFactory { HostName = hostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Fanout);
        }

        public Task PublishMessageAsync(Message message)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            _channel.BasicPublish(_exchangeName, "", null, body);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}