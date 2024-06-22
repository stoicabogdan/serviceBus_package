using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace ServiceBus.Package.Telemetry
{
    internal class ServiceBusTelemetryReducer(ITelemetryProcessor next) : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next = next;

        public void Process(ITelemetry item)
        {
            var isServiceBusReceiveTelemetry = item is DependencyTelemetry telemetry
                && (telemetry.Type?.Equals("Azure Service Bus", System.StringComparison.OrdinalIgnoreCase)).GetValueOrDefault()
                 && (telemetry.Name?.Equals("ServiceBusReceiver.Receive", System.StringComparison.OrdinalIgnoreCase)).GetValueOrDefault();

            if (isServiceBusReceiveTelemetry)
                _next.Process(item);
        }
    }
}
