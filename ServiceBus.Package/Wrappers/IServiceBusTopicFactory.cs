namespace ServiceBus.Package.Wrappers
{
    public interface IServiceBusTopicFactory<T>: IServiceBusTopicFactory
    {
        IServiceBusSenderWrapper<T> CreateSender();
    }

    /*
     This interface is useful for event dispatcher type services where you choose which of a list of topics you dispatch to
     */
    public interface IServiceBusTopicFactory
    {
        Type MessageType { get; }
    }
}
