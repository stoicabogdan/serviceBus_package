using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceBus.Package.Hosting;
using ServiceBus.Package.Options;
using ServiceBus.Package.Wrappers;

namespace ServiceBus.Package.Configuration
{
    public static class ServiceCollectionExtensions
    {
        // ADD COMMENT
        public static void AddServiceBusClientWithConnectionString(
            this IServiceCollection services, 
            string connectionString)
        {
            services.AddAzureClients(cfg =>
            {
                cfg.AddServiceBusClient(connectionString);
            });
        }

        // ADD COMMENT
        public static void AddServiceBusClientWithNameSpaceAndDefaultAzureCredential(
            this IServiceCollection services,
            string fullyQualifiedNamespace)
        {
            services.AddAzureClients(cfg =>
            {
                cfg.AddServiceBusClientWithNamespace(fullyQualifiedNamespace);
                cfg.UseCredential(new DefaultAzureCredential());
                services.AddSingleton<TokenCredential>(new DefaultAzureCredential());
            });
        }

        public static void AddServiceBusListenerService<T, THandler>(
            this IServiceCollection services,
            HostBuilderContext context,
            IEnumerable<Type> nonRetryableExceptions = default,
            bool isLocal = false)
            where T: class
            where THandler : IMessageHandler<T>
        {
            services.ConfigureServiceBusQueueOptions<T>(context, isLocal);

            services.AddTransient<ITelemetryClientWrapper, TelemetryClientWrapper>();
            services.AddHostedService<ServiceBusListenerHostedService<T>>();

            services.AddSingleton(new ExceptionsConfiguration<T>(nonRetryableExceptions));

            services.AddQueueFactory<T>();
            services.AddSingleton(typeof(IMessageHandler<T>), typeof(THandler));
        }

        public static void AddServiceBusQueuePublisherServices<T>(
            this IServiceCollection services,
            HostBuilderContext context,
            bool isLocal = false)
            where T : class
        {
            AddServiceBusQueuePublisherServices<T>(services, context.Configuration, isLocal);
        }

        public static void AddServiceBusQueuePublisherServices<T>(
            this IServiceCollection services,
            IConfiguration configuration,
            bool isLocal = false)
            where T : class
        {
            services
                .AddQueueFactory<T>()
                .ConfigureServiceBusQueueOptions<T>(configuration, isLocal)
                .AddQueueSender<T>();
        }

        public static void AddServiceBusTopicPublisherServices<T>(
            this IServiceCollection services,
            HostBuilderContext context,
            bool isLocal = false)
            where T : class
        {
            AddServiceBusTopicPublisherServices<T>(services, context.Configuration, isLocal);
        }

        public static void AddServiceBusTopicPublisherServices<T>(
            this IServiceCollection services,
            IConfiguration configuration,
            bool isLocal = false)
            where T : class
        {
            services
                .AddTopicFactory<T>()
                .ConfigureServiceBusTopicOptions<T>(configuration, isLocal)
                .AddTopicSender<T>();
        }

        private static void ConfigureServiceBusQueueOptions<T>(
            this IServiceCollection services,
            HostBuilderContext context,
            bool isLocal) where T: class
        {
            ConfigureServiceBusQueueOptions<T>(services, context.Configuration, isLocal);
        }

        private static IServiceCollection ConfigureServiceBusQueueOptions<T>(
            this IServiceCollection services,
            IConfiguration configuration,
            bool isLocal) where T: class
        {
            return services
                .AddOptions()
                .ConfigureQueueOptions<T>(configuration, isLocal);
        }

        private static void ConfigureServiceBusTopicOptions<T>(
            this IServiceCollection services,
            HostBuilderContext context,
            bool isLocal) where T : class
        {
            ConfigureServiceBusTopicOptions<T>(services, context.Configuration, isLocal);
        }

        private static IServiceCollection ConfigureServiceBusTopicOptions<T>(
            this IServiceCollection services,
            IConfiguration configuration,
            bool isLocal) where T : class
        {
            return services
                .AddOptions()
                .ConfigureTopicOptions<T>(configuration, isLocal);
        }

        private static IServiceCollection AddQueueFactory<T>(this IServiceCollection services)
        {
            services.AddSingleton<IServiceBusQueueFactory<T>, ServiceBusQueueFactory<T>>();
            return services.AddSingleton(x => x.GetRequiredService<IServiceBusQueueFactory<T>>() as IServiceBusQueueFactory);
        }

        private static IServiceCollection AddTopicFactory<T>(this IServiceCollection services)
        {
            services.AddSingleton<IServiceBusTopicFactory<T>, ServiceBusTopicFactory<T>>();
            return services.AddSingleton(x => x.GetRequiredService<IServiceBusTopicFactory<T>>() as IServiceBusTopicFactory);
        }

        private static IServiceCollection AddQueueSender<T>(this IServiceCollection services)
        {
            services.AddSingleton<IServiceBusSenderWrapper<T>>(x =>
            {
                var factory = x.GetRequiredService<IServiceBusQueueFactory<T>>();
                var sender = factory.CreateSender();
                return sender;
            });
            return services.AddSingleton(x => x.GetRequiredService<IServiceBusSenderWrapper<T>>() as IServiceBusSenderWrapper);
        }

        private static IServiceCollection AddTopicSender<T>(this IServiceCollection services)
        {
            services.AddSingleton<IServiceBusSenderWrapper<T>>(x =>
            {
                var factory = x.GetRequiredService<IServiceBusTopicFactory<T>>();
                var sender = factory.CreateSender();
                return sender;
            });
            return services.AddSingleton(x => x.GetRequiredService<IServiceBusSenderWrapper<T>>() as IServiceBusSenderWrapper);
        }
    }
}
