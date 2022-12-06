using Dto;
using SQSLib.Message;

namespace Subscriber.Handler;

public class CustomerCreateHandler : IMessageHandler
{
    public Task HandleAsync(IMessage message)
    {
        var customerCreated = (User)message;
        Console.WriteLine($"Customer {customerCreated.Name} created");
        return Task.CompletedTask;
    }
}