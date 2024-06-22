using Azure.Messaging.ServiceBus;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text.Json;

namespace ServiceBus.Package.Messaging
{
    public static class MessageExtensions
    {
        private static readonly DataContractSerializer _serializer = new(typeof(string));

        private const string ActivityIdPropertyName = "Diagnostic-Id";
        private const string CorrelationContextPropertyName = "Correlation-Id";

        public static ServiceBusMessage ToMessage<T>(this T item)
        {
            return new ServiceBusMessage
            {
                ContentType = "application/json",
                Body = new System.BinaryData(item)
            };
        }

        public static T GetJsonBody<T>(this ServiceBusReceivedMessage message)
        {
            string jsonBody = message.Body.ToString();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            return JsonSerializer.Deserialize<T>(jsonBody, options);
        }

        public static ServiceBusMessage AddHeadersFromActivity(
            this ServiceBusMessage message,
            Activity activity,
            Dictionary<string,string> extraApplicationProperties = null)
        {
            message
                .AddApplicationProperty(ActivityIdPropertyName, activity.Id)
                .AddApplicationProperty(CorrelationContextPropertyName, 
                    activity.Baggage.Select(item => $"{item.Key}={item.Value}").Join(','));
            if(extraApplicationProperties != null)
            {
                foreach (var extraApplicationProperty in extraApplicationProperties)
                {
                    message.AddApplicationProperty(extraApplicationProperty.Key, extraApplicationProperty.Value);
                }
            }
            return message;
        }

        private static ServiceBusMessage AddApplicationProperty(
            this ServiceBusMessage message,
            string key,
            string value)
        {
            message.ApplicationProperties.Add(new KeyValuePair<string,object>(key, value));
            return message;
        }

        private static string Join(this IEnumerable<string> strings, char separator)
            => string.Join(separator, strings);

        /*
         Extracted from: 
         */
        internal static bool TryExtractContext(this ServiceBusReceivedMessage message, out IList<KeyValuePair<string, string>> context)
        {
            context = null;
            try
            {
                if(message.ApplicationProperties.TryGetValue(CorrelationContextPropertyName, out object ctxObj))
                {
                    string ctxStr = ctxObj as string;
                    if(string.IsNullOrEmpty(ctxStr))
                        return false;
                    
                    var ctxList = ctxStr.Split(',');
                    
                    if(ctxList.Length == 0)
                        return false;

                    context = new List<KeyValuePair<string, string>>();
                    foreach (var item in ctxList)
                    {
                        var kvp = item.Split('=');
                        if (kvp.Length == 2)
                            context.Add(new KeyValuePair<string, string>(kvp[0], kvp[1]));
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                /* 
                 * Ignored, if the context is invalid there is nothing we can do because an invalid context is created by the consumer. 
                 * Throwing here would break message processing in the producer
                */
            }
            return false;
        }
    }
}
