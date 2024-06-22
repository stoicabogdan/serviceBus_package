using Azure.Messaging.ServiceBus;
using ServiceBus.Package.Options;

namespace ServiceBus.Package.Wrappers
{
    public class ServiceBusQueueFactory<T> : IServiceBusQueueFactory<T>
    {
        private readonly ServiceBusClient _client;
        private readonly QueueOptions<T> _queueOptions;
        public ServiceBusQueueFactory(
            ServiceBusClient client, 
            QueueOptions<T> queueOptions)
        {
            _client = client;
            _queueOptions = queueOptions;
        }

        public Type MessageType => typeof(T);

        public IServiceBusProcessorWrapper CreateProcessor(ServiceBusProcessorOptions serviceBusProcessorOptions)
        {
            var queueName = _queueOptions.Name;
            return new ServiceBusProcessorWrapper(_client.CreateProcessor(queueName, serviceBusProcessorOptions));
        }

        public IServiceBusSenderWrapper<T> CreateSender()
        {
            var queueName = _queueOptions.Name;
            return new ServiceBusSenderWrapper<T>(_client.CreateSender(queueName));
        }
    }
}
