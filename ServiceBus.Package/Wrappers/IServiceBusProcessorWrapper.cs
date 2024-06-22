using Azure.Messaging.ServiceBus;

namespace ServiceBus.Package.Wrappers
{
    public interface IServiceBusProcessorWrapper
    {
        Task StartProcessingAsync();
        
        Task CloseAsync();

        event Func<ProcessMessageEventArgs, Task> ProcessMessageAsync;

        event Func<ProcessErrorEventArgs, Task> ProcessErrorAsync;
    }
}
