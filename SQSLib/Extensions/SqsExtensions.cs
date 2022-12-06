using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using SQSLib.Publish;

namespace SQSLib.Extensions;

public static class SqsExtensions
{
    public static IServiceCollection AddSqs(this IServiceCollection services)
    {
        services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient());
        
        services.AddSingleton<ISqsPublisher, SqsPublisher>();
        
        
        return services;
    }
}