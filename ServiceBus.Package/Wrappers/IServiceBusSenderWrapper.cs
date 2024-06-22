namespace ServiceBus.Package.Wrappers
{
    public interface IServiceBusSenderWrapper<T>: IServiceBusSenderWrapper
    {
    }

    public interface IServiceBusSenderWrapper
    {
        Type MessageType { get; }

        Task SendMessageAsync(
            object message, 
            Dictionary<string, string> extraApplciationProperties = null, 
            CancellationToken cancellationToken = default);
    }
}
