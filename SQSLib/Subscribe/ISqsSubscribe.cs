namespace SQSLib.Subscribe;

public interface ISqsSubscribe
{
    Task<bool>  SubscribeAsync(string queueName,CancellationToken cancellationToken);
}