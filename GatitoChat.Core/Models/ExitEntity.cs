using System.Text.Json.Serialization;

namespace GatitoChat.Core.Models;

/// <summary>
/// ws: exit all rooms
/// </summary>
public sealed class ExitEntity
{
    [JsonPropertyName("roomIds")]
    public string[]? RoomIds { get; set; }
    [JsonPropertyName("name")] 
    public string Name { get; set; }=string.Empty;
}