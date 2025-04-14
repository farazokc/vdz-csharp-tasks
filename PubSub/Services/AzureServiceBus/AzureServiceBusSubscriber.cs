using Azure.Messaging.ServiceBus;
using PubSub.Context;
using PubSub.Interfaces;
using PubSub.Models;
using System.Text.Json;

namespace PubSub.Services.AzureServiceBus
{
    public class AzureServiceBusSubscriber : ISubscriber, IAsyncDisposable
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusProcessor _processor;
        private readonly onboardingContext _dbContext;
        private readonly string _subscriberId;

        public AzureServiceBusSubscriber(
            string connectionString,
            string topicName,
            string subscriptionName,
            onboardingContext dbContext,
            string subscriberId)
        {
            _dbContext = dbContext;
            _subscriberId = subscriberId;
            _client = new ServiceBusClient(connectionString);
            _processor = _client.CreateProcessor(topicName, subscriptionName);
            _processor.ProcessMessageAsync += ProcessMessageAsync;
            _processor.ProcessErrorAsync += ProcessErrorAsync;
        }

        public async Task StartAsync()
        {
            await _processor.StartProcessingAsync();
        }

        public async Task StopAsync()
        {
            await _processor.StopProcessingAsync();
        }

        private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            var message = JsonSerializer.Deserialize<Message>(args.Message.Body.ToString());
            message.Source = "AzureServiceBus";

            //var messageLog = new MessageLog
            //{
            //    MessageId = message.MessageId,
            //    SubscriberId = _subscriberId,
            //    Content = message.Content,
            //    Timestamp = message.Timestamp,
            //    Source = "AzureServiceBus"
            //};

            await _dbContext.Messages.AddAsync(message);
            await _dbContext.SaveChangesAsync();

            await args.CompleteMessageAsync(args.Message);
        }

        private Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Error occurred: {args.Exception.Message}");
            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            await _processor.DisposeAsync();
            await _client.DisposeAsync();
        }
    }
}