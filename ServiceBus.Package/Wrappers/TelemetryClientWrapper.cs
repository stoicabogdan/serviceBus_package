using Microsoft.ApplicationInsights;

namespace ServiceBus.Package.Wrappers
{
    public class TelemetryClientWrapper : ITelemetryClientWrapper
    {
        private readonly TelemetryClient _telemetryClient;

        public TelemetryClientWrapper(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public void TrackException(Exception exception, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            _telemetryClient.TrackException(exception, properties, metrics);
        }
    }
}
