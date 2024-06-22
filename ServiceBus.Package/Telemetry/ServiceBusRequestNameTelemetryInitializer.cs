using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System.Diagnostics;

namespace ServiceBus.Package.Telemetry
{
    public class ServiceBusRequestNameTelemetryInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry is RequestTelemetry req && req.Name == TelemetryConstants.DefaultServiceBusRequestName)
                TryOverrideRequestNameFromActivity(req);
        }

        private static void TryOverrideRequestNameFromActivity(RequestTelemetry req)
        {
            if (Activity.Current == null ||
                !Activity.Current.Tags.Any(t => t.Key == TelemetryConstants.MessageTypeTagName))
                return;

            var tag = Activity.Current.Tags.First(t => t.Key == TelemetryConstants.MessageTypeTagName);
            req.Name = $"Process {tag.Value}";

            if (!req.Properties.ContainsKey(TelemetryConstants.MessageTypeTagName))
                req.Properties.Add(TelemetryConstants.MessageTypeTagName, tag.Value);
        }
    }
}
