namespace PubSub.Interfaces
{
    public interface ISubscriber
    {
        Task StartAsync();
        Task StopAsync();
    }
}
