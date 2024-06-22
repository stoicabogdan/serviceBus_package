namespace ServiceBus.Package.Telemetry
{
    public record TelemetryConstants
    {
        public const string DefaultServiceBusRequestName = "ServiceBusProcessor.ProcessMessage";

        public const string MessageTypeTagName = "MessageType";
    }
}
