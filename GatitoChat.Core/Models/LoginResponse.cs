using System.Text.Json.Serialization;

namespace GatitoChat.Core.Models;

public sealed class LoginResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    [JsonPropertyName("userName")]
    public string? UserName { get; set; }
    [JsonPropertyName("uid")]
    public string? Uid { get; set; }
    [JsonPropertyName("signature")]
    public string? Token { get; set; }
    [JsonIgnore]
    public string? RandomSeed { get; set; }
    [JsonIgnore]
    public string? Sign { get; set; }
}