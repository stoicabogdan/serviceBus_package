using Azure.Messaging.ServiceBus;
using ServiceBus.Package.Messaging;
using System.Diagnostics;

namespace ServiceBus.Package.Wrappers
{
    public class ServiceBusSenderWrapper<T> : IServiceBusSenderWrapper<T>
    {
        private readonly ServiceBusSender _serviceBusSender;

        public ServiceBusSenderWrapper(ServiceBusSender serviceBusSender)
        {
            _serviceBusSender = serviceBusSender;
        }

        public Type MessageType => typeof(T);

        public Task SendMessageAsync(
            object message, 
            Dictionary<string, string> extraApplciationProperties = null, 
            CancellationToken cancellationToken = default)
        {
            var serviceBusMessage = message.ToMessage();

            if(Activity.Current != null)
            {
                // Azure Messaging ServiceBus does not support Correlation Context
                serviceBusMessage.AddHeadersFromActivity(Activity.Current, extraApplciationProperties);
            }

            return _serviceBusSender.SendMessageAsync(serviceBusMessage, cancellationToken);
        }
    }
}
