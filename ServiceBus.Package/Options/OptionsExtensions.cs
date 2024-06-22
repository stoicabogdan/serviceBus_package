using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ServiceBus.Package.Options
{
    public static class OptionsExtensions
    {
        private const string QueuesConfigSectionName = "Queues";
        private const string TopicConfigSectionName = "Topics";
        private const string OptionsPrefix = "Options";

        public static IServiceCollection ConfigureOptionsByTypeName<T>(this IServiceCollection services, HostBuilderContext context) where T : class
            => services.Configure<T>(GetConfigurationSection(typeof(T).Name, context));

        public static IServiceCollection ConfigureTopicOptions<T>(this IServiceCollection services, HostBuilderContext context, bool isLocal) where T : class
            => ConfigureTopicOptions<T>(services, context.Configuration, isLocal);

        public static IServiceCollection ConfigureTopicOptions<T>(this IServiceCollection services, IConfiguration configuration, bool isLocal) where T : class
            => services.ConfigureQueueTopicOptions<TopicOptions<T>>($"{TopicConfigSectionName}:{typeof(T).Name}", configuration, isLocal);

        public static IServiceCollection ConfigureQueueOptions<T>(this IServiceCollection services, HostBuilderContext context, bool isLocal)
           => ConfigureQueueOptions<T>(services, context.Configuration, isLocal);        

        public static IServiceCollection ConfigureQueueOptions<T>(this IServiceCollection services, IConfiguration configuration, bool isLocal)
            => services.ConfigureQueueTopicOptions<QueueOptions<T>>($"{QueuesConfigSectionName}:{typeof(T).Name}", configuration, isLocal);

        private static IServiceCollection ConfigureQueueTopicOptions<T>(
            this IServiceCollection services,
            string name,
            HostBuilderContext context,
            bool isLocal)
            where T : ServiceBusEntityOptions, new()
            => ConfigureQueueTopicOptions<T>(services, name, context.Configuration, isLocal);

        private static IServiceCollection ConfigureQueueTopicOptions<T>(
            this IServiceCollection services,
            string name,
            IConfiguration configuration,
            bool isLocal)
            where T : ServiceBusEntityOptions, new()
            => services
                    .Configure<T>(GetConfigurationSection(name, configuration))
                    .AddSingleton<ServiceBusEntityOptions>(s =>
                    {
                        var opt = s.GetRequiredService<IOptions<T>>().Value;
                        if(isLocal)
                        {
                            // can help when making local queues
                        }
                        return opt;
                    })
                    .AddSingleton(s =>
                    {
                        var opt = s.GetRequiredService<IOptions<T>>().Value;
                        if (isLocal)
                        {
                            // can help when making local queues
                        }
                        return opt;
                    });

        private static IConfiguration GetConfigurationSection(string name, HostBuilderContext context)
            => GetConfigurationSection(name, context.Configuration);

        private static IConfiguration GetConfigurationSection(string name, IConfiguration configuration)
            => configuration.GetSection(name.Replace(OptionsPrefix, string.Empty));
    }
}
