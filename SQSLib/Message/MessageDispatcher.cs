using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SQSLib.Message;

public class MessageDispatcher
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Dictionary<string, Type> _messageMappings;
    private readonly Dictionary<string, Func<IServiceProvider, IMessageHandler>> _handlers;

    public MessageDispatcher(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _messageMappings = Assembly.GetExecutingAssembly().DefinedTypes
            .Where(x => typeof(IMessage).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .ToDictionary(info => info.Name, info => info.AsType());

        _handlers = Assembly.GetExecutingAssembly().DefinedTypes
            .Where(x => typeof(IMessageHandler).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .ToDictionary<TypeInfo, string, Func<IServiceProvider, IMessageHandler>>(
                info => ((Type) info.GetProperty(nameof(IMessageHandler.MessageType))!.GetValue(null)!)!.Name,
                info => provider => (IMessageHandler) provider.GetRequiredService(info.AsType()));
    }

    public async Task DispatchAsync<TMessage>(TMessage message)
        where TMessage : IMessage
    {
        using var scope = _scopeFactory.CreateScope();
        var handler = _handlers[message.MessageType](scope.ServiceProvider);
        await handler.HandleAsync(message);
    }

    public bool CanHandleMessageType(string messageTypeName) => _handlers.ContainsKey(messageTypeName);


    public Type? GetMessageTypeByName(string messageTypeName) => _messageMappings.GetValueOrDefault(messageTypeName);
}