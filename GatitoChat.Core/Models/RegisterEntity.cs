using System.Text.Json.Serialization;

namespace GatitoChat.Core.Models;

public sealed class RegisterEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }=string.Empty;
    [JsonPropertyName("uid")]
    public string Uid { get; set; }=string.Empty;
    [JsonPropertyName("pkSign")]
    public string PkSign { get; set; }=string.Empty;

    [JsonPropertyName("rnd")]
    public string Rnd { get; set; } = string.Empty;
    [JsonPropertyName("sessionId")]
    public string SessionId { get; set; } = string.Empty;
    [JsonPropertyName("vfCode")]
    public string VfCode { get; set; } = string.Empty;
    [JsonPropertyName("oriVf")]
    public string OriVfCode { get; set;} = string.Empty;
}