using System.Text.Json.Serialization;

namespace GatitoChat.Core.Models;

public sealed class RoomManageEntity
{
    [JsonPropertyName("token")]
    public string Token { get; set; }=string.Empty;
    [JsonPropertyName("room")]
    public string Room { get; set; }=string.Empty;
}