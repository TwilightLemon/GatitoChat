using System.Text.Json.Serialization;

namespace GatitoChat.Core.Models;

public sealed class VerificationEntity
{
    [JsonPropertyName("uid")]
    public string Uid { get; set; }=string.Empty;
    [JsonPropertyName("sign")]
    public string Sign { get; set; }=string.Empty;
    [JsonPropertyName("rnd")]
    public string RandomSeed { get; set; }=string.Empty;
    [JsonPropertyName("token")]
    public string Token { get; set; }=string.Empty;
}

public sealed class ActionResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    [JsonPropertyName("msg")]
    public string Message { get; set; }=string.Empty;
}