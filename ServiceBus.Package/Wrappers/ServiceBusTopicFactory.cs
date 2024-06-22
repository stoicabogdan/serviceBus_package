using Azure.Messaging.ServiceBus;
using ServiceBus.Package.Options;

namespace ServiceBus.Package.Wrappers
{
    public class ServiceBusTopicFactory<T>: IServiceBusTopicFactory<T>
    {
        private readonly ServiceBusClient _client;

        private readonly TopicOptions<T> _topicOptions;

        public ServiceBusTopicFactory(
            ServiceBusClient client, 
            TopicOptions<T> topicOptions)
        {
            _client = client;
            _topicOptions = topicOptions;
        }

        public Type MessageType => typeof(T);

        public IServiceBusSenderWrapper<T> CreateSender() 
        {
            var topicName = _topicOptions.Name;
            return new ServiceBusSenderWrapper<T>(_client.CreateSender(topicName));
        }
    }
}
