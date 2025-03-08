using System.Text.Json.Serialization;

namespace GatitoChat.Core.Models;

public sealed class MessageResponse
{
    [JsonPropertyName("type")]
    public MessageType Type { get; set; }=MessageType.Chat;
    [JsonPropertyName("message")]
    public string Message { get; set; }=string.Empty;
}