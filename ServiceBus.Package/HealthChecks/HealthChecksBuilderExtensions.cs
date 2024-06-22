using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ServiceBus.Package.Options;

namespace ServiceBus.Package.HealthChecks
{
    public static class HealthChecksBuilderExtensions
    {
        public const string DefaultConnectionStringName = "ServiceBusEndpoint";
        public const string DefaultFullyQualifiedNamespaceName = "ServiceBusFullyQuallifiedNamespace";

        public static IHealthChecksBuilder AddAzureServiceBusQueue<T>(
            this IHealthChecksBuilder builder,
            HostBuilderContext context,
            bool isLocal = false,
            bool usePeekMode = true,
            string connectionStringName = DefaultConnectionStringName,
            string fullyQualifiedNamespaceName = DefaultFullyQualifiedNamespaceName)
        {
            if(isLocal)
            {
                return builder.AddAzureServiceBusQueue(
                    services => context.Configuration.GetConnectionString(connectionStringName),
                    services => services.GetRequiredService<IOptions<QueueOptions<T>>>().Value.Name,
                    conf => conf.UsePeekMode = usePeekMode,
                    name: $"AzureQueue-{typeof(T).Name}");
            }

            return builder.AddAzureServiceBusQueue(
                    services => context.Configuration.GetConnectionString(fullyQualifiedNamespaceName),
                    services => services.GetRequiredService<IOptions<QueueOptions<T>>>().Value.Name,
                    conf => conf.UsePeekMode = usePeekMode,
                    name: $"AzureQueue-{typeof(T).Name}");
        }

        public static IHealthChecksBuilder AddAzureServiceBusTopic<T>(
            this IHealthChecksBuilder builder,
            HostBuilderContext context,
            bool isLocal = false,
            string connectionStringName = DefaultConnectionStringName,
            string fullyQualifiedNamespaceName = DefaultFullyQualifiedNamespaceName)
        {
            if (isLocal)
            {
                return builder.AddAzureServiceBusTopic(
                    services => context.Configuration.GetConnectionString(connectionStringName),
                    services => services.GetRequiredService<IOptions<TopicOptions<T>>>().Value.Name,
                    name: $"AzureTopic-{typeof(T).Name}");
            }

            return builder.AddAzureServiceBusTopic(
                    services => context.Configuration.GetConnectionString(fullyQualifiedNamespaceName),
                    services => services.GetRequiredService<IOptions<TopicOptions<T>>>().Value.Name,
                    name: $"AzureTopic-{typeof(T).Name}");
        }
    }
}
