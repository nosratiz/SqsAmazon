using System.Text.Json.Serialization;
using SQSLib.Message;

namespace Dto;

public class User : IMessage
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    [JsonIgnore] 
    public string MessageType => nameof(User);
}