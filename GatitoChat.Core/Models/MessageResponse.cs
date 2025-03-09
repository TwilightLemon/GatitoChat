using System.Text.Json.Serialization;

namespace GatitoChat.Core.Models;

public sealed class MessageResponse
{
    [JsonPropertyName("type")]
    public string Type { get; set; }=MessageType.Chat;
    [JsonPropertyName("message")]
    public string Message { get; set; }=string.Empty;
    [JsonPropertyName("roomId")]
    public string RoomId { get; set; }=string.Empty;
    [JsonPropertyName("name")]
    public string SenderName { get; set; }=string.Empty;
}