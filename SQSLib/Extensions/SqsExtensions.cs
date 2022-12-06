using System.Reflection;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using SQSLib.Message;
using SQSLib.Publish;
using SQSLib.Subscribe;

namespace SQSLib.Extensions;

public static class SqsExtensions
{
    public static IServiceCollection AddSqs(this IServiceCollection services)
    {
        services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient());
        
        services.AddSingleton<ISqsPublisher, SqsPublisher>();
        services.AddSingleton<ISqsSubscribe, SqsSubscribe>();
        
        var handlers = Assembly.GetExecutingAssembly().DefinedTypes
            .Where(x => typeof(IMessageHandler).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

        foreach (var handler in handlers)
        {
            var handlerType = handler.AsType();
            services.AddScoped(handlerType);
        }
        return services;
    }
}