namespace ServiceBus.Package.Options
{
    public class TopicOptions : ServiceBusEntityOptions
    {
        public string SubscriptionName { get; set; }
    }

    public class TopicOptions<T> : TopicOptions { }
}
