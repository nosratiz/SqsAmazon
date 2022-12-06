using Amazon.SQS;
using Amazon.SQS.Model;

namespace SQSLib.Subscribe;

public interface ISqsSubscribe
{
    Task<ReceiveMessageResponse?>  SubscribeAsync(string queueName,CancellationToken cancellationToken);
}

public class SqsSubscribe : ISqsSubscribe
{
    private readonly IAmazonSQS _sqs;

    public SqsSubscribe(IAmazonSQS sqs)
    {
        _sqs = sqs;
    }

    public Task<ReceiveMessageResponse?> SubscribeAsync(string queueName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}