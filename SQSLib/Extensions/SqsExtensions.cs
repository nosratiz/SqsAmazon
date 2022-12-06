using System.Reflection;
using Amazon.Extensions.NETCore.Setup;
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
        var awsSqsOptions = new AWSOptions
        {
            DefaultClientConfig =
            {
                ServiceURL = "http://localhost:4566"
            }
        };
        services.AddAWSService<IAmazonSQS>(awsSqsOptions);

        services.AddSingleton<ISqsPublisher, SqsPublisher>();
        services.AddSingleton<ISqsSubscribe, SqsSubscribe>();
        services.AddSingleton<MessageDispatcher>();
        
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