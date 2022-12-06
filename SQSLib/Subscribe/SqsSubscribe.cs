using System.Net;
using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;
using SQSLib.Message;
using SQSLib.Options;

namespace SQSLib.Subscribe;

public class SqsSubscribe : ISqsSubscribe
{
    private readonly IAmazonSQS _sqs;
    private readonly List<string> _messageAttributeNames = new() {"All"};
    private readonly IOptionsMonitor<SqsOptions> _options;
    private readonly MessageDispatcher _dispatcher;

    public SqsSubscribe(IAmazonSQS sqs, IOptionsMonitor<SqsOptions> options, MessageDispatcher dispatcher)
    {
        _sqs = sqs;
        _options = options;
        _dispatcher = dispatcher;
    }

    public async Task<bool> SubscribeAsync(string queueName, CancellationToken cancellationToken)
    {
        var queueUrl = await _sqs.GetQueueUrlAsync(queueName, cancellationToken);

        var receiveMessageRequest = new ReceiveMessageRequest
        {
            QueueUrl = queueUrl.QueueUrl,
            MessageAttributeNames = _messageAttributeNames,
            AttributeNames = _messageAttributeNames,
            MaxNumberOfMessages = _options.CurrentValue.MaxNumberOfMessages,
            WaitTimeSeconds = _options.CurrentValue.WaitTimeSeconds
        };

        var receiveMessage = await _sqs.ReceiveMessageAsync(receiveMessageRequest, cancellationToken);

        if (receiveMessage.HttpStatusCode != HttpStatusCode.OK)
        {
            ///TODO: Log
            return false;
        }

        foreach (var message in receiveMessage.Messages)
        {
            var messageTypeName = message.MessageAttributes
                .GetValueOrDefault(nameof(IMessage.MessageType))?.StringValue;

            if (string.IsNullOrEmpty(messageTypeName))
                continue;

            if (!_dispatcher.CanHandleMessageType(messageTypeName))
                continue;

            var messageType = _dispatcher.GetMessageTypeByName(messageTypeName)!;

            var messageAsType = (IMessage) JsonSerializer.Deserialize(message.Body, messageType)!;

            await _dispatcher.DispatchAsync(messageAsType);

            await _sqs.DeleteMessageAsync(queueName, message.ReceiptHandle, cancellationToken);

        }

        return true;
    }

}