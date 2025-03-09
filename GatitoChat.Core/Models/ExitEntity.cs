using System.Text.Json.Serialization;

namespace GatitoChat.Core.Models;

public sealed class ExitEntity
{
    [JsonPropertyName("roomIds")]
    public string[]? RoomIds { get; set; }
    [JsonPropertyName("name")] 
    public string Name { get; set; }=string.Empty;
}