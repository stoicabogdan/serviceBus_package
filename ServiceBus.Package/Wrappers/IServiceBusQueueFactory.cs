using Azure.Messaging.ServiceBus;

namespace ServiceBus.Package.Wrappers
{
    public interface IServiceBusQueueFactory<T>: IServiceBusQueueFactory
    {
        IServiceBusSenderWrapper<T> CreateSender();
    }


    // This interface us useful for event dispatcher type services where you use a group of queues and decide which one to publish to based on their messageType
    public interface IServiceBusQueueFactory
    {
        Type MessageType { get; }

        IServiceBusProcessorWrapper CreateProcessor(ServiceBusProcessorOptions serviceBusProcessorOptions);
    }
}
