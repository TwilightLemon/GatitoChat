using System.Text.Json.Serialization;

namespace GatitoChat.Core.Models;

/// <summary>
/// Auth Server: Login
/// </summary>
public sealed class LoginEntity
{
    [JsonPropertyName("uid")] 
    public string Uid { get; set; } =string.Empty;
    [JsonPropertyName("sign")]
    public string? Sign { get; set; }

    [JsonPropertyName("rnd")] 
    public string? Rnd { get; set; }
}