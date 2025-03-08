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
}