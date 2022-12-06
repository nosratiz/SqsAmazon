namespace SQSLib.Message;

public interface IMessageHandler
{
    public Task HandleAsync(IMessage message);
    
    public static abstract Type MessageType { get; }
}