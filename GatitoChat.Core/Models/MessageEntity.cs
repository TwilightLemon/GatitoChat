using System.Text.Json.Serialization;

namespace GatitoChat.Core.Models;

public static class MessageType
{
    public const string Chat = "chat";
    public const string Leave = "leave";
    public const string Join = "join";
}
public sealed class MessageEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }=string.Empty;
    [JsonPropertyName("senderId")]
    public string SenderId { get; set; }=string.Empty;
    [JsonPropertyName("token")]
    public string Token { get; set; }=string.Empty;
    [JsonPropertyName("sign")]
    public string Sign { get; set; }=string.Empty;
    [JsonPropertyName("room")]
    public string RoomHash { get; set; }=string.Empty;
    [JsonPropertyName("type")]
    public string Type { get; set; }=MessageType.Chat;
    [JsonPropertyName("message")]
    public string Message { get; set; }=string.Empty;
}