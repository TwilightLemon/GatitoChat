using GatitoChat.Core.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GatitoChat.Models;
public enum ContentType
{
    PlainText, Image
}
public record class MessageContent(ContentType Type, string Content)
{
    public static (string?,string?) Parse(MessageResponse msg,SenderType type)
    {
        string displayText = msg.Message;
        string? imageData = null;
        if (type != SenderType.System)
        {
            var msgContent = JsonSerializer.Deserialize(msg.Message, AppJsonContext.Default.MessageContent);
            if (msgContent is null) return(null,null);

            displayText = msgContent.Type switch
            {
                ContentType.Image => "[Image]",
                _ => msgContent.Content
            };
            imageData = msgContent.Type == ContentType.Image ? msgContent.Content : null;
        }
        return (displayText, imageData);
    }
}

[JsonSerializable(typeof(MessageContent))]
[JsonSourceGenerationOptions]
internal partial class AppJsonContext : JsonSerializerContext
{
}