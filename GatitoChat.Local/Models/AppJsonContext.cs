using System.Text.Json.Serialization;

namespace GatitoChat.Local.Models;

[JsonSerializable(typeof(UniversalMessageEntity))]
[JsonSourceGenerationOptions]
internal partial class AppJsonContext:JsonSerializerContext{}