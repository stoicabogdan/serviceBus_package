namespace ServiceBus.Package.Options
{
    public abstract class ServiceBusEntityOptions
    {
        public string Name { get; set; }

        public int MaxConcurrentCalls { get; set; }

        public int MaxAutoRenewDurationSeconds { get; set; }
    }
}
