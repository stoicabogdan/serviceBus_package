namespace ServiceBus.Package.Wrappers
{
    public interface ITelemetryClientWrapper
    {
        void TrackException(Exception exception, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null);
    }
}
