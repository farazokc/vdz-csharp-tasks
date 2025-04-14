using PubSub.Models;

namespace PubSub.Interfaces
{
    public interface IPublisher
    {
        Task PublishMessageAsync(Message message);
    }
}
