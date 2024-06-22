using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using ServiceBus.Package.Configuration;
using ServiceBus.Package.Messaging;
using ServiceBus.Package.Options;
using ServiceBus.Package.Telemetry;
using ServiceBus.Package.Wrappers;
using System.Diagnostics;

namespace ServiceBus.Package.Hosting
{
    public class ServiceBusListenerHostedService<T> : IHostedService 
        where T: class
    {
        private const int DefaultMaxConcurrentCalls = 10;

        private readonly IServiceBusQueueFactory<T> _factory;

        private readonly IMessageHandler<T> _handler;

        private readonly ITelemetryClientWrapper _telemetryClient;

        private readonly ExceptionsConfiguration<T> _exceptionsConfiguration;

        private readonly QueueOptions<T> _queueOptions;

        private IServiceBusProcessorWrapper _processor;


        public ServiceBusListenerHostedService(
            IServiceBusQueueFactory<T> factory,
            IMessageHandler<T> handler,
            ITelemetryClientWrapper telemetryClient,
            ExceptionsConfiguration<T> exceptionsConfiguration,
            QueueOptions<T> queueOptions)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _handler = handler;
            _telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            _exceptionsConfiguration = exceptionsConfiguration;
            _queueOptions = queueOptions;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var serviceBusProcessorOptions = new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = DefaultMaxConcurrentCalls,
            };

            if(_queueOptions.MaxAutoRenewDurationSeconds > 0)
                serviceBusProcessorOptions.MaxAutoLockRenewalDuration = TimeSpan.FromSeconds(_queueOptions.MaxAutoRenewDurationSeconds);

            if(_queueOptions.MaxConcurrentCalls > 0)
                serviceBusProcessorOptions.MaxConcurrentCalls = _queueOptions.MaxConcurrentCalls;

            _processor = _factory.CreateProcessor(serviceBusProcessorOptions);
            _processor.ProcessMessageAsync += HandleMessage;
            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => _processor.CloseAsync();

        private async Task HandleMessage(ProcessMessageEventArgs args)
        {
            try
            {
                Activity.Current?.AddTag(TelemetryConstants.MessageTypeTagName, typeof(T).Name);

                if(args.Message.TryExtractContext(out IList<KeyValuePair<string, string>> ctx))
                {
                    foreach (var kvp in ctx)
                    {
                        Activity.Current?.AddBaggage(kvp.Key, kvp.Value);
                    }
                }

                await _handler.Handle(args.Message.GetJsonBody<T>(), args.CancellationToken);
                await args.CompleteMessageAsync(args.Message, args.CancellationToken);
            }
            catch (Exception ex)
            {
                if (_exceptionsConfiguration.NonRetryableExceptions.Contains(ex.GetType()))
                {
                    _telemetryClient.TrackException(ex, new Dictionary<string, string> { { "CustomException", "Non-retryable" } });
                    await args.DeadLetterMessageAsync(
                        args.Message,
                        ex.GetType().Name,
                        ex.Message,
                        args.CancellationToken);
                }
                else
                {
                    _telemetryClient.TrackException(ex, new Dictionary<string, string> { { "CustomException", "Retryable" } });
                    await args.AbandonMessageAsync(args.Message, cancellationToken: args.CancellationToken);
                }
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            /*
             * Mandatory Implementation for the processor to start
             * No-op, unhandled exceptions need to be managed by handler
             */
            return Task.CompletedTask;
        }
    }
}
