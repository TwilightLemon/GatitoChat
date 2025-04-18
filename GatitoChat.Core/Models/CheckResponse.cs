using System.Text.Json.Serialization;

namespace GatitoChat.Core.Models;

public class CheckResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    [JsonPropertyName ("userName")]
    public string? Username { get; set; }
    [JsonPropertyName("uid")]
    public string? Uid { get; set; }
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }
    public string? rnd= null;
}
