using System.Text.Json.Serialization;

namespace GatitoChat.Core.Models;

public enum MessageType
{
    [JsonStringEnumMemberName("chat")]
    Chat,
    [JsonStringEnumMemberName("leave")]
    Leave,
    [JsonStringEnumMemberName("join")]
    Join
}
public sealed class MessageEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }=string.Empty;
    [JsonPropertyName("token")]
    public string Token { get; set; }=string.Empty;
    [JsonPropertyName("room")]
    public string RoomHash { get; set; }=string.Empty;
    [JsonPropertyName("type")]
    public  MessageType Type { get; set; }=MessageType.Chat;
    [JsonPropertyName("message")]
    public string Message { get; set; }=string.Empty;
}