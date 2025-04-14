using Azure.Messaging.ServiceBus;
using PubSub.Interfaces;
using PubSub.Models;
using System.Text.Json;

namespace PubSub.Services.AzureServiceBus
{
    public class AzureServiceBusPublisher : IPublisher, IAsyncDisposable
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;
        private readonly string _topicName;

        public AzureServiceBusPublisher(string connectionString, string topicName)
        {
            _topicName = topicName;
            _client = new ServiceBusClient(connectionString);
            _sender = _client.CreateSender(topicName);
        }

        public async Task PublishMessageAsync(Message message)
        {
            var serviceBusMessage = new ServiceBusMessage(JsonSerializer.Serialize(message));
            serviceBusMessage.MessageId = message.MessageId;
            await _sender.SendMessageAsync(serviceBusMessage);
        }

        public async ValueTask DisposeAsync()
        {
            await _sender.DisposeAsync();
            await _client.DisposeAsync();
        }
    }
}