namespace ServiceBus.Package.Configuration
{
    public class ExceptionsConfiguration<T>
    {
        public IEnumerable<Type> NonRetryableExceptions { get; }
        public ExceptionsConfiguration(IEnumerable<Type> nonRetryableExceptions) 
        {
            NonRetryableExceptions = nonRetryableExceptions ?? Array.Empty<Type>();
        }
    }
}
