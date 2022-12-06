using SQSLib.Subscribe;

namespace Subscriber;

public class UserSubscribeService : BackgroundService
{
    private readonly ISqsSubscribe _sqs;

    public UserSubscribeService(ISqsSubscribe sqs)
    {
        _sqs = sqs;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
           await _sqs.SubscribeAsync("sample-queue", stoppingToken);
        }
        
    }
    
}