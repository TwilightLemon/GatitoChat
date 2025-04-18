using System.Text.Json.Serialization;

namespace GatitoChat.Core.Models;

public class VerifierEntity
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    [JsonPropertyName("uid")]
    public string BlindedUid { get; set; } = string.Empty;
    [JsonPropertyName("rnd")]
    public string Rnd { get; set; } = string.Empty;
    [JsonPropertyName("sessionId")]
    public string SessionId { get; set; } = string.Empty;
}

public class VerifierResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}
