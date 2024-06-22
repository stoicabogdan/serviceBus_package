using Azure.Messaging.ServiceBus;

namespace ServiceBus.Package.Wrappers
{
    internal class ServiceBusProcessorWrapper : IServiceBusProcessorWrapper
    {
        private readonly ServiceBusProcessor _serviceBusProcessor;

        public ServiceBusProcessorWrapper(ServiceBusProcessor serviceBusProcessor)
        {
            _serviceBusProcessor = serviceBusProcessor;
        }

        public event Func<ProcessMessageEventArgs, Task> ProcessMessageAsync
        {
            add
            {
                _serviceBusProcessor.ProcessMessageAsync += value;
            }

            remove
            {
                _serviceBusProcessor.ProcessMessageAsync -= value;
            }
        }

        public event Func<ProcessErrorEventArgs, Task> ProcessErrorAsync
        {
            add
            {
                _serviceBusProcessor.ProcessErrorAsync += value;
            }

            remove
            {
                _serviceBusProcessor.ProcessErrorAsync -= value;
            }
        }

        public Task CloseAsync()
        {
            return _serviceBusProcessor.CloseAsync();
        }

        public Task StartProcessingAsync()
        {
            return _serviceBusProcessor.StartProcessingAsync();
        }
    }
}
