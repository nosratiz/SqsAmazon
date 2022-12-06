namespace SQSLib.Options;

public class SqsOptions
{
    public int MaxNumberOfMessages { get; set; } = 10;
    public int WaitTimeSeconds { get; set; } = 20;
}